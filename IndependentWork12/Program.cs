using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    class Program
    {
        static void Main(string[] args)
        {
            // Тестування продуктивності на різних обсягах даних
            int[] sizes = { 1_000_000, 10_000_000 };

            foreach (var size in sizes)
            {
                Console.WriteLine($"=== Розмір колекції: {size:N0} елементів ===");
                var data = GenerateData(size);

                // Звичайний LINQ
                var sw = Stopwatch.StartNew();
                var linqResult = data
                    .Where(x => IsPrime(x))
                    .Select(x => Math.Sqrt(x))
                    .ToList();
                sw.Stop();
                Console.WriteLine($"LINQ час:   {sw.ElapsedMilliseconds} ms (Знайдено: {linqResult.Count})");

                // PLINQ
                sw.Restart();
                var plinqResult = data
                    .AsParallel()
                    .Where(x => IsPrime(x))
                    .Select(x => Math.Sqrt(x))
                    .ToList();
                sw.Stop();
                Console.WriteLine($"PLINQ час:  {sw.ElapsedMilliseconds} ms (Знайдено: {plinqResult.Count})\n");
            }

            // Дослідження побічних ефектів та потокобезпеки
            DemonstrateSideEffectsAndFix();
        }

        /// <summary>
        /// Генерація псевдовипадкових чисел
        /// </summary>
        static List<int> GenerateData(int count)
        {
            var rand = new Random(42);
            var list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(rand.Next(1, 100_000));
            }
            return list;
        }

        /// <summary>
        /// Обчислювально інтенсивна операція: перевірка на просте число
        /// </summary>
        static bool IsPrime(int number)
        {
            if (number < 2) return false;
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        /// <summary>
        /// Демонстрація побічних ефектів у PLINQ та їх вирішення за допомогою lock
        /// </summary>
        static void DemonstrateSideEffectsAndFix()
        {
            Console.WriteLine("=== Дослідження побічних ефектів (Race Condition) ===");
            var smallData = Enumerable.Range(1, 100_000).ToList();

            // 1. Некоректний підхід (зміна спільної змінної без синхронізації)
            int unsafeCounter = 0;
            smallData.AsParallel().ForAll(x =>
            {
                if (IsPrime(x))
                {
                    unsafeCounter++; // Гонка потоків призводить до заниженого результату
                }
            });
            Console.WriteLine($"Змінна з гонкою потоків (Unsafe Counter): {unsafeCounter}");

            // 2. Коректний підхід з використанням lock
            int safeCounter = 0;
            object syncLock = new object();
            smallData.AsParallel().ForAll(x =>
            {
                if (IsPrime(x))
                {
                    lock (syncLock)
                    {
                        safeCounter++;
                    }
                }
            });
            Console.WriteLine($"Коректний лічильник через lock (Safe Counter): {safeCounter}\n");
        }
    }
}
