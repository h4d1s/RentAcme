using Inventory.Application.Exceptions;
using Inventory.Application.Features.Brands.Commands.DeleteBrand;
using Inventory.Domain.AggregatesModel.ModelAggregate;
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
    private readonly IModelRepository _modelRepository;

    public DeleteModelHandler(
        IModelRepository modelRepository)
    {
        _modelRepository = modelRepository;
    }

    public async Task<Unit> Handle(DeleteModelCommand request, CancellationToken cancellationToken)
    {
        var model = await _modelRepository.GetByIdAsync(request.Id);

        if (model is null)
        {
            throw new NotFoundException($"Model with {request.Id} not found.");
        }

        _modelRepository.Delete(model);
        await _modelRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
