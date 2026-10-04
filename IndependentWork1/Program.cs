using System;

namespace IndependentWork1
{
    // Клас 1: Працівник (Employee)
    public class Employee
    {
        private string _name;
        private double _salary;

        public string Name
        {
            get => _name;
            set => _name = !string.IsNullOrWhiteSpace(value) ? value : "Невідомо";
        }

        public double Salary
        {
            get => _salary;
            set => _salary = value >= 0 ? value : 0.0;
        }

        public Employee(string name, double salary)
        {
            Name = name;
            Salary = salary;
        }

        public double CalculateAnnualIncome(double bonusPercentage)
        {
            return (_salary * 12) * (1 + bonusPercentage);
        }
    }

    // Клас 2: Прямокутник (Rectangle)
    public class Rectangle
    {
        private double _width;
        private double _height;

        public double Width
        {
            get => _width;
            set => _width = value > 0 ? value : 1.0;
        }

        public double Height
        {
            get => _height;
            set => _height = value > 0 ? value : 1.0;
        }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double GetArea()
        {
            return _width * _height;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №1 ===");

            Employee emp = new Employee("Олександр", 25000);
            double annualIncome = emp.CalculateAnnualIncome(0.15);
            Console.WriteLine($"Працівник: {emp.Name}, Річний дохід з премією: {annualIncome} грн");

            Rectangle rect = new Rectangle(5.5, 10.0);
            Console.WriteLine($"Прямокутник (Ширина: {rect.Width}, Висота: {rect.Height})");
            Console.WriteLine($"Площа прямокутника: {rect.GetArea()}");

            Console.WriteLine("============================");
        }
    }
}
