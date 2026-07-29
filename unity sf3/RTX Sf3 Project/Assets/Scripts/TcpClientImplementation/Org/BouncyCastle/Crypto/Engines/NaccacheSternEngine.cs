using System;
using System.Collections;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class NaccacheSternEngine : IAsymmetricBlockCipher
	{
		private bool forEncryption;

		private NaccacheSternKeyParameters key;

		private IList[] lookup;

		private bool debug;

		public string AlgorithmName
		{
			get
			{
				return "NaccacheStern";
			}
		}

		public virtual bool Debug
		{
			set
			{
				debug = value;
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.forEncryption = forEncryption;
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			key = (NaccacheSternKeyParameters)parameters;
			if (this.forEncryption)
			{
				return;
			}
			if (debug)
			{
				Console.WriteLine("Constructing lookup Array");
			}
			NaccacheSternPrivateKeyParameters naccacheSternPrivateKeyParameters = (NaccacheSternPrivateKeyParameters)key;
			IList smallPrimesList = naccacheSternPrivateKeyParameters.SmallPrimesList;
			lookup = new IList[smallPrimesList.Count];
			for (int i = 0; i < smallPrimesList.Count; i++)
			{
				BigInteger bigInteger = (BigInteger)smallPrimesList[i];
				int intValue = bigInteger.IntValue;
				lookup[i] = Platform.CreateArrayList(intValue);
				lookup[i].Add(BigInteger.One);
				if (debug)
				{
					Console.WriteLine("Constructing lookup ArrayList for " + intValue);
				}
				BigInteger bigInteger2 = BigInteger.Zero;
				for (int j = 1; j < intValue; j++)
				{
					bigInteger2 = bigInteger2.Add(naccacheSternPrivateKeyParameters.PhiN);
					BigInteger e = bigInteger2.Divide(bigInteger);
					lookup[i].Add(naccacheSternPrivateKeyParameters.G.ModPow(e, naccacheSternPrivateKeyParameters.Modulus));
				}
			}
		}

		public virtual int GetInputBlockSize()
		{
			if (forEncryption)
			{
				return (key.LowerSigmaBound + 7) / 8 - 1;
			}
			return key.Modulus.BitLength / 8 + 1;
		}

		public virtual int GetOutputBlockSize()
		{
			if (forEncryption)
			{
				return key.Modulus.BitLength / 8 + 1;
			}
			return (key.LowerSigmaBound + 7) / 8 - 1;
		}

		public virtual byte[] ProcessBlock(byte[] inBytes, int inOff, int length)
		{
			if (key == null)
			{
				throw new InvalidOperationException("NaccacheStern engine not initialised");
			}
			if (length > GetInputBlockSize() + 1)
			{
				throw new DataLengthException("input too large for Naccache-Stern cipher.\n");
			}
			if (!forEncryption && length < GetInputBlockSize())
			{
				throw new InvalidCipherTextException("BlockLength does not match modulus for Naccache-Stern cipher.\n");
			}
			BigInteger bigInteger = new BigInteger(1, inBytes, inOff, length);
			if (debug)
			{
				Console.WriteLine("input as BigInteger: " + bigInteger);
			}
			if (forEncryption)
			{
				return Encrypt(bigInteger);
			}
			IList list = Platform.CreateArrayList();
			NaccacheSternPrivateKeyParameters naccacheSternPrivateKeyParameters = (NaccacheSternPrivateKeyParameters)key;
			IList smallPrimesList = naccacheSternPrivateKeyParameters.SmallPrimesList;
			for (int i = 0; i < smallPrimesList.Count; i++)
			{
				BigInteger bigInteger2 = bigInteger.ModPow(naccacheSternPrivateKeyParameters.PhiN.Divide((BigInteger)smallPrimesList[i]), naccacheSternPrivateKeyParameters.Modulus);
				IList list2 = lookup[i];
				if (lookup[i].Count != ((BigInteger)smallPrimesList[i]).IntValue)
				{
					if (debug)
					{
						Console.WriteLine(string.Concat("Prime is ", smallPrimesList[i], ", lookup table has size ", list2.Count));
					}
					throw new InvalidCipherTextException("Error in lookup Array for " + ((BigInteger)smallPrimesList[i]).IntValue + ": Size mismatch. Expected ArrayList with length " + ((BigInteger)smallPrimesList[i]).IntValue + " but found ArrayList of length " + lookup[i].Count);
				}
				int num = list2.IndexOf(bigInteger2);
				if (num == -1)
				{
					if (debug)
					{
						Console.WriteLine("Actual prime is " + smallPrimesList[i]);
						Console.WriteLine("Decrypted value is " + bigInteger2);
						Console.WriteLine(string.Concat("LookupList for ", smallPrimesList[i], " with size ", lookup[i].Count, " is: "));
						for (int j = 0; j < lookup[i].Count; j++)
						{
							Console.WriteLine(lookup[i][j]);
						}
					}
					throw new InvalidCipherTextException("Lookup failed");
				}
				list.Add(BigInteger.ValueOf(num));
			}
			BigInteger bigInteger3 = chineseRemainder(list, smallPrimesList);
			return bigInteger3.ToByteArray();
		}

		public virtual byte[] Encrypt(BigInteger plain)
		{
			byte[] array = new byte[key.Modulus.BitLength / 8 + 1];
			byte[] array2 = key.G.ModPow(plain, key.Modulus).ToByteArray();
			Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
			if (debug)
			{
				Console.WriteLine("Encrypted value is:  " + new BigInteger(array));
			}
			return array;
		}

		public virtual byte[] AddCryptedBlocks(byte[] block1, byte[] block2)
		{
			if (forEncryption)
			{
				if (block1.Length > GetOutputBlockSize() || block2.Length > GetOutputBlockSize())
				{
					throw new InvalidCipherTextException("BlockLength too large for simple addition.\n");
				}
			}
			else if (block1.Length > GetInputBlockSize() || block2.Length > GetInputBlockSize())
			{
				throw new InvalidCipherTextException("BlockLength too large for simple addition.\n");
			}
			BigInteger bigInteger = new BigInteger(1, block1);
			BigInteger bigInteger2 = new BigInteger(1, block2);
			BigInteger bigInteger3 = bigInteger.Multiply(bigInteger2);
			bigInteger3 = bigInteger3.Mod(key.Modulus);
			if (debug)
			{
				Console.WriteLine("c(m1) as BigInteger:....... " + bigInteger);
				Console.WriteLine("c(m2) as BigInteger:....... " + bigInteger2);
				Console.WriteLine("c(m1)*c(m2)%n = c(m1+m2)%n: " + bigInteger3);
			}
			byte[] array = new byte[key.Modulus.BitLength / 8 + 1];
			byte[] array2 = bigInteger3.ToByteArray();
			Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
			return array;
		}

		public virtual byte[] ProcessData(byte[] data)
		{
			if (debug)
			{
				Console.WriteLine();
			}
			if (data.Length > GetInputBlockSize())
			{
				int inputBlockSize = GetInputBlockSize();
				int outputBlockSize = GetOutputBlockSize();
				if (debug)
				{
					Console.WriteLine("Input blocksize is:  " + inputBlockSize + " bytes");
					Console.WriteLine("Output blocksize is: " + outputBlockSize + " bytes");
					Console.WriteLine("Data has length:.... " + data.Length + " bytes");
				}
				int num = 0;
				int num2 = 0;
				byte[] array = new byte[(data.Length / inputBlockSize + 1) * outputBlockSize];
				while (num < data.Length)
				{
					byte[] array2;
					if (num + inputBlockSize < data.Length)
					{
						array2 = ProcessBlock(data, num, inputBlockSize);
						num += inputBlockSize;
					}
					else
					{
						array2 = ProcessBlock(data, num, data.Length - num);
						num += data.Length - num;
					}
					if (debug)
					{
						Console.WriteLine("new datapos is " + num);
					}
					if (array2 != null)
					{
						array2.CopyTo(array, num2);
						num2 += array2.Length;
						continue;
					}
					if (debug)
					{
						Console.WriteLine("cipher returned null");
					}
					throw new InvalidCipherTextException("cipher returned null");
				}
				byte[] array3 = new byte[num2];
				Array.Copy(array, 0, array3, 0, num2);
				if (debug)
				{
					Console.WriteLine("returning " + array3.Length + " bytes");
				}
				return array3;
			}
			if (debug)
			{
				Console.WriteLine("data size is less then input block size, processing directly");
			}
			return ProcessBlock(data, 0, data.Length);
		}

		private static BigInteger chineseRemainder(IList congruences, IList primes)
		{
			BigInteger bigInteger = BigInteger.Zero;
			BigInteger bigInteger2 = BigInteger.One;
			for (int i = 0; i < primes.Count; i++)
			{
				bigInteger2 = bigInteger2.Multiply((BigInteger)primes[i]);
			}
			for (int j = 0; j < primes.Count; j++)
			{
				BigInteger bigInteger3 = (BigInteger)primes[j];
				BigInteger bigInteger4 = bigInteger2.Divide(bigInteger3);
				BigInteger val = bigInteger4.ModInverse(bigInteger3);
				BigInteger bigInteger5 = bigInteger4.Multiply(val);
				bigInteger5 = bigInteger5.Multiply((BigInteger)congruences[j]);
				bigInteger = bigInteger.Add(bigInteger5);
			}
			return bigInteger.Mod(bigInteger2);
		}
	}
}
