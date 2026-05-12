using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagaOrchestrationStateMachine.States;

namespace SagaOrchestrationStateMachine.StateMaps;

public class StateMachineMap : SagaClassMap<BookingState>
{
    protected override void Configure(EntityTypeBuilder<BookingState> entity, ModelBuilder model)
    {
        entity
            .HasKey(s => s.CorrelationId);
        entity
            .Property(b => b.Price)
            .HasColumnType("decimal(10, 2)")
            .HasPrecision(10, 2);
        entity.Property(x => x.CurrentState)
            .HasMaxLength(128);
    }
}
