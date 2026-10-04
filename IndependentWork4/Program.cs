using System;

namespace IndependentWork4
{
    public class Product
    {
        private int _id;
        private string _name;
        private decimal _price;
        private string _category;
        private int _stockCount;

        // Публічні властивості з read-only доступом (тільки get)
        public int Id => _id;
        public string Name => _name;
        public decimal Price => _price;
        public string Category => _category;
        public int StockCount => _stockCount;

        // Конструктор 1 (основний): приймає всі п'ять параметрів
        public Product(int id, string name, decimal price, string category, int stockCount)
        {
            _id = id;
            _name = name;
            _price = price;
            _category = category;
            _stockCount = stockCount;
        }

        // Конструктор 2 (для швидкого створення товару): делегує виклик основному через this(...)
        public Product(int id, string name, decimal price) 
            : this(id, name, price, "Uncategorized", 0)
        {
        }

        // Конструктор 3 (копіювання): приймає інший об'єкт Product та викликає основний конструктор через this(...)
        public Product(Product other) 
            : this(other._id, other._name, other._price, other._category, other._stockCount)
        {
        }

        // Перевизначення методу ToString()
        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Price: {Price:C}, Category: {Category}, Stock: {StockCount}";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Демонстрація роботи в Main
            Product product1 = new Product(101, "Laptop", 35000.00m, "Electronics", 5);
            Product product2 = new Product(102, "Mouse", 800.00m);
            Product product3 = new Product(product1);

            // Виведення інформації у консоль
            Console.WriteLine($"Товар 1 (основний конструктор): {product1}");
            Console.WriteLine($"Товар 2 (скорочений конструктор): {product2}");
            Console.WriteLine($"Товар 3 (конструктор копіювання): {product3}");
        }
    }
}
