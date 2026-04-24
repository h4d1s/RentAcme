using Inventory.Domain.AggregatesModel.ModelAggregate;
using MediatR;

namespace Inventory.Application.Features.Models.Commands.CreateModel;

public record CreateModelCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int YearOfProduction { get; set; }
    public int NumberOfSeats { get; set; }
    public Category Category { get; set; }
    public Guid BrandId { get; set; }
}
