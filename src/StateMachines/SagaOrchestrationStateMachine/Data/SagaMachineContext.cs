using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using SagaOrchestrationStateMachine.StateMaps;

namespace SagaOrchestrationStateMachine.Data;

public class SagaMachineContext : SagaDbContext
{
    public SagaMachineContext(DbContextOptions<SagaMachineContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new StateMachineMap(); }
    }
}
