using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jint.Native;
using Jint.Native.Argument;
using Jint.Native.Array;
using Jint.Native.Boolean;
using Jint.Native.Date;
using Jint.Native.Error;
using Jint.Native.Function;
using Jint.Native.Global;
using Jint.Native.Json;
using Jint.Native.Math;
using Jint.Native.Number;
using Jint.Native.Object;
using Jint.Native.RegExp;
using Jint.Native.String;
using Jint.Parser;
using Jint.Parser.Ast;
using Jint.Runtime;
using Jint.Runtime.CallStack;
using Jint.Runtime.Debugger;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Environments;
using Jint.Runtime.Interop;
using Jint.Runtime.References;

namespace Jint
{
	public class Engine
	{
		public delegate StepMode DebugStepDelegate(object sender, DebugInformation e);

		public delegate StepMode BreakDelegate(object sender, DebugInformation e);

		private readonly ExpressionInterpreter _expressions;

		private readonly StatementInterpreter _statements;

		private readonly Stack<ExecutionContext> _executionContexts;

		private JsValue _completionValue = JsValue.Undefined;

		private int _statementsCount;

		private long _timeoutTicks;

		private SyntaxNode _lastSyntaxNode;

		public ITypeConverter ClrTypeConverter;

		internal Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

		internal static Dictionary<Type, Func<Engine, object, JsValue>> TypeMappers = new Dictionary<Type, Func<Engine, object, JsValue>>
		{
			{
				typeof(bool),
				(Engine engine, object v) => new JsValue((bool)v)
			},
			{
				typeof(byte),
				(Engine engine, object v) => new JsValue((int)(byte)v)
			},
			{
				typeof(char),
				(Engine engine, object v) => new JsValue((int)(char)v)
			},
			{
				typeof(DateTime),
				(Engine engine, object v) => engine.Date.Construct((DateTime)v)
			},
			{
				typeof(DateTimeOffset),
				(Engine engine, object v) => engine.Date.Construct((DateTimeOffset)v)
			},
			{
				typeof(decimal),
				(Engine engine, object v) => new JsValue((double)(decimal)v)
			},
			{
				typeof(double),
				(Engine engine, object v) => new JsValue((double)v)
			},
			{
				typeof(short),
				(Engine engine, object v) => new JsValue((short)v)
			},
			{
				typeof(int),
				(Engine engine, object v) => new JsValue((int)v)
			},
			{
				typeof(long),
				(Engine engine, object v) => new JsValue((long)v)
			},
			{
				typeof(sbyte),
				(Engine engine, object v) => new JsValue((sbyte)v)
			},
			{
				typeof(float),
				(Engine engine, object v) => new JsValue((float)v)
			},
			{
				typeof(string),
				(Engine engine, object v) => new JsValue((string)v)
			},
			{
				typeof(ushort),
				(Engine engine, object v) => new JsValue((int)(ushort)v)
			},
			{
				typeof(uint),
				(Engine engine, object v) => new JsValue((uint)v)
			},
			{
				typeof(ulong),
				(Engine engine, object v) => new JsValue((ulong)v)
			},
			{
				typeof(JsValue),
				(Engine engine, object v) => (JsValue)v
			},
			{
				typeof(Regex),
				(Engine engine, object v) => engine.RegExp.Construct(((Regex)v).ToString().Trim('/'))
			}
		};

		internal JintCallStack CallStack = new JintCallStack();

		public LexicalEnvironment GlobalEnvironment;

		public GlobalObject Global { get; private set; }

		public ObjectConstructor Object { get; private set; }

		public FunctionConstructor Function { get; private set; }

		public ArrayConstructor Array { get; private set; }

		public StringConstructor String { get; private set; }

		public RegExpConstructor RegExp { get; private set; }

		public BooleanConstructor Boolean { get; private set; }

		public NumberConstructor Number { get; private set; }

		public DateConstructor Date { get; private set; }

		public MathInstance Math { get; private set; }

		public JsonInstance Json { get; private set; }

		public EvalFunctionInstance Eval { get; private set; }

		public ErrorConstructor Error { get; private set; }

		public ErrorConstructor EvalError { get; private set; }

