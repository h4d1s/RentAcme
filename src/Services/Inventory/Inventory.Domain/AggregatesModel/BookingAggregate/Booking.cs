using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.AggregatesModel.BookingAggregate;

public class Booking
    : Entity, IAggregateRoot
{
    public BookingStatus Status { get; private set; }
    public DateTime? PickupDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    public Guid VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; } = null!;

    public Booking(
        BookingStatus status,
        Guid vehicleId)
    {
        Status = status;
        VehicleId = vehicleId;
    }

    public void SetPickupDate(DateTime? pickupDate)
    {
        if (Status != BookingStatus.Reserved)
        {
            throw new InventoryDomainException("Cannot set pickup date for a reservation that is not reserved.");
        }

        PickupDate = pickupDate;
    }

    public void SetReturnDate(DateTime? returnDate)
    {
        if (Status != BookingStatus.Reserved)
        {
            throw new InventoryDomainException("Cannot set pickup date for a reservation that is not reserved.");
        }

        ReturnDate = returnDate;
    }

    public void SetReservedStatus()
    {
        if (Status == BookingStatus.Reserved)
        {
            return;
        }

        Status = BookingStatus.Reserved;
    }

    public void SetAvaliableStatus()
    {
        if (Status == BookingStatus.Avaliable)
        {
            return;
        }

        Status = BookingStatus.Avaliable;
    }
}
