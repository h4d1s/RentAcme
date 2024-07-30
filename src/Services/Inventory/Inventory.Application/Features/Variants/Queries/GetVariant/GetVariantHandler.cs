using Inventory.Application.Exceptions;
using Inventory.Application.Features.Models.Queries.GetModel;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Variants.Queries.GetVariant;

public class GetVariantHandler : IRequestHandler<GetVariantQuery, Variant>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetVariantHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Variant> Handle(GetVariantQuery request, CancellationToken cancellationToken)
    {
        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(request.Id);

        if (variant == null)
        {
            throw new NotFoundException($"Variant with {request.Id} not found.");
        }

        return variant;
    }
}
