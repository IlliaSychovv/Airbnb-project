namespace Airbnb.Application.Interfaces;

public interface IEventSender
{
    Task SendEvent<T>(string topic, string key, T jsonMessage);
}