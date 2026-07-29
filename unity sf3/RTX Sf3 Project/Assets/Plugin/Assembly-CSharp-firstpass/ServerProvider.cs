public class ServerProvider : ServerProviderBase
{
	private static ServerProvider _instance;

	public static ServerProvider Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = ServerProviderBase.Init<ServerProvider>();
				_instance.Init();
			}
			return _instance;
		}
	}

	protected override string GetServer()
	{
		return "http://95.213.134.90";
	}

	protected override void Init()
	{
	}
}
