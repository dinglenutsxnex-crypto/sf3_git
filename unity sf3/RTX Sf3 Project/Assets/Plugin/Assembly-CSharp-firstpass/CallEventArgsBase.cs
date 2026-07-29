public class CallEventArgsBase<T>
{
	public object Target { get; private set; }

	public T Event { get; private set; }

	public object Content { get; private set; }

	public void Setup(T evt, object content, object target)
	{
		Target = target;
		Content = content;
		Event = evt;
	}

	public CallEventArgsBase<T> SwitchTarget(object target)
	{
		Target = target;
		return this;
	}
}
