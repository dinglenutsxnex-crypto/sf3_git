using System;
using System.Collections.Generic;
using System.Globalization;

public class RpnParser
{
	public enum RpnItemType
	{
		Operator = 0,
		Operand = 1
	}

	public delegate double OperatorFunction(List<double> arguments);

	public delegate object ObjectDelegate();

	public delegate object ParameterDelegate(List<object> objects);

	public enum OperatorArgCount
	{
		OperatorAddArgCount = 2,
		OperatorSubArgCount = 2,
		OperatorMultArgCount = 2,
		OperatorDivArgCount = 2,
		OperatorModArgCount = 2,
		OperatorPowArgCount = 2,
		OperatorRootArgCount = 2,
		OperatorBrackLArgCount = 0,
		OperatorBrackRArgCount = 0,
		OperatorSinArgCount = 1,
		OperatorCosArgCount = 1,
		OperatorMaxArgCount = 2,
		OperatorMinArgCount = 2,
		OperatorSqrtArgCount = 1,
		OperatorAbsArgCount = 1,
		OperatorExpArgCount = 1,
		OperatorLnArgCount = 1,
		OperatorLgArgCount = 1,
		OperatorLogArgCount = 2,
		OperatorComparisonArgCount = 2
	}

	public enum OperatorPriority
	{
		OperatorAddPrior = 1,
		OperatorSubPrior = 1,
		OperatorMultPrior = 2,
		OperatorDivPrior = 2,
		OperatorModPrior = 2,
		OperatorPowPrior = 3,
		OperatorSqrtPrior = 3,
		OperatorRootPrior = 3,
		OperatorBrackLPrior = 0,
		OperatorBrackRPrior = 0,
		OperatorSinPrior = 4,
		OperatorCosPrior = 4,
		OperatorMaxPrior = 4,
		OperatorMinPrior = 4,
		OperatorAbsPrior = 4,
		OperatorLnPrior = 4,
		OperatorLgPrior = 4,
		OperatorLogPrior = 4,
		OperatorExpPrior = 4,
		OperatorComparisonPrior = 4
	}

	public enum DirectionOfCalc
	{
		DirectionRight = 0,
		DirectionLeft = 1
	}

	public enum OperatorDirection
	{
		OperatorAddDirect = 0,
		OperatorSubDirect = 0,
		OperatorMultDirect = 0,
		OperatorDivDirect = 0,
		OperatorModDirect = 0,
		OperatorPowDirect = 1,
		OperatorRootDirect = 0,
		OperatorBrackLDirect = 0,
		OperatorBrackRDirect = 0,
		OperatorSinDirect = 1,
		OperatorCosDirect = 1,
		OperatorMaxDirect = 1,
		OperatorMinDirect = 1,
		OperatorAbsDirect = 1,
		OperatorSqrtDirect = 1,
		OperatorExpDirect = 1,
		OperatorLnDirect = 1,
		OperatorLgDirect = 1,
		OperatorLogDirect = 1,
		OperatorComparisonDirect = 0
	}

	private class RpnOperator
	{
		public OperatorPriority prior;

		public OperatorFunction func;

		public OperatorArgCount argCount;

		public OperatorDirection direct;

		public RpnOperator(OperatorPriority _prior, OperatorFunction _func, OperatorArgCount _argCount, OperatorDirection _direct)
		{
			prior = _prior;
			func = _func;
			argCount = _argCount;
			direct = _direct;
		}

		public static double operatorAddFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return arguments[0] + arguments[1];
		}

