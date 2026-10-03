using System;

namespace OOP_Shynkaruk.lab2v3
{
    public class Student
    {
        private string _name;
        private string _studentId;
        private double _averageMark;

        public string Name
        {
            get => _name;
            set => _name = !string.IsNullOrWhiteSpace(value) ? value : "Невідомо";
        }

        public string StudentId
        {
            get => _studentId;
            set => _studentId = !string.IsNullOrWhiteSpace(value) ? value : "N/A";
        }

        public double AverageMark
        {
            get => _averageMark;
            set
            {
                if (value >= 0.0 && value <= 100.0)
                    _averageMark = value;
                else
                    _averageMark = 0.0;
            }
        }

        // Конструктор за замовчуванням із ланцюговим викликом
        public Student() : this("New Student", "N/A", 0.0)
        {
            Console.WriteLine("[Конструктор за замовчуванням викликано]");
        }

        // Параметризований конструктор
        public Student(string name, string studentId, double averageMark)
        {
            Name = name;
            StudentId = studentId;
            AverageMark = averageMark;
            Console.WriteLine($"[Параметризований конструктор викликано для: {Name}]");
        }

        public string GetStudentCard()
        {
            return $"Студент: {Name} | Квиток: {StudentId} | Середній бал: {AverageMark}";
        }

        // Деструктор (фіналізатор)
        ~Student()
        {
            Console.WriteLine($"[Деструктор викликано]: Об'єкт студент '{_name}' знищується.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating objects...");

            Student student1 = new Student();
            Student student2 = new Student("Тарас Шинкарук", "KB12345678", 95.5);

            Console.WriteLine(student1.GetStudentCard());
            Console.WriteLine(student2.GetStudentCard());

            Console.WriteLine("Objects created");
            Console.WriteLine("End of Main, preparing for GC");

            student1 = null;
            student2 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Program finished.");
        }
    }
}
