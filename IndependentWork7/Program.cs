using System;

namespace IndependentWork7
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Демонстрація роботи Cache<TKey, TValue> (FIFO) ===\n");

            var cache = new Cache<int, string>(3);

            cache.Add(1, "Один");
            cache.Add(2, "Два");
            cache.Add(3, "Три");

            Console.WriteLine($"Додано 3 елементи. Кількість у кеші: {cache.Count}");
            Console.WriteLine($"Містить ключ 1: {cache.ContainsKey(1)}");

            cache.Add(4, "Чотири");

            Console.WriteLine($"\nКількість у кеші після переповнення: {cache.Count}");
            Console.WriteLine($"Містить ключ 1 (старий): {cache.ContainsKey(1)}");
            Console.WriteLine($"Містить ключ 4 (новий): {cache.ContainsKey(4)}");

            try
            {
                Console.WriteLine($"Спроба отримати видалений ключ 1: {cache.Get(1)}");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Очікувана помилка: {ex.Message}");
            }
        }
    }
}
