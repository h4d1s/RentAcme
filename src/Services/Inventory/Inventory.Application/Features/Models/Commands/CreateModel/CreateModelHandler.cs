using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.Common;
using MassTransit.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Commands.CreateModel;

public class CreateModelHandler : IRequestHandler<CreateModelCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateModelCommand> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateModelHandler(
        IUnitOfWork unitOfWork,
        IValidator<CreateModelCommand> validator,
        IMapper mapper,
        ILogger<CreateBrandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(CreateModelCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var model = _mapper.Map<Model>(request);

        _logger.LogInformation("Creating model - Model: {@model}", model);

        var id = await _unitOfWork.ModelRepository.AddAsync(model);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return id;
    }
}
