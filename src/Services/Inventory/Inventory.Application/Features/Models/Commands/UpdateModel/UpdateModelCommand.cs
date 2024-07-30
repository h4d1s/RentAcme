using Inventory.Domain.AggregatesModel.ModelAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Commands.UpdateModel;

public record UpdateModelCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int YearOfProduction { get; set; }
    public int NumberOfSeats { get; set; }
    public Category Category { get; set; }
    public Guid BrandId { get; set; }
}
