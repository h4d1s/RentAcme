using MassTransit;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SagaOrchestrationStateMachine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
