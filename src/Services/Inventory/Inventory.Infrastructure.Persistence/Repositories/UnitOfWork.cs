using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using Inventory.Infrastructure.Persistence.Data;
using Inventory.Infrastructure.Persistence.MediatR;
using Inventory.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly InventoryContext _context;
    private readonly IMediator _mediator;
    private IDbContextTransaction _currentTransaction;

    private IVehicleRepository _vehicleRepository = null!;
    private IBookingRepository _bookingRepository = null!;
    private IBrandRepository _brandRepository = null!;
    private IModelRepository _modelRepository = null!;
    private IVariantRepository _variantRepository = null!;

    public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction != null;

    public IVehicleRepository VehicleRepository
    {
        get
        {
            if (_vehicleRepository == null)
            {
                _vehicleRepository = new VehicleRepository(_context);
            }
            return _vehicleRepository;
        }
    }
    public IBookingRepository BookingRepository
    {
        get
        {
            if (_bookingRepository == null)
            {
                _bookingRepository = new BookingRepository(_context);
            }
            return _bookingRepository;
        }
    }
    public IBrandRepository BrandRepository
    {
        get
        {
            if (_brandRepository == null)
            {
                _brandRepository = new BrandRepository(_context);
            }
            return _brandRepository;
        }
    }
    public IModelRepository ModelRepository
    {
        get
        {
            if (_modelRepository == null)
            {
                _modelRepository = new ModelRepository(_context);
            }
            return _modelRepository;
        }
    }
    public IVariantRepository VariantRepository
    {
        get
        {
            if (_variantRepository == null)
            {
                _variantRepository = new VariantRepository(_context);
            }
            return _variantRepository;
        }
    }

    public UnitOfWork(
        InventoryContext context,
        IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEventsAsync(_context);

        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await _context.Database.BeginTransactionAsync();

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await _context.SaveChangesAsync();
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

    public void Dispose()
    {
        _context.Dispose();
    }
}
