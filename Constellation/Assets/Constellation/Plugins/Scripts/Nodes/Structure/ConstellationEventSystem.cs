public class ConstellationEventSystem {
	public delegate void Event(string eventName, Ray eventValue);
	Event OnEvent;

	public void Register(Event onEvent)
	{
		OnEvent += onEvent;
	}

	public void Unregister(Event onEvent)
	{
		OnEvent -= onEvent;
	}

	public void SendEvent(string eventName, Ray eventValue)
	{
		OnEvent(eventName, eventValue);
	}
}
