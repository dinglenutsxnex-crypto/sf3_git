using System.IO;

namespace Google.Protobuf
{
	public static class MessageExtensions
	{
		public static void MergeFrom(this IMessage message, byte[] data)
		{
			ProtoPreconditions.CheckNotNull(message, "message");
			ProtoPreconditions.CheckNotNull(data, "data");
			CodedInputStream codedInputStream = new CodedInputStream(data);
			message.MergeFrom(codedInputStream);
			codedInputStream.CheckReadEndOfStreamTag();
		}

		public static void MergeFrom(this IMessage message, ByteString data)
		{
			ProtoPreconditions.CheckNotNull(message, "message");
			ProtoPreconditions.CheckNotNull(data, "data");
			CodedInputStream codedInputStream = data.CreateCodedInput();
			message.MergeFrom(codedInputStream);
			codedInputStream.CheckReadEndOfStreamTag();
		}

		public static void MergeFrom(this IMessage message, Stream input)
		{
			ProtoPreconditions.CheckNotNull(message, "message");
			ProtoPreconditions.CheckNotNull(input, "input");
			CodedInputStream codedInputStream = new CodedInputStream(input);
			message.MergeFrom(codedInputStream);
			codedInputStream.CheckReadEndOfStreamTag();
		}

		public static void MergeDelimitedFrom(this IMessage message, Stream input)
		{
			ProtoPreconditions.CheckNotNull(message, "message");
			ProtoPreconditions.CheckNotNull(input, "input");
			int size = (int)CodedInputStream.ReadRawVarint32(input);
			Stream input2 = new LimitedInputStream(input, size);
			message.MergeFrom(input2);
		}

		public static byte[] ToByteArray(this IMessage message)
		{
			ProtoPreconditions.CheckNotNull(message, "message");
			byte[] array = new byte[message.CalculateSize()];
			CodedOutputStream codedOutputStream = new CodedOutputStream(array);
			message.WriteTo(codedOutputStream);
			codedOutputStream.CheckNoSpaceLeft();
			return array;
		}

		public static void WriteTo(this IMessage message, Stream output)
		{
			ProtoPreconditions.CheckNotNull(message, "message");
			ProtoPreconditions.CheckNotNull(output, "output");
			CodedOutputStream codedOutputStream = new CodedOutputStream(output);
			message.WriteTo(codedOutputStream);
			codedOutputStream.Flush();
		}

		public static void WriteDelimitedTo(this IMessage message, Stream output)
		{
			ProtoPreconditions.CheckNotNull(message, "message");
			ProtoPreconditions.CheckNotNull(output, "output");
			CodedOutputStream codedOutputStream = new CodedOutputStream(output);
			codedOutputStream.WriteRawVarint32((uint)message.CalculateSize());
			message.WriteTo(codedOutputStream);
			codedOutputStream.Flush();
		}

		public static ByteString ToByteString(this IMessage message)
		{
			ProtoPreconditions.CheckNotNull(message, "message");
			return ByteString.AttachBytes(message.ToByteArray());
		}
	}
}
