using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jint.Native;

namespace Jint.Runtime.Interop
{
	public class DefaultTypeConverter : ITypeConverter
	{
		private readonly Engine _engine;

		private static readonly Dictionary<string, bool> _knownConversions = new Dictionary<string, bool>();

		private static readonly object _lockObject = new object();

		private static MethodInfo convertChangeType = typeof(Convert).GetMethod("ChangeType", new Type[3]
		{
			typeof(object),
			typeof(Type),
			typeof(IFormatProvider)
		});

		private static MethodInfo jsValueFromObject = typeof(JsValue).GetMethod("FromObject");

		private static MethodInfo jsValueToObject = typeof(JsValue).GetMethod("ToObject");

		public DefaultTypeConverter(Engine engine)
		{
			_engine = engine;
		}

		public virtual object Convert(object value, Type type, IFormatProvider formatProvider)
		{
			if (value == null)
			{
				if (TypeConverter.TypeIsNullable(type))
				{
					return null;
				}
				throw new NotSupportedException(string.Format("Unable to convert null to '{0}'", type.FullName));
			}
			if (type.IsInstanceOfType(value))
			{
				return value;
			}
			if (type.IsEnum)
			{
				object obj = System.Convert.ChangeType(value, typeof(int), formatProvider);
				if (obj == null)
				{
					throw new ArgumentOutOfRangeException();
				}
				return Enum.ToObject(type, obj);
			}
			Type type2 = value.GetType();
			if (type2 == typeof(Func<JsValue, JsValue[], JsValue>))
			{
				Func<JsValue, JsValue[], JsValue> function = (Func<JsValue, JsValue[], JsValue>)value;
				if (type.IsGenericType)
				{
					Type genericTypeDefinition = type.GetGenericTypeDefinition();
					if (genericTypeDefinition.Name.StartsWith("Action"))
					{
						Type[] genericArguments = type.GetGenericArguments();
						ParameterExpression[] array = new ParameterExpression[genericArguments.Count()];
						for (int i = 0; i < array.Count(); i++)
						{
							array[i] = Expression.Parameter(genericArguments[i], genericArguments[i].Name + i);
						}
						Expression[] array2 = new Expression[array.Length];
						for (int j = 0; j < array.Count(); j++)
						{
							ParameterExpression parameterExpression = array[j];
							if (parameterExpression.Type.IsValueType)
							{
								UnaryExpression unaryExpression = Expression.Convert(parameterExpression, typeof(object));
								array2[j] = Expression.Call(null, jsValueFromObject, Expression.Constant(_engine, typeof(Engine)), unaryExpression);
							}
							else
							{
								array2[j] = Expression.Call(null, jsValueFromObject, Expression.Constant(_engine, typeof(Engine)), parameterExpression);
							}
						}
						NewArrayExpression newArrayExpression = Expression.NewArrayInit(typeof(JsValue), array2);
						List<Expression> list = new List<Expression>();
						list.Add(Expression.Call(Expression.Call(Expression.Constant(function.Target), function.Method, Expression.Constant(JsValue.Undefined, typeof(JsValue)), newArrayExpression), jsValueToObject));
						list.Add(null);
						Expression body = Block(list);
						return Expression.Lambda(body, new ReadOnlyCollection<ParameterExpression>(array).ToArray()).Compile();
					}
					if (genericTypeDefinition.Name.StartsWith("Func"))
					{
						Type[] genericArguments2 = type.GetGenericArguments();
						Type type3 = genericArguments2.Last();
						ParameterExpression[] array3 = new ParameterExpression[genericArguments2.Count() - 1];
						for (int k = 0; k < array3.Count(); k++)
						{
							array3[k] = Expression.Parameter(genericArguments2[k], genericArguments2[k].Name + k);
						}
						NewArrayExpression newArrayExpression2 = Expression.NewArrayInit(typeof(JsValue), array3.Select(delegate(ParameterExpression p)
						{
							UnaryExpression unaryExpression2 = Expression.Convert(p, typeof(object));
							ConstantExpression constantExpression = Expression.Constant(_engine, typeof(Engine));
							return Expression.Call(null, jsValueFromObject, constantExpression, unaryExpression2);
						}).ToArray());
						UnaryExpression body2 = Expression.Convert(Expression.Call(null, convertChangeType, Expression.Call(Expression.Call(Expression.Constant(function.Target), function.Method, Expression.Constant(JsValue.Undefined, typeof(JsValue)), newArrayExpression2), jsValueToObject), Expression.Constant(type3, typeof(Type)), Expression.Constant(CultureInfo.InvariantCulture, typeof(IFormatProvider))), type3);
						return Expression.Lambda(body2, new ReadOnlyCollection<ParameterExpression>(array3).ToArray()).Compile();
					}
				}
				else
				{
					if (type == typeof(Action))
					{
						return (Action)delegate
						{
							function(JsValue.Undefined, new JsValue[0]);
						};
					}
					if (type.IsSubclassOf(typeof(MulticastDelegate)))
					{
						MethodInfo method = type.GetMethod("Invoke");
						ParameterInfo[] parameters = method.GetParameters();
						ParameterExpression[] array4 = new ParameterExpression[parameters.Count()];
						for (int l = 0; l < array4.Count(); l++)
						{
							array4[l] = Expression.Parameter(typeof(object), parameters[l].Name);
						}
						NewArrayExpression newArrayExpression3 = Expression.NewArrayInit(typeof(JsValue), array4.Select((ParameterExpression p) => Expression.Call(null, typeof(JsValue).GetMethod("FromObject"), Expression.Constant(_engine, typeof(Engine)), p)).ToArray());
						List<Expression> list = new List<Expression>();
						list.Add(Expression.Call((Expression)Expression.Call(Expression.Constant(function.Target), function.Method, Expression.Constant(JsValue.Undefined, typeof(JsValue)), newArrayExpression3), typeof(JsValue).GetMethod("ToObject"), (Expression[])null));
						Expression body3 = Block(list);
						InvocationExpression body4 = Expression.Invoke(Expression.Lambda(body3, new ReadOnlyCollection<ParameterExpression>(array4).ToArray()), new ReadOnlyCollection<ParameterExpression>(array4).ToArray());
						return Expression.Lambda(type, body4, new ReadOnlyCollection<ParameterExpression>(array4)).Compile();
					}
				}
			}
			if (type.IsArray)
			{
				object[] array5 = value as object[];
				if (array5 == null)
				{
					throw new ArgumentException(string.Format("Value of object[] type is expected, but actual type is {0}.", value.GetType()));
				}
				Type targetElementType = type.GetElementType();
				object[] array6 = array5.Select((object o) => Convert(o, targetElementType, formatProvider)).ToArray();
				Array array7 = Array.CreateInstance(targetElementType, array5.Length);
				array6.CopyTo(array7, 0);
				return array7;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = Nullable.GetUnderlyingType(type);
			}
			return System.Convert.ChangeType(value, type, formatProvider);
		}

