using System.Text.Json.Serialization;

namespace Inventory.Domain.AggregatesModel.BookingAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatus
{
    Reserved = 0,
    Avaliable = 1
}