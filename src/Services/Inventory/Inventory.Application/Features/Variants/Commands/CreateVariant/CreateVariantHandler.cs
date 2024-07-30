using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Application.Features.Models.Commands.CreateModel;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateVariantCommand> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateVariantHandler(
        IUnitOfWork unitOfWork,
        IValidator<CreateVariantCommand> validator,
        IMapper mapper,
        ILogger<CreateBrandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(CreateVariantCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var variant = _mapper.Map<Variant>(request);

        _logger.LogInformation("Creating variant - Variant: {@variant}", variant);

        var id = await _unitOfWork.VariantRepository.AddAsync(variant);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return id;
    }
}
