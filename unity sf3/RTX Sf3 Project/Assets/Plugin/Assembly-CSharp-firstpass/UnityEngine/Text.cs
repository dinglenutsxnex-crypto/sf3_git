namespace UnityEngine
{
	public class Text : Object
	{
		private readonly string _content;

		public Text(string str)
		{
			_content = str;
		}

		public override string ToString()
		{
			return _content;
		}

		public static implicit operator string(Text s)
		{
			return s._content;
		}
	}
}
