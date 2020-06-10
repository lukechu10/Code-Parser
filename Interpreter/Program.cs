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

			while (true) {
				Console.Write("ready> ");
				parser.GetNextToken();
				switch (parser.CurrentToken) {
					case Lexer.Token.EndOfFile:
						return; // exit program
					default:
						Stopwatch stopwatch = new Stopwatch();

						stopwatch.Start();
						FunctionAST ast = parser.HandleTopLevelExpression(); // top level anonymous function
						stopwatch.Stop();

						if (ast != null) {
							string yaml = serializer.Serialize(ast.Body); // serialize abstract syntax tree to YAML
							Console.WriteLine(yaml);
							Console.WriteLine($"Parsed input in {stopwatch.ElapsedMilliseconds}ms");
							Console.WriteLine($"Evaluated result: {Evaluation.Evaluator.Evaluate(ast.Body)}");
						}
						else {
							Console.WriteLine("Invalid syntax, no abstract syntax tree generated");
						}
						break;
				}
			}
		}
	}
}
