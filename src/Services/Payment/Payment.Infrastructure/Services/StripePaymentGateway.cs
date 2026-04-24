using Microsoft.Extensions.Logging;
using Payment.Application.Infrastructure.Services;
using Payment.Application.Models;
using Stripe;

namespace Payment.Infrastructure.Services;

public class StripePaymentGateway : IPaymentGateway
{
    private readonly ILogger<StripePaymentGateway> _logger;

    public StripePaymentGateway(
        ILogger<StripePaymentGateway> logger)
    {
        _logger = logger;
    }

    public async Task ChargeAsync(string customerId, decimal amount)
    {
        var service = new PaymentIntentService();
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)amount,
            Currency = "usd",
            Customer = customerId,
            PaymentMethod = "pm_card_visa",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
            Confirm = true,
            OffSession = true,
        };
        try
        {
            await service.CreateAsync(options);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
    {
        var customerModel = new CustomerDto();

        try
        {
            var service = new CustomerService();
            var stripeCustomers = await service.ListAsync(new CustomerListOptions()
            {
                Email = email
            });

            if (!stripeCustomers.Any())
            {
                return null;
            }

            var stripeCustomer = stripeCustomers.First();

            customerModel.Id = stripeCustomer.Id;
            customerModel.Email = stripeCustomer.Email;
            customerModel.Name = stripeCustomer.Name;

        }
        catch (Exception)
        {
            throw;
        }

        return customerModel;
    }

    public async Task<bool> CreateCustomerAsync(string firstName, string lastName, string email)
    {
        _logger.LogInformation("Creating customer in stripe.");
        try
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = $"{firstName} {lastName}",
            };
            var service = new CustomerService();
            await service.CreateAsync(options);

            _logger.LogInformation("Customer created succesfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"An error occurred during customer creation, {ex}");
            return false;
        }
    }
}
