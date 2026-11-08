using System;
using System.Collections.Generic;

namespace OOP_Shynkaruk
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            T maxItem = default(T);
            TKey maxKey = default(TKey);
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }
            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №15: Власні утиліти для колекцій ===\n");

            var products = new List<Product>
            {
                new Product { Name = "Laptop", Price = 25000m },
                new Product { Name = "Mouse", Price = 500m },
                new Product { Name = "Keyboard", Price = 1500m }
            };

            var names = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Names: " + string.Join(", ", names));

            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Most expensive: {mostExpensive.Name} - {mostExpensive.Price} грн");

            var expensive = CollectionUtils.Filter(products, p => p.Price > 1000);
            Console.WriteLine("Expensive products:");
            CollectionUtils.ForEach(expensive, p => Console.WriteLine($" - {p.Name}"));

            Console.WriteLine("\nAll products:");
            CollectionUtils.ForEach(products, p => Console.WriteLine($"{p.Name}: {p.Price} грн"));
        }
    }
}
