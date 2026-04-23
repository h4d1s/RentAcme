using System;
using System.Collections.Generic;
using System.Text;

namespace User.Domain.Exceptions;

public class ApplicationUserDomainException : Exception
{
    public ApplicationUserDomainException()
    { }

    public ApplicationUserDomainException(string message)
        : base(message)
    { }

    public ApplicationUserDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}