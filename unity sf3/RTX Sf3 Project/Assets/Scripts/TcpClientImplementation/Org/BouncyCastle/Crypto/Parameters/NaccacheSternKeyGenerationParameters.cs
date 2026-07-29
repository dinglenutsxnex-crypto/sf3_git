using System;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class NaccacheSternKeyGenerationParameters : KeyGenerationParameters
	{
		private readonly int certainty;

		private readonly int countSmallPrimes;

		private bool debug;

		public int Certainty
		{
			get
			{
				return certainty;
			}
		}

		public int CountSmallPrimes
		{
			get
			{
				return countSmallPrimes;
			}
		}

		public bool IsDebug
		{
			get
			{
				return debug;
			}
		}

		public NaccacheSternKeyGenerationParameters(SecureRandom random, int strength, int certainty, int countSmallPrimes)
			: this(random, strength, certainty, countSmallPrimes, false)
		{
		}

		public NaccacheSternKeyGenerationParameters(SecureRandom random, int strength, int certainty, int countSmallPrimes, bool debug)
			: base(random, strength)
		{
			if (countSmallPrimes % 2 == 1)
			{
				throw new ArgumentException("countSmallPrimes must be a multiple of 2");
			}
			if (countSmallPrimes < 30)
			{
				throw new ArgumentException("countSmallPrimes must be >= 30 for security reasons");
			}
			this.certainty = certainty;
			this.countSmallPrimes = countSmallPrimes;
			this.debug = debug;
		}
	}
}
