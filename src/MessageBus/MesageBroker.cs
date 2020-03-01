using MessageBus.Messages;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IMessageBroker> _log;

        public MesageBroker(ILogger<IMessageBroker> logger)
        {
            _log = logger;

            _mqttClient = MqttClient.CreateAsync("localhost").Result;

            _mqttClient.MessageStream.Subscribe(OnMessageReceived);

            _log.LogDebug($"Connecting to mqtt server...");

            var sessionState = _mqttClient.ConnectAsync().Result;
        }

        public async Task PublishAsync<T>(T message) where T : IMessage
        {
            var topic = GetTopic(typeof(T));

            var serializedMessage = JsonConvert.SerializeObject(message);

            _log.LogDebug($"Publishing on ({topic}): {serializedMessage}");

            var wrapper = new MessageWrapper
            {
                Payload = serializedMessage,
                Type = typeof(T).FullName
            };

            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(wrapper));

            if (!_mqttClient.IsConnected)
            {
                var ex = new Exception($"Mqtt client is not connected!");
                _log.LogError(ex.Message, ex);
                throw ex;
            }
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

            try
            {
                var messageWrapper = (MessageWrapper)JsonConvert.DeserializeObject(payload, typeof(MessageWrapper));

                _log.LogDebug($"Received from ({mqttMessage.Topic}): {messageWrapper.Payload}");

                var subscription = Subscriptions.FirstOrDefault(s => s.Topic == mqttMessage.Topic);

                if (subscription != null)
                {
                    var message = (IMessage)JsonConvert.DeserializeObject(messageWrapper.Payload, subscription.Type);

                    try
                    {
                        subscription.Callback(message);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError($"Exception unhandled by Message callback.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Error on message received on ({mqttMessage.Topic}): {payload}", ex);
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
