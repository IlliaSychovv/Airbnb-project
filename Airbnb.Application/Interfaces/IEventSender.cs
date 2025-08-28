namespace Airbnb.Application.Interfaces;

public interface IEventSender
{ 
    Task SendEvent<TEvent>(object key, TEvent message);
}