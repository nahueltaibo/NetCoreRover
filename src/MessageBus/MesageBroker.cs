using MessageBus.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mqtt;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus
{
    public class MesageBroker : IMessageBroker
    {
        private readonly IMqttClient _mqttClient;
        private readonly IList<Subscription> Subscriptions = new List<Subscription>();
        private readonly ILogger<IMessageBroker> _log;

        public MesageBroker(ILogger<IMessageBroker> logger)
        {
            _log = logger;

            _mqttClient = MqttClient.CreateAsync("localhost").Result;

            _mqttClient.MessageStream.Subscribe(OnMessageReceived);

            var sessionState = _mqttClient.ConnectAsync().Result;

        }

        public async Task PublishAsync<T>(T message) where T : IMessage
        {
            var topic = GetTopic(typeof(T));


            var serializedMessage = JsonConvert.SerializeObject(message);

            _log.LogDebug($"Publishing topic={topic} payload={serializedMessage}");

            var wrapper = new MessageWrapper
            {
                Payload = serializedMessage,
                Type = typeof(T).FullName
            };

            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(wrapper));

            await _mqttClient.PublishAsync(new MqttApplicationMessage(topic, payload), MqttQualityOfService.ExactlyOnce);
        }

        public async Task SubscribeAsync<T>(Action<IMessage> callback) where T : IMessage
        {
            var topic = GetTopic(typeof(T));

            _log.LogDebug($"Subscribing to topic={topic}");

            Subscriptions.Add(new Subscription
            {
                Topic = topic,
                Callback = callback,
                Type = typeof(T)
            });
            await _mqttClient.SubscribeAsync(topic, MqttQualityOfService.ExactlyOnce);
        }

        private void OnMessageReceived(MqttApplicationMessage mqttMessage)
        {
            var payload = Encoding.UTF8.GetString(mqttMessage.Payload);

            _log.LogDebug($"Message received: topic= {mqttMessage.Topic} payload= {payload}");

            var subscription = Subscriptions.FirstOrDefault(s => s.Topic == mqttMessage.Topic);

            if (subscription != null)
            {
                var messageWrapper = (MessageWrapper)JsonConvert.DeserializeObject(payload, typeof(MessageWrapper));

                var message = (IMessage)JsonConvert.DeserializeObject(messageWrapper.Payload, subscription.Type);

                subscription.Callback(message);
            }
        }

        private string GetTopic(Type type)
        {
            if (!(type.GetCustomAttributes(typeof(TopicAttribute), true).FirstOrDefault() is TopicAttribute topicAttribute))
            {
                throw new ArgumentException($"TopicAttribute missing in the Message type {type.FullName}");
            }

            return topicAttribute.Topic;
        }
    }
}
