using System;
using System.Collections.Generic;

namespace DataBaseRecordChaneNotificationWithBlazor.Data
{
    public delegate void WeatherForecastDelegate(object sender, WeatherForecastChangeEventArgs args);

    public class WeatherForecastChangeEventArgs : EventArgs
    {
        public WeatherForecast NewWeatherForecast { get; }
        public WeatherForecast OldWeatherForecast { get; }

        public WeatherForecastChangeEventArgs(WeatherForecast newWeatherForecast, WeatherForecast oldWeatherForecast)
        {
            this.NewWeatherForecast = newWeatherForecast;
            this.OldWeatherForecast = oldWeatherForecast;
        }
    }

    public interface IWeatherForecastService
    {
        public event WeatherForecastDelegate OnWeatherForecastChanged;
        IList<WeatherForecast> GetForecast();
    }
}