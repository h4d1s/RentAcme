using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;
using User.Domain.Common;

namespace User.Domain.AggregatesModel.ApplicationUserAggregate;

public class Address : ValueObject
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }

    public Address(string street, string city, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new ArgumentException("Street cannot be empty");
        }
        Street = street;

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty");
        }
        City = city;

        if (postalCode.Length < 5)
        {
            throw new ArgumentException("Invalid Postal Code");
        }
        PostalCode = postalCode;

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Country cannot be empty");
        }
        Country = country;
    }
}
