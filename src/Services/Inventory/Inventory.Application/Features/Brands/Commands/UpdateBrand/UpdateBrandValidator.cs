using FluentValidation;

namespace Inventory.Application.Features.Brands.Commands.UpdateBrand;

public class UpdateBrandValidator : AbstractValidator<UpdateBrandCommand>
{
    public UpdateBrandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty();
    }
}
