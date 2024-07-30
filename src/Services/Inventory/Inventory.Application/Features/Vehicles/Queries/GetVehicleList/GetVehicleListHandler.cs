using AutoMapper;
using Common.Models;
using Inventory.Application.Exceptions;
using Inventory.Application.Models;
using Inventory.Application.Specifications.Vehicles;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Inventory.Application.Features.Vehicles.Queries.GetVehicleList;

public class GetVehicleListHandler : IRequestHandler<GetVehicleListQuery, PagedResponse<VehicleResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetVehicleListHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<VehicleResponse>> Handle(GetVehicleListQuery request, CancellationToken cancellationToken)
    {
        var specification = new VehicleListPaginatedSpecification(
            request.page,
            request.pageSize,
            request.order,
            request.orderBy,
            request.RentalPricePerDayFrom,
            request.RentalPricePerDayTo);
        var vehicleList = await _unitOfWork.VehicleRepository.ListAsync(specification);

        var vehicleResponseList = vehicleList
            .Select(vehicle => _mapper.Map<VehicleResponse>(vehicle));

        specification = new VehicleListPaginatedSpecification(
            null,
            null,
            request.order,
            request.orderBy,
            request.RentalPricePerDayFrom,
            request.RentalPricePerDayTo);
        var bookingListAllCount = await _unitOfWork.VehicleRepository.CountAsync(specification);

        return new PagedResponse<VehicleResponse>(
            request.page,
            request.pageSize,
            bookingListAllCount,
            vehicleResponseList
        );
    }
}
