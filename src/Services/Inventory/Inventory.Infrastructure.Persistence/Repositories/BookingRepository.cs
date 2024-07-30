using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(InventoryContext context) : base(context)
    {
    }
}
