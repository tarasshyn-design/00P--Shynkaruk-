using System;
using System.Threading;

namespace IndependentWork14
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;
        private readonly Random _random = new Random();

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool> shouldRetry = null)
        {
            int retryAttempt = 0;
            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    if (retryAttempt >= _maxRetries || (shouldRetry != null && !shouldRetry(ex)))
                    {
                        Console.WriteLine($"[RetryPolicy] Досягнуто максимальну кількість спроб ({_maxRetries}) або виняток не підлягає повтору.");
                        throw;
                    }

                    // Формула затримки з експоненційним backoff та jitter
                    double multiplier = Math.Pow(2, retryAttempt);
                    double jitter = 1 + (_random.NextDouble() * _jitterFactor * 2 - _jitterFactor);
                    double delayMs = _initialDelay.TotalMilliseconds * multiplier * jitter;
                    TimeSpan delay = TimeSpan.FromMilliseconds(Math.Max(0, delayMs));

                    Console.WriteLine($"[RetryPolicy] Спроба {retryAttempt + 1} невдала: {ex.Message}. Очікування {delay.TotalSeconds:F2}с...");
                    Thread.Sleep(delay);

                    retryAttempt++;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var policy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromSeconds(1), jitterFactor: 0.2);

            // Сценарій 1: Успіх після кількох невдалих спроб
            Console.WriteLine("=== Сценарій 1: Успіх після збоїв ===");
            int attemptCount = 0;
            try
            {
                string result = policy.Execute(() =>
                {
                    attemptCount++;
                    Console.WriteLine($"Виконання операції (спроба {attemptCount})...");
                    if (attemptCount < 3)
                    {
                        throw new TimeoutException("Тимчасовий збій мережі.");
                    }
                    return "Операція успішна!";
                });
                Console.WriteLine($"Результат: {result}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критична помилка: {ex.Message}\n");
            }

            // Сценарій 2: Завжди невдала операція (викидання винятку)
            Console.WriteLine("=== Сценарій 2: Завжди невдала операція ===");
            try
            {
                policy.Execute<int>(() =>
                {
                    Console.WriteLine("Виконання ненадійного сервісу...");
                    throw new InvalidOperationException("Сервіс недоступний.");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Перехоплено остаточний виняток: {ex.Message}\n");
            }
        }
    }
}
