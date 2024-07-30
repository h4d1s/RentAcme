﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Exceptions;

public class InventoryDomainException : Exception
{
    public InventoryDomainException()
    { }

    public InventoryDomainException(string message)
        : base(message)
    { }

    public InventoryDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
