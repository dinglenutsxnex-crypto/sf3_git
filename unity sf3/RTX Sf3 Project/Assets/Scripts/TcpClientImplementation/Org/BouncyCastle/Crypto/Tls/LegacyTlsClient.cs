using System;
using System.Collections.Generic;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class LegacyTlsClient : DefaultTlsClient
	{
		protected Uri TargetUri;

		protected ICertificateVerifyer verifyer;

		protected IClientCredentialsProvider credProvider;

		public LegacyTlsClient(Uri targetUri, ICertificateVerifyer verifyer, IClientCredentialsProvider prov, List<string> hostNames)
		{
			TargetUri = targetUri;
			this.verifyer = verifyer;
			credProvider = prov;
			base.HostNames = hostNames;
		}

		public override TlsAuthentication GetAuthentication()
		{
			return new LegacyTlsAuthentication(TargetUri, verifyer, credProvider);
		}
	}
}
