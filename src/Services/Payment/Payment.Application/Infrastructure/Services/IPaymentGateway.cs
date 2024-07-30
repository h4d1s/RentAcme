using Payment.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Infrastructure.Services;

public interface IPaymentGateway
{
    public Task ChargeAsync(string customerId, decimal amount);
    public Task<CustomerDto> GetCustomerByEmailAsync(string email);
    public Task<bool> CreateCustomerAsync(string firstName, string lastName, string email);
}