		public ErrorConstructor SyntaxError { get; private set; }

		public ErrorConstructor TypeError { get; private set; }

		public ErrorConstructor RangeError { get; private set; }

		public ErrorConstructor ReferenceError { get; private set; }

		public ErrorConstructor UriError { get; private set; }

		public ExecutionContext ExecutionContext
		{
			get
			{
				return _executionContexts.Peek();
			}
		}

		internal Options Options { get; private set; }

		internal DebugHandler DebugHandler { get; private set; }

		public List<BreakPoint> BreakPoints { get; private set; }

		public event DebugStepDelegate Step;

		public event BreakDelegate Break;

		public Engine()
			: this(null)
		{
		}

		public Engine(Action<Options> options)
		{
			_executionContexts = new Stack<ExecutionContext>();
			Global = GlobalObject.CreateGlobalObject(this);
			Object = ObjectConstructor.CreateObjectConstructor(this);
			Function = FunctionConstructor.CreateFunctionConstructor(this);
			Array = ArrayConstructor.CreateArrayConstructor(this);
			String = StringConstructor.CreateStringConstructor(this);
			RegExp = RegExpConstructor.CreateRegExpConstructor(this);
			Number = NumberConstructor.CreateNumberConstructor(this);
			Boolean = BooleanConstructor.CreateBooleanConstructor(this);
			Date = DateConstructor.CreateDateConstructor(this);
			Math = MathInstance.CreateMathObject(this);
			Json = JsonInstance.CreateJsonObject(this);
			Error = ErrorConstructor.CreateErrorConstructor(this, "Error");
			EvalError = ErrorConstructor.CreateErrorConstructor(this, "EvalError");
			RangeError = ErrorConstructor.CreateErrorConstructor(this, "RangeError");
			ReferenceError = ErrorConstructor.CreateErrorConstructor(this, "ReferenceError");
			SyntaxError = ErrorConstructor.CreateErrorConstructor(this, "SyntaxError");
			TypeError = ErrorConstructor.CreateErrorConstructor(this, "TypeError");
			UriError = ErrorConstructor.CreateErrorConstructor(this, "URIError");
			Global.Configure();
			Object.Configure();
			Object.PrototypeObject.Configure();
			Function.Configure();
			Function.PrototypeObject.Configure();
			Array.Configure();
			Array.PrototypeObject.Configure();
			String.Configure();
			String.PrototypeObject.Configure();
			RegExp.Configure();
			RegExp.PrototypeObject.Configure();
			Number.Configure();
			Number.PrototypeObject.Configure();
			Boolean.Configure();
			Boolean.PrototypeObject.Configure();
			Date.Configure();
			Date.PrototypeObject.Configure();
			Math.Configure();
			Json.Configure();
			Error.Configure();
			Error.PrototypeObject.Configure();
			GlobalEnvironment = LexicalEnvironment.NewObjectEnvironment(this, Global, null, false);
			EnterExecutionContext(GlobalEnvironment, GlobalEnvironment, Global);
			Options = new Options();
			if (options != null)
			{
				options(Options);
			}
			Eval = new EvalFunctionInstance(this, new string[0], LexicalEnvironment.NewDeclarativeEnvironment(this, ExecutionContext.LexicalEnvironment), StrictModeScope.IsStrictModeCode);
			Global.FastAddProperty("eval", Eval, true, false, true);
			_statements = new StatementInterpreter(this);
			_expressions = new ExpressionInterpreter(this);
			if (Options._IsClrAllowed)
			{
				Global.FastAddProperty("System", new NamespaceReference(this, "System"), false, false, false);
				Global.FastAddProperty("importNamespace", new ClrFunctionInstance(this, (JsValue thisObj, JsValue[] arguments) => new NamespaceReference(this, TypeConverter.ToString(arguments.At(0)))), false, false, false);
			}
			ClrTypeConverter = new DefaultTypeConverter(this);
			BreakPoints = new List<BreakPoint>();
			DebugHandler = new DebugHandler(this);
		}

		internal StepMode? InvokeStepEvent(DebugInformation info)
		{
			if (this.Step != null)
			{
				return this.Step(this, info);
			}
			return null;
		}

