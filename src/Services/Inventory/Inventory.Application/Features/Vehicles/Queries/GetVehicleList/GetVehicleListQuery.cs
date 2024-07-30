using Common.Models;
using Inventory.Application.Models;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Vehicles.Queries.GetVehicleList;

public record GetVehicleListQuery : IRequest<PagedResponse<VehicleResponse>>
{
    public Gearbox? Gearbox { get; set; }
    public int? YearOfProduction { get; set; }
    public int? Power { get; set; }
    public int? EngineSize { get; set; }
    public DateTime? PickupDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public decimal? RentalPricePerDayFrom { get; set; }
    public decimal? RentalPricePerDayTo { get; set; }
    public Guid? ModelId { get; set; }

    public string orderBy { get; set; } = string.Empty;
    public string order { get; set; } = string.Empty;
    public int page { get; set; } = 1;
    public int pageSize { get; set; } = 10;
}
