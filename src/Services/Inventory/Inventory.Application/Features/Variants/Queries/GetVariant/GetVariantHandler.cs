using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;

namespace Inventory.Application.Features.Variants.Queries.GetVariant;

public class GetVariantHandler : IRequestHandler<GetVariantQuery, Variant>
{
    private readonly IVariantRepository _variantRepository;

    public GetVariantHandler(
        IVariantRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public async Task<Variant> Handle(GetVariantQuery request, CancellationToken cancellationToken)
    {
        var variant = await _variantRepository.GetByIdAsync(request.Id);

        if (variant is null)
        {
            throw new NotFoundException($"Variant with {request.Id} not found.");
        }

        return variant;
    }
}
