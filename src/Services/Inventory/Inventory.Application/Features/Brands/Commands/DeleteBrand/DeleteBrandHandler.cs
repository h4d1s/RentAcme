using Inventory.Application.Exceptions;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Brands.Commands.DeleteBrand;

public class DeleteBrandHandler : IRequestHandler<DeleteBrandCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBrandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Unit> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.BrandRepository.GetByIdAsync(request.Id);

        if (brand == null)
        {
            throw new NotFoundException($"Brand with {request.Id} not found.");
        }

        _unitOfWork.BrandRepository.Delete(brand);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
