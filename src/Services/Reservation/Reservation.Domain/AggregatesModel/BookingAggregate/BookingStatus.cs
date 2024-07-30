using System.Text.Json.Serialization;

namespace Reservation.Domain.AggregatesModel.BookingAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatus
{
    Reserved = 0,
    Canceled = 1,
    Completed = 2
}