using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Application.Features.Models.Commands.CreateModel;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Variants.Commands.CreateVariant;

public class CreateVariantHandler : IRequestHandler<CreateVariantCommand, Guid>
{
    private readonly IVariantRepository _variantRepository;
    private readonly IValidator<CreateVariantCommand> _validator;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateVariantHandler(
        IVariantRepository variantRepository,
        IValidator<CreateVariantCommand> validator,
        ILogger<CreateBrandHandler> logger)
    {
        _variantRepository = variantRepository ?? throw new ArgumentNullException(nameof(variantRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(CreateVariantCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var variant = new Variant(
            request.Name,
            request.Gearbox,
            request.FuelType,
            request.Power,
            request.EngineSize,
            request.ModelId);

        _logger.LogInformation("Creating variant - Variant: {@variant}", variant);

        var id = await _variantRepository.AddAsync(variant);
        await _variantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return id;
    }
}
