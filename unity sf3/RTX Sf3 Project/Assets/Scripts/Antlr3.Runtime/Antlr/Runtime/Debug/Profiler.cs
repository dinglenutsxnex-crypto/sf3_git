using System;
using System.Collections;
using System.IO;
using System.Text;
using Antlr.Runtime.Misc;

namespace Antlr.Runtime.Debug
{
	public class Profiler : BlankDebugEventListener
	{
		public const string Version = "2";

		public const string RUNTIME_STATS_FILENAME = "runtime.stats";

		public const int NUM_RUNTIME_STATS = 29;

		public DebugParser parser;

		protected internal int ruleLevel;

		protected internal int decisionLevel;

		protected internal int maxLookaheadInCurrentDecision;

		protected internal CommonToken lastTokenConsumed;

		protected IList lookaheadStack = new ArrayList();

		public int numRuleInvocations;

		public int numGuessingRuleInvocations;

		public int maxRuleInvocationDepth;

		public int numFixedDecisions;

		public int numCyclicDecisions;

		public int numBacktrackDecisions;

		public int[] decisionMaxFixedLookaheads = new int[200];

		public int[] decisionMaxCyclicLookaheads = new int[200];

		public IList decisionMaxSynPredLookaheads = new ArrayList();

		public int numHiddenTokens;

		public int numCharsMatched;

		public int numHiddenCharsMatched;

		public int numSemanticPredicates;

		public int numSyntacticPredicates;

		protected int numberReportedErrors;

		public int numMemoizationCacheMisses;

		public int numMemoizationCacheHits;

		public int numMemoizationCacheEntries;

		public virtual DebugParser Parser
		{
			set
			{
				parser = value;
			}
		}

		public Profiler()
		{
		}

		public Profiler(DebugParser parser)
		{
			this.parser = parser;
		}

		public override void EnterRule(string grammarFileName, string ruleName)
		{
			ruleLevel++;
			numRuleInvocations++;
			if (ruleLevel > maxRuleInvocationDepth)
			{
				maxRuleInvocationDepth = ruleLevel;
			}
		}

		public void ExamineRuleMemoization(IIntStream input, int ruleIndex, string ruleName)
		{
			int ruleMemoization = parser.GetRuleMemoization(ruleIndex, input.Index());
			if (ruleMemoization == -1)
			{
				numMemoizationCacheMisses++;
				numGuessingRuleInvocations++;
			}
			else
			{
				numMemoizationCacheHits++;
			}
		}

		public void Memoize(IIntStream input, int ruleIndex, int ruleStartIndex, string ruleName)
		{
			numMemoizationCacheEntries++;
		}

		public override void ExitRule(string grammarFileName, string ruleName)
		{
			ruleLevel--;
		}

		public override void EnterDecision(int decisionNumber)
		{
			decisionLevel++;
			int num = parser.TokenStream.Index();
			lookaheadStack.Add(num);
		}

		public override void ExitDecision(int decisionNumber)
		{
			if (parser.isCyclicDecision)
			{
				numCyclicDecisions++;
			}
			else
			{
				numFixedDecisions++;
			}
			lookaheadStack.Remove(lookaheadStack.Count - 1);
			decisionLevel--;
			if (parser.isCyclicDecision)
			{
				if (numCyclicDecisions >= decisionMaxCyclicLookaheads.Length)
				{
					int[] destinationArray = new int[decisionMaxCyclicLookaheads.Length * 2];
					Array.Copy(decisionMaxCyclicLookaheads, 0, destinationArray, 0, decisionMaxCyclicLookaheads.Length);
					decisionMaxCyclicLookaheads = destinationArray;
				}
				decisionMaxCyclicLookaheads[numCyclicDecisions - 1] = maxLookaheadInCurrentDecision;
			}
			else
			{
				if (numFixedDecisions >= decisionMaxFixedLookaheads.Length)
				{
					int[] destinationArray2 = new int[decisionMaxFixedLookaheads.Length * 2];
					Array.Copy(decisionMaxFixedLookaheads, 0, destinationArray2, 0, decisionMaxFixedLookaheads.Length);
					decisionMaxFixedLookaheads = destinationArray2;
				}
				decisionMaxFixedLookaheads[numFixedDecisions - 1] = maxLookaheadInCurrentDecision;
			}
			parser.isCyclicDecision = false;
			maxLookaheadInCurrentDecision = 0;
		}

		public override void ConsumeToken(IToken token)
		{
			lastTokenConsumed = (CommonToken)token;
		}

		public bool InDecision()
		{
			return decisionLevel > 0;
		}

