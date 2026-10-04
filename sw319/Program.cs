using System;

namespace sw319
{
    public class EventLogger : IDisposable
    {
        private bool _disposed = false;
        private bool _isLogging;
        private string _logName;

        public string LogName
        {
            get => _logName;
            set => _logName = !string.IsNullOrWhiteSpace(value) ? value : "SystemLog";
        }

        public bool IsLogging
        {
            get => _isLogging;
            set => _isLogging = value;
        }

        public EventLogger(string logName)
        {
            LogName = logName;
            IsLogging = true;
            Console.WriteLine($"[Конструктор]: Логер '{_logName}' ініціалізовано, ресурс активний.");
        }

        public void LogEvent(string eventName)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EventLogger), "Неможливо писати лог: об'єкт вже звільнено.");
            }
            Console.WriteLine($"[Логування]: Запис події '{eventName}' у лог '{_logName}'.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Звільнення керованих ресурсів
                    Console.WriteLine($"[Dispose(true)]: Звільнення керованих ресурсів для логера '{_logName}'.");
                }

                // Звільнення некерованих ресурсів
                if (_isLogging)
                {
                    Console.WriteLine($"[Dispose]: Зупинено логування для '{_logName}' (некерований ресурс звільнено).");
                    _isLogging = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EventLogger()
        {
            Dispose(false);
            Console.WriteLine($"[Деструктор]: Викликано фіналізатор для логера '{_logName}'.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Сценарій 1: Використання оператора using ===");
            using (EventLogger logger1 = new EventLogger("SecurityLog"))
            {
                logger1.LogEvent("Вхід користувача в систему");
            } // Тут автоматично викликається Dispose()

            Console.WriteLine("\n=== Сценарій 2: Явний виклик Dispose() ===");
            EventLogger logger2 = new EventLogger("ApplicationLog");
            logger2.LogEvent("Старт фонової служби");
            logger2.Dispose();
            
            // Повторний виклик Dispose не викликає помилки (перевірка прапорця _disposed)
            logger2.Dispose(); 

            Console.WriteLine("\n=== Сценарій 3: Робота збирача сміття (GC) ===");
            CreateObjectWithoutDispose();

            // Примусовий виклик збирача сміття для демонстрації деструктора
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nПрограму завершено.");
        }

        static void CreateObjectWithoutDispose()
        {
            EventLogger logger3 = new EventLogger("TempLog");
            logger3.LogEvent("Тимчасова подія");
            // Без виклику Dispose та без using — об'єкт знищить збирач сміття
        }
    }
}