		public virtual bool TryConvert(object value, Type type, IFormatProvider formatProvider, out object converted)
		{
			string key = ((value != null) ? string.Format("{0}->{1}", value.GetType(), type) : string.Format("Null->{0}", type));
			bool value2;
			if (!_knownConversions.TryGetValue(key, out value2))
			{
				lock (_lockObject)
				{
					if (!_knownConversions.TryGetValue(key, out value2))
					{
						try
						{
							converted = Convert(value, type, formatProvider);
							_knownConversions.Add(key, true);
							return true;
						}
						catch
						{
							converted = null;
							_knownConversions.Add(key, false);
							return false;
						}
					}
				}
			}
			if (value2)
			{
				converted = Convert(value, type, formatProvider);
				return true;
			}
			converted = null;
			return false;
		}

		public static Expression Block(IEnumerable<Expression> expressions)
		{
			MethodInfo method = typeof(DefaultTypeConverter).GetMethod("InvokeAll", BindingFlags.Static | BindingFlags.NonPublic);
			Expression<Action>[] initializers = expressions.Select((Expression e) => Expression.Lambda<Action>(e, new ParameterExpression[0])).ToArray();
			NewArrayExpression newArrayExpression = Expression.NewArrayInit(typeof(Action), initializers);
			return Expression.Call(method, newArrayExpression);
		}
	}
}
