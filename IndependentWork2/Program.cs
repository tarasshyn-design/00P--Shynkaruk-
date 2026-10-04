using System;
using System.Collections.Generic;

namespace IndependentWork2
{
    public class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }

        public Product(string name, double price)
        {
            Name = name;
            Price = price;
        }
    }

    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    public class Cart
    {
        private List<CartItem> _items = new List<CartItem>();

        public void AddItem(Product product, int quantity)
        {
            _items.Add(new CartItem(product, quantity));
        }

        public double GetTotal()
        {
            double total = 0;
            foreach (var item in _items)
            {
                double itemSum = item.Product.Price * item.Quantity;
                
                if (item.Product.Price > 500)
                {
                    itemSum *= 0.9; 
                }

                total += itemSum;
            }
            return total;
        }
    }

    public class ProceduralShop
    {
        public static double CalculateProceduralTotal(string[] names, double[] prices, int[] quantities)
        {
            double total = 0;
            for (int i = 0; i < names.Length; i++)
            {
                double itemSum = prices[i] * quantities[i];
                
                if (prices[i] > 500)
                {
                    itemSum *= 0.9;
                }

                total += itemSum;
            }
            return total;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ПРОЦЕДУРНИЙ ПІДХІД ===");
            string[] procNames = { "Мишка", "Монітор", "Клавіатура" };
            double[] procPrices = { 300, 4500, 1200 };
            int[] procQuantities = { 2, 1, 1 };

            double proceduralTotal = ProceduralShop.CalculateProceduralTotal(procNames, procPrices, procQuantities);
            Console.WriteLine($"Підсумок кошика (процедурний): {proceduralTotal} грн\n");


            Console.WriteLine("=== ОБ'ЄКТНО-ОРІЄНТОВАНИЙ ПІДХІД ===");
            Cart cart = new Cart();
            cart.AddItem(new Product("Мишка", 300), 2);
            cart.AddItem(new Product("Монітор", 4500), 1);
            cart.AddItem(new Product("Клавіатура", 1200), 1);

            double ooTotal = cart.GetTotal();
            Console.WriteLine($"Підсумок кошика (об'єктний): {ooTotal} грн");
        }
    }
}
