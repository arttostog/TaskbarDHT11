using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TaskbarApp.Properties;

namespace TaskbarApp
{
    internal class TaskbarApp : ApplicationContext
    {
        private readonly Thread _updateDataThread;
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("http://192.168.1.64")
        };
        private readonly NotifyIcon _notifyIcon = new()
        {
            Visible = true,
            Icon = Resources.icon
        };

        public TaskbarApp()
        {
            _updateDataThread = new(UpdateData);
            _updateDataThread.Start();

            Application.ApplicationExit += OnExit;
        }

        private void UpdateData()
        {
            while (true)
            {
                SetData();
                Thread.Sleep(60 * 1000);
            }
        }

        private async void SetData()
        {
            DHT11Data sensorData = await _httpClient.GetFromJsonAsync<DHT11Data>("/");

            StringBuilder data = new("Temperature: ");
            data.AppendLine(sensorData.Temperature.ToString());
            data.Append("Humidity: ");
            data.AppendLine(sensorData.Humidity.ToString());

            _notifyIcon.Text = data.ToString();
        }

        private void OnExit(object sender, EventArgs eventArgs)
        {
            _notifyIcon.Visible = false;
            _updateDataThread.Abort();
        }
    }
}
