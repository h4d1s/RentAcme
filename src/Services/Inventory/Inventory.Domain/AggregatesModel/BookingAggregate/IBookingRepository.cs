using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.AggregatesModel.BookingAggregate;

public interface IBookingRepository : IRepository<Booking>
{
    public Task<IReadOnlyList<Booking>> ListAsync(Specification<Booking> spec);
    public Task<int> CountAsync(Specification<Booking> spec);
}
