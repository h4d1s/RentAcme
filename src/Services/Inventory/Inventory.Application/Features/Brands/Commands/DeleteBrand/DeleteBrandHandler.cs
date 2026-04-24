using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using MediatR;

namespace Inventory.Application.Features.Brands.Commands.DeleteBrand;

public class DeleteBrandHandler : IRequestHandler<DeleteBrandCommand, Unit>
{
    private readonly IBrandRepository _brandRepository;

    public DeleteBrandHandler(
        IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public async Task<Unit> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id);

        if (brand is null)
        {
            throw new NotFoundException($"Brand with {request.Id} not found.");
        }

        _brandRepository.Delete(brand);
        await _brandRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
