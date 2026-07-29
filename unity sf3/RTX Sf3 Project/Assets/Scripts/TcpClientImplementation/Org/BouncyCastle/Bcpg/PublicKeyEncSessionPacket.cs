using System.IO;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Bcpg
{
	public class PublicKeyEncSessionPacket : ContainedPacket
	{
		private int version;

		private long keyId;

		private PublicKeyAlgorithmTag algorithm;

		private BigInteger[] data;

		public int Version
		{
			get
			{
				return version;
			}
		}

		public long KeyId
		{
			get
			{
				return keyId;
			}
		}

		public PublicKeyAlgorithmTag Algorithm
		{
			get
			{
				return algorithm;
			}
		}

		internal PublicKeyEncSessionPacket(BcpgInputStream bcpgIn)
		{
			version = bcpgIn.ReadByte();
			keyId |= (long)bcpgIn.ReadByte() << 56;
			keyId |= (long)bcpgIn.ReadByte() << 48;
			keyId |= (long)bcpgIn.ReadByte() << 40;
			keyId |= (long)bcpgIn.ReadByte() << 32;
			keyId |= (long)bcpgIn.ReadByte() << 24;
			keyId |= (long)bcpgIn.ReadByte() << 16;
			keyId |= (long)bcpgIn.ReadByte() << 8;
			keyId |= (uint)bcpgIn.ReadByte();
			algorithm = (PublicKeyAlgorithmTag)bcpgIn.ReadByte();
			switch (algorithm)
			{
			case PublicKeyAlgorithmTag.RsaGeneral:
			case PublicKeyAlgorithmTag.RsaEncrypt:
				data = new BigInteger[1] { new MPInteger(bcpgIn).Value };
				break;
			case PublicKeyAlgorithmTag.ElGamalEncrypt:
			case PublicKeyAlgorithmTag.ElGamalGeneral:
				data = new BigInteger[2]
				{
					new MPInteger(bcpgIn).Value,
					new MPInteger(bcpgIn).Value
				};
				break;
			default:
				throw new IOException("unknown PGP public key algorithm encountered");
			}
		}

		public PublicKeyEncSessionPacket(long keyId, PublicKeyAlgorithmTag algorithm, BigInteger[] data)
		{
			version = 3;
			this.keyId = keyId;
			this.algorithm = algorithm;
			this.data = (BigInteger[])data.Clone();
		}

		public BigInteger[] GetEncSessionKey()
		{
			return (BigInteger[])data.Clone();
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream);
			bcpgOutputStream.WriteByte((byte)version);
			bcpgOutputStream.WriteLong(keyId);
			bcpgOutputStream.WriteByte((byte)algorithm);
			for (int i = 0; i != data.Length; i++)
			{
				MPInteger.Encode(bcpgOutputStream, data[i]);
			}
			bcpgOut.WritePacket(PacketTag.PublicKeyEncryptedSession, memoryStream.ToArray(), true);
		}
	}
}
