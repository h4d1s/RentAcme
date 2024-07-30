using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Queries.GetModel;

public class GetModelHandler : IRequestHandler<GetModelQuery, Model>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetModelHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Model> Handle(GetModelQuery request, CancellationToken cancellationToken)
    {
        var model = await _unitOfWork.ModelRepository.GetByIdAsync(request.Id);

        if (model == null)
        {
            throw new NotFoundException($"Model with {request.Id} not found.");
        }

        return model;
    }
}
