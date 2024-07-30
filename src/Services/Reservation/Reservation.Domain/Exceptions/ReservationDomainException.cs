using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Domain.Exceptions;

public class ReservationDomainException : Exception
{
    public ReservationDomainException()
    { }

    public ReservationDomainException(string message)
        : base(message)
    { }

    public ReservationDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
