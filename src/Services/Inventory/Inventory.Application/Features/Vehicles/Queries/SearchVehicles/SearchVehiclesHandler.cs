using Inventory.Application.Models;
using MediatR;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using AutoMapper;
using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Application.Specifications.Vehicles;
using Common.Models;

namespace Inventory.Application.Features.Vehicles.Queries.SearchVehicles;

public class SearchVehiclesHandler : IRequestHandler<SearchVehiclesQuery, PagedResponse<VehicleSearchResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SearchVehiclesHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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
        var vehicleList = await _unitOfWork.VehicleRepository.ListAsync(specification);

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
        var vehicleListAllCount = await _unitOfWork.VehicleRepository.CountAsync(specification);

        return new PagedResponse<VehicleSearchResponse>(
            request.Page,
            request.PageSize,
            vehicleListAllCount,
            vehicleResponseList
        );
    }
}
