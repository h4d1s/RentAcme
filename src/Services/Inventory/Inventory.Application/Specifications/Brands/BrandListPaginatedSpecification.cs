﻿using Inventory.Domain.AggregatesModel.BrandAggregate;
using MassTransit.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Specifications.Brands;

public class BrandListPaginatedSpecification : BaseSpecification<Brand>
{
    public BrandListPaginatedSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy)
        : base(null)
    {
        if (!string.IsNullOrEmpty(orderBy))
        {
            var ordering = "";
            switch (orderBy.ToLower())
            {
                case "name":
                    ordering = "Name";
                    break;
                default:
                    ordering = "Id";
                    break;
            }

            var orderLinq = "asc";
            if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
            {
                orderLinq = "desc";
            }

            ApplyOrderBy(ordering + " " + orderLinq);
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }
}