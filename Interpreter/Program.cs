using Interpreter.AST;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using YamlDotNet.Serialization;

namespace Interpreter
{
	internal class Program
	{
		private static readonly ISerializer serializer = new SerializerBuilder().WithMaximumRecursion(10000).Build();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="showAst">Show the abstract syntax tree generated</param>
		/// <param name="showDebugInfo">Show parsing debug information</param>
		public static void Main(bool showAst = false, bool showDebugInfo = false)
		{
			var evaluator = new Evaluation.Evaluator();

			while (true)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write("> ");

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

					if (showAst)
					{
						Log.Secondary(yaml); // print abstract syntax tree in dark gray
					}

					if (showDebugInfo)
					{
						Log.Secondary($"Parsed input in {stopwatch.ElapsedMilliseconds}ms");
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write("Evaluated result: ");
					}

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
