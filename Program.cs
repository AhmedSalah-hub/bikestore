using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using bikestore.Data;
using bikestore.Models;

class Program
{
    static void Main()
    {
        using var context = new ApplicationDbContext();

        // 1. List all customers' first and last names along with their email addresses.
        var customers = context.Customers
            .Select(c => new { c.FirstName, c.LastName, c.Email })
            .ToList();

        Console.WriteLine("1. Customers (First, Last, Email):");
        customers.ForEach(c => Console.WriteLine($"{c.FirstName} {c.LastName} - {c.Email}"));

        // 2. Retrieve all orders processed by a specific staff member (e.g., staff_id = 3).
        int staffId = 3;
        var ordersByStaff = context.Orders
            .Where(o => o.StaffId == staffId)
            .Include(o => o.Customer)
            .ToList();

        Console.WriteLine($"\n2. Orders processed by staff {staffId}:");
        ordersByStaff.ForEach(o => Console.WriteLine($"OrderId: {o.OrderId}, Customer: {o.Customer?.FirstName} {o.Customer?.LastName}, Status: {o.OrderStatus}"));

        // 3. Get all products that belong to a category named "Mountain Bikes".
        var mountainProducts = context.Products
            .Where(p => p.Category.CategoryName == "Mountain Bikes")
            .ToList();

        Console.WriteLine("\n3. Mountain Bikes Products:");
        mountainProducts.ForEach(p => Console.WriteLine($"{p.ProductId}: {p.ProductName}"));

        // 4. Count the total number of orders per store.
        var ordersPerStore = context.Orders
            .GroupBy(o => o.StoreId)
            .Select(g => new { StoreId = g.Key, Count = g.Count() })
            .ToList();

        Console.WriteLine("\n4. Orders per store:");
        ordersPerStore.ForEach(s => Console.WriteLine($"Store {s.StoreId}: {s.Count}"));

        // 5. List all orders that have not been shipped yet (shipped_date is null).
        var notShipped = context.Orders
            .Where(o => o.ShippedDate == null)
            .Include(o => o.Customer)
            .ToList();

        Console.WriteLine("\n5. Orders not shipped:");
        notShipped.ForEach(o => Console.WriteLine($"OrderId: {o.OrderId}, Customer: {o.Customer?.FirstName} {o.Customer?.LastName}"));

        // 6. Display each customer’s full name and the number of orders they have placed.
        var customerOrderCounts = context.Customers
            .Select(c => new { FullName = c.FirstName + " " + c.LastName, Orders = c.Orders.Count })
            .ToList();

        Console.WriteLine("\n6. Customer order counts:");
        customerOrderCounts.ForEach(c => Console.WriteLine($"{c.FullName}: {c.Orders}"));

        // 7. List all products that have never been ordered (not found in order_items).
        var neverOrdered = context.Products
            .Where(p => !p.OrderItems.Any())
            .ToList();

        Console.WriteLine("\n7. Products never ordered:");
        neverOrdered.ForEach(p => Console.WriteLine($"{p.ProductId}: {p.ProductName}"));

        // 8. Display products that have a quantity of less than 5 in any store stock.
        var lowStockProducts = context.Stocks
            .Where(s => s.Quantity.HasValue && s.Quantity < 5)
            .Select(s => s.Product)
            .Distinct()
            .ToList();

        Console.WriteLine("\n8. Products with stock < 5 in any store:");
        lowStockProducts.ForEach(p => Console.WriteLine($"{p.ProductId}: {p.ProductName}"));

        // 9. Retrieve the first product from the products table.
        var firstProduct = context.Products.FirstOrDefault();
        Console.WriteLine("\n9. First product:");
        if (firstProduct != null)
            Console.WriteLine($"{firstProduct.ProductId}: {firstProduct.ProductName}");

        // 10. Retrieve all products from the products table with a certain model year.
        short modelYear = 2020;
        var productsByYear = context.Products
            .Where(p => p.ModelYear == modelYear)
            .ToList();

        Console.WriteLine($"\n10. Products with model year {modelYear}:");
        productsByYear.ForEach(p => Console.WriteLine($"{p.ProductId}: {p.ProductName}"));

        // 11. Display each product with the number of times it was ordered.
        var productOrderCounts = context.Products
            .Select(p => new { p.ProductId, p.ProductName, TimesOrdered = p.OrderItems.Count })
            .ToList();

        Console.WriteLine("\n11. Product times ordered:");
        productOrderCounts.ForEach(p => Console.WriteLine($"{p.ProductId}: {p.ProductName} - {p.TimesOrdered}"));

        // 12. Count the number of products in a specific category.
        int categoryId = 1;
        var productsInCategory = context.Products.Count(p => p.CategoryId == categoryId);
        Console.WriteLine($"\n12. Number of products in category {categoryId}: {productsInCategory}");

        // 13. Calculate the average list price of products.
        var avgPrice = context.Products.Average(p => p.ListPrice);
        Console.WriteLine($"\n13. Average list price: {avgPrice:C}");

        // 14. Retrieve a specific product from the products table by ID.
        int productId = 1;
        var specificProduct = context.Products.Find(productId);
        Console.WriteLine("\n14. Specific product by ID:");
        if (specificProduct != null)
            Console.WriteLine($"{specificProduct.ProductId}: {specificProduct.ProductName}");

        // 15. List all products that were ordered with a quantity greater than 3 in any order.
        var productsQtyGt3 = context.OrderItems
            .Where(oi => oi.Quantity > 3)
            .Select(oi => oi.Product)
            .Distinct()
            .ToList();

        Console.WriteLine("\n15. Products ordered with quantity > 3:");
        productsQtyGt3.ForEach(p => Console.WriteLine($"{p.ProductId}: {p.ProductName}"));

        // 16. Display each staff member’s name and how many orders they processed.
        var staffOrderCounts = context.Staffs
            .Select(s => new { FullName = s.FirstName + " " + s.LastName, Orders = s.Orders.Count })
            .ToList();

        Console.WriteLine("\n16. Staff order counts:");
        staffOrderCounts.ForEach(s => Console.WriteLine($"{s.FullName}: {s.Orders}"));

        // 17. List active staff members only (active = true) along with their phone numbers.
        var activeStaff = context.Staffs
            .Where(s => s.Active == 1)
            .Select(s => new { s.FirstName, s.LastName, s.Phone })
            .ToList();

        Console.WriteLine("\n17. Active staff with phone:");
        activeStaff.ForEach(s => Console.WriteLine($"{s.FirstName} {s.LastName}: {s.Phone}"));

        // 18. List all products with their brand name and category name.
        var productsWithBrandCategory = context.Products
            .Select(p => new { p.ProductId, p.ProductName, Brand = p.Brand.BrandName, Category = p.Category.CategoryName })
            .ToList();

        Console.WriteLine("\n18. Products with brand and category:");
        productsWithBrandCategory.ForEach(p => Console.WriteLine($"{p.ProductId}: {p.ProductName} - {p.Brand} / {p.Category}"));

        // 19. Retrieve orders that are completed.
        // Assuming order_status == 5 indicates 'completed' (adjust as needed)
        var completedOrders = context.Orders
            .Where(o => o.OrderStatus == 5)
            .ToList();

        Console.WriteLine("\n19. Completed orders:");
        completedOrders.ForEach(o => Console.WriteLine($"OrderId: {o.OrderId}, Status: {o.OrderStatus}"));

        // 20. List each product with the total quantity sold (sum of quantity from order_items).
        var totalQuantityPerProduct = context.Products
            .Select(p => new { p.ProductId, p.ProductName, TotalSold = p.OrderItems.Sum(oi => (int?)oi.Quantity) ?? 0 })
            .ToList();

        Console.WriteLine("\n20. Total quantity sold per product:");
        totalQuantityPerProduct.ForEach(p => Console.WriteLine($"{p.ProductId}: {p.ProductName} - {p.TotalSold}"));
    }
}