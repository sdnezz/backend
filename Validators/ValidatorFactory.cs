using FluentValidation;
using Models.DTO.V1.Requests;

namespace SolutionLab1.Validators;

public class ValidatorFactory(IServiceProvider serviceProvider)
{
    public IValidator<T> GetValidator<T>()
    {
        return serviceProvider.GetRequiredService<IValidator<T>>()!;
    }
}