		public static double operatorSubFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return arguments[0] - arguments[1];
		}

		public static double operatorMultFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return arguments[0] * arguments[1];
		}

		public static double operatorDivFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			if (arguments[1] != 0.0)
			{
				return arguments[0] / arguments[1];
			}
			return 0.0;
		}

		public static double operatorModFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (int)arguments[0] % (int)arguments[1];
		}

		public static double operatorPowFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Math.Pow(arguments[0], arguments[1]);
		}

		public static double operatorRootFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Convert.ToDouble(Convert.ToBoolean(arguments[0]) | Convert.ToBoolean(arguments[1]));
		}

		public static double operatorSinFunc(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Sin(arguments[0]);
		}

		public static double operatorCosFunc(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Cos(arguments[0]);
		}

		public static double operatorMaxFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Math.Max(arguments[0], arguments[1]);
		}

		public static double operatorMinFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Math.Min(arguments[0], arguments[1]);
		}

		public static double operatorSqrtFunc(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Sqrt(arguments[0]);
		}

		public static double operatorAbsFunc(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Abs(arguments[0]);
		}

		public static double operatorLnFunc(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Log(arguments[0], Math.E);
		}

		public static double operatorLgFunc(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Log10(arguments[0]);
		}

		public static double operatorLogFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Math.Log(arguments[0], arguments[1]);
		}

		public static double operatorExpFunc(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Exp(arguments[0]);
		}

		public static double operatorMoreFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] > arguments[1]) ? 1 : 0;
		}

		public static double operatorLessFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] < arguments[1]) ? 1 : 0;
		}

		public static double operatorMoreEqualFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] >= arguments[1]) ? 1 : 0;
		}

		public static double operatorLessEqualFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] <= arguments[1]) ? 1 : 0;
		}

		public static double operatorEqualFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] == arguments[1]) ? 1 : 0;
		}

		public static double operatorNotEqualFunc(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] != arguments[1]) ? 1 : 0;
		}
	}

	private class RpnOperand
	{
		public virtual object getValue()
		{
			return null;
		}
	}

	private class Constant : RpnOperand
	{
		private object _value;

		public Constant(object value)
		{
			_value = value;
		}

		public override object getValue()
		{
			return _value;
		}
	}

	private class Variable : RpnOperand
	{
		private ObjectDelegate _variableDelegate;

		public Variable(ObjectDelegate variableDelegate)
		{
			_variableDelegate = variableDelegate;
		}

		public override object getValue()
		{
			return _variableDelegate();
		}
	}

	private class Function : RpnOperand
	{
		private ParameterDelegate _parameterDelegate;

		private List<RpnOperand> _arguments;

		public Function(List<RpnOperand> arguments, ParameterDelegate functionDelegate)
		{
			_arguments = arguments;
			_parameterDelegate = functionDelegate;
		}

		public override object getValue()
		{
			List<object> list = new List<object>(1);
			foreach (RpnOperand argument in _arguments)
			{
				list.Add(argument.getValue());
			}
			return _parameterDelegate(list);
		}
	}

	private class RpnItem
	{
		public RpnItemType type;

		public RpnOperator rpnOperator;

		public RpnOperand rpnOperand;
	}

	public class Formula
	{
		private List<RpnItem> _items = new List<RpnItem>();

		public Formula(string humanReadableFormula)
		{
			if (!_isInited)
			{
				throw new Exception("RpnParser is not inited!");
			}
			_items = parseFormula(humanReadableFormula);
		}

		public object calculate()
		{
			return RpnParser.calculate(_items);
		}
	}

	private static Dictionary<string, RpnOperator> _mapOperator;

	private static Dictionary<string, ObjectDelegate> _mapObjectsDelegates;

	private static Dictionary<string, ParameterDelegate> _mapParametersDelegates;

	private static bool _isInited;

	private const string SEPARATORS = "+-*^()|/&#";

	private const char SYMBOL_FUNCTION = '?';

	private const char SYMBOL_FUNCTION_SEPARATOR = '.';

	private const char SYMBOL_VARIABLE = '$';

	private const char SYMBOL_COMMA = ',';

	public static void init(Dictionary<string, ObjectDelegate> objectsDelegates, Dictionary<string, ParameterDelegate> parametersDelegates)
	{
		if (!_isInited)
		{
			initMap();
			_mapObjectsDelegates = objectsDelegates;
			_mapParametersDelegates = parametersDelegates;
			_isInited = true;
		}
	}

	private static void initMap()
	{
		if (_mapOperator == null)
		{
			_mapOperator = new Dictionary<string, RpnOperator>(18);
			_mapOperator["+"] = new RpnOperator(OperatorPriority.OperatorAddPrior, RpnOperator.operatorAddFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["-"] = new RpnOperator(OperatorPriority.OperatorAddPrior, RpnOperator.operatorSubFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["*"] = new RpnOperator(OperatorPriority.OperatorMultPrior, RpnOperator.operatorMultFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["/"] = new RpnOperator(OperatorPriority.OperatorMultPrior, RpnOperator.operatorDivFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["%"] = new RpnOperator(OperatorPriority.OperatorMultPrior, RpnOperator.operatorModFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["^"] = new RpnOperator(OperatorPriority.OperatorPowPrior, RpnOperator.operatorPowFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["|"] = new RpnOperator(OperatorPriority.OperatorPowPrior, RpnOperator.operatorRootFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["("] = new RpnOperator(OperatorPriority.OperatorBrackLPrior, null, OperatorArgCount.OperatorBrackLArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator[")"] = new RpnOperator(OperatorPriority.OperatorBrackLPrior, null, OperatorArgCount.OperatorBrackLArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["sin"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorSinFunc, OperatorArgCount.OperatorSinArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["cos"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorCosFunc, OperatorArgCount.OperatorSinArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["max"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorMaxFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["min"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorMinFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["pow"] = new RpnOperator(OperatorPriority.OperatorPowPrior, RpnOperator.operatorPowFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["sqrt"] = new RpnOperator(OperatorPriority.OperatorPowPrior, RpnOperator.operatorSqrtFunc, OperatorArgCount.OperatorSinArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["abs"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorAbsFunc, OperatorArgCount.OperatorSinArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["ln"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorLnFunc, OperatorArgCount.OperatorSinArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["lg"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorLgFunc, OperatorArgCount.OperatorSinArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["log"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorLogFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator["exp"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorExpFunc, OperatorArgCount.OperatorSinArgCount, OperatorDirection.OperatorPowDirect);
			_mapOperator[">"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorMoreFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["<"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorLessFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["=>"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorMoreEqualFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["=<"] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorLessEqualFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["=="] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorEqualFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
			_mapOperator["!="] = new RpnOperator(OperatorPriority.OperatorSinPrior, RpnOperator.operatorNotEqualFunc, OperatorArgCount.OperatorAddArgCount, OperatorDirection.OperatorAddDirect);
		}
	}

	private static List<RpnItem> parseFormula(string formula)
	{
		List<RpnItem> list = new List<RpnItem>();
		if (formula == string.Empty)
		{
			throw new Exception("Formula can not be empty");
		}
		formula = delSpaces(formula);
		formula = addZeros(formula);
		List<RpnItem> list2 = parseExpToArray(formula);
		if (!bracketsCountsAreEqual(list2))
		{
			throw new Exception("Formula can not be empty");
		}
		list = rpnTransform(list2);
		if (list.Count == 0)
		{
			throw new Exception("Formula has no items");
		}
		return list;
	}

	private static object calculate(List<RpnItem> items)
	{
		if (items.Count == 0 && items[0].type == RpnItemType.Operand)
		{
			return items[0].rpnOperand.getValue();
		}
		List<object> list = new List<object>(2);
		for (int i = 0; i != items.Count; i++)
		{
			if (items[i].type == RpnItemType.Operand)
			{
				list.Add(items[i].rpnOperand.getValue());
			}
			else
			{
				if (items[i].type != 0)
				{
					continue;
				}
				List<double> list2 = new List<double>();
				int num = (int)(list.Count - items[i].rpnOperator.argCount);
				for (int j = num; j < list.Count; j++)
				{
					double result = 0.0;
					if (!double.TryParse(list[j].ToString(), out result))
					{
						throw new Exception("Double expected because there are math operators");
					}
					list2.Add(result);
				}
				double num2 = items[i].rpnOperator.func(list2);
				list.RemoveRange(num, (int)items[i].rpnOperator.argCount);
				list.Add(num2);
			}
		}
		return list[0];
	}

	private static string addZeros(string formula)
	{
		for (int i = 0; i < formula.Length; i++)
		{
			if ((formula[i] == '-' || formula[i] == '+') && (i == 0 || formula[i - 1] == '('))
			{
				formula = formula.Insert(i, "0");
			}
		}
		return formula;
	}

	private static string delSpaces(string formula)
	{
		return formula.Replace(" ", string.Empty);
	}

	private static bool isSeparator(string symbol)
	{
		if (!"+-*^()|/&#".Contains(symbol))
		{
			return false;
		}
		return true;
	}

	private static bool stringIsOperator(string str)
	{
		string key = str.ToLower();
		return _mapOperator.ContainsKey(key);
	}

	private static bool stringIsFunction(string str)
	{
		if (str[0] == '?')
		{
			return true;
		}
		foreach (char c in str)
		{
			if (c == '.' || c == '[' || c == ']')
			{
				return true;
			}
		}
		return false;
	}

	private static bool stringIsVariable(string str)
	{
		if (str[0] == '$')
		{
			return true;
		}
		return false;
	}

	private static List<RpnItem> parseExpToArray(string formula)
	{
		List<RpnItem> list = new List<RpnItem>();
		int i = 0;
		int length = formula.Length;
		while (i < length)
		{
			RpnItem rpnItem = new RpnItem();
			if (char.IsDigit(formula[i]))
			{
				string text = string.Empty;
				for (; i < length && (char.IsDigit(formula[i]) || formula[i] == '.'); i++)
				{
					text += formula[i];
				}
				double num = Convert.ToDouble(text, CultureInfo.InvariantCulture);
				rpnItem.type = RpnItemType.Operand;
				rpnItem.rpnOperand = new Constant(num);
			}
			else if (!char.IsDigit(formula[i]))
			{
				if (formula[i] == ',')
				{
					i++;
					continue;
				}
				string text2 = string.Empty;
				while (i < length && (!isSeparator(formula[i].ToString()) || !isSquareBracketsEquals(text2) || text2.Length == 0))
				{
					text2 += formula[i];
					i++;
					if (stringIsOperator(text2))
					{
						break;
					}
				}
				if (stringIsOperator(text2))
				{
					rpnItem.type = RpnItemType.Operator;
					string key = text2.ToLower();
					rpnItem.rpnOperator = _mapOperator[key];
				}
				else
				{
					rpnItem.type = RpnItemType.Operand;
					rpnItem.rpnOperand = parseOperand(text2);
				}
			}
			list.Add(rpnItem);
		}
		return list;
	}

	private static bool isSquareBracketsEquals(string str)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < str.Length; i++)
		{
			switch (str[i])
			{
			case '[':
				num++;
				break;
			case ']':
				num2++;
				break;
			}
		}
		return num == num2;
	}

	private static Function parseFunction(string str)
	{
		int num = str.IndexOf("[");
		string text = str.Substring(0, num);
		string operandsText = str.Substring(num + 1, str.Length - num - 2);
		string text2 = text.Substring(1, text.Length - 1);
		if (!_mapParametersDelegates.ContainsKey(text2))
		{
			throw new Exception("Unknown function name " + text2);
		}
		List<RpnOperand> arguments = parseOperands(operandsText);
		return new Function(arguments, _mapParametersDelegates[text2]);
	}

	private static RpnOperand parseOperand(string operandText)
	{
		RpnOperand rpnOperand = null;
		if (stringIsFunction(operandText))
		{
			operandText = makeCorrectOrder(operandText);
			return parseFunction(operandText);
		}
		if (char.IsDigit(operandText[0]))
		{
			double num = Convert.ToDouble(operandText);
			return new Constant(num);
		}
		if (stringIsVariable(operandText))
		{
			string text = operandText.Substring(1, operandText.Length - 1);
			if (_mapObjectsDelegates.ContainsKey(text))
			{
				return new Variable(_mapObjectsDelegates[text]);
			}
			throw new Exception("Unknown variable " + text);
		}
		return new Constant(operandText);
	}

	private static List<RpnOperand> parseOperands(string operandsText)
	{
		if (operandsText.Length == 0)
		{
			List<RpnOperand> list = new List<RpnOperand>();
			list.Add(new RpnOperand());
			return list;
		}
		List<RpnOperand> list2 = new List<RpnOperand>();
		operandsText = operandsText.Trim();
		string[] array = SplitArguments(operandsText, ',');
		string[] array2 = array;
		foreach (string operandText in array2)
		{
			RpnOperand item = parseOperand(operandText);
			list2.Add(item);
		}
		return list2;
	}

	private static string[] SplitArguments(string expr, char simbol)
	{
		List<string> list = new List<string>();
		int num = 0;
		do
		{
			int num2;
			if (expr[num] == '?')
			{
				num2 = GetEndOfFunc(expr, num) + 1;
			}
			else
			{
				num2 = expr.IndexOf(',', num);
				if (num2 == -1)
				{
					list.Add(expr.Substring(num));
					break;
				}
			}
			list.Add(expr.Substring(num, num2 - num));
			num = num2 + 1;
		}
		while (num < expr.Length);
		return list.ToArray();
	}

	private static int GetEndOfFunc(string expr, int startIndex)
	{
		int num = expr.IndexOf('[', startIndex);
		if (num < 0)
		{
			return -1;
		}
		num++;
		int num2 = 1;
		for (; num < expr.Length; num++)
		{
			if (num2 == 0)
			{
				break;
			}
			if (expr[num] == '[')
			{
				num2++;
			}
			else if (expr[num] == ']')
			{
				num2--;
			}
		}
		return num - 1;
	}

	private static string makeCorrectOrder(string str)
	{
		while (str.Contains("."))
		{
			str = removeLastDot(str);
		}
		return str;
	}

	private static string removeLastDot(string str)
	{
		int num = str.IndexOf('.');
		string text = str.Substring(0, num);
		string text2 = "?" + str.Substring(num + 1, str.Length - num - 1);
		str = text2 + "[" + text + "]";
		return str;
	}

	private static bool bracketsCountsAreEqual(List<RpnItem> items)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i != items.Count; i++)
		{
			if (items[i].type == RpnItemType.Operator)
			{
				if (items[i].rpnOperator == _mapOperator["("])
				{
					num++;
				}
				else if (items[i].rpnOperator == _mapOperator[")"])
				{
					num2++;
				}
			}
		}
		return num == num2;
	}

	private static List<RpnItem> rpnTransform(List<RpnItem> unsortedItems)
	{
		List<RpnItem> list = new List<RpnItem>();
		List<RpnItem> list2 = new List<RpnItem>();
		int num = 0;
		while (num != unsortedItems.Count)
		{
			if (unsortedItems[num].type == RpnItemType.Operand)
			{
				list.Add(unsortedItems[num]);
				num++;
			}
			else if (unsortedItems[num].rpnOperator == _mapOperator["("])
			{
				list2.Add(unsortedItems[num]);
				num++;
			}
			else if (unsortedItems[num].rpnOperator == _mapOperator[")"])
			{
				while (list2.Count != 0 && list2[list2.Count - 1].rpnOperator != _mapOperator["("])
				{
					list.Add(list2[list2.Count - 1]);
					list2.RemoveAt(list2.Count - 1);
				}
				if (list2.Count != 0 && list2[list2.Count - 1].rpnOperator == _mapOperator["("])
				{
					list2.RemoveAt(list2.Count - 1);
					if (list2.Count != 0 && list2[list2.Count - 1].rpnOperator.direct == OperatorDirection.OperatorPowDirect)
					{
						list.Add(list2[list2.Count - 1]);
						list2.RemoveAt(list2.Count - 1);
					}
				}
				num++;
			}
			else if (list2.Count == 0)
			{
				list2.Add(unsortedItems[num]);
				num++;
			}
			else if ((unsortedItems[num].rpnOperator.direct == OperatorDirection.OperatorAddDirect && list2[list2.Count - 1].rpnOperator.prior < unsortedItems[num].rpnOperator.prior) || (unsortedItems[num].rpnOperator.direct == OperatorDirection.OperatorPowDirect && list2[list2.Count - 1].rpnOperator.prior <= unsortedItems[num].rpnOperator.prior))
			{
				list2.Add(unsortedItems[num]);
				num++;
			}
			else if ((unsortedItems[num].rpnOperator.direct == OperatorDirection.OperatorAddDirect && list2[list2.Count - 1].rpnOperator.prior >= unsortedItems[num].rpnOperator.prior) || (unsortedItems[num].rpnOperator.direct == OperatorDirection.OperatorPowDirect && list2[list2.Count - 1].rpnOperator.prior > unsortedItems[num].rpnOperator.prior))
			{
				list.Add(list2[list2.Count - 1]);
				list2.RemoveAt(list2.Count - 1);
			}
		}
		while (list2.Count != 0)
		{
			list.Add(list2[list2.Count - 1]);
			list2.RemoveAt(list2.Count - 1);
		}
		return list;
	}
}
