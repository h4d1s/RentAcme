using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Features.Models.Commands.CreateModel;

public class CreateModelHandler : IRequestHandler<CreateModelCommand, Guid>
{
    private readonly IModelRepository _modelRepository;
    private readonly IValidator<CreateModelCommand> _validator;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateModelHandler(
        IModelRepository modelRepository,
        IValidator<CreateModelCommand> validator,
        ILogger<CreateBrandHandler> logger)
    {
        _modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(CreateModelCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var model = new Model(request.Name, request.YearOfProduction, request.NumberOfSeats, request.Category, request.BrandId);

        _logger.LogInformation("Creating model - Model: {@model}", model);

        var id = await _modelRepository.AddAsync(model);
        await _modelRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return id;
    }
}