		internal StepMode? InvokeBreakEvent(DebugInformation info)
		{
			if (this.Break != null)
			{
				return this.Break(this, info);
			}
			return null;
		}

		public ExecutionContext EnterExecutionContext(LexicalEnvironment lexicalEnvironment, LexicalEnvironment variableEnvironment, JsValue thisBinding)
		{
			ExecutionContext executionContext = new ExecutionContext();
			executionContext.LexicalEnvironment = lexicalEnvironment;
			executionContext.VariableEnvironment = variableEnvironment;
			executionContext.ThisBinding = thisBinding;
			ExecutionContext executionContext2 = executionContext;
			_executionContexts.Push(executionContext2);
			return executionContext2;
		}

		public Engine SetValue(string name, Delegate value)
		{
			Global.FastAddProperty(name, new DelegateWrapper(this, value), true, false, true);
			return this;
		}

		public Engine SetValue(string name, string value)
		{
			return SetValue(name, new JsValue(value));
		}

		public Engine SetValue(string name, double value)
		{
			return SetValue(name, new JsValue(value));
		}

		public Engine SetValue(string name, bool value)
		{
			return SetValue(name, new JsValue(value));
		}

		public Engine SetValue(string name, JsValue value)
		{
			Global.Put(name, value, false);
			return this;
		}

		public Engine SetValue(string name, object obj)
		{
			return SetValue(name, JsValue.FromObject(this, obj));
		}

		public void LeaveExecutionContext()
		{
			_executionContexts.Pop();
		}

		public void ResetStatementsCount()
		{
			_statementsCount = 0;
		}

		public void ResetTimeoutTicks()
		{
			long ticks = Options._TimeoutInterval.Ticks;
			_timeoutTicks = ((ticks <= 0) ? 0 : (DateTime.UtcNow.Ticks + ticks));
		}

		public void ResetCallStack()
		{
			CallStack.Clear();
		}

		public Engine Execute(string source)
		{
			JavaScriptParser javaScriptParser = new JavaScriptParser();
			return Execute(javaScriptParser.Parse(source));
		}

		public Engine Execute(string source, ParserOptions parserOptions)
		{
			JavaScriptParser javaScriptParser = new JavaScriptParser();
			return Execute(javaScriptParser.Parse(source, parserOptions));
		}

		public Engine Execute(Program program)
		{
			ResetStatementsCount();
			ResetTimeoutTicks();
			ResetLastStatement();
			ResetCallStack();
			using (new StrictModeScope(Options._IsStrict || program.Strict))
			{
				DeclarationBindingInstantiation(DeclarationBindingType.GlobalCode, program.FunctionDeclarations, program.VariableDeclarations, null, null);
				Completion completion = _statements.ExecuteProgram(program);
				if (completion.Type == Completion.Throw)
				{
					JavaScriptException ex = new JavaScriptException(completion.GetValueOrDefault());
					ex.Location = completion.Location;
					throw ex;
				}
				_completionValue = completion.GetValueOrDefault();
				return this;
			}
		}

		private void ResetLastStatement()
		{
			_lastSyntaxNode = null;
		}

		public JsValue GetCompletionValue()
		{
			return _completionValue;
		}

