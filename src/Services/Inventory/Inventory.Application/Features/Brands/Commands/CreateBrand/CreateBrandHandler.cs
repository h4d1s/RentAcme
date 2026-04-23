using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.BookingAggregate;
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
    private readonly IBrandRepository _brandRepository;
    private readonly IValidator<CreateBrandCommand> _validator;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateBrandHandler(
        IBrandRepository brandRepository,
        IValidator<CreateBrandCommand> validator,
        ILogger<CreateBrandHandler> logger)
    {
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var brand = new Brand(request.Name);

        _logger.LogInformation("Creating brand - Brand: {@Brand}", brand);

        var id = await _brandRepository.AddAsync(brand);
        await _brandRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return id;
    }
}
