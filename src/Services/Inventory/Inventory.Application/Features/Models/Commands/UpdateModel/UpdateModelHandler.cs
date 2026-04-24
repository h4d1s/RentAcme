using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using MediatR;

namespace Inventory.Application.Features.Models.Commands.UpdateModel;

public class UpdateModelHandler : IRequestHandler<UpdateModelCommand, Unit>
{
    private readonly IModelRepository _modelRepository;
    private readonly IValidator<UpdateModelCommand> _validator;

    public UpdateModelHandler(
        IModelRepository modelRepository,
        IValidator<UpdateModelCommand> validator)
    {
        _modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Unit> Handle(UpdateModelCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid Model", validationResult);
        }

        var model = await _modelRepository.GetByIdAsync(request.Id);

        if (model is null)
        {
            throw new NotFoundException($"Model with {request.Id} not found.");
        }

        model.UpdateName(request.Name);
        model.UpdateYearOfProduction(request.YearOfProduction);
        model.UpdateNumberOfSeats(request.NumberOfSeats);
        model.UpdateCategory(request.Category);
        model.UpdateBrandId(request.BrandId);

        _modelRepository.Update(model);
        await _modelRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
