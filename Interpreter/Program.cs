using Interpreter.AST;
using System;
using System.Diagnostics;
using YamlDotNet.Serialization;

namespace Interpreter
{
	internal class Program
	{
		private static readonly ISerializer serializer = new SerializerBuilder().WithMaximumRecursion(10000).Build();

		public static void Main(string[] args)
		{
			var evaluator = new Evaluation.Evaluator();

			while (true)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write("ready> ");

				string line = Console.ReadLine();
				var tokenStream = new TokenStream(line);
				var parser = new Parser.Parser(tokenStream);

				Statement ast;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ast = parser.Handle();

				stopwatch.Stop();

				if (ast != null)
				{
					string yaml = serializer.Serialize(ast); // serialize abstract syntax tree to YAML

					Log.Secondary(yaml); // print abstract syntax tree in dark gray

					Log.Secondary($"Parsed input in {stopwatch.ElapsedMilliseconds}ms");
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("Evaluated result: ");

					object evaluateResult = evaluator.EvaluateStatement(ast);
					if (evaluateResult is string errorMessage)
					{
						Log.Error(errorMessage);
					}
					else if (evaluateResult is double doubleResult)
					{
						Log.Emphasis(doubleResult);
					}
					else
					{
						Log.Info("None");
					}
				}
				else
				{
					Log.Warning("Invalid syntax, no abstract syntax tree generated");
				}
			}
		}
	}
}
