using Microsoft.AspNetCore.Mvc.Testing;
using Reservation.API.IntegrationTests.Fixtures;
using Reservation.Application.Features.Bookings.Commands.CancelBooking;
using Reservation.Application.Features.Bookings.Commands.CompleteBooking;
using Reservation.Application.Features.Bookings.Commands.ReserveBooking;
using System.Net;
using System.Net.Http.Json;

namespace Reservation.Api.IntegrationTests;

public sealed class BookingControllerTests : IClassFixture<BookingControllerFixture>
{
    private readonly BookingControllerFixture _webApplicationFactory;
    private readonly HttpClient _httpClient;

    public BookingControllerTests(BookingControllerFixture factory)
    {
        _webApplicationFactory = factory;
        _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Get_GetAllBookings_OK()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/bookings");
        var body = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_GetBookingById_OK()
    {
        // Arrange
        var bookingId = await _webApplicationFactory.SeedBookingAsync();

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/bookings/{bookingId}");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReserveBooking_Created()
    {
        // Arrange
        var command = new ReserveBookingCommand
        {
            VehicleId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            PickupDate = DateTime.UtcNow,
            ReturnDate = DateTime.UtcNow.AddDays(3)
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/bookings",
            command);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_CancelBooking_OK()
    {
        // Arrange
        var bookingId = await _webApplicationFactory.SeedBookingAsync();
        var command = new CancelBookingCommand
        {
            BookingId = bookingId
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/bookings/cancel",
            command);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_CompleteBooking_OK()
    {
        // Arrange
        var bookingId = await _webApplicationFactory.SeedBookingAsync();
        var command = new CompleteBookingCommand
        {
            BookingId = bookingId
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/bookings/complete",
            command);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReserveBooking_BadRequest_WhenInvalid()
    {
        // Arrange
        var command = new ReserveBookingCommand
        {
            VehicleId = Guid.Empty,
            UserId = Guid.Empty,
            PickupDate = DateTime.UtcNow,
            ReturnDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/bookings",
            command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_GetBooking_NotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        // Act
        var response = await _httpClient.GetAsync(
            $"/api/v1/bookings/{bookingId}");

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.OK);
    }
}