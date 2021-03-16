namespace WebApplication2.Pages
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.RazorPages;

    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;

    public class IndexModel : PageModel
    {
        private readonly IMqttClientOptions mqttClientOptions;

        private readonly IMqttClient mqttClient;

        public string LastData;

        public IndexModel(IMqttClient mqttClient, IMqttClientOptions mqttClientOptions)
        {
            this.mqttClient = mqttClient;
            this.mqttClientOptions = mqttClientOptions;
        }

        public async Task OnGetAsync()
        {
            try
            {
                await this.mqttClient.ConnectAsync(this.mqttClientOptions);
                await this.mqttClient.PublishAsync(
                    new MqttApplicationMessage { Topic = "test", Payload = Encoding.UTF8.GetBytes("Some payload") });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
