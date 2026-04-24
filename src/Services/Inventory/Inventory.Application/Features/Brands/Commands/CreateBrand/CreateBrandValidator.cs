using FluentValidation;

namespace Inventory.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty();
    }
}
