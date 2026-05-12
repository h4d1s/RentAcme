using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using Inventory.Infrastructure.Persistence.EntityConfigurations;
using Inventory.Infrastructure.Persistence.MediatR;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inventory.Infrastructure.Persistence.Data;

public class InventoryDbContext : DbContext, IUnitOfWork
{
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<Variant> Variants { get; set; }

    private readonly IMediator _mediator;

    private IDbContextTransaction? _currentTransaction;
    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction != null;

    public InventoryDbContext(
        DbContextOptions<InventoryDbContext> options,
        IMediator mediator)
        : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VehicleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BrandEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ModelEntityConfiguration());
        modelBuilder.ApplyConfiguration(new VariantEntityConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEventsAsync(this);

        return await base.SaveChangesAsync();
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEventsAsync(this);

        _ = await base.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await Database.BeginTransactionAsync();

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
