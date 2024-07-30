using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.Exceptions;

public class AuthenticationException : Exception
{
    public AuthenticationException() : base("Unauthorized") { }

    public AuthenticationException(string message) : base(message) { }

    public AuthenticationException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
