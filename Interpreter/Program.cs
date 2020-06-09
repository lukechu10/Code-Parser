﻿using Interpreter.AST;
using System;
using YamlDotNet.Serialization;

namespace Interpreter {
	internal class Program {
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
						var serializer = new SerializerBuilder().Build();
						var yaml = serializer.Serialize(ast.Body); // serialize abstract syntax tree to YAML
						Console.WriteLine(yaml);
						break;
				}
			}
		}
	}
}
