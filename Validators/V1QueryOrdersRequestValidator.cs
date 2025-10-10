using FluentValidation;
using Models.DTO.V1.Requests;

namespace SolutionLab1.Validators;

public class V1QueryOrdersRequestValidator : AbstractValidator<QueryOrderItemsModel>
{
    public V1QueryOrdersRequestValidator()
    {
        // Если заданы Ids — все должны быть > 0
        RuleForEach(x => x.Ids)
            .GreaterThan(0)
            .When(x => x.Ids != null && x.Ids.Length > 0)
            .WithMessage("Ids must contain ids greater than 0");

        // Если заданы CustomerIds — все должны быть > 0
        RuleForEach(x => x.CustomerIds)
            .GreaterThan(0)
            .When(x => x.CustomerIds != null && x.CustomerIds.Length > 0)
            .WithMessage("CustomerIds must contain ids greater than 0");

        // Page и PageSize не могут быть отрицательными
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Page must be greater than or equal to 0");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(0)
            .WithMessage("PageSize must be greater than or equal to 0");

        // IncludeOrderItems — булево, дополнительной валидации не требуется
    }
}