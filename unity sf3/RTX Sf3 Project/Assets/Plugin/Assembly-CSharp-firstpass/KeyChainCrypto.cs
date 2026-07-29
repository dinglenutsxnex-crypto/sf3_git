using System;
using System.Security.Cryptography;
using System.Text;

public class KeyChainCrypto
{
	private static void GetKeys(string secret, out byte[] key, out byte[] iv)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(secret);
		key = new byte[8];
		iv = new byte[8];
		for (int i = 0; i < key.Length; i++)
		{
			if (bytes.Length <= i)
			{
				key[i] = bytes[bytes.Length - 1];
			}
			else
			{
				key[i] = bytes[i];
			}
		}
		for (int j = 0; j < iv.Length; j++)
		{
			if (bytes.Length - 1 - j < 0)
			{
				iv[j] = bytes[0];
			}
			else
			{
				iv[j] = bytes[bytes.Length - 1 - j];
			}
		}
	}

	public static string Crypt(string text, string secret)
	{
		byte[] key;
		byte[] iv;
		GetKeys(secret, out key, out iv);
		SymmetricAlgorithm symmetricAlgorithm = DES.Create();
		ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor(key, iv);
		byte[] bytes = Encoding.Unicode.GetBytes(text);
		byte[] inArray = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
		return Convert.ToBase64String(inArray);
	}

	public static string Decrypt(string text, string secret)
	{
		byte[] key;
		byte[] iv;
		GetKeys(secret, out key, out iv);
		SymmetricAlgorithm symmetricAlgorithm = DES.Create();
		ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor(key, iv);
		byte[] array = Convert.FromBase64String(text);
		byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		return Encoding.Unicode.GetString(bytes);
	}
}
