using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Commands.UpdateModel;

public class UpdateModelHandler : IRequestHandler<UpdateModelCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateModelCommand> _validator;

    public UpdateModelHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateModelCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Unit> Handle(UpdateModelCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid Model", validationResult);
        }

        var model = await _unitOfWork.ModelRepository.GetByIdAsync(request.Id);

        if (model == null)
        {
            throw new NotFoundException($"Model with {request.Id} not found.");
        }

        _mapper.Map(request, model);

        _unitOfWork.ModelRepository.Update(model);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
