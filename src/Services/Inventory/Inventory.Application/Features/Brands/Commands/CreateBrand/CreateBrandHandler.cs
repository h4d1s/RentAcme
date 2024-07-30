using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.Common;
using MassTransit.Transports;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandHandler : IRequestHandler<CreateBrandCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateBrandCommand> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateBrandHandler(
        IUnitOfWork unitOfWork,
        IValidator<CreateBrandCommand> validator,
        IMapper mapper,
        ILogger<CreateBrandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var brand = _mapper.Map<Brand>(request);

        _logger.LogInformation("Creating brand - Brand: {@Brand}", brand);

        var id = await _unitOfWork.BrandRepository.AddAsync(brand);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return id;
    }
}
