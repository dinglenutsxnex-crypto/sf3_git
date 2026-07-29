using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Cms
{
	internal class KekRecipientInfoGenerator : RecipientInfoGenerator
	{
		private static readonly CmsEnvelopedHelper Helper = CmsEnvelopedHelper.Instance;

		private KeyParameter keyEncryptionKey;

		private string keyEncryptionKeyOID;

		private KekIdentifier kekIdentifier;

		private AlgorithmIdentifier keyEncryptionAlgorithm;

		internal KekIdentifier KekIdentifier
		{
			set
			{
				kekIdentifier = value;
			}
		}

		internal KeyParameter KeyEncryptionKey
		{
			set
			{
				keyEncryptionKey = value;
				keyEncryptionAlgorithm = DetermineKeyEncAlg(keyEncryptionKeyOID, keyEncryptionKey);
			}
		}

		internal string KeyEncryptionKeyOID
		{
			set
			{
				keyEncryptionKeyOID = value;
			}
		}

		internal KekRecipientInfoGenerator()
		{
		}

		public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
		{
			byte[] key = contentEncryptionKey.GetKey();
			IWrapper wrapper = Helper.CreateWrapper(keyEncryptionAlgorithm.ObjectID.Id);
			wrapper.Init(true, new ParametersWithRandom(keyEncryptionKey, random));
			Asn1OctetString encryptedKey = new DerOctetString(wrapper.Wrap(key, 0, key.Length));
			return new RecipientInfo(new KekRecipientInfo(kekIdentifier, keyEncryptionAlgorithm, encryptedKey));
		}

		private static AlgorithmIdentifier DetermineKeyEncAlg(string algorithm, KeyParameter key)
		{
			if (algorithm.StartsWith("DES"))
			{
				return new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgCms3DesWrap, DerNull.Instance);
			}
			if (algorithm.StartsWith("RC2"))
			{
				return new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgCmsRC2Wrap, new DerInteger(58));
			}
			if (algorithm.StartsWith("AES"))
			{
				DerObjectIdentifier objectID;
				switch (key.GetKey().Length * 8)
				{
				case 128:
					objectID = NistObjectIdentifiers.IdAes128Wrap;
					break;
				case 192:
					objectID = NistObjectIdentifiers.IdAes192Wrap;
					break;
				case 256:
					objectID = NistObjectIdentifiers.IdAes256Wrap;
					break;
				default:
					throw new ArgumentException("illegal keysize in AES");
				}
				return new AlgorithmIdentifier(objectID);
			}
			if (algorithm.StartsWith("SEED"))
			{
				return new AlgorithmIdentifier(KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap);
			}
			if (algorithm.StartsWith("CAMELLIA"))
			{
				DerObjectIdentifier objectID2;
				switch (key.GetKey().Length * 8)
				{
				case 128:
					objectID2 = NttObjectIdentifiers.IdCamellia128Wrap;
					break;
				case 192:
					objectID2 = NttObjectIdentifiers.IdCamellia192Wrap;
					break;
				case 256:
					objectID2 = NttObjectIdentifiers.IdCamellia256Wrap;
					break;
				default:
					throw new ArgumentException("illegal keysize in Camellia");
				}
				return new AlgorithmIdentifier(objectID2);
			}
			throw new ArgumentException("unknown algorithm");
		}
	}
}
