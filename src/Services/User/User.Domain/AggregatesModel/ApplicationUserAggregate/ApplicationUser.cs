using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using User.Domain.AggregatesModel.ApplicationUserAggregate;
using User.Domain.AggregatesModel.ApplicationUserAggregate.Events;
using User.Domain.Common;
using User.Domain.Exceptions;

namespace User.Domain.AggregatesModel.ApplicationUserAggregate;

public class ApplicationUser
    : Entity, IAggregateRoot
{
    public string ExternalId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;

    public Address? Address { get; private set; }

    public ApplicationUser(
        string externalId,
        string email,
        string userName,
        string firstName,
        string lastName,
        string phoneNumber)
    {
        ValidateExternalId(externalId);
        ValidateEmail(email);
        ValidateUserName(userName);
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
        ValidatePhoneNumber(phoneNumber);

        ExternalId = externalId;
        Email = email;
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;

        AddDomainEvent(new ApplicationUserCreatedDomainEvent(this));
    }

    public void UpdateEmail(string email)
    {
        ValidateEmail(email);

        this.Email = email;
    }

    public void UpdateUserName(string userName)
    {
        ValidateUserName(userName);

        this.UserName = userName;
    }

    public void UpdateFirstName(string firstName)
    {
        ValidateFirstName(firstName);

        this.FirstName = firstName;
    }

    public void UpdateLastName(string lastName)
    {
        ValidateFirstName(lastName);

        this.LastName = lastName;
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        ValidatePhoneNumber(phoneNumber);
        
        this.PhoneNumber = phoneNumber;
    }

    public void UpdateAddress(string street, string city, string postalCode, string country)
    {
        if (string.IsNullOrEmpty(street))
        {
            throw new ApplicationUserDomainException($"{nameof(street)} must not be empty.");
        }
        if (string.IsNullOrEmpty(city))
        {
            throw new ApplicationUserDomainException($"{nameof(city)} must not be empty.");
        }
        if (string.IsNullOrEmpty(postalCode))
        {
            throw new ApplicationUserDomainException($"{nameof(postalCode)} must not be empty.");
        }
        if (string.IsNullOrEmpty(country))
        {
            throw new ApplicationUserDomainException($"{nameof(country)} must not be empty.");
        }
        Address = new Address(street, city, postalCode, country);
    }

    private void ValidateExternalId(string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
        {
            throw new ApplicationUserDomainException($"{nameof(externalId)} must not be empty.");
        }
    }

    private void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ApplicationUserDomainException($"{nameof(email)} must not be empty.");
        }

        var emailChecker = new EmailAddressAttribute();

        if (!emailChecker.IsValid(email))
        {
            throw new ApplicationUserDomainException($"{nameof(email)} is not a valid email address.");
        }
    }

    private void ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ApplicationUserDomainException($"{nameof(userName)} must not be empty.");
        }
    }

    private void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ApplicationUserDomainException($"{nameof(firstName)} must not be empty.");
        }
    }

    private void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ApplicationUserDomainException($"{nameof(lastName)} must not be empty.");
        }
    }

    private void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ApplicationUserDomainException($"{nameof(phoneNumber)} must not be empty.");
        }
    }
}
