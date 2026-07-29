using System;
using Jint.Native;
using Jint.Native.Error;
using Jint.Native.Object;
using Jint.Parser;

namespace Jint.Runtime
{
	public class JavaScriptException : Exception
	{
		private readonly JsValue _errorObject;

		public JsValue Error
		{
			get
			{
				return _errorObject;
			}
		}

		public Location Location { get; set; }

		public int LineNumber
		{
			get
			{
				return (Location != null) ? Location.Start.Line : 0;
			}
		}

		public int Column
		{
			get
			{
				return (Location != null) ? Location.Start.Column : 0;
			}
		}

		public JavaScriptException(ErrorConstructor errorConstructor)
			: base(string.Empty)
		{
			_errorObject = errorConstructor.Construct(Arguments.Empty);
		}

		public JavaScriptException(ErrorConstructor errorConstructor, string message)
			: base(message)
		{
			_errorObject = errorConstructor.Construct(new JsValue[1] { message });
		}

		public JavaScriptException(JsValue error)
			: base(GetErrorMessage(error))
		{
			_errorObject = error;
		}

		private static string GetErrorMessage(JsValue error)
		{
			if (error.IsObject())
			{
				ObjectInstance objectInstance = error.AsObject();
				return objectInstance.Get("message").AsString();
			}
			return string.Empty;
		}

		public override string ToString()
		{
			return _errorObject.ToString();
		}
	}
}
