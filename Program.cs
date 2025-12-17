//Weather App C# Project Done by AHMED MD SHAKIL, CST, 2021, ID: 2130130203//
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string apiKey = "5c05e502387370283e249b6857d8b621";
        HttpClient client = new HttpClient();

        ShowWelcomeMessage();

        while (true)
        {
            Console.Write("\n🔍 Enter city name: ");
            string? city = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("⚠️ City name cannot be empty.");
                continue;
            }

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("main", out JsonElement main) &&
                    root.TryGetProperty("weather", out JsonElement weatherArray) &&
                    root.TryGetProperty("wind", out JsonElement wind) &&
                    root.TryGetProperty("name", out JsonElement cityName))
                {
                    float tempC = main.GetProperty("temp").GetSingle();
                    float tempF = tempC * 9 / 5 + 32;
                    int humidity = main.GetProperty("humidity").GetInt32();
                    string description = weatherArray[0].GetProperty("description").GetString() ?? "unknown";
                    float windSpeed = wind.GetProperty("speed").GetSingle();
                    int windDeg = wind.GetProperty("deg").GetInt32();
                    string windDir = GetWindDirection(windDeg);

                    string country = root.GetProperty("sys").GetProperty("country").GetString() ?? "";

                    Console.WriteLine("\n📍 Location: " + cityName.GetString() + ", " + country);
                    Console.WriteLine("📅 Date: " + DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                    Console.WriteLine("⏰ Time: " + DateTime.Now.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                    Console.WriteLine("🌡️ Temperature: {0}°C / {1}°F", tempC, tempF);
                    Console.WriteLine("☁️ Weather: {0} {1}", description, GetWeatherIcon(description));
                    Console.WriteLine("💧 Humidity: {0}%", humidity);
                    Console.WriteLine("🌬️ Wind: {0} m/s, Direction: {1} ({2}°)", windSpeed, windDir, windDeg);
                }
                else
                {
                    Console.WriteLine("❌ City not found or API error.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error: {ex.Message}");
            }

            Console.Write("\n🔁 Do you want to check another city? (yes/no): ");
            string? choice = Console.ReadLine()?.ToLower();
            if (choice != "yes" && choice != "y")
            {
                Console.WriteLine("\n👋 Thank you for using WeatherApp. Stay safe and have a great day!");
                break;
            }
        }
    }

    static void ShowWelcomeMessage()
    {
        Console.WriteLine("🌤️--------------------------------------------------------------🌤️");
        Console.WriteLine("        WELCOME TO THE DAILY WEATHER APP BY AHMED MD SHAKIL        ");
        Console.WriteLine("🌤️--------------------------------------------------------------🌤️");
        Console.WriteLine("📌 Type the name of any city in the world to get real-time weather updates.");
        Console.WriteLine("📌 This app will show temperature, weather condition, humidity, wind info, etc.");
        Console.WriteLine("📌 After viewing, you can choose to check another city or exit the app.");
        Console.WriteLine("🌍 Powered by (AHMED MD SHAKIL CST 2021)");
        Console.WriteLine("---------------------------------------------------\n");
    }

    static string GetWeatherIcon(string desc)
    {
        desc = desc.ToLower();
        if (desc.Contains("cloud")) return "☁️";
        if (desc.Contains("rain")) return "🌧️";
        if (desc.Contains("clear")) return "☀️";
        if (desc.Contains("snow")) return "❄️";
        if (desc.Contains("storm") || desc.Contains("thunder")) return "🌩️";
        if (desc.Contains("mist") || desc.Contains("fog")) return "🌫️";
        return "🌈";
    }

    static string GetWindDirection(int deg)
    {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
        return directions[(int)Math.Round(((double)deg % 360) / 45)];
    }
}