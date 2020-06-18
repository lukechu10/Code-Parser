using Interpreter.AST;
using System;
using System.Diagnostics;
using YamlDotNet.Serialization;

namespace Interpreter {
	internal class Program {
		private static readonly ISerializer serializer = new SerializerBuilder().WithMaximumRecursion(10000).Build();

		public static void Main(string[] args) {
			Lexer.Lexer scanner = new Lexer.Lexer(Console.In);
			Parser.Parser parser = new Parser.Parser(scanner);
			
			var evaluator = new Evaluation.Evaluator();

			while (true) {
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write("ready> ");
				parser.GetNextToken();

				FunctionAST functionAST;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				switch (parser.CurrentToken.TokenType) {
					case Lexer.TokenType.EndOfFile:
						return; // exit program
					case Lexer.TokenType.Keyword_FUNCTION:
						functionAST = parser.HandleFunction();
						break;
					default:
						functionAST = parser.HandleTopLevelExpression(); // top level anonymous function
						break;
				}

				stopwatch.Stop();

				if (functionAST != null) {
					string yaml = serializer.Serialize(functionAST); // serialize abstract syntax tree to YAML

					Log.Secondary(yaml); // print abstract syntax tree in dark gray

					Log.Secondary($"Parsed input in {stopwatch.ElapsedMilliseconds}ms");
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("Evaluated result: ");

					object evaluateResult = evaluator.EvaluateExpression(functionAST);
					if (evaluateResult is string errorMessage) {
						Log.Error(errorMessage);
					}
					else if (evaluateResult is double doubleResult) {
						Log.Emphasis(doubleResult);
					}
				}
				else {
					Log.Warning("Invalid syntax, no abstract syntax tree generated");
				}				
			}
		}
	}
}
