﻿using Inventory.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.AggregatesModel.BrandAggregate;

public interface IBrandRepository : IRepository<Brand>
{
}