		public Completion ExecuteStatement(Statement statement)
		{
			int maxStatements = Options._MaxStatements;
			if (maxStatements > 0 && _statementsCount++ > maxStatements)
			{
				throw new StatementsCountOverflowException();
			}
			if (_timeoutTicks > 0 && _timeoutTicks < DateTime.UtcNow.Ticks)
			{
				throw new TimeoutException();
			}
			_lastSyntaxNode = statement;
			if (Options._IsDebugMode)
			{
				DebugHandler.OnStep(statement);
			}
			switch (statement.Type)
			{
			case SyntaxNodes.BlockStatement:
				return _statements.ExecuteBlockStatement(statement.As<BlockStatement>());
			case SyntaxNodes.BreakStatement:
				return _statements.ExecuteBreakStatement(statement.As<BreakStatement>());
			case SyntaxNodes.ContinueStatement:
				return _statements.ExecuteContinueStatement(statement.As<ContinueStatement>());
			case SyntaxNodes.DoWhileStatement:
				return _statements.ExecuteDoWhileStatement(statement.As<DoWhileStatement>());
			case SyntaxNodes.DebuggerStatement:
				return _statements.ExecuteDebuggerStatement(statement.As<DebuggerStatement>());
			case SyntaxNodes.EmptyStatement:
				return _statements.ExecuteEmptyStatement(statement.As<EmptyStatement>());
			case SyntaxNodes.ExpressionStatement:
				return _statements.ExecuteExpressionStatement(statement.As<ExpressionStatement>());
			case SyntaxNodes.ForStatement:
				return _statements.ExecuteForStatement(statement.As<ForStatement>());
			case SyntaxNodes.ForInStatement:
				return _statements.ExecuteForInStatement(statement.As<ForInStatement>());
			case SyntaxNodes.FunctionDeclaration:
				return new Completion(Completion.Normal, null, null);
			case SyntaxNodes.IfStatement:
				return _statements.ExecuteIfStatement(statement.As<IfStatement>());
			case SyntaxNodes.LabeledStatement:
				return _statements.ExecuteLabelledStatement(statement.As<LabelledStatement>());
			case SyntaxNodes.ReturnStatement:
				return _statements.ExecuteReturnStatement(statement.As<ReturnStatement>());
			case SyntaxNodes.SwitchStatement:
				return _statements.ExecuteSwitchStatement(statement.As<SwitchStatement>());
			case SyntaxNodes.ThrowStatement:
				return _statements.ExecuteThrowStatement(statement.As<ThrowStatement>());
			case SyntaxNodes.TryStatement:
				return _statements.ExecuteTryStatement(statement.As<TryStatement>());
			case SyntaxNodes.VariableDeclaration:
				return _statements.ExecuteVariableDeclaration(statement.As<VariableDeclaration>());
			case SyntaxNodes.WhileStatement:
				return _statements.ExecuteWhileStatement(statement.As<WhileStatement>());
			case SyntaxNodes.WithStatement:
				return _statements.ExecuteWithStatement(statement.As<WithStatement>());
			case SyntaxNodes.Program:
				return _statements.ExecuteProgram(statement.As<Program>());
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public object EvaluateExpression(Expression expression)
		{
			_lastSyntaxNode = expression;
			switch (expression.Type)
			{
			case SyntaxNodes.AssignmentExpression:
				return _expressions.EvaluateAssignmentExpression(expression.As<AssignmentExpression>());
			case SyntaxNodes.ArrayExpression:
				return _expressions.EvaluateArrayExpression(expression.As<ArrayExpression>());
			case SyntaxNodes.BinaryExpression:
				return _expressions.EvaluateBinaryExpression(expression.As<BinaryExpression>());
			case SyntaxNodes.CallExpression:
				return _expressions.EvaluateCallExpression(expression.As<CallExpression>());
			case SyntaxNodes.ConditionalExpression:
				return _expressions.EvaluateConditionalExpression(expression.As<ConditionalExpression>());
			case SyntaxNodes.FunctionExpression:
				return _expressions.EvaluateFunctionExpression(expression.As<FunctionExpression>());
			case SyntaxNodes.Identifier:
				return _expressions.EvaluateIdentifier(expression.As<Identifier>());
			case SyntaxNodes.Literal:
				return _expressions.EvaluateLiteral(expression.As<Literal>());
			case SyntaxNodes.RegularExpressionLiteral:
				return _expressions.EvaluateLiteral(expression.As<Literal>());
			case SyntaxNodes.LogicalExpression:
				return _expressions.EvaluateLogicalExpression(expression.As<LogicalExpression>());
			case SyntaxNodes.MemberExpression:
				return _expressions.EvaluateMemberExpression(expression.As<MemberExpression>());
			case SyntaxNodes.NewExpression:
				return _expressions.EvaluateNewExpression(expression.As<NewExpression>());
			case SyntaxNodes.ObjectExpression:
				return _expressions.EvaluateObjectExpression(expression.As<ObjectExpression>());
			case SyntaxNodes.SequenceExpression:
				return _expressions.EvaluateSequenceExpression(expression.As<SequenceExpression>());
			case SyntaxNodes.ThisExpression:
				return _expressions.EvaluateThisExpression(expression.As<ThisExpression>());
			case SyntaxNodes.UpdateExpression:
				return _expressions.EvaluateUpdateExpression(expression.As<UpdateExpression>());
			case SyntaxNodes.UnaryExpression:
				return _expressions.EvaluateUnaryExpression(expression.As<UnaryExpression>());
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public JsValue GetValue(object value)
		{
			Reference reference = value as Reference;
			if (reference == null)
			{
				Completion completion = value as Completion;
				if (completion != null)
				{
					return GetValue(completion.Value);
				}
				return (JsValue)value;
			}
			if (reference.IsUnresolvableReference())
			{
				throw new JavaScriptException(ReferenceError, reference.GetReferencedName() + " is not defined");
			}
			JsValue @base = reference.GetBase();
			if (reference.IsPropertyReference())
			{
				if (!reference.HasPrimitiveBase())
				{
					ObjectInstance objectInstance = TypeConverter.ToObject(this, @base);
					return objectInstance.Get(reference.GetReferencedName());
				}
				ObjectInstance objectInstance2 = TypeConverter.ToObject(this, @base);
				PropertyDescriptor property = objectInstance2.GetProperty(reference.GetReferencedName());
				if (property == PropertyDescriptor.Undefined)
				{
					return JsValue.Undefined;
				}
				if (property.IsDataDescriptor())
				{
					return property.Value;
				}
				JsValue get = property.Get;
				if (get == Undefined.Instance)
				{
					return Undefined.Instance;
				}
				ICallable callable = (ICallable)get.AsObject();
				return callable.Call(@base, Arguments.Empty);
			}
			EnvironmentRecord environmentRecord = @base.As<EnvironmentRecord>();
			if (environmentRecord == null)
			{
				throw new ArgumentException();
			}
			return environmentRecord.GetBindingValue(reference.GetReferencedName(), reference.IsStrict());
		}

		public void PutValue(Reference reference, JsValue value)
		{
			if (reference.IsUnresolvableReference())
			{
				if (reference.IsStrict())
				{
					throw new JavaScriptException(ReferenceError);
				}
				Global.Put(reference.GetReferencedName(), value, false);
				return;
			}
			if (reference.IsPropertyReference())
			{
				JsValue @base = reference.GetBase();
				if (!reference.HasPrimitiveBase())
				{
					@base.AsObject().Put(reference.GetReferencedName(), value, reference.IsStrict());
				}
				else
				{
					PutPrimitiveBase(@base, reference.GetReferencedName(), value, reference.IsStrict());
				}
				return;
			}
			JsValue base2 = reference.GetBase();
			EnvironmentRecord environmentRecord = base2.As<EnvironmentRecord>();
			if (environmentRecord == null)
			{
				throw new ArgumentNullException();
			}
			environmentRecord.SetMutableBinding(reference.GetReferencedName(), value, reference.IsStrict());
		}

		public void PutPrimitiveBase(JsValue b, string name, JsValue value, bool throwOnError)
		{
			ObjectInstance objectInstance = TypeConverter.ToObject(this, b);
			if (!objectInstance.CanPut(name))
			{
				if (throwOnError)
				{
					throw new JavaScriptException(TypeError);
				}
				return;
			}
			PropertyDescriptor ownProperty = objectInstance.GetOwnProperty(name);
			if (ownProperty.IsDataDescriptor())
			{
				if (!throwOnError)
				{
					return;
				}
				throw new JavaScriptException(TypeError);
			}
			PropertyDescriptor property = objectInstance.GetProperty(name);
			if (property.IsAccessorDescriptor())
			{
				ICallable callable = (ICallable)property.Set.AsObject();
				callable.Call(b, new JsValue[1] { value });
			}
			else if (throwOnError)
			{
				throw new JavaScriptException(TypeError);
			}
		}

		public JsValue Invoke(string propertyName, params object[] arguments)
		{
			return Invoke(propertyName, null, arguments);
		}

		public JsValue Invoke(string propertyName, object thisObj, object[] arguments)
		{
			JsValue value = GetValue(propertyName);
			return Invoke(value, thisObj, arguments);
		}

		public JsValue Invoke(JsValue value, params object[] arguments)
		{
			return Invoke(value, null, arguments);
		}

		public JsValue Invoke(JsValue value, object thisObj, object[] arguments)
		{
			ICallable callable = value.TryCast<ICallable>();
			if (callable == null)
			{
				throw new ArgumentException("Can only invoke functions");
			}
			return callable.Call(JsValue.FromObject(this, thisObj), arguments.Select((object x) => JsValue.FromObject(this, x)).ToArray());
		}

		public JsValue GetValue(string propertyName)
		{
			return GetValue(Global, propertyName);
		}

		public SyntaxNode GetLastSyntaxNode()
		{
			return _lastSyntaxNode;
		}

		public JsValue GetValue(JsValue scope, string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentException("propertyName");
			}
			Reference value = new Reference(scope, propertyName, Options._IsStrict);
			return GetValue(value);
		}

		public void DeclarationBindingInstantiation(DeclarationBindingType declarationBindingType, IList<FunctionDeclaration> functionDeclarations, IList<VariableDeclaration> variableDeclarations, FunctionInstance functionInstance, JsValue[] arguments)
		{
			EnvironmentRecord record = ExecutionContext.VariableEnvironment.Record;
			bool flag = declarationBindingType == DeclarationBindingType.EvalCode;
			bool isStrictModeCode = StrictModeScope.IsStrictModeCode;
			if (declarationBindingType == DeclarationBindingType.FunctionCode)
			{
				int num = arguments.Length;
				int num2 = 0;
				string[] formalParameters = functionInstance.FormalParameters;
				foreach (string name in formalParameters)
				{
					num2++;
					JsValue value = ((num2 <= num) ? arguments[num2 - 1] : Undefined.Instance);
					if (!record.HasBinding(name))
					{
						record.CreateMutableBinding(name);
					}
					record.SetMutableBinding(name, value, isStrictModeCode);
				}
			}
			foreach (FunctionDeclaration functionDeclaration in functionDeclarations)
			{
				string name2 = functionDeclaration.Id.Name;
				FunctionInstance functionInstance2 = Function.CreateFunctionObject(functionDeclaration);
				if (!record.HasBinding(name2))
				{
					record.CreateMutableBinding(name2, flag);
				}
				else if (record == GlobalEnvironment.Record)
				{
					GlobalObject global = Global;
					PropertyDescriptor property = global.GetProperty(name2);
					if (property.Configurable.Value)
					{
						global.DefineOwnProperty(name2, new PropertyDescriptor(Undefined.Instance, true, true, flag), true);
					}
					else if (property.IsAccessorDescriptor() || !property.Enumerable.Value)
					{
						throw new JavaScriptException(TypeError);
					}
				}
				record.SetMutableBinding(name2, functionInstance2, isStrictModeCode);
			}
			bool flag2 = record.HasBinding("arguments");
			if (declarationBindingType == DeclarationBindingType.FunctionCode && !flag2)
			{
				ArgumentsInstance argumentsInstance = ArgumentsInstance.CreateArgumentsObject(this, functionInstance, functionInstance.FormalParameters, arguments, record, isStrictModeCode);
				if (isStrictModeCode)
				{
					DeclarativeEnvironmentRecord declarativeEnvironmentRecord = record as DeclarativeEnvironmentRecord;
					if (declarativeEnvironmentRecord == null)
					{
						throw new ArgumentException();
					}
					declarativeEnvironmentRecord.CreateImmutableBinding("arguments");
					declarativeEnvironmentRecord.InitializeImmutableBinding("arguments", argumentsInstance);
				}
				else
				{
					record.CreateMutableBinding("arguments");
					record.SetMutableBinding("arguments", argumentsInstance, false);
				}
			}
			foreach (VariableDeclarator item in variableDeclarations.SelectMany((VariableDeclaration x) => x.Declarations))
			{
				string name3 = item.Id.Name;
				if (!record.HasBinding(name3))
				{
					record.CreateMutableBinding(name3, flag);
					record.SetMutableBinding(name3, Undefined.Instance, isStrictModeCode);
				}
			}
		}
	}
}
