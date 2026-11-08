using System;
using System.Collections.Generic;

namespace OOP_ВашеПрізвище
{
    // Допоміжний клас для демонстрації
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }

    // Клас-утиліта згідно з варіантом 19
    public static class CollectionHelper<T>
    {
        public static double Average(IEnumerable<T> collection, Func<T, double> selector)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            double sum = 0;
            int count = 0;

            foreach (var item in collection)
            {
                sum += selector(item);
                count++;
            }

            if (count == 0)
            {
                throw new InvalidOperationException("Колекція не містить елементів для обчислення середнього.");
            }

            return sum / count;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №8 (Варіант 19) ===\n");

            // 1. Демонстрація для List<Product>
            List<Product> products = new List<Product>
            {
                new Product("Ноутбук", 25000.0),
                new Product("Мишка", 800.0),
                new Product("Клавіатура", 1200.0)
            };

            double avgPrice = CollectionHelper<Product>.Average(products, p => (double)p.Price);
            Console.WriteLine($"1. Середня ціна продуктів: {avgPrice} грн");

            // 2. Демонстрація для List<string>
            List<string> words = new List<string> { "Програмування", "Csharp", "Делегати", "Лямбда" };

            double avgLength = CollectionHelper<string>.Average(words, s => s.Length);
            Console.WriteLine($"2. Середня довжина рядка: {avgLength} символів");
        }
    }
}