		public override void ConsumeHiddenToken(IToken token)
		{
			lastTokenConsumed = (CommonToken)token;
		}

		public override void LT(int i, IToken t)
		{
			if (InDecision())
			{
				int index = lookaheadStack.Count - 1;
				int num = (int)lookaheadStack[index];
				int num2 = parser.TokenStream.Index();
				int numberOfHiddenTokens = GetNumberOfHiddenTokens(num, num2);
				int num3 = i + num2 - num - numberOfHiddenTokens;
				if (num3 > maxLookaheadInCurrentDecision)
				{
					maxLookaheadInCurrentDecision = num3;
				}
			}
		}

		public override void BeginBacktrack(int level)
		{
			numBacktrackDecisions++;
		}

		public override void EndBacktrack(int level, bool successful)
		{
			decisionMaxSynPredLookaheads.Add(maxLookaheadInCurrentDecision);
		}

		public override void RecognitionException(RecognitionException e)
		{
			numberReportedErrors++;
		}

		public override void SemanticPredicate(bool result, string predicate)
		{
			if (InDecision())
			{
				numSemanticPredicates++;
			}
		}

		public override void Terminate()
		{
			string text = ToNotifyString();
			try
			{
				Stats.WriteReport("runtime.stats", text);
			}
			catch (IOException ex)
			{
				Console.Error.WriteLine(ex);
				Console.Error.WriteLine(ex.StackTrace);
			}
			Console.Out.WriteLine(ToString(text));
		}

