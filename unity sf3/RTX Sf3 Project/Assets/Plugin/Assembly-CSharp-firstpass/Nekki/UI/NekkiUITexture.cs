using UnityEngine;

namespace Nekki.UI
{
	public class NekkiUITexture : UITexture
	{
		[SerializeField]
		protected Texture Low;

		[SerializeField]
		protected Texture High;

		public Texture texture
		{
			get
			{
				return mainTexture;
			}
			set
			{
				mainTexture = value;
			}
		}
	}
}
