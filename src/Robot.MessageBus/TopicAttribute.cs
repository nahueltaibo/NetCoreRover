using System;

namespace Robot.MessageBus
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TopicAttribute : Attribute
    {
        public TopicAttribute(string topic)
        {
            Topic = topic;
        }

        public string Topic { get; set; }
    }
}
