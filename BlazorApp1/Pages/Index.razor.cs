namespace BlazorApp1.Pages
{
    using System;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;

    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;

    public class IndexModel : ComponentBase
    {
        private IMqttClientOptions mqttClientOptions;

        private IMqttClient mqttClient;

        protected override async Task OnInitializedAsync()
        {
            this.mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId("ClientId")
                .WithTcpServer("mybroker.com", 8883)
                .WithCredentials("Username", "Password")
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    SslProtocol = SslProtocols.Tls12
                })
                .WithCleanSession()
                .Build();
            var factory = new MqttFactory();
            this.mqttClient = factory.CreateMqttClient();

            await this.mqttClient.ConnectAsync(this.mqttClientOptions);

            this.mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                this.LastData = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            });

            var topic = new MqttTopicFilterBuilder().WithTopic("my/topic").Build();
            await this.mqttClient.SubscribeAsync(topic);
        }

        protected string LastData { get; set; }

        protected string Topic { get; set; }

        protected string Payload { get; set; }

        protected string Status { get; set; }

        protected async Task SendMqttMessage(MouseEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.Topic))
                {
                    throw new ArgumentNullException(nameof(this.Topic));
                }

                if (string.IsNullOrWhiteSpace(this.Payload))
                {
                    throw new ArgumentNullException(nameof(this.Payload));
                }

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(this.Topic)
                    .WithPayload(this.Payload)
                    .WithAtLeastOnceQoS()
                    .Build();

                await this.mqttClient.PublishAsync(message, CancellationToken.None);
            }
            catch (Exception ex)
            {
                this.Status = ex.ToString();
            }
        }
    }
}
