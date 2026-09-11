using System.Text;
using OOP_Shynkaruk;

Console.OutputEncoding = Encoding.UTF8;

Weather forecast = new Weather(
    city: "Рівне",
    date: DateTime.Today,
    temperature: 18.5);

forecast.PrintForecast();
