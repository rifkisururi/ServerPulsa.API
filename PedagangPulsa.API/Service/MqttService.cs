using MQTTnet.Client;
using MQTTnet;
using System.Text;

namespace PedagangPulsa.API.Service
{
    public class MqttService
    {
        private readonly IConfiguration _configuration;
        private readonly IMqttClient _mqttClient;

        public MqttService(IConfiguration configuration)
        {
            _configuration = configuration;

            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            // Baca konfigurasi dari appsettings.json

            string ClientId = _configuration["MqttSettings:ClientId"].ToString();
            string BrokerUrl = _configuration["MqttSettings:BrokerUrl"].ToString();
            string Username = _configuration["MqttSettings:Username"].ToString();
            string Password = _configuration["MqttSettings:Password"].ToString();
            string Port = _configuration["MqttSettings:Port"].ToString();

            var mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId(ClientId)
                .WithTcpServer(BrokerUrl, int.Parse(Port))
                .WithCredentials(Username, Password)
                .WithTls()
                .WithCleanSession()
                .Build();

            _mqttClient.ConnectAsync(mqttOptions, CancellationToken.None).Wait();
        }

        public async Task PublishMessageAsync(string topic, string message)
        {
            // Cek apakah client terhubung, jika tidak, lakukan koneksi ulang
            if (!_mqttClient.IsConnected)
            {
                await _mqttClient.ReconnectAsync(); // Koneksi ulang jika terputus
            }

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(message))
                .WithRetainFlag(false)
                .Build();

            if (_mqttClient.IsConnected)
            {
                await _mqttClient.PublishAsync(mqttMessage, CancellationToken.None);
            }
        }
    }
}
