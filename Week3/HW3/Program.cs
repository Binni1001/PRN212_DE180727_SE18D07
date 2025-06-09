using System;
using System.Collections.Generic;

namespace DesignPatterns.Homework
{
    // This homework is based on the Observer Pattern
    // Students will implement a weather monitoring system using the Observer pattern

    #region Observer Pattern Interfaces

    // The subject interface that all weather stations must implement
    public interface IWeatherStation
    {
        void RegisterObserver(IWeatherObserver observer);
        void RemoveObserver(IWeatherObserver observer);
        void NotifyObservers();
        
        float Temperature { get; }
        float Humidity { get; }
        float Pressure { get; }
    }

    // The observer interface that all display devices must implement
    public interface IWeatherObserver
    {
        void Update(float temperature, float humidity, float pressure);
    }

    #endregion

    #region Weather Station Implementation

    // Concrete implementation of a weather station
    public class WeatherStation : IWeatherStation
    {
        private List<IWeatherObserver> _observers;
        private float _temperature;
        private float _humidity;
        private float _pressure;
        
        public WeatherStation()
        {
            _observers = new List<IWeatherObserver>();
        }
        
        public void RegisterObserver(IWeatherObserver observer)
        {
            _observers.Add(observer);
            Console.WriteLine("Observer registered.");
        }
        
        public void RemoveObserver(IWeatherObserver observer)
        {
            _observers.Remove(observer);
            Console.WriteLine("Observer removed.");
        }
        
        public void NotifyObservers()
        {
            Console.WriteLine("Notifying observers of weather update...");
            foreach (var observer in _observers)
            {
                observer.Update(_temperature, _humidity, _pressure);
            }
        }
        
        public float Temperature => _temperature;
        public float Humidity => _humidity;
        public float Pressure => _pressure;
        
        public void SetMeasurements(float temperature, float humidity, float pressure)
        {
            Console.WriteLine("\n--- Weather Station: Weather measurements updated ---");
            _temperature = temperature;
            _humidity = humidity;
            _pressure = pressure;
            
            Console.WriteLine($"Temperature: {_temperature}°C");
            Console.WriteLine($"Humidity: {_humidity}%");
            Console.WriteLine($"Pressure: {_pressure} hPa");
            
            NotifyObservers();
        }
    }

    #endregion

    #region Observer Implementations

    public class CurrentConditionsDisplay : IWeatherObserver
    {
        private float _temperature;
        private float _humidity;
        private float _pressure;
        private IWeatherStation _weatherStation;

        public CurrentConditionsDisplay(IWeatherStation weatherStation)
        {
            _weatherStation = weatherStation;
            _weatherStation.RegisterObserver(this);
        }

        public void Update(float temperature, float humidity, float pressure)
        {
            _temperature = temperature;
            _humidity = humidity;
            _pressure = pressure;
        }

        public void Display()
        {
            Console.WriteLine($"Current Conditions: {_temperature}°C, {_humidity}% humidity, {_pressure} hPa pressure");
        }
    }

    public class StatisticsDisplay : IWeatherObserver
    {
        private float _minTemperature = float.MaxValue;
        private float _maxTemperature = float.MinValue;
        private float _sumTemperature = 0;
        private int _count = 0;
        private IWeatherStation _weatherStation;

        public StatisticsDisplay(IWeatherStation weatherStation)
        {
            _weatherStation = weatherStation;
            _weatherStation.RegisterObserver(this);
        }

        public void Update(float temperature, float humidity, float pressure)
        {
            _minTemperature = Math.Min(_minTemperature, temperature);
            _maxTemperature = Math.Max(_maxTemperature, temperature);
            _sumTemperature += temperature;
            _count++;
        }

        public void Display()
        {
            float avgTemperature = _count > 0 ? _sumTemperature / _count : 0;
            Console.WriteLine($"Temperature Statistics: Min {_minTemperature}°C, Max {_maxTemperature}°C, Avg {avgTemperature:F1}°C");
        }
    }

    public class ForecastDisplay : IWeatherObserver
    {
        private float _lastPressure;
        private float _currentPressure;
        private bool _isFirstUpdate = true;
        private IWeatherStation _weatherStation;

        public ForecastDisplay(IWeatherStation weatherStation)
        {
            _weatherStation = weatherStation;
            _weatherStation.RegisterObserver(this);
        }

        public void Update(float temperature, float humidity, float pressure)
        {
            _lastPressure = _isFirstUpdate ? pressure : _currentPressure;
            _currentPressure = pressure;
            _isFirstUpdate = false;
        }

        public void Display()
        {
            string forecast;
            if (_isFirstUpdate)
            {
                forecast = "No forecast available yet.";
            }
            else if (_currentPressure > _lastPressure)
            {
                forecast = "Improving weather";
            }
            else if (_currentPressure < _lastPressure)
            {
                forecast = "Cooler, rainy weather";
            }
            else
            {
                forecast = "Same weather";
            }
            Console.WriteLine($"Weather Forecast: {forecast}");
        }
    }

    #endregion

    #region Application

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Observer Pattern Homework - Weather Station\n");
            
            try
            {
                WeatherStation weatherStation = new WeatherStation();
                
                Console.WriteLine("Creating display devices...");
                CurrentConditionsDisplay currentDisplay = new CurrentConditionsDisplay(weatherStation);
                StatisticsDisplay statisticsDisplay = new StatisticsDisplay(weatherStation);
                ForecastDisplay forecastDisplay = new ForecastDisplay(weatherStation);
                
                Console.WriteLine("\nSimulating weather changes...");
                weatherStation.SetMeasurements(25.2f, 65.3f, 1013.1f);
                
                Console.WriteLine("\n--- Displaying Information ---");
                currentDisplay.Display();
                statisticsDisplay.Display();
                forecastDisplay.Display();
                
                weatherStation.SetMeasurements(28.5f, 70.2f, 1012.5f);
                
                Console.WriteLine("\n--- Displaying Updated Information ---");
                currentDisplay.Display();
                statisticsDisplay.Display();
                forecastDisplay.Display();
                
                weatherStation.SetMeasurements(22.1f, 90.7f, 1009.2f);
                
                Console.WriteLine("\n--- Displaying Final Information ---");
                currentDisplay.Display();
                statisticsDisplay.Display();
                forecastDisplay.Display();
                
                Console.WriteLine("\nRemoving CurrentConditionsDisplay...");
                weatherStation.RemoveObserver(currentDisplay);
                
                weatherStation.SetMeasurements(24.5f, 80.1f, 1010.3f);
                
                Console.WriteLine("\n--- Displaying Information After Removal ---");
                statisticsDisplay.Display();
                forecastDisplay.Display();
                
                Console.WriteLine("\nObserver Pattern demonstration complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    #endregion
}