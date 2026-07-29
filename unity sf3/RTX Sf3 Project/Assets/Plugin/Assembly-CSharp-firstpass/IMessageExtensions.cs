using System;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using Google.Protobuf.Collections;

public static class IMessageExtensions
{
	public static byte[] ToBinary(this IMessage e)
	{
		MemoryStream memoryStream = new MemoryStream();
		e.WriteTo(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] ToBinary<T>(this T e) where T : IMessage<T>, new()
	{
		MemoryStream memoryStream = new MemoryStream();
		e.WriteTo(memoryStream);
		return memoryStream.ToArray();
	}

	public static IMessage FromBinary(this byte[] binary, Type type)
	{
		IMessage message = Activator.CreateInstance(type) as IMessage;
		message.MergeFrom(binary);
		return message;
	}

	public static T FromBinary<T>(this byte[] binary) where T : IMessage<T>, new()
	{
		MessageParser<T> messageParser = new MessageParser<T>(() => new T());
		return messageParser.ParseFrom(binary);
	}

	public static List<T> RepeatedToList<T>(this RepeatedField<T> repeated)
	{
		return new List<T>(repeated);
	}
}
