using Interpreter.AST;
using System;
using YamlDotNet.Serialization;

namespace Interpreter {
	internal class Program {
		private static readonly ISerializer serializer = new SerializerBuilder().Build();

		public static void Main(string[] args) {
			Console.WriteLine("Code Parser");

			Lexer.Lexer scanner = new Lexer.Lexer(Console.In);
			Parser.Parser parser = new Parser.Parser(scanner);

			while (true) {
				Console.Write(">>> ");
				parser.GetNextToken();
				switch (parser.CurrentToken) {
					case Lexer.Token.EndOfFile:
						return; // exit program
					default:
						FunctionAST ast = parser.HandleTopLevelExpression(); // top level anonymous function
						if(ast != null) {
							string yaml = serializer.Serialize(ast.Body); // serialize abstract syntax tree to YAML
							Console.WriteLine(yaml);

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
