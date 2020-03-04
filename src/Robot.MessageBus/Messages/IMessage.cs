using System;

namespace MessageBus
{
    /// <summary>
    /// Messages should implement this interface and also have a <seealso cref="TopicAttribute"/>
    /// </summary>
    public interface IMessage : IConvertible
    {
    }
}
