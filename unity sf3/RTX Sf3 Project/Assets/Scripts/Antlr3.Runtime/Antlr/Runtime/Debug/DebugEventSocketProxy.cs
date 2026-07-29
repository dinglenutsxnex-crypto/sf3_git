using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Antlr.Runtime.Tree;

namespace Antlr.Runtime.Debug
{
	public class DebugEventSocketProxy : BlankDebugEventListener
	{
		public const int DEFAULT_DEBUGGER_PORT = 49100;

		protected int port = 49100;

		protected TcpListener serverSocket;

		protected TcpClient socket;

		protected string grammarFileName;

		protected StreamWriter writer;

		protected StreamReader reader;

		protected BaseRecognizer recognizer;

		protected ITreeAdaptor adaptor;

		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return adaptor;
			}
			set
			{
				adaptor = value;
			}
		}

		public DebugEventSocketProxy(BaseRecognizer recognizer, ITreeAdaptor adaptor)
			: this(recognizer, 49100, adaptor)
		{
		}

		public DebugEventSocketProxy(BaseRecognizer recognizer, int port, ITreeAdaptor adaptor)
		{
			grammarFileName = recognizer.GrammarFileName;
			this.port = port;
			this.adaptor = adaptor;
		}

		public virtual void Handshake()
		{
			if (serverSocket == null)
			{
				serverSocket = new TcpListener(port);
				serverSocket.Start();
				socket = serverSocket.AcceptTcpClient();
				socket.NoDelay = true;
				reader = new StreamReader(socket.GetStream(), Encoding.UTF8);
				writer = new StreamWriter(socket.GetStream(), Encoding.UTF8);
				writer.WriteLine("ANTLR " + Constants.DEBUG_PROTOCOL_VERSION);
				writer.WriteLine("grammar \"" + grammarFileName);
				writer.Flush();
				Ack();
			}
		}

		public override void Commence()
		{
		}

		public override void Terminate()
		{
			Transmit("terminate");
			writer.Close();
			try
			{
				socket.Close();
			}
			catch (IOException ex)
			{
				Console.Error.WriteLine(ex.StackTrace);
			}
		}

		protected internal virtual void Ack()
		{
			try
			{
				reader.ReadLine();
			}
			catch (IOException ex)
			{
				Console.Error.WriteLine(ex.StackTrace);
			}
		}

		protected internal virtual void Transmit(string eventLabel)
		{
			writer.WriteLine(eventLabel);
			writer.Flush();
			Ack();
		}

		public override void EnterRule(string grammarFileName, string ruleName)
		{
			Transmit("enterRule\t" + grammarFileName + "\t" + ruleName);
		}

		public override void EnterAlt(int alt)
		{
			Transmit("enterAlt\t" + alt);
		}

		public override void ExitRule(string grammarFileName, string ruleName)
		{
			Transmit("exitRule\t" + grammarFileName + "\t" + ruleName);
		}

		public override void EnterSubRule(int decisionNumber)
		{
			Transmit("enterSubRule\t" + decisionNumber);
		}

		public override void ExitSubRule(int decisionNumber)
		{
			Transmit("exitSubRule\t" + decisionNumber);
		}

		public override void EnterDecision(int decisionNumber)
		{
			Transmit("enterDecision\t" + decisionNumber);
		}

		public override void ExitDecision(int decisionNumber)
		{
			Transmit("exitDecision\t" + decisionNumber);
		}

		public override void ConsumeToken(IToken t)
		{
			string text = SerializeToken(t);
			Transmit("consumeToken\t" + text);
		}

		public override void ConsumeHiddenToken(IToken t)
		{
			string text = SerializeToken(t);
			Transmit("consumeHiddenToken\t" + text);
		}

		public override void LT(int i, IToken t)
		{
			if (t != null)
			{
				Transmit("LT\t" + i + "\t" + SerializeToken(t));
			}
		}

		public override void Mark(int i)
		{
			Transmit("mark\t" + i);
		}

		public override void Rewind(int i)
		{
			Transmit("rewind\t" + i);
		}

		public override void Rewind()
		{
			Transmit("rewind");
		}

		public override void BeginBacktrack(int level)
		{
			Transmit("beginBacktrack\t" + level);
		}

		public override void EndBacktrack(int level, bool successful)
		{
			Transmit("endBacktrack\t" + level + "\t" + ((!successful) ? false.ToString() : true.ToString()));
		}

		public override void Location(int line, int pos)
		{
			Transmit("location\t" + line + "\t" + pos);
		}

		public override void RecognitionException(RecognitionException e)
		{
			StringBuilder stringBuilder = new StringBuilder(50);
			stringBuilder.Append("exception\t");
			stringBuilder.Append(e.GetType().FullName);
			stringBuilder.Append("\t");
			stringBuilder.Append(e.Index);
			stringBuilder.Append("\t");
			stringBuilder.Append(e.Line);
			stringBuilder.Append("\t");
			stringBuilder.Append(e.CharPositionInLine);
			Transmit(stringBuilder.ToString());
		}

		public override void BeginResync()
		{
			Transmit("beginResync");
		}

		public override void EndResync()
		{
			Transmit("endResync");
		}

		public override void SemanticPredicate(bool result, string predicate)
		{
			StringBuilder stringBuilder = new StringBuilder(50);
			stringBuilder.Append("semanticPredicate\t");
			stringBuilder.Append(result);
			SerializeText(stringBuilder, predicate);
			Transmit(stringBuilder.ToString());
		}

		public override void ConsumeNode(object t)
		{
			StringBuilder stringBuilder = new StringBuilder(50);
			stringBuilder.Append("consumeNode\t");
			SerializeNode(stringBuilder, t);
			Transmit(stringBuilder.ToString());
		}

		public override void LT(int i, object t)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			string nodeText = adaptor.GetNodeText(t);
			int nodeType = adaptor.GetNodeType(t);
			StringBuilder stringBuilder = new StringBuilder(50);
			stringBuilder.Append("LN\t");
			stringBuilder.Append(i);
			SerializeNode(stringBuilder, t);
			Transmit(stringBuilder.ToString());
		}

		public override void GetNilNode(object t)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			Transmit("nilNode\t" + uniqueID);
		}

		public override void ErrorNode(object t)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			string text = t.ToString();
			StringBuilder stringBuilder = new StringBuilder(50);
			stringBuilder.Append("errorNode\t");
			stringBuilder.Append(uniqueID);
			stringBuilder.Append("\t");
			stringBuilder.Append(0);
			SerializeText(stringBuilder, text);
			Transmit(stringBuilder.ToString());
		}

		public override void CreateNode(object t)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			string nodeText = adaptor.GetNodeText(t);
			int nodeType = adaptor.GetNodeType(t);
			StringBuilder stringBuilder = new StringBuilder(50);
			stringBuilder.Append("createNodeFromTokenElements\t");
			stringBuilder.Append(uniqueID);
			stringBuilder.Append("\t");
			stringBuilder.Append(nodeType);
			SerializeText(stringBuilder, nodeText);
			Transmit(stringBuilder.ToString());
		}

		public override void CreateNode(object node, IToken token)
		{
			int uniqueID = adaptor.GetUniqueID(node);
			int tokenIndex = token.TokenIndex;
			Transmit("createNode\t" + uniqueID + "\t" + tokenIndex);
		}

		public override void BecomeRoot(object newRoot, object oldRoot)
		{
			int uniqueID = adaptor.GetUniqueID(newRoot);
			int uniqueID2 = adaptor.GetUniqueID(oldRoot);
			Transmit("becomeRoot\t" + uniqueID + "\t" + uniqueID2);
		}

		public override void AddChild(object root, object child)
		{
			int uniqueID = adaptor.GetUniqueID(root);
			int uniqueID2 = adaptor.GetUniqueID(child);
			Transmit("addChild\t" + uniqueID + "\t" + uniqueID2);
		}

		public override void SetTokenBoundaries(object t, int tokenStartIndex, int tokenStopIndex)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			Transmit("setTokenBoundaries\t" + uniqueID + "\t" + tokenStartIndex + "\t" + tokenStopIndex);
		}

		protected internal virtual string SerializeToken(IToken t)
		{
			StringBuilder stringBuilder = new StringBuilder(50);
			stringBuilder.Append(t.TokenIndex);
			stringBuilder.Append('\t');
			stringBuilder.Append(t.Type);
			stringBuilder.Append('\t');
			stringBuilder.Append(t.Channel);
			stringBuilder.Append('\t');
			stringBuilder.Append(t.Line);
			stringBuilder.Append('\t');
			stringBuilder.Append(t.CharPositionInLine);
			SerializeText(stringBuilder, t.Text);
			return stringBuilder.ToString();
		}

		protected internal virtual string EscapeNewlines(string txt)
		{
			txt = txt.Replace("%", "%25");
			txt = txt.Replace("\n", "%0A");
			txt = txt.Replace("\r", "%0D");
			return txt;
		}

		protected internal void SerializeNode(StringBuilder buf, object t)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			string nodeText = adaptor.GetNodeText(t);
			int nodeType = adaptor.GetNodeType(t);
			buf.Append("\t");
			buf.Append(uniqueID);
			buf.Append("\t");
			buf.Append(nodeType);
			IToken token = adaptor.GetToken(t);
			int value = -1;
			int value2 = -1;
			if (token != null)
			{
				value = token.Line;
				value2 = token.CharPositionInLine;
			}
			buf.Append("\t");
			buf.Append(value);
			buf.Append("\t");
			buf.Append(value2);
			int tokenStartIndex = adaptor.GetTokenStartIndex(t);
			buf.Append("\t");
			buf.Append(tokenStartIndex);
			SerializeText(buf, nodeText);
		}

		protected void SerializeText(StringBuilder buf, string text)
		{
			buf.Append("\t\"");
			if (text == null)
			{
				text = string.Empty;
			}
			text = EscapeNewlines(text);
			buf.Append(text);
		}
	}
}
