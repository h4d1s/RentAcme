using Common.Models;
using Inventory.Application.Models;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Vehicles.Queries.SearchVehicles;

public class SearchVehiclesQuery : IRequest<PagedResponse<VehicleSearchResponse>>
{
    public Gearbox? Gearbox { get; set; }
    public DateTime? PickupDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public decimal? RentalPricePerDayFrom { get; set; }
    public decimal? RentalPricePerDayTo { get; set; }
    public Category? Category { get; set; }

    public string OrderBy { get; set; } = string.Empty;
    public string Order { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
