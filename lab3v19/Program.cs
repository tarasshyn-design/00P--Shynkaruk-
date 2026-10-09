using System;

namespace Lab3
{
    // Варіант 19: Клас EventLogger (Імітує логування подій у систему)
    public class EventLogger : IDisposable
    {
        private bool _disposed = false;
        private string _logName;
        private bool _isLogging; // Поле для імітації некерованого ресурсу

        public string LogName => _logName;
        public bool IsLogging => _isLogging;

        // Конструктор, який "виділяє ресурс"
        public EventLogger(string logName)
        {
            _logName = logName;
            _isLogging = true;
            Console.WriteLine($"[EventLogger]: Запущено логування подій для '{_logName}'.");
        }

        // Метод LogEvent виводить повідомлення, якщо логування активне
        public void LogEvent(string eventName)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EventLogger), "Неможливо записати подію: логер вже зупинено та знищено.");
            }

            if (_isLogging)
            {
                Console.WriteLine($"[Журнал '{_logName}']: Записано подію -> {eventName}");
            }
        }

        // Захищений віртуальний метод для реалізації патерну Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Звільнення керованих ресурсів (якщо вони є)
                    Console.WriteLine("--> Звільнення керованих ресурсів (Managed resources)...");
                }

                // Звільнення некерованих ресурсів ("зупиняє логування")
                if (_isLogging)
                {
                    Console.WriteLine($"--> Зупинка логування для '{_logName}' (Unmanaged resource released).");
                    _isLogging = false;
                }

                _disposed = true;
            }
        }

        // Публічний метод Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Скасовуємо виклик деструктора, бо ресурси вже звільнено
        }

        // Деструктор (фіналізатор)
        ~EventLogger()
        {
            Console.WriteLine("\n[Деструктор]: Виклик деструктора ~EventLogger().");
            Dispose(false);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("СЦЕНАРІЙ 1: Використання блоку 'using'");
            using (EventLogger logger1 = new EventLogger("SystemEvents"))
            {
                logger1.LogEvent("Запуск служби");
                logger1.LogEvent("Користувач увійшов у систему");
            } // Тут Dispose() викликається автоматично

            Console.WriteLine("\nСЦЕНАРІЙ 2: Явний виклик Dispose()");
            EventLogger logger2 = new EventLogger("SecurityEvents");
            logger2.LogEvent("Перевірка прав доступу");
            logger2.Dispose(); // Явний виклик звільнення ресурсів

            Console.WriteLine("\nСЦЕНАРІЙ 3: Робота деструктора через Garbage Collector");
            CreateUnmanagedObject();

            Console.WriteLine("Запуск збирача сміття (GC.Collect)...");
            GC.Collect();
            GC.WaitForPendingFinalizers(); // Очікування завершення фіналізаторів

            Console.WriteLine("\nЗавершення виконання програми.");
        }

        // Допоміжний метод для створення об'єкта поза зоною видимості Main
        static void CreateUnmanagedObject()
        {
            EventLogger logger3 = new EventLogger("ApplicationEvents");
            logger3.LogEvent("Фоновий процес розпочато");
            // Метод завершується, посилання на logger3 втрачається, але Dispose() не викликано
        }
    }
}
