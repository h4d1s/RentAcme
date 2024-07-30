using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using System.Drawing;
using System.Text.Json;

namespace Inventory.Application.Infrastructure.Persistence;

public class InventoryContextSeed : IDbSeeder<InventoryContext>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<InventoryContextSeed> _logger;

    public InventoryContextSeed(
        IUnitOfWork unitOfWork,
        IWebHostEnvironment env,
        ILogger<InventoryContextSeed> logger
    )
    {
        _unitOfWork = unitOfWork;
        _env = env;
        _logger = logger;
    }

    public async Task SeedAsync(InventoryContext context)
    {
        if (
            await _unitOfWork.VehicleRepository.AnyAsync() &&
            await _unitOfWork.VariantRepository.AnyAsync() &&
            await _unitOfWork.ModelRepository.AnyAsync() &&
            await _unitOfWork.BrandRepository.AnyAsync() &&
            await _unitOfWork.BookingRepository.AnyAsync()
        )
        {
            return;   // DB has been seeded
        }

        try
        {
            var contentRootPath = _env.ContentRootPath;
            var sourcePath = Path.Combine(contentRootPath, "Setup", "brands.json");
            var sourceJson = File.ReadAllText(sourcePath);
            var sourceBrands = JsonSerializer.Deserialize<BrandSourceEntry[]>(sourceJson);
            var random = new Random();

            var variants = new List<Variant>();
            foreach (var brandSource in sourceBrands)
            {
                var brand = new Brand(brandSource.Name);
                await _unitOfWork.BrandRepository.AddAsync(brand);

                foreach (var modelSource in brandSource.Models)
                {
                    var randomYearOfProduction = random.Next(DateTime.Now.Year - 5, DateTime.Now.Year + 1);
                    var randomNumberOfSeats = random.Next(2, 7);

                    var categories = Enum.GetValues(typeof(Category));
                    var randomCategory = (Category)categories?.GetValue(random.Next(categories.Length));

                    var model = new Model(
                        modelSource.Name,
                        randomYearOfProduction,
                        randomNumberOfSeats,
                        randomCategory,
                        brand.Id);

                    foreach (var variantSource in modelSource.Variants)
                    {
                        var gearboxes = Enum.GetValues(typeof(Gearbox));
                        var randomGearbox = (Gearbox)gearboxes?.GetValue(random.Next(gearboxes.Length));

                        var fuelTypes = Enum.GetValues(typeof(FuelType));
                        var randomFuelType = (FuelType)fuelTypes?.GetValue(random.Next(fuelTypes.Length));

                        var randomPower = random.Next(44, 300);
                        var randomEngineSize = random.Next(1000, 4000);

                        model.AddVariant(variantSource.Name, randomGearbox, randomFuelType, randomPower, randomEngineSize);
                    }

                    await _unitOfWork.ModelRepository.AddAsync(model);
                    variants.AddRange(model.Variants);
                }
            }

            var vehicles = new List<Vehicle>();
            variants.ForEach(v =>
            {
                var randomRentalPricePerDay = random.NextDouble() * (500.0 - 10.0) + 10.0;
                var pickupDate = DateTime.UtcNow.AddDays(-random.Next(5, 10));
                var returnDate = DateTime.UtcNow.AddDays(-random.Next(1, 5));
                var plates = GenerateRandomRegistrationPlates();
                var vehicle = new Vehicle((decimal)randomRentalPricePerDay, plates, v.Id);
                vehicle.AddBooking(BookingStatus.Reserved, pickupDate, returnDate);
                vehicles.Add(vehicle);
            });

            await _unitOfWork.VehicleRepository.AddRangeAsync(vehicles);
            await _unitOfWork.SaveEntitiesAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private string GenerateRandomRegistrationPlates()
    {
        var rand = new Random();
        var stringlen = 8;
        var randValue = 0;
        var plates = "";
        char letter;
        for (int i = 0; i < stringlen; i++)
        {
            randValue = rand.Next(0, 26);
            letter = Convert.ToChar(randValue + 65);
            plates += letter;
        }
        return plates;
    }

    private class VariantSourceEntry
    {
        public string Name { get; set; } = string.Empty;
    }
    private class ModelSourceEntry
    {
        public string Name { get; set; } = string.Empty;
        public List<VariantSourceEntry> Variants { get; set; } = null!;
        public string Category { get; set; } = string.Empty;
    }
    private class BrandSourceEntry
    {
        public string Name { get; set; } = string.Empty;
        public List<ModelSourceEntry> Models { get; set; } = null!;
    }
}
