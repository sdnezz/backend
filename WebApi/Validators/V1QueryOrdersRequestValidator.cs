using FluentValidation;
using Models.DTO.V1.Requests;

namespace WebApi.Validators;

public class V1QueryOrdersRequestValidator : AbstractValidator<QueryOrderItemsModel>
{
    public V1QueryOrdersRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => (x.Ids != null && x.Ids.Length > 0) || (x.CustomerIds != null && x.CustomerIds.Length > 0))
            .WithMessage("Ids or CustomerIds must be");

        RuleForEach(x => x.Ids)
            .GreaterThan(0)
            .When(x => x.Ids != null && x.Ids.Length > 0)
            .WithMessage("Ids must contain ids greater than 0");
        
        RuleForEach(x => x.CustomerIds)
            .GreaterThan(0)
            .When(x => x.CustomerIds != null && x.CustomerIds.Length > 0)
            .WithMessage("CustomerIds must contain ids greater than 0");
        
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");
    }
}