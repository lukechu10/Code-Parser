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
				switch (parser.CurrentToken.TokenType) {
					case Lexer.TokenType.EndOfFile:
						return; // exit program
					default:
						Stopwatch stopwatch = new Stopwatch();

						stopwatch.Start();
						FunctionAST ast = parser.HandleTopLevelExpression(); // top level anonymous function
						stopwatch.Stop();

						if (ast != null) {
							string yaml = serializer.Serialize(ast.Body); // serialize abstract syntax tree to YAML

							Log.Secondary(yaml); // print abstract syntax tree in dark gray

							Log.Secondary($"Parsed input in {stopwatch.ElapsedMilliseconds}ms");
							Console.ForegroundColor = ConsoleColor.DarkGray;
							Console.Write("Evaluated result: ");

							object evaluateResult = evaluator.EvaluateExpression(ast.Body);
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
						break;
				}
			}
		}
	}
}
