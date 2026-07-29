using System.Collections.Generic;

public class NekkiWebHandlerRequest : NekkiWebHandler
{
	private readonly List<byte> _buffer;

	public NekkiWebHandlerRequest(NekkiUri uriRequest)
		: base(uriRequest)
	{
		_buffer = new List<byte>();
	}

	protected override void SetBytes(byte[] data, int dataOffset, int dataLength)
	{
		_buffer.Capacity += dataLength;
		for (int i = 0; i < dataLength; i++)
		{
			_buffer.Add(data[i]);
		}
	}

	protected override byte[] GetData()
	{
		return _buffer.ToArray();
	}
}
