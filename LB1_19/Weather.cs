namespace OOP_Shynkaruk;

/// <summary>
/// Зберігає прогноз погоди для певного міста й дати.
/// </summary>
public class Weather
{
    // Поля відповідно до умови завдання.
    private string city;
    private DateTime date;

    // Властивість відповідно до умови завдання.
    public double Temperature { get; set; }

    public Weather(string city, DateTime date, double temperature)
    {
        this.city = city;
        this.date = date;
        Temperature = temperature;
    }

    // Метод відповідно до умови завдання.
    public void PrintForecast()
    {
        Console.WriteLine(
            $"Прогноз погоди для міста {city} на {date:dd.MM.yyyy}: " +
            $"{Temperature:+0.0;-0.0;0.0} °C.");
    }
}