		public virtual string ToNotifyString()
		{
			ITokenStream tokenStream = parser.TokenStream;
			for (int i = 0; i < tokenStream.Count; i++)
			{
				if (lastTokenConsumed == null)
				{
					break;
				}
				if (i > lastTokenConsumed.TokenIndex)
				{
					break;
				}
				IToken token = tokenStream.Get(i);
				if (token.Channel != 0)
				{
					numHiddenTokens++;
					numHiddenCharsMatched += token.Text.Length;
				}
			}
			numCharsMatched = lastTokenConsumed.StopIndex + 1;
			decisionMaxFixedLookaheads = Trim(decisionMaxFixedLookaheads, numFixedDecisions);
			decisionMaxCyclicLookaheads = Trim(decisionMaxCyclicLookaheads, numCyclicDecisions);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("2");
			stringBuilder.Append('\t');
			stringBuilder.Append(parser.GetType().FullName);
			stringBuilder.Append('\t');
			stringBuilder.Append(numRuleInvocations);
			stringBuilder.Append('\t');
			stringBuilder.Append(maxRuleInvocationDepth);
			stringBuilder.Append('\t');
			stringBuilder.Append(numFixedDecisions);
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Min(decisionMaxFixedLookaheads));
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Max(decisionMaxFixedLookaheads));
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Avg(decisionMaxFixedLookaheads));
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Stddev(decisionMaxFixedLookaheads));
			stringBuilder.Append('\t');
			stringBuilder.Append(numCyclicDecisions);
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Min(decisionMaxCyclicLookaheads));
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Max(decisionMaxCyclicLookaheads));
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Avg(decisionMaxCyclicLookaheads));
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Stddev(decisionMaxCyclicLookaheads));
			stringBuilder.Append('\t');
			stringBuilder.Append(numBacktrackDecisions);
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Min(ToArray(decisionMaxSynPredLookaheads)));
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Max(ToArray(decisionMaxSynPredLookaheads)));
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Avg(ToArray(decisionMaxSynPredLookaheads)));
			stringBuilder.Append('\t');
			stringBuilder.Append(Stats.Stddev(ToArray(decisionMaxSynPredLookaheads)));
			stringBuilder.Append('\t');
			stringBuilder.Append(numSemanticPredicates);
			stringBuilder.Append('\t');
			stringBuilder.Append(parser.TokenStream.Count);
			stringBuilder.Append('\t');
			stringBuilder.Append(numHiddenTokens);
			stringBuilder.Append('\t');
			stringBuilder.Append(numCharsMatched);
			stringBuilder.Append('\t');
			stringBuilder.Append(numHiddenCharsMatched);
			stringBuilder.Append('\t');
			stringBuilder.Append(numberReportedErrors);
			stringBuilder.Append('\t');
			stringBuilder.Append(numMemoizationCacheHits);
			stringBuilder.Append('\t');
			stringBuilder.Append(numMemoizationCacheMisses);
			stringBuilder.Append('\t');
			stringBuilder.Append(numGuessingRuleInvocations);
			stringBuilder.Append('\t');
			stringBuilder.Append(numMemoizationCacheEntries);
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return ToString(ToNotifyString());
		}

		protected static string[] DecodeReportData(string data)
		{
			string[] array = data.Split('\t');
			if (array.Length != 29)
			{
				return null;
			}
			return array;
		}

		public static string ToString(string notifyDataLine)
		{
			string[] array = DecodeReportData(notifyDataLine);
			if (array == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ANTLR Runtime Report; Profile Version ");
			stringBuilder.Append(array[0]);
			stringBuilder.Append('\n');
			stringBuilder.Append("parser name ");
			stringBuilder.Append(array[1]);
			stringBuilder.Append('\n');
			stringBuilder.Append("Number of rule invocations ");
			stringBuilder.Append(array[2]);
			stringBuilder.Append('\n');
			stringBuilder.Append("Number of rule invocations in \"guessing\" mode ");
			stringBuilder.Append(array[27]);
			stringBuilder.Append('\n');
			stringBuilder.Append("max rule invocation nesting depth ");
			stringBuilder.Append(array[3]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of fixed lookahead decisions ");
			stringBuilder.Append(array[4]);
			stringBuilder.Append('\n');
			stringBuilder.Append("min lookahead used in a fixed lookahead decision ");
			stringBuilder.Append(array[5]);
			stringBuilder.Append('\n');
			stringBuilder.Append("max lookahead used in a fixed lookahead decision ");
			stringBuilder.Append(array[6]);
			stringBuilder.Append('\n');
			stringBuilder.Append("average lookahead depth used in fixed lookahead decisions ");
			stringBuilder.Append(array[7]);
			stringBuilder.Append('\n');
			stringBuilder.Append("standard deviation of depth used in fixed lookahead decisions ");
			stringBuilder.Append(array[8]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of arbitrary lookahead decisions ");
			stringBuilder.Append(array[9]);
			stringBuilder.Append('\n');
			stringBuilder.Append("min lookahead used in an arbitrary lookahead decision ");
			stringBuilder.Append(array[10]);
			stringBuilder.Append('\n');
			stringBuilder.Append("max lookahead used in an arbitrary lookahead decision ");
			stringBuilder.Append(array[11]);
			stringBuilder.Append('\n');
			stringBuilder.Append("average lookahead depth used in arbitrary lookahead decisions ");
			stringBuilder.Append(array[12]);
			stringBuilder.Append('\n');
			stringBuilder.Append("standard deviation of depth used in arbitrary lookahead decisions ");
			stringBuilder.Append(array[13]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of evaluated syntactic predicates ");
			stringBuilder.Append(array[14]);
			stringBuilder.Append('\n');
			stringBuilder.Append("min lookahead used in a syntactic predicate ");
			stringBuilder.Append(array[15]);
			stringBuilder.Append('\n');
			stringBuilder.Append("max lookahead used in a syntactic predicate ");
			stringBuilder.Append(array[16]);
			stringBuilder.Append('\n');
			stringBuilder.Append("average lookahead depth used in syntactic predicates ");
			stringBuilder.Append(array[17]);
			stringBuilder.Append('\n');
			stringBuilder.Append("standard deviation of depth used in syntactic predicates ");
			stringBuilder.Append(array[18]);
			stringBuilder.Append('\n');
			stringBuilder.Append("rule memoization cache size ");
			stringBuilder.Append(array[28]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of rule memoization cache hits ");
			stringBuilder.Append(array[25]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of rule memoization cache misses ");
			stringBuilder.Append(array[26]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of evaluated semantic predicates ");
			stringBuilder.Append(array[19]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of tokens ");
			stringBuilder.Append(array[20]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of hidden tokens ");
			stringBuilder.Append(array[21]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of char ");
			stringBuilder.Append(array[22]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of hidden char ");
			stringBuilder.Append(array[23]);
			stringBuilder.Append('\n');
			stringBuilder.Append("number of syntax errors ");
			stringBuilder.Append(array[24]);
			stringBuilder.Append('\n');
			return stringBuilder.ToString();
		}

		protected int[] Trim(int[] X, int n)
		{
			if (n < X.Length)
			{
				int[] array = new int[n];
				Array.Copy(X, 0, array, 0, n);
				X = array;
			}
			return X;
		}

		protected int[] ToArray(IList a)
		{
			int[] array = new int[a.Count];
			a.CopyTo(array, 0);
			return array;
		}

		public int GetNumberOfHiddenTokens(int i, int j)
		{
			int num = 0;
			ITokenStream tokenStream = parser.TokenStream;
			for (int k = i; k < tokenStream.Count && k <= j; k++)
			{
				IToken token = tokenStream.Get(k);
				if (token.Channel != 0)
				{
					num++;
				}
			}
			return num;
		}
	}
}
