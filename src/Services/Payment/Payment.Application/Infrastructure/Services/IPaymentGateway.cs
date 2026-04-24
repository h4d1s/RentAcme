using Payment.Application.Models;

namespace Payment.Application.Infrastructure.Services;

public interface IPaymentGateway
{
    public Task ChargeAsync(string customerId, decimal amount);
    public Task<CustomerDto?> GetCustomerByEmailAsync(string email);
    public Task<bool> CreateCustomerAsync(string firstName, string lastName, string email);
}
