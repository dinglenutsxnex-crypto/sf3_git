using System.IO;
using Org.BouncyCastle.Bcpg.Sig;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class SignatureSubpacketsParser
	{
		private readonly Stream input;

		public SignatureSubpacketsParser(Stream input)
		{
			this.input = input;
		}

		public SignatureSubpacket ReadPacket()
		{
			int num = input.ReadByte();
			if (num < 0)
			{
				return null;
			}
			int num2 = 0;
			if (num < 192)
			{
				num2 = num;
			}
			else if (num <= 223)
			{
				num2 = (num - 192 << 8) + input.ReadByte() + 192;
			}
			else if (num == 255)
			{
				num2 = (input.ReadByte() << 24) | (input.ReadByte() << 16) | (input.ReadByte() << 8) | input.ReadByte();
			}
			int num3 = input.ReadByte();
			if (num3 < 0)
			{
				throw new EndOfStreamException("unexpected EOF reading signature sub packet");
			}
			byte[] array = new byte[num2 - 1];
			if (Streams.ReadFully(input, array) < array.Length)
			{
				throw new EndOfStreamException();
			}
			bool critical = (num3 & 0x80) != 0;
			SignatureSubpacketTag signatureSubpacketTag = (SignatureSubpacketTag)(num3 & 0x7F);
			switch (signatureSubpacketTag)
			{
			case SignatureSubpacketTag.CreationTime:
				return new SignatureCreationTime(critical, array);
			case SignatureSubpacketTag.KeyExpireTime:
				return new KeyExpirationTime(critical, array);
			case SignatureSubpacketTag.ExpireTime:
				return new SignatureExpirationTime(critical, array);
			case SignatureSubpacketTag.Revocable:
				return new Revocable(critical, array);
			case SignatureSubpacketTag.Exportable:
				return new Exportable(critical, array);
			case SignatureSubpacketTag.IssuerKeyId:
				return new IssuerKeyId(critical, array);
			case SignatureSubpacketTag.TrustSig:
				return new TrustSignature(critical, array);
			case SignatureSubpacketTag.PreferredSymmetricAlgorithms:
			case SignatureSubpacketTag.PreferredHashAlgorithms:
			case SignatureSubpacketTag.PreferredCompressionAlgorithms:
				return new PreferredAlgorithms(signatureSubpacketTag, critical, array);
			case SignatureSubpacketTag.KeyFlags:
				return new KeyFlags(critical, array);
			case SignatureSubpacketTag.PrimaryUserId:
				return new PrimaryUserId(critical, array);
			case SignatureSubpacketTag.SignerUserId:
				return new SignerUserId(critical, array);
			case SignatureSubpacketTag.NotationData:
				return new NotationData(critical, array);
			default:
				return new SignatureSubpacket(signatureSubpacketTag, critical, array);
			}
		}
	}
}
