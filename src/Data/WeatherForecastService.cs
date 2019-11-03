using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;

namespace DataBaseRecordChaneNotificationWithBlazor.Data
{
    public class WeatherForecastService : IWeatherForecastService, IDisposable
    {
        private const string TableName = "WeatherForecasts";
        private SqlTableDependency<WeatherForecast> _notifier;
        private IConfiguration _configuration;
        
        public event WeatherForecastDelegate OnWeatherForecastChanged;

        public WeatherForecastService(IConfiguration configuration)
        {
            _configuration = configuration;

            _notifier = new SqlTableDependency<WeatherForecast>(_configuration["ConnectionString"], TableName);
            _notifier.OnChanged += this.TableDependency_Changed;
            _notifier.Start();
        }

        private void TableDependency_Changed(object sender, RecordChangedEventArgs<WeatherForecast> e)
        { 
            if (this.OnWeatherForecastChanged != null)
            {
                this.OnWeatherForecastChanged(this, new WeatherForecastChangeEventArgs(e.Entity, e.EntityOldValues));
            }
        }

        public IList<WeatherForecast> GetForecast()
        {
            var result = new List<WeatherForecast>();

            using (var sqlConnection = new SqlConnection(_configuration["ConnectionString"]))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM " + TableName;
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.Add(new WeatherForecast
                                {
                                    City = reader.GetString(reader.GetOrdinal("City")),
                                    Temperature = reader.GetInt32(reader.GetOrdinal("Temperature"))
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        public void Dispose()
        {
            _notifier.Stop();
            _notifier.Dispose();
        }
    }
}
