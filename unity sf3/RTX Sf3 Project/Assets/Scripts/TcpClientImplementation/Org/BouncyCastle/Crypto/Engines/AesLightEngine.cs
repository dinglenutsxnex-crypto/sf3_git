using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class AesLightEngine : IBlockCipher
	{
		private const uint m1 = 2155905152u;

		private const uint m2 = 2139062143u;

		private const uint m3 = 27u;

		private const int BLOCK_SIZE = 16;

		private static readonly byte[] S = new byte[256]
		{
			99, 124, 119, 123, 242, 107, 111, 197, 48, 1,
			103, 43, 254, 215, 171, 118, 202, 130, 201, 125,
			250, 89, 71, 240, 173, 212, 162, 175, 156, 164,
			114, 192, 183, 253, 147, 38, 54, 63, 247, 204,
			52, 165, 229, 241, 113, 216, 49, 21, 4, 199,
			35, 195, 24, 150, 5, 154, 7, 18, 128, 226,
			235, 39, 178, 117, 9, 131, 44, 26, 27, 110,
			90, 160, 82, 59, 214, 179, 41, 227, 47, 132,
			83, 209, 0, 237, 32, 252, 177, 91, 106, 203,
			190, 57, 74, 76, 88, 207, 208, 239, 170, 251,
			67, 77, 51, 133, 69, 249, 2, 127, 80, 60,
			159, 168, 81, 163, 64, 143, 146, 157, 56, 245,
			188, 182, 218, 33, 16, 255, 243, 210, 205, 12,
			19, 236, 95, 151, 68, 23, 196, 167, 126, 61,
			100, 93, 25, 115, 96, 129, 79, 220, 34, 42,
			144, 136, 70, 238, 184, 20, 222, 94, 11, 219,
			224, 50, 58, 10, 73, 6, 36, 92, 194, 211,
			172, 98, 145, 149, 228, 121, 231, 200, 55, 109,
			141, 213, 78, 169, 108, 86, 244, 234, 101, 122,
			174, 8, 186, 120, 37, 46, 28, 166, 180, 198,
			232, 221, 116, 31, 75, 189, 139, 138, 112, 62,
			181, 102, 72, 3, 246, 14, 97, 53, 87, 185,
			134, 193, 29, 158, 225, 248, 152, 17, 105, 217,
			142, 148, 155, 30, 135, 233, 206, 85, 40, 223,
			140, 161, 137, 13, 191, 230, 66, 104, 65, 153,
			45, 15, 176, 84, 187, 22
		};

		private static readonly byte[] Si = new byte[256]
		{
			82, 9, 106, 213, 48, 54, 165, 56, 191, 64,
			163, 158, 129, 243, 215, 251, 124, 227, 57, 130,
			155, 47, 255, 135, 52, 142, 67, 68, 196, 222,
			233, 203, 84, 123, 148, 50, 166, 194, 35, 61,
			238, 76, 149, 11, 66, 250, 195, 78, 8, 46,
			161, 102, 40, 217, 36, 178, 118, 91, 162, 73,
			109, 139, 209, 37, 114, 248, 246, 100, 134, 104,
			152, 22, 212, 164, 92, 204, 93, 101, 182, 146,
			108, 112, 72, 80, 253, 237, 185, 218, 94, 21,
			70, 87, 167, 141, 157, 132, 144, 216, 171, 0,
			140, 188, 211, 10, 247, 228, 88, 5, 184, 179,
			69, 6, 208, 44, 30, 143, 202, 63, 15, 2,
			193, 175, 189, 3, 1, 19, 138, 107, 58, 145,
			17, 65, 79, 103, 220, 234, 151, 242, 207, 206,
			240, 180, 230, 115, 150, 172, 116, 34, 231, 173,
			53, 133, 226, 249, 55, 232, 28, 117, 223, 110,
			71, 241, 26, 113, 29, 41, 197, 137, 111, 183,
			98, 14, 170, 24, 190, 27, 252, 86, 62, 75,
			198, 210, 121, 32, 154, 219, 192, 254, 120, 205,
			90, 244, 31, 221, 168, 51, 136, 7, 199, 49,
			177, 18, 16, 89, 39, 128, 236, 95, 96, 81,
			127, 169, 25, 181, 74, 13, 45, 229, 122, 159,
			147, 201, 156, 239, 160, 224, 59, 77, 174, 42,
			245, 176, 200, 235, 187, 60, 131, 83, 153, 97,
			23, 43, 4, 126, 186, 119, 214, 38, 225, 105,
			20, 99, 85, 33, 12, 125
		};

		private static readonly byte[] rcon = new byte[30]
		{
			1, 2, 4, 8, 16, 32, 64, 128, 27, 54,
			108, 216, 171, 77, 154, 47, 94, 188, 99, 198,
			151, 53, 106, 212, 179, 125, 250, 239, 197, 145
		};

		private int ROUNDS;

		private uint[][] WorkingKey;

		private uint C0;

		private uint C1;

		private uint C2;

		private uint C3;

		private bool forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "AES";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		private static uint Shift(uint r, int shift)
		{
			return (r >> shift) | (r << 32 - shift);
		}

		private static uint FFmulX(uint x)
		{
			return ((x & 0x7F7F7F7F) << 1) ^ (((x & 0x80808080u) >> 7) * 27);
		}

		private static uint Mcol(uint x)
		{
			uint num = FFmulX(x);
			return num ^ Shift(x ^ num, 8) ^ Shift(x, 16) ^ Shift(x, 24);
		}

		private static uint Inv_Mcol(uint x)
		{
			uint num = FFmulX(x);
			uint num2 = FFmulX(num);
			uint num3 = FFmulX(num2);
			uint num4 = x ^ num3;
			return num ^ num2 ^ num3 ^ Shift(num ^ num4, 8) ^ Shift(num2 ^ num4, 16) ^ Shift(num4, 24);
		}

		private static uint SubWord(uint x)
		{
			return (uint)(S[x & 0xFF] | (S[(x >> 8) & 0xFF] << 8) | (S[(x >> 16) & 0xFF] << 16) | (S[(x >> 24) & 0xFF] << 24));
		}

		private uint[][] GenerateWorkingKey(byte[] key, bool forEncryption)
		{
			int num = key.Length / 4;
			if (num != 4 && num != 6 && num != 8)
			{
				throw new ArgumentException("Key length not 128/192/256 bits.");
			}
			ROUNDS = num + 6;
			uint[][] array = new uint[ROUNDS + 1][];
			for (int i = 0; i <= ROUNDS; i++)
			{
				array[i] = new uint[4];
			}
			int num2 = 0;
			int num3 = 0;
			while (num3 < key.Length)
			{
				array[num2 >> 2][num2 & 3] = Pack.LE_To_UInt32(key, num3);
				num3 += 4;
				num2++;
			}
			int num4 = ROUNDS + 1 << 2;
			for (int j = num; j < num4; j++)
			{
				uint num5 = array[j - 1 >> 2][(j - 1) & 3];
				if (j % num == 0)
				{
					num5 = SubWord(Shift(num5, 8)) ^ rcon[j / num - 1];
				}
				else if (num > 6 && j % num == 4)
				{
					num5 = SubWord(num5);
				}
				array[j >> 2][j & 3] = array[j - num >> 2][(j - num) & 3] ^ num5;
			}
			if (!forEncryption)
			{
				for (int k = 1; k < ROUNDS; k++)
				{
					uint[] array2 = array[k];
					for (int l = 0; l < 4; l++)
					{
						array2[l] = Inv_Mcol(array2[l]);
					}
				}
			}
			return array;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			KeyParameter keyParameter = parameters as KeyParameter;
			if (keyParameter == null)
			{
				throw new ArgumentException("invalid parameter passed to AES init - " + parameters.GetType().Name);
			}
			WorkingKey = GenerateWorkingKey(keyParameter.GetKey(), forEncryption);
			this.forEncryption = forEncryption;
		}

		public virtual int GetBlockSize()
		{
			return 16;
		}

		public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (WorkingKey == null)
			{
				throw new InvalidOperationException("AES engine not initialised");
			}
			Check.DataLength(input, inOff, 16, "input buffer too short");
			Check.OutputLength(output, outOff, 16, "output buffer too short");
			UnPackBlock(input, inOff);
			if (forEncryption)
			{
				EncryptBlock(WorkingKey);
			}
			else
			{
				DecryptBlock(WorkingKey);
			}
			PackBlock(output, outOff);
			return 16;
		}

		public virtual void Reset()
		{
		}

		private void UnPackBlock(byte[] bytes, int off)
		{
			C0 = Pack.LE_To_UInt32(bytes, off);
			C1 = Pack.LE_To_UInt32(bytes, off + 4);
			C2 = Pack.LE_To_UInt32(bytes, off + 8);
			C3 = Pack.LE_To_UInt32(bytes, off + 12);
		}

		private void PackBlock(byte[] bytes, int off)
		{
			Pack.UInt32_To_LE(C0, bytes, off);
			Pack.UInt32_To_LE(C1, bytes, off + 4);
			Pack.UInt32_To_LE(C2, bytes, off + 8);
			Pack.UInt32_To_LE(C3, bytes, off + 12);
		}

		private void EncryptBlock(uint[][] KW)
		{
			uint[] array = KW[0];
			uint num = C0 ^ array[0];
			uint num2 = C1 ^ array[1];
			uint num3 = C2 ^ array[2];
			uint num4 = C3 ^ array[3];
			int num5 = 1;
			uint num6;
			uint num7;
			uint num8;
			while (num5 < ROUNDS - 1)
			{
				array = KW[num5++];
				num6 = Mcol((uint)(S[num & 0xFF] ^ (S[(num2 >> 8) & 0xFF] << 8) ^ (S[(num3 >> 16) & 0xFF] << 16) ^ (S[(num4 >> 24) & 0xFF] << 24))) ^ array[0];
				num7 = Mcol((uint)(S[num2 & 0xFF] ^ (S[(num3 >> 8) & 0xFF] << 8) ^ (S[(num4 >> 16) & 0xFF] << 16) ^ (S[(num >> 24) & 0xFF] << 24))) ^ array[1];
				num8 = Mcol((uint)(S[num3 & 0xFF] ^ (S[(num4 >> 8) & 0xFF] << 8) ^ (S[(num >> 16) & 0xFF] << 16) ^ (S[(num2 >> 24) & 0xFF] << 24))) ^ array[2];
				num4 = Mcol((uint)(S[num4 & 0xFF] ^ (S[(num >> 8) & 0xFF] << 8) ^ (S[(num2 >> 16) & 0xFF] << 16) ^ (S[(num3 >> 24) & 0xFF] << 24))) ^ array[3];
				array = KW[num5++];
				num = Mcol((uint)(S[num6 & 0xFF] ^ (S[(num7 >> 8) & 0xFF] << 8) ^ (S[(num8 >> 16) & 0xFF] << 16) ^ (S[(num4 >> 24) & 0xFF] << 24))) ^ array[0];
				num2 = Mcol((uint)(S[num7 & 0xFF] ^ (S[(num8 >> 8) & 0xFF] << 8) ^ (S[(num4 >> 16) & 0xFF] << 16) ^ (S[(num6 >> 24) & 0xFF] << 24))) ^ array[1];
				num3 = Mcol((uint)(S[num8 & 0xFF] ^ (S[(num4 >> 8) & 0xFF] << 8) ^ (S[(num6 >> 16) & 0xFF] << 16) ^ (S[(num7 >> 24) & 0xFF] << 24))) ^ array[2];
				num4 = Mcol((uint)(S[num4 & 0xFF] ^ (S[(num6 >> 8) & 0xFF] << 8) ^ (S[(num7 >> 16) & 0xFF] << 16) ^ (S[(num8 >> 24) & 0xFF] << 24))) ^ array[3];
			}
			array = KW[num5++];
			num6 = Mcol((uint)(S[num & 0xFF] ^ (S[(num2 >> 8) & 0xFF] << 8) ^ (S[(num3 >> 16) & 0xFF] << 16) ^ (S[(num4 >> 24) & 0xFF] << 24))) ^ array[0];
			num7 = Mcol((uint)(S[num2 & 0xFF] ^ (S[(num3 >> 8) & 0xFF] << 8) ^ (S[(num4 >> 16) & 0xFF] << 16) ^ (S[(num >> 24) & 0xFF] << 24))) ^ array[1];
			num8 = Mcol((uint)(S[num3 & 0xFF] ^ (S[(num4 >> 8) & 0xFF] << 8) ^ (S[(num >> 16) & 0xFF] << 16) ^ (S[(num2 >> 24) & 0xFF] << 24))) ^ array[2];
			num4 = Mcol((uint)(S[num4 & 0xFF] ^ (S[(num >> 8) & 0xFF] << 8) ^ (S[(num2 >> 16) & 0xFF] << 16) ^ (S[(num3 >> 24) & 0xFF] << 24))) ^ array[3];
			array = KW[num5];
			C0 = (uint)(S[num6 & 0xFF] ^ (S[(num7 >> 8) & 0xFF] << 8) ^ (S[(num8 >> 16) & 0xFF] << 16) ^ (S[(num4 >> 24) & 0xFF] << 24)) ^ array[0];
			C1 = (uint)(S[num7 & 0xFF] ^ (S[(num8 >> 8) & 0xFF] << 8) ^ (S[(num4 >> 16) & 0xFF] << 16) ^ (S[(num6 >> 24) & 0xFF] << 24)) ^ array[1];
			C2 = (uint)(S[num8 & 0xFF] ^ (S[(num4 >> 8) & 0xFF] << 8) ^ (S[(num6 >> 16) & 0xFF] << 16) ^ (S[(num7 >> 24) & 0xFF] << 24)) ^ array[2];
			C3 = (uint)(S[num4 & 0xFF] ^ (S[(num6 >> 8) & 0xFF] << 8) ^ (S[(num7 >> 16) & 0xFF] << 16) ^ (S[(num8 >> 24) & 0xFF] << 24)) ^ array[3];
		}

		private void DecryptBlock(uint[][] KW)
		{
			uint[] array = KW[ROUNDS];
			uint num = C0 ^ array[0];
			uint num2 = C1 ^ array[1];
			uint num3 = C2 ^ array[2];
			uint num4 = C3 ^ array[3];
			int num5 = ROUNDS - 1;
			uint num6;
			uint num7;
			uint num8;
			while (num5 > 1)
			{
				array = KW[num5--];
				num6 = Inv_Mcol((uint)(Si[num & 0xFF] ^ (Si[(num4 >> 8) & 0xFF] << 8) ^ (Si[(num3 >> 16) & 0xFF] << 16) ^ (Si[(num2 >> 24) & 0xFF] << 24))) ^ array[0];
				num7 = Inv_Mcol((uint)(Si[num2 & 0xFF] ^ (Si[(num >> 8) & 0xFF] << 8) ^ (Si[(num4 >> 16) & 0xFF] << 16) ^ (Si[(num3 >> 24) & 0xFF] << 24))) ^ array[1];
				num8 = Inv_Mcol((uint)(Si[num3 & 0xFF] ^ (Si[(num2 >> 8) & 0xFF] << 8) ^ (Si[(num >> 16) & 0xFF] << 16) ^ (Si[(num4 >> 24) & 0xFF] << 24))) ^ array[2];
				num4 = Inv_Mcol((uint)(Si[num4 & 0xFF] ^ (Si[(num3 >> 8) & 0xFF] << 8) ^ (Si[(num2 >> 16) & 0xFF] << 16) ^ (Si[(num >> 24) & 0xFF] << 24))) ^ array[3];
				array = KW[num5--];
				num = Inv_Mcol((uint)(Si[num6 & 0xFF] ^ (Si[(num4 >> 8) & 0xFF] << 8) ^ (Si[(num8 >> 16) & 0xFF] << 16) ^ (Si[(num7 >> 24) & 0xFF] << 24))) ^ array[0];
				num2 = Inv_Mcol((uint)(Si[num7 & 0xFF] ^ (Si[(num6 >> 8) & 0xFF] << 8) ^ (Si[(num4 >> 16) & 0xFF] << 16) ^ (Si[(num8 >> 24) & 0xFF] << 24))) ^ array[1];
				num3 = Inv_Mcol((uint)(Si[num8 & 0xFF] ^ (Si[(num7 >> 8) & 0xFF] << 8) ^ (Si[(num6 >> 16) & 0xFF] << 16) ^ (Si[(num4 >> 24) & 0xFF] << 24))) ^ array[2];
				num4 = Inv_Mcol((uint)(Si[num4 & 0xFF] ^ (Si[(num8 >> 8) & 0xFF] << 8) ^ (Si[(num7 >> 16) & 0xFF] << 16) ^ (Si[(num6 >> 24) & 0xFF] << 24))) ^ array[3];
			}
			array = KW[1];
			num6 = Inv_Mcol((uint)(Si[num & 0xFF] ^ (Si[(num4 >> 8) & 0xFF] << 8) ^ (Si[(num3 >> 16) & 0xFF] << 16) ^ (Si[(num2 >> 24) & 0xFF] << 24))) ^ array[0];
			num7 = Inv_Mcol((uint)(Si[num2 & 0xFF] ^ (Si[(num >> 8) & 0xFF] << 8) ^ (Si[(num4 >> 16) & 0xFF] << 16) ^ (Si[(num3 >> 24) & 0xFF] << 24))) ^ array[1];
			num8 = Inv_Mcol((uint)(Si[num3 & 0xFF] ^ (Si[(num2 >> 8) & 0xFF] << 8) ^ (Si[(num >> 16) & 0xFF] << 16) ^ (Si[(num4 >> 24) & 0xFF] << 24))) ^ array[2];
			num4 = Inv_Mcol((uint)(Si[num4 & 0xFF] ^ (Si[(num3 >> 8) & 0xFF] << 8) ^ (Si[(num2 >> 16) & 0xFF] << 16) ^ (Si[(num >> 24) & 0xFF] << 24))) ^ array[3];
			array = KW[0];
			C0 = (uint)(Si[num6 & 0xFF] ^ (Si[(num4 >> 8) & 0xFF] << 8) ^ (Si[(num8 >> 16) & 0xFF] << 16) ^ (Si[(num7 >> 24) & 0xFF] << 24)) ^ array[0];
			C1 = (uint)(Si[num7 & 0xFF] ^ (Si[(num6 >> 8) & 0xFF] << 8) ^ (Si[(num4 >> 16) & 0xFF] << 16) ^ (Si[(num8 >> 24) & 0xFF] << 24)) ^ array[1];
			C2 = (uint)(Si[num8 & 0xFF] ^ (Si[(num7 >> 8) & 0xFF] << 8) ^ (Si[(num6 >> 16) & 0xFF] << 16) ^ (Si[(num4 >> 24) & 0xFF] << 24)) ^ array[2];
			C3 = (uint)(Si[num4 & 0xFF] ^ (Si[(num8 >> 8) & 0xFF] << 8) ^ (Si[(num7 >> 16) & 0xFF] << 16) ^ (Si[(num6 >> 24) & 0xFF] << 24)) ^ array[3];
		}
	}
}
