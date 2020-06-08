using Interpreter.Lexer;
using System;
using System.IO;

namespace Interpreter {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Hello World!");

			TextReader reader = Console.In;
			Lexer.Lexer lexer = new Lexer.Lexer(reader);
		}
	}
}
