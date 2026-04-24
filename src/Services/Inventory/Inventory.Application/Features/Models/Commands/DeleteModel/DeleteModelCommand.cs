using MediatR;

namespace Inventory.Application.Features.Models.Commands.DeleteModel;

public record DeleteModelCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
