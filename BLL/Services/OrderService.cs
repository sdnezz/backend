using SolutionLab1.BLL.Models;
using SolutionLab1.DAL;
using SolutionLab1.DAL.Interfaces;
using SolutionLab1.DAL.Models;

namespace SolutionLab1.BLL.Services;

public class OrderService(UnitOfWork unitOfWork, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
{
    /// <summary>
    /// Метод создания заказов
    /// </summary>
    public async Task<OrderUnit[]> BatchInsert(OrderUnit[] orderUnits, CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        await using var transaction = await unitOfWork.BeginTransactionAsync(token);

        try
        {
// 1) Подготовим массив V1OrderDal для bulk-insert'а заказов
            var ordersToInsert = orderUnits.Select(o => new V1OrderDal
            {
                // Не включаем Id — это заполнит БД (bigserial)
                CustomerId = o.CustomerId,
                DeliveryAddress = o.DeliveryAddress,
                TotalPriceCents = o.TotalPriceCents,
                TotalPriceCurrency = o.TotalPriceCurrency,
                CreatedAt = now,
                UpdatedAt = now
            }).ToArray();

            // 2) Вставляем заказы и получаем их с заполненными id (RETURNING)
            var insertedOrders = await orderRepository.BulkInsert(ordersToInsert, token);

            // 3) Подготовим все позиции, привязав их к соответствующим вставленным заказам.
            // Предполагаем, что insertedOrders возвращается в том же порядке, что и входные ordersToInsert.
            var allOrderItems = new List<V1OrderItemDal>();
            for (int i = 0; i < insertedOrders.Length; i++)
            {
                var createdOrder = insertedOrders[i];
                var sourceOrderUnit = orderUnits.Length > i ? orderUnits[i] : null;
                var sourceItems = sourceOrderUnit?.OrderItems ?? Array.Empty<OrderItemUnit>();

                foreach (var it in sourceItems)
                {
                    allOrderItems.Add(new V1OrderItemDal
                    {
                        // Привязываем к id созданного заказа
                        OrderId = createdOrder.Id,
                        ProductId = it.ProductId,
                        Quantity = it.Quantity,
                        ProductTitle = it.ProductTitle,
                        ProductUrl = it.ProductUrl,
                        PriceCents = it.PriceCents,
                        PriceCurrency = it.PriceCurrency,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
            }

            // 4) Если есть позиции — вставляем их
            V1OrderItemDal[] insertedItems = Array.Empty<V1OrderItemDal>();
            if (allOrderItems.Count > 0)
            {
                insertedItems = await orderItemRepository.BulkInsert(allOrderItems.ToArray(), token);
            }

            // 5) Коммит транзакции
            await transaction.CommitAsync(token);

            // 6) Построим lookup по order_id, чтобы Map вернул OrderUnit с вложенными позициями
            var lookup = insertedItems.ToLookup(x => x.OrderId);

            // 7) Возвращаем результат (Map умеет принимать orderItemLookup = null)
            return Map(insertedOrders, lookup);
        }
        catch (Exception e) 
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    
    /// <summary>
    /// Метод получения заказов
    /// </summary>
    public async Task<OrderUnit[]> GetOrders(QueryOrderItemsModel model, CancellationToken token)
    {
        var orders = await orderRepository.Query(new QueryOrdersDalModel
        {
            Ids = model.Ids,
            CustomerIds = model.CustomerIds,
            Limit = model.PageSize,
            Offset = (model.Page - 1) * model.PageSize
        }, token);

        if (orders.Length is 0)
        {
            return Array.Empty<OrderUnit>();
        }
        
        ILookup<long, V1OrderItemDal> orderItemLookup = null;
        if (model.IncludeOrderItems)
        {
            var orderItems = await orderItemRepository.Query(new QueryOrderItemsDalModel
            {
                OrderIds = orders.Select(x => x.Id).ToArray(),
            }, token);

            orderItemLookup = orderItems.ToLookup(x => x.OrderId);
        }

        return Map(orders, orderItemLookup);
    }
    
    private OrderUnit[] Map(V1OrderDal[] orders, ILookup<long, V1OrderItemDal> orderItemLookup = null)
    {
        return orders.Select(x => new OrderUnit
        {
            Id = x.Id,
            CustomerId = x.CustomerId,
            DeliveryAddress = x.DeliveryAddress,
            TotalPriceCents = x.TotalPriceCents,
            TotalPriceCurrency = x.TotalPriceCurrency,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            OrderItems = orderItemLookup?[x.Id].Select(o => new OrderItemUnit
            {
                Id = o.Id,
                OrderId = o.OrderId,
                ProductId = o.ProductId,
                Quantity = o.Quantity,
                ProductTitle = o.ProductTitle,
                ProductUrl = o.ProductUrl,
                PriceCents = o.PriceCents,
                PriceCurrency = o.PriceCurrency,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            }).ToArray() ?? []
        }).ToArray();
    }
}