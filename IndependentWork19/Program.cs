using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace IndependentWork19
{
    // 3. Власний тип винятку з додатковими властивостями та трьома стандартними конструкторами
    public class InsufficientFundsException : Exception
    {
        public decimal RequestedAmount { get; }
        public decimal CurrentBalance { get; }

        public InsufficientFundsException() : base("Недостатньо коштів на рахунку.") { }

        public InsufficientFundsException(string message) : base(message) { }

        public InsufficientFundsException(string message, Exception inner) : base(message, inner) { }

        public InsufficientFundsException(string message, decimal requested, decimal balance) : base(message)
        {
            RequestedAmount = requested;
            CurrentBalance = balance;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Демонстрація власного винятку
            Console.WriteLine("=== Демонстрація власного винятку ===");
            try
            {
                decimal balance = 500m;
                decimal requested = 1200m;
                if (requested > balance)
                {
                    throw new InsufficientFundsException("Помилка транзакції: перевищено ліміт коштів.", requested, balance);
                }
            }
            resing (InsufficientFundsException ex)
            {
                Console.WriteLine($"[Виняток перехоплено]: {ex.Message}");
                Console.WriteLine($"Запитано: {ex.RequestedAmount}, Баланс: {ex.CurrentBalance}\n");
            }

            // 4. Сценарій 1: Retry із експоненційною затримкою та Jitter для зовнішнього API
            Console.WriteLine("=== Сценарій 1: Polly Retry (Зовнішній API) ===");
            var retryPipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"[Retry Log] Спроба #{args.AttemptNumber} невдала. Повтор через {args.RetryDelay.TotalSeconds:F2}с.");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            int apiCallAttempts = 0;
            try
            {
                await retryPipeline.ExecuteAsync(async token =>
                {
                    apiCallAttempts++;
                    Console.WriteLine($"Виклик зовнішнього API (спроба {apiCallAttempts})...");
                    if (apiCallAttempts < 3)
                    {
                        throw new HttpRequestException("Тимчасовий збій мережі.");
                    }
                    Console.WriteLine("API успішно відповів!");
                    await Task.CompletedTask;
                }, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Остаточна невдача API: {ex.Message}");
            }

            Console.WriteLine();

            // 4. Сценарій 2: Circuit Breaker для бази даних
            Console.WriteLine("=== Сценарій 2: Polly Circuit Breaker (База даних) ===");
            var circuitBreakerPipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<TimeoutException>(),
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 2,
                    BreakDuration = TimeSpan.FromSeconds(3),
                    OnOpened = args =>
                    {
                        Console.WriteLine("[Circuit Breaker] Ланцюг РОЗІМКНЕНО (Open)! Сервіс БД перевантажений.");
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        Console.WriteLine("[Circuit Breaker] Ланцюг ЗАМКНЕНО (Closed). Робота стабілізована.");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        Console.WriteLine("[Circuit Breaker] Ланцюг НАПІВРОЗІМКНЕНО (Half-Open). Тестовий запит...");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            // Симуляція серії збоїв для спрацювання Circuit Breaker
            for (int i = 1; i <= 4; i++)
            {
                try
                {
                    await circuitBreakerPipeline.ExecuteAsync(async token =>
                    {
                        Console.WriteLine($"Запит до БД #{i}...");
                        throw new TimeoutException("Таймаут підключення до БД.");
                    }, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка запиту: {ex.Message}\n");
                }
                await Task.Delay(500);
            }
        }
    }
}
