using Inventory.Application.Exceptions;
using Inventory.Application.Features.Models.Commands.DeleteModel;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Variants.Commands.DeleteVariant;

public class DeleteVariantHandler : IRequestHandler<DeleteVariantCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVariantHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(request.Id);

        if (variant == null)
        {
            throw new NotFoundException($"Variant with {request.Id} not found.");
        }

        _unitOfWork.VariantRepository.Delete(variant);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
