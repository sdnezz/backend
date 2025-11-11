using FluentValidation;
using Models.DTO.V1.Requests;

namespace WebApplication1.Validators;

public class V1CreateAuditLogOrderRequestValidator : AbstractValidator<V1AuditLogOrderRequest>
{
    public V1CreateAuditLogOrderRequestValidator()
    {
        RuleForEach(x => x.Orders)
            .NotNull();
        RuleForEach(x => x.Orders)
            .ChildRules(order =>
            {
                order.RuleFor(o => o.OrderId)
                    .GreaterThan(0)
                    .WithMessage("OrderId must be greater than 0");
                order.RuleFor(o => o.OrderItemId)
                    .GreaterThan(0)
                    .WithMessage("OrderItemId must be greater than 0");
                order.RuleFor(o => o.CustomerId)
                    .GreaterThan(0)
                    .WithMessage("CustomerId must be greater than 0");
                order.RuleFor(o => o.OrderStatus)
                    .NotEmpty()
                    .WithMessage("OrderStatus must not be empty");
            });
    }
}