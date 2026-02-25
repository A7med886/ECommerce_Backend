using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var electronicsCategoryId = new Guid("AB187206-4337-4739-BECF-925FFD2F35DB");
            var clothingCategoryId = new Guid("7B40D22F-538C-4FEB-99FE-D41B6ECE99CB");
            var booksCategoryId = new Guid("C6595E30-8926-4B8A-CA8C-08DE6E5B770C");
            var homeGardenCategoryId = new Guid("B730D5AB-D9C9-4BBE-CA8D-08DE6E5B770C");

            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                new Category { Id = electronicsCategoryId, Name = "Electronics", Description = "Electronic devices and gadgets" },
                new Category { Id = clothingCategoryId, Name = "Clothing", Description = "Fashion and apparel" },
                new Category { Id = booksCategoryId, Name = "Books", Description = "Books and publications" },
                new Category { Id = homeGardenCategoryId, Name = "Home & Garden", Description = "Home improvement and garden" }
            };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                var products = new[]
                {
                new Product { Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, Stock = 50, CategoryId = electronicsCategoryId },
                new Product { Name = "Smartphone", Description = "Latest smartphone", Price = 699.99m, Stock = 100, CategoryId = electronicsCategoryId },
                new Product { Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, Stock = 200, CategoryId = clothingCategoryId },
                new Product { Name = "Jeans", Description = "Denim jeans", Price = 49.99m, Stock = 150, CategoryId = clothingCategoryId }
            };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var users = new[]
                {
                new User
                {
                    Email = "admin@ecommerce.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    FirstName = "Admin",
                    LastName = "User",
                    Role = UserRole.Admin
                },
                new User
                {
                    Email = "customer@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                    FirstName = "Ahmed",
                    LastName = "Tarek",
                    Role = UserRole.Customer
                }
            };
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }
        }
    }
}
