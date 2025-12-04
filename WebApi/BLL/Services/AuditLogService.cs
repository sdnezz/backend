using Models.DTO.Common;
using WebApi.DAL;
using WebApi.DAL.Interfaces;
using WebApi.DAL.Models;

namespace WebApi.BLL.Services;

public class AuditLogService(UnitOfWork unitOfWork, IAuditLogOrderRepository auditLogOrderRepository)
{
    public async Task<AuditLogOrderUnit[]> BatchInsert(AuditLogOrderUnit[] auditLogOrderUnits, CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;

        var logs = auditLogOrderUnits.Select(o => new V1AuditLogOrderDal
        {
            OrderId = o.OrderId,
            OrderItemId = o.OrderItemId,
            CustomerId = o.CustomerId,
            OrderStatus = o.OrderStatus,
            CreatedAt = now,
            UpdatedAt = now
        }).ToArray();
            
        var insertedAuditLogOrders = await auditLogOrderRepository.BulkInsert(logs, token);

        var result = insertedAuditLogOrders.Select(x => new AuditLogOrderUnit
        {
            Id = x.Id,
            OrderId = x.OrderId,
            OrderItemId = x.OrderItemId,
            CustomerId = x.CustomerId,
            OrderStatus = x.OrderStatus,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToArray();

        return result;
    }
}