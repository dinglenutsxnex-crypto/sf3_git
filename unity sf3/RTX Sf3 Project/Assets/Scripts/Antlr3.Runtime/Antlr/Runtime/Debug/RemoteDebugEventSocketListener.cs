using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Antlr.Runtime.Tree;

namespace Antlr.Runtime.Debug
{
	public class RemoteDebugEventSocketListener
	{
		public class ProxyToken : IToken
		{
			internal int index;

			internal int type;

			internal int channel;

			internal int line;

			internal int charPos;

			internal string text;

			public int Type
			{
				get
				{
					return type;
				}
				set
				{
					type = value;
				}
			}

			public int Line
			{
				get
				{
					return line;
				}
				set
				{
					line = value;
				}
			}

			public int CharPositionInLine
			{
				get
				{
					return charPos;
				}
				set
				{
					charPos = value;
				}
			}

			public int Channel
			{
				get
				{
					return channel;
				}
				set
				{
					channel = value;
				}
			}

			public int TokenIndex
			{
				get
				{
					return index;
				}
				set
				{
					index = value;
				}
			}

			public string Text
			{
				get
				{
					return text;
				}
				set
				{
					text = value;
				}
			}

			public ICharStream InputStream
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public ProxyToken(int index)
			{
				this.index = index;
			}

			public ProxyToken(int index, int type, int channel, int line, int charPos, string text)
			{
				this.index = index;
				this.type = type;
				this.channel = channel;
				this.line = line;
				this.charPos = charPos;
				this.text = text;
			}

			public override string ToString()
			{
				string text = string.Empty;
				if (channel != 0)
				{
					text = ",channel=" + channel;
				}
				return "[" + Text + "/<" + type + ">" + text + "," + line + ":" + CharPositionInLine + ",@" + index + "]";
			}
		}

		public class ProxyTree : BaseTree
		{
			public int ID;

			public int type;

			public int line;

			public int charPos = -1;

			public int tokenIndex = -1;

			public string text;

			public override int TokenStartIndex
			{
				get
				{
					return tokenIndex;
				}
				set
				{
				}
			}

			public override int TokenStopIndex
			{
				get
				{
					return 0;
				}
				set
				{
				}
			}

			public override int Type
			{
				get
				{
					return type;
				}
			}

			public override string Text
			{
				get
				{
					return text;
				}
			}

			public ProxyTree(int ID)
			{
				this.ID = ID;
			}

			public ProxyTree(int ID, int type, int line, int charPos, int tokenIndex, string text)
			{
				this.ID = ID;
				this.type = type;
				this.line = line;
				this.charPos = charPos;
				this.tokenIndex = tokenIndex;
				this.text = text;
			}

			public override ITree DupNode()
			{
				return null;
			}

			public override string ToString()
			{
				return "fix this";
			}
		}

		internal const int MAX_EVENT_ELEMENTS = 8;

		internal IDebugEventListener listener;

		internal string hostName;

		internal int port;

		internal TcpClient channel;

		internal StreamWriter writer;

		internal StreamReader reader;

		internal string eventLabel;

		public string version;

		public string grammarFileName;

		private int previousTokenIndex = -1;

		private bool tokenIndexesInvalid;

		public bool TokenIndexesAreInvalid
		{
			get
			{
				return false;
			}
		}

		public RemoteDebugEventSocketListener(IDebugEventListener listener, string hostName, int port)
		{
			this.listener = listener;
			this.hostName = hostName;
			this.port = port;
			if (!OpenConnection())
			{
				throw new Exception();
			}
		}

		protected virtual void EventHandler()
		{
			try
			{
				Handshake();
				for (eventLabel = reader.ReadLine(); eventLabel != null; eventLabel = reader.ReadLine())
				{
					Dispatch(eventLabel);
					Ack();
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				Console.Error.WriteLine(ex.StackTrace);
			}
			finally
			{
				CloseConnection();
			}
		}

		protected virtual bool OpenConnection()
		{
			bool result = false;
			try
			{
				channel = new TcpClient(hostName, port);
				channel.NoDelay = true;
				writer = new StreamWriter(channel.GetStream(), Encoding.UTF8);
				reader = new StreamReader(channel.GetStream(), Encoding.UTF8);
				result = true;
			}
			catch (Exception value)
			{
				Console.Error.WriteLine(value);
			}
			return result;
		}

		protected virtual void CloseConnection()
		{
			try
			{
				reader.Close();
				reader = null;
				writer.Close();
				writer = null;
				channel.Close();
				channel = null;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				Console.Error.WriteLine(ex.StackTrace);
			}
			finally
			{
				if (reader != null)
				{
					try
					{
						reader.Close();
					}
					catch (IOException value)
					{
						Console.Error.WriteLine(value);
					}
				}
				if (writer != null)
				{
					writer.Close();
				}
				if (channel != null)
				{
					try
					{
						channel.Close();
					}
					catch (IOException value2)
					{
						Console.Error.WriteLine(value2);
					}
				}
			}
		}

		protected virtual void Handshake()
		{
			string text = reader.ReadLine();
			string[] eventElements = GetEventElements(text);
			version = eventElements[1];
			string text2 = reader.ReadLine();
			string[] eventElements2 = GetEventElements(text2);
			grammarFileName = eventElements2[1];
			Ack();
			listener.Commence();
		}

		protected virtual void Ack()
		{
			writer.WriteLine("ack");
			writer.Flush();
		}

		protected virtual void Dispatch(string line)
		{
			string[] eventElements = GetEventElements(line);
			if (eventElements == null || eventElements[0] == null)
			{
				Console.Error.WriteLine("unknown debug event: " + line);
				return;
			}
			if (eventElements[0].Equals("enterRule"))
			{
				listener.EnterRule(eventElements[1], eventElements[2]);
				return;
			}
			if (eventElements[0].Equals("exitRule"))
			{
				listener.ExitRule(eventElements[1], eventElements[2]);
				return;
			}
			if (eventElements[0].Equals("enterAlt"))
			{
				listener.EnterAlt(int.Parse(eventElements[1], CultureInfo.InvariantCulture));
				return;
			}
			if (eventElements[0].Equals("enterSubRule"))
			{
				listener.EnterSubRule(int.Parse(eventElements[1], CultureInfo.InvariantCulture));
				return;
			}
			if (eventElements[0].Equals("exitSubRule"))
			{
				listener.ExitSubRule(int.Parse(eventElements[1], CultureInfo.InvariantCulture));
				return;
			}
			if (eventElements[0].Equals("enterDecision"))
			{
				listener.EnterDecision(int.Parse(eventElements[1], CultureInfo.InvariantCulture));
				return;
			}
			if (eventElements[0].Equals("exitDecision"))
			{
				listener.ExitDecision(int.Parse(eventElements[1], CultureInfo.InvariantCulture));
				return;
			}
			if (eventElements[0].Equals("location"))
			{
				listener.Location(int.Parse(eventElements[1], CultureInfo.InvariantCulture), int.Parse(eventElements[2], CultureInfo.InvariantCulture));
				return;
			}
			if (eventElements[0].Equals("consumeToken"))
			{
				ProxyToken proxyToken = DeserializeToken(eventElements, 1);
				if (proxyToken.TokenIndex == previousTokenIndex)
				{
					tokenIndexesInvalid = true;
				}
				previousTokenIndex = proxyToken.TokenIndex;
				listener.ConsumeToken(proxyToken);
				return;
			}
			if (eventElements[0].Equals("consumeHiddenToken"))
			{
				ProxyToken proxyToken2 = DeserializeToken(eventElements, 1);
				if (proxyToken2.TokenIndex == previousTokenIndex)
				{
					tokenIndexesInvalid = true;
				}
				previousTokenIndex = proxyToken2.TokenIndex;
				listener.ConsumeHiddenToken(proxyToken2);
				return;
			}
			if (eventElements[0].Equals("LT"))
			{
				IToken t = DeserializeToken(eventElements, 2);
				listener.LT(int.Parse(eventElements[1], CultureInfo.InvariantCulture), t);
				return;
			}
			if (eventElements[0].Equals("mark"))
			{
				listener.Mark(int.Parse(eventElements[1], CultureInfo.InvariantCulture));
				return;
			}
			if (eventElements[0].Equals("rewind"))
			{
				if (eventElements[1] != null)
				{
					listener.Rewind(int.Parse(eventElements[1], CultureInfo.InvariantCulture));
				}
				else
				{
					listener.Rewind();
				}
				return;
			}
			if (eventElements[0].Equals("beginBacktrack"))
			{
				listener.BeginBacktrack(int.Parse(eventElements[1], CultureInfo.InvariantCulture));
				return;
			}
			if (eventElements[0].Equals("endBacktrack"))
			{
				int level = int.Parse(eventElements[1], CultureInfo.InvariantCulture);
				int num = int.Parse(eventElements[2], CultureInfo.InvariantCulture);
				listener.EndBacktrack(level, num == 1);
				return;
			}
			if (eventElements[0].Equals("exception"))
			{
				string typeName = eventElements[1];
				string s = eventElements[2];
				string s2 = eventElements[3];
				string s3 = eventElements[4];
				Type type = null;
				try
				{
					type = Type.GetType(typeName);
					RecognitionException ex = (RecognitionException)Activator.CreateInstance(type);
					ex.Index = int.Parse(s, CultureInfo.InvariantCulture);
					ex.Line = int.Parse(s2, CultureInfo.InvariantCulture);
					ex.CharPositionInLine = int.Parse(s3, CultureInfo.InvariantCulture);
					listener.RecognitionException(ex);
					return;
				}
				catch (UnauthorizedAccessException ex2)
				{
					Console.Error.WriteLine("can't access class " + ex2);
					Console.Error.WriteLine(ex2.StackTrace);
					return;
				}
			}
			if (eventElements[0].Equals("beginResync"))
			{
				listener.BeginResync();
			}
			else if (eventElements[0].Equals("endResync"))
			{
				listener.EndResync();
			}
			else if (eventElements[0].Equals("terminate"))
			{
				listener.Terminate();
			}
			else if (eventElements[0].Equals("semanticPredicate"))
			{
				bool result = bool.Parse(eventElements[1]);
				string txt = eventElements[2];
				txt = UnEscapeNewlines(txt);
				listener.SemanticPredicate(result, txt);
			}
			else if (eventElements[0].Equals("consumeNode"))
			{
				ProxyTree t2 = DeserializeNode(eventElements, 1);
				listener.ConsumeNode(t2);
			}
			else if (eventElements[0].Equals("LN"))
			{
				int i = int.Parse(eventElements[1], CultureInfo.InvariantCulture);
				ProxyTree t3 = DeserializeNode(eventElements, 2);
				listener.LT(i, t3);
			}
			else if (eventElements[0].Equals("createNodeFromTokenElements"))
			{
				int iD = int.Parse(eventElements[1], CultureInfo.InvariantCulture);
				int type2 = int.Parse(eventElements[2], CultureInfo.InvariantCulture);
				string txt2 = eventElements[3];
				txt2 = UnEscapeNewlines(txt2);
				ProxyTree t4 = new ProxyTree(iD, type2, -1, -1, -1, txt2);
				listener.CreateNode(t4);
			}
			else if (eventElements[0].Equals("createNode"))
			{
				int iD2 = int.Parse(eventElements[1], CultureInfo.InvariantCulture);
				int index = int.Parse(eventElements[2], CultureInfo.InvariantCulture);
				ProxyTree node = new ProxyTree(iD2);
				ProxyToken token = new ProxyToken(index);
				listener.CreateNode(node, token);
			}
			else if (eventElements[0].Equals("nilNode"))
			{
				int iD3 = int.Parse(eventElements[1], CultureInfo.InvariantCulture);
				ProxyTree t5 = new ProxyTree(iD3);
				listener.GetNilNode(t5);
			}
			else if (eventElements[0].Equals("errorNode"))
			{
				int iD4 = int.Parse(eventElements[1], CultureInfo.InvariantCulture);
				int type3 = int.Parse(eventElements[2], CultureInfo.InvariantCulture);
				string txt3 = eventElements[3];
				txt3 = UnEscapeNewlines(txt3);
				ProxyTree t6 = new ProxyTree(iD4, type3, -1, -1, -1, txt3);
				listener.ErrorNode(t6);
			}
			else if (eventElements[0].Equals("becomeRoot"))
			{
				int iD5 = int.Parse(eventElements[1], CultureInfo.InvariantCulture);
				int iD6 = int.Parse(eventElements[2], CultureInfo.InvariantCulture);
				ProxyTree newRoot = new ProxyTree(iD5);
				ProxyTree oldRoot = new ProxyTree(iD6);
				listener.BecomeRoot(newRoot, oldRoot);
			}
			else if (eventElements[0].Equals("addChild"))
			{
				int iD7 = int.Parse(eventElements[1], CultureInfo.InvariantCulture);
				int iD8 = int.Parse(eventElements[2], CultureInfo.InvariantCulture);
				ProxyTree root = new ProxyTree(iD7);
				ProxyTree child = new ProxyTree(iD8);
				listener.AddChild(root, child);
			}
			else if (eventElements[0].Equals("setTokenBoundaries"))
			{
				int iD9 = int.Parse(eventElements[1], CultureInfo.InvariantCulture);
				ProxyTree t7 = new ProxyTree(iD9);
				listener.SetTokenBoundaries(t7, int.Parse(eventElements[2], CultureInfo.InvariantCulture), int.Parse(eventElements[3], CultureInfo.InvariantCulture));
			}
			else
			{
				Console.Error.WriteLine("unknown debug event: " + line);
			}
		}

		protected internal ProxyTree DeserializeNode(string[] elements, int offset)
		{
			int iD = int.Parse(elements[offset], CultureInfo.InvariantCulture);
			int type = int.Parse(elements[offset + 1], CultureInfo.InvariantCulture);
			int line = int.Parse(elements[offset + 2], CultureInfo.InvariantCulture);
			int charPos = int.Parse(elements[offset + 3], CultureInfo.InvariantCulture);
			int tokenIndex = int.Parse(elements[offset + 4], CultureInfo.InvariantCulture);
			string txt = elements[offset + 5];
			txt = UnEscapeNewlines(txt);
			return new ProxyTree(iD, type, line, charPos, tokenIndex, txt);
		}

		protected internal virtual ProxyToken DeserializeToken(string[] elements, int offset)
		{
			string s = elements[offset];
			string s2 = elements[offset + 1];
			string s3 = elements[offset + 2];
			string s4 = elements[offset + 3];
			string s5 = elements[offset + 4];
			string txt = elements[offset + 5];
			txt = UnEscapeNewlines(txt);
			int index = int.Parse(s, CultureInfo.InvariantCulture);
			return new ProxyToken(index, int.Parse(s2, CultureInfo.InvariantCulture), int.Parse(s3, CultureInfo.InvariantCulture), int.Parse(s4, CultureInfo.InvariantCulture), int.Parse(s5, CultureInfo.InvariantCulture), txt);
		}

		public virtual void start()
		{
			Thread thread = new Thread(Run);
			thread.Start();
		}

		public virtual void Run()
		{
			EventHandler();
		}

		public virtual string[] GetEventElements(string eventLabel)
		{
			if (eventLabel == null)
			{
				return null;
			}
			string[] array = new string[8];
			string text = null;
			try
			{
				int num = eventLabel.IndexOf('"');
				if (num >= 0)
				{
					string text2 = eventLabel.Substring(0, num);
					text = eventLabel.Substring(num + 1, eventLabel.Length - (num + 1));
					eventLabel = text2;
				}
				string[] array2 = eventLabel.Split('\t');
				int i;
				for (i = 0; i < array2.Length; i++)
				{
					if (i >= 8)
					{
						return array;
					}
					array[i] = array2[i];
				}
				if (text != null)
				{
					array[i] = text;
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.StackTrace);
			}
			return array;
		}

		protected string UnEscapeNewlines(string txt)
		{
			txt = txt.Replace("%0A", "\n");
			txt = txt.Replace("%0D", "\r");
			txt = txt.Replace("%25", "%");
			return txt;
		}
	}
}
