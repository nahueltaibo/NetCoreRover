using MessageBus.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MessageBus
{
    public class MesageBroker : IMessageBroker
    {
        private readonly MqttClient _mqttClient;
        private readonly IList<Subscription> Subscriptions = new List<Subscription>();
        private readonly ILogger<IMessageBroker> _log;

        public MesageBroker(ILogger<IMessageBroker> logger)
        {
            _log = logger;

            _mqttClient = new MqttClient("localhost");

            _log.LogDebug($"Connecting to mqtt server...");

            // register to message received
            _mqttClient.MqttMsgPublishReceived += OnMessageReceived;

            var sessionState = _mqttClient.Connect(Guid.NewGuid().ToString());
        }

        public void Publish<T>(T message) where T : IMessage
        {
            var topic = GetTopic(typeof(T));

            var serializedMessage = JsonConvert.SerializeObject(message);

            _log.LogTrace($"Publishing on ({topic}): {serializedMessage}");

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

            _mqttClient.Publish(topic, payload, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        public void Subscribe<T>(Action<IMessage> callback) where T : IMessage
        {
            var topic = GetTopic(typeof(T));

            _log.LogDebug($"Subscribing to topic={topic}");

            Subscriptions.Add(new Subscription
            {
                Topic = topic,
                Callback = callback,
                Type = typeof(T)
            });

            _mqttClient.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        private void OnMessageReceived(object sender, MqttMsgPublishEventArgs events)
        {
            var payload = Encoding.UTF8.GetString(events.Message);

            try
            {
                var messageWrapper = (MessageWrapper)JsonConvert.DeserializeObject(payload, typeof(MessageWrapper));

                _log.LogTrace($"Received from ({events.Topic}): {messageWrapper.Payload}");

                var subscription = Subscriptions.FirstOrDefault(s => s.Topic == events.Topic);

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
                _log.LogError($"Error on message received on ({events.Topic}): {payload}", ex);
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
