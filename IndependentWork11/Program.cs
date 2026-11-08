using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork11
{
    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Генерація тестових даних з фіксованим Seed
            var random = new Random(42);

            var categories = new[] { "Electronics", "Clothing", "Home", "Books", "Sports" };
            var cities = new[] { "Rivne", "Kyiv", "Lviv", "Odessa", "Kharkiv" };

            // 1. Генерація продуктів (50 елементів)
            var products = Enumerable.Range(1, 50).Select(i => new Product
            {
                Id = i,
                Name = $"Product {i}",
                Category = categories[random.Next(categories.Length)],
                UnitPrice = Math.Round((decimal)(random.NextDouble() * 500 + 10), 2)
            }).ToList();

            // 2. Генерація клієнтів (50 елементів)
            var customers = Enumerable.Range(1, 50).Select(i => new Customer
            {
                Id = i,
                Name = $"Customer {i}",
                City = cities[random.Next(cities.Length)]
            }).ToList();

            // 3. Генерація продажів (100 елементів)
            var startDate = new DateTime(2024, 1, 1);
            var sales = Enumerable.Range(1, 100).Select(i => new Sale
            {
                Id = i,
                ProductId = random.Next(1, 51),
                Quantity = random.Next(1, 10),
                SaleDate = startDate.AddDays(random.Next(0, 365)),
                CustomerId = random.Next(1, 51)
            }).ToList();

            Console.WriteLine($"Згенеровано: Продуктів - {products.Count}, Клієнтів - {customers.Count}, Продажів - {sales.Count}\n");

            // Виконання та вимірювання продуктивності пайплайнів
            TopProductsByCategory(sales, products);
            RevenueByCustomerCity(sales, products, customers);
            SalesByYearAndMonth(sales, products);
        }

        /// <summary>
        /// Пайплайн 1: Найпопулярніші продукти за категоріями
        /// Використовує: GroupBy, Select, Join, OrderByDescending, Take
        /// </summary>
        public static void TopProductsByCategory(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { p.Name, p.Category, s.TotalQuantity })
                .GroupBy(x => x.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TopProducts = g.OrderByDescending(x => x.TotalQuantity).Take(3).ToList()
                })
                .ToList();

            sw.Stop();

            Console.WriteLine("=== Пайплайн 1: Найпопулярніші продукти за категоріями ===");
            foreach (var cat in result)
            {
                Console.WriteLine($"Категорія: {cat.Category}");
                foreach (var prod in cat.TopProducts)
                {
                    Console.WriteLine($"  - {prod.Name}: {prod.TotalQuantity} шт.");
                }
            }
            Console.WriteLine($"Час виконання: {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds} ms)\n");
        }

        /// <summary>
        /// Пайплайн 2: Загальний дохід за містами клієнтів
        /// Використовує: Join, GroupBy, Select, OrderByDescending
        /// </summary>
        public static void RevenueByCustomerCity(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
                .Join(customers, sp => sp.CustomerId, c => c.Id, (sp, c) => new { c.City, sp.Revenue })
                .GroupBy(x => x.City)
                .Select(g => new
                {
                    City = g.Key,
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();

            Console.WriteLine("=== Пайплайн 2: Загальний дохід за містами клієнтів ===");
            foreach (var item in result)
            {
                Console.WriteLine($"Місто: {item.City} | Дохід: {item.TotalRevenue:C}");
            }
            Console.WriteLine($"Час виконання: {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds} ms)\n");
        }

        /// <summary>
        /// Пайплайн 3: Продажі за місяцями та роками
        /// Використовує: Join, GroupBy, Select, OrderBy, ThenBy
        /// </summary>
        public static void SalesByYearAndMonth(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            sw.Stop();

            Console.WriteLine("=== Пайплайн 3: Продажі за місяцями та роками ===");
            foreach (var item in result)
            {
                Console.WriteLine($"Рік: {item.Year}, Місяць: {item.Month:D2} | Кількість: {item.TotalQuantity} шт. | Дохід: {item.TotalRevenue:C}");
            }
            Console.WriteLine($"Час виконання: {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds} ms)\n");
        }
    }
}
