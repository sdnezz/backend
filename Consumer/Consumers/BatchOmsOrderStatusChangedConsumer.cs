using Common;
using Consumer.Base;
using Consumer.Config;
using Messages;
using Microsoft.Extensions.Options;
using Models.DTO.V1.Requests;

namespace Consumer.Consumers;

public class BatchOmsOrderStatusChangedConsumer(
    IOptions<RabbitMqSettings> rabbitMqSettings,
    IServiceProvider serviceProvider)
    : BaseBatchMessageConsumer<OmsOrderStatusChangedMessage>(
        rabbitMqSettings.Value,
        settings => settings.OrderStatusChanged)
{
    protected override async Task ProcessMessages(OmsOrderStatusChangedMessage[] messages)
    {
        using var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<OmsClient>();

        await client.LogOrder(new V1AuditLogOrderRequest
        {
            Orders = messages.Select(order => new V1AuditLogOrderRequest.LogOrder
            {
                OrderId = order.OrderId,
                OrderStatus = order.OrderStatus
            }).ToArray()
        }, CancellationToken.None);
    }
}