using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Models;

public static class Permissions
{
    public static class Bookings
    {
        public const string Reserve = "booking.reserve";
        public const string Cancel = "booking.cancel";
        public const string Complete = "booking.complete";
        public const string ViewOwn = "booking.view.own";
        public const string ViewAny = "booking.view.any";
    }

    public static class Brands
    {
        public const string Create = "brand.create";
        public const string UpdateOwn = "brand.update.own";
        public const string UpdateAny = "brand.update.any";
        public const string DeleteOwn = "brand.delete.own";
        public const string DeleteAny = "brand.delete.any";
    }

    public static class Models
    {
        public const string Create = "model.create";
        public const string UpdateOwn = "model.update.own";
        public const string UpdateAny = "model.update.any";
        public const string DeleteOwn = "model.delete.own";
        public const string DeleteAny = "model.delete.any";
    }

    public static class Variants
    {
        public const string Create = "variant.create";
        public const string UpdateOwn = "variant.update.own";
        public const string UpdateAny = "variant.update.any";
        public const string DeleteOwn = "variant.delete.own";
        public const string DeleteAny = "variant.delete.any";
    }

    public static class Vehicles
    {
        public const string Create = "vehicle.create";
        public const string UpdateOwn = "vehicle.update.own";
        public const string UpdateAny = "vehicle.update.any";
        public const string DeleteOwn = "vehicle.delete.own";
        public const string DeleteAny = "vehicle.delete.any";
    }

    public static class Users
    {
        public const string ViewOwn = "user.view.own";
        public const string ViewAny = "user.view.any";
        public const string UpdateOwn = "user.update.own";
        public const string UpdateAny = "user.update.any";
        public const string DeleteOwn = "user.delete.own";
        public const string DeleteAny = "user.delete.any";
    }
}