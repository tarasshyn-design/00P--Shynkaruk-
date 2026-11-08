using System;
using System.Collections.Generic;

namespace OOP_Shynkaruk
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. HashSet<T>: Унікальні типи звітів
            HashSet<string> reportTypes = new HashSet<string> { "Фінансовий", "Продажі", "Склад" };
            reportTypes.Add("Аналітика");
            reportTypes.Add("Фінансовий");

            Console.WriteLine($"Усі унікальні типи звітів (всього {reportTypes.Count}):");
            foreach (var type in reportTypes)
            {
                Console.WriteLine($"- {type}");
            }

            string searchType = "Продажі";
            Console.WriteLine($"Чи містить колекція тип '{searchType}': {reportTypes.Contains(searchType)}");
            
            reportTypes.Remove("Склад");
            Console.WriteLine($"Кількість типів після видалення: {reportTypes.Count}\n");

            // 2. Queue<T>: Черга звітів на генерацію (FIFO)
            Queue<string> reportQueue = new Queue<string>();
            reportQueue.Enqueue("Звіт №1: Фінансовий за місяць");
            reportQueue.Enqueue("Звіт №2: Аналітика продажу товарів");
            reportQueue.Enqueue("Звіт №3: Інвентаризація складу");

            Console.WriteLine($"Звітів у черзі: {reportQueue.Count}");
            Console.WriteLine($"Наступний у черзі (Peek): {reportQueue.Peek()}");

            Console.WriteLine("\nГенерація звітів:");
            while (reportQueue.Count > 0)
            {
                Console.WriteLine($"[Генерація] Згенеровано: {reportQueue.Dequeue()}");
            }
            Console.WriteLine();

            // 3. Stack<T>: Історія згенерованих звітів (LIFO)
            Stack<string> reportHistory = new Stack<string>();
            reportHistory.Push("Фінансовий за місяць");
            reportHistory.Push("Аналітика продажу товарів");
            reportHistory.Push("Інвентаризація складу");

            Console.WriteLine($"Записів в історії: {reportHistory.Count}");
            Console.WriteLine($"Останній доданий (Peek): {reportHistory.Peek()}");

            Console.WriteLine("\nПерегляд історії (LIFO):");
            while (reportHistory.Count > 0)
            {
                Console.WriteLine($"[Історія] Відкрито звіт: {reportHistory.Pop()}");
            }
        }
    }
}
