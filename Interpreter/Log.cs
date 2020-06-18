using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
	public sealed class Log
	{
		private static ConsoleColor _initialConsoleColor = Console.ForegroundColor;

		public static void Error(object message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ForegroundColor = _initialConsoleColor;
		}

		public static void Warning(object message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ForegroundColor = _initialConsoleColor;
		}

		public static void Secondary(object message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(message);
			Console.ForegroundColor = _initialConsoleColor;
		}

		public static void Info(object message)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(message);
			Console.ForegroundColor = _initialConsoleColor;
		}

		public static void Emphasis(object message)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine(message);
			Console.ForegroundColor = _initialConsoleColor;
		}
	}
}
