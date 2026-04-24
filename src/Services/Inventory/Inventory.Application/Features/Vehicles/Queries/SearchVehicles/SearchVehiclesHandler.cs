using AutoMapper;
using Common.Models;
using Inventory.Application.Models;
using Inventory.Application.Specifications.Vehicles;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MediatR;

namespace Inventory.Application.Features.Vehicles.Queries.SearchVehicles;

public class SearchVehiclesHandler : IRequestHandler<SearchVehiclesQuery, PagedResponse<VehicleSearchResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public SearchVehiclesHandler(
        IVehicleRepository vehicleRepository,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResponse<VehicleSearchResponse>> Handle(SearchVehiclesQuery request, CancellationToken cancellationToken)
    {
        var specification = new VehicleFilterPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy,
            request.Gearbox,
            request.Category,
            request.RentalPricePerDayFrom,
            request.RentalPricePerDayTo,
            request.PickupDate,
            request.ReturnDate
        );
        var vehicleList = await _vehicleRepository.ListAsync(specification);

        var vehicleResponseList = vehicleList
            .Select(vehicle => _mapper.Map<VehicleSearchResponse>(vehicle));

        specification = new VehicleFilterPaginatedSpecification(
            null,
            null,
            request.Order,
            request.OrderBy,
            request.Gearbox,
            request.Category,
            request.RentalPricePerDayFrom,
            request.RentalPricePerDayTo,
            request.PickupDate,
            request.ReturnDate
        );
        var vehicleListAllCount = await _vehicleRepository.CountAsync(specification);

        return new PagedResponse<VehicleSearchResponse>(
            request.Page,
            request.PageSize,
            vehicleListAllCount,
            vehicleResponseList
        );
    }
}
