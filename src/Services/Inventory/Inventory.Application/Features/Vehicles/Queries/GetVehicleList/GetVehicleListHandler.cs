using AutoMapper;
using Common.Models;
using Inventory.Application.Models;
using Inventory.Application.Specifications.Vehicles;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MediatR;

namespace Inventory.Application.Features.Vehicles.Queries.GetVehicleList;

public class GetVehicleListHandler : IRequestHandler<GetVehicleListQuery, PagedResponse<VehicleResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public GetVehicleListHandler(
        IVehicleRepository vehicleRepository,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResponse<VehicleResponse>> Handle(GetVehicleListQuery request, CancellationToken cancellationToken)
    {
        var specification = new VehicleListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy,
            request.RentalPricePerDayFrom,
            request.RentalPricePerDayTo);
        var vehicleList = await _vehicleRepository.ListAsync(specification);

        var vehicleResponseList = vehicleList
            .Select(vehicle => _mapper.Map<VehicleResponse>(vehicle));

        specification = new VehicleListPaginatedSpecification(
            null,
            null,
            request.Order,
            request.OrderBy,
            request.RentalPricePerDayFrom,
            request.RentalPricePerDayTo);
        var vehiclesListAllCount = await _vehicleRepository.CountAsync(specification);

        return new PagedResponse<VehicleResponse>(
            request.Page,
            request.PageSize,
            vehiclesListAllCount,
            vehicleResponseList
        );
    }
}
