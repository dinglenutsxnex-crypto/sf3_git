using System;

public class BinaryReaderNekki : IDisposable
{
	private const int INT_BYTE = 1;

	private const int INT_16 = 2;

	private const int INT_32 = 4;

	private const int INT_64 = 8;

	public readonly int Size;

	private byte[] _bytes;

	public int Position { get; private set; }

	public BinaryReaderNekki(byte[] data)
	{
		_bytes = data;
		Size = _bytes.Length;
	}

	public virtual bool ReadBoolean()
	{
		bool result = BitConverter.ToBoolean(_bytes, Position);
		SetPosition(1);
		return result;
	}

	public virtual byte ReadByte()
	{
		byte result = _bytes[Position];
		SetPosition(1);
		return result;
	}

	public virtual short ReadInt16()
	{
		short result = BitConverter.ToInt16(_bytes, Position);
		SetPosition(2);
		return result;
	}

	public virtual int ReadInt32()
	{
		int result = BitConverter.ToInt32(_bytes, Position);
		SetPosition(4);
		return result;
	}

	public virtual long ReadInt64()
	{
		long result = BitConverter.ToInt64(_bytes, Position);
		SetPosition(8);
		return result;
	}

	public virtual float ReadSingle()
	{
		float result = BitConverter.ToSingle(_bytes, Position);
		SetPosition(4);
		return result;
	}

	public float[] ConvertByteArrayToFloat(int count)
	{
		int num = count * 4;
		float[] array = new float[count];
		Buffer.BlockCopy(_bytes, Position, array, 0, num);
		SetPosition(num);
		return array;
	}

	private void SetPosition(int value)
	{
		Position += value;
	}

	public void Dispose()
	{
		_bytes = null;
	}
}
