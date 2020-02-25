using MessageBus.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus
{
    public class MesageBroker : IMessageBroker
    {
        private readonly IMqttClient _mqttClient;
        private readonly IList<Subscription> Subscriptions = new List<Subscription>();

        public MesageBroker()
        {
            _mqttClient = MqttClient.CreateAsync("localhost").Result;

            _mqttClient.MessageStream.Subscribe(msg =>
            {
                var payload = Encoding.UTF8.GetString(msg.Payload);
                Console.WriteLine(payload);

                var subscription = Subscriptions.FirstOrDefault(s => s.Topic == msg.Topic);

                if (subscription != null)
                {
                    var deserialized = JsonConvert.DeserializeObject(payload);

                    var typed = (IMessage)Convert.ChangeType(deserialized, subscription.Type);

                    subscription.Callback(typed);
                }
            });

            var sessionState = _mqttClient.ConnectAsync().Result;
        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            var wrapper = new MessageWrapper
            {
                Payload = JsonConvert.SerializeObject(message),
                Type = typeof(T).FullName
            };

            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(wrapper));

            await _mqttClient.PublishAsync(new MqttApplicationMessage(topic, payload), MqttQualityOfService.ExactlyOnce);
        }

        public async Task SubscribeAsync<T>(string topic, Action<IMessage> callback)
        {
            Subscriptions.Add(new Subscription
            {
                Topic = topic,
                Callback = callback,
                Type = typeof(T)
            });
            await _mqttClient.SubscribeAsync(topic, MqttQualityOfService.ExactlyOnce);
        }
    }
}
