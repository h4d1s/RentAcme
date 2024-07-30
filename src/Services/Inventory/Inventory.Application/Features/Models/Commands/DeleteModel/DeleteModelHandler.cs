using Inventory.Application.Exceptions;
using Inventory.Application.Features.Brands.Commands.DeleteBrand;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Commands.DeleteModel;

public class DeleteModelHandler : IRequestHandler<DeleteModelCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteModelHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteModelCommand request, CancellationToken cancellationToken)
    {
        var model = await _unitOfWork.ModelRepository.GetByIdAsync(request.Id);

        if (model == null)
        {
            throw new NotFoundException($"Model with {request.Id} not found.");
        }

        _unitOfWork.ModelRepository.Delete(model);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
