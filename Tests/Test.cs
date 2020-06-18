using System;
using Xunit;
using System.IO;
using Interpreter;
using Interpreter.Lexer;
using Interpreter.Parser;
using Interpreter.Evaluation;
using Interpreter.AST;

namespace Tests
{
	public class E2E
	{
		[Theory]
		// binary operators
		[InlineData("1 + 1;", 2)]
		[InlineData("1 - 1;", 0)]
		[InlineData("0 - 1;", -1)]
		[InlineData("2 * 3;", 6)]
		[InlineData("1 / 2;", 0.5)]
		[InlineData("1 / 0;", double.PositiveInfinity)]
		// comparison operators
		[InlineData("1 < 0;", 0)]
		[InlineData("1 > 0;", 1)]
		public void Expression(string testString, double expectedResult)
		{
			var tokenStream = new TokenStream(testString);
			var parser = new Parser(tokenStream);
			var evaluator = new Evaluator();

			Statement expression = parser.Handle();

			object value = evaluator.EvaluateStatement(expression);

			Assert.Equal(expectedResult, value);
		}

		[Theory]
		[InlineData("let x;", null)]
		[InlineData("let x = 1;", null)]
		[InlineData("let x = 1 + 1;", null)]
		public void Variables(string testString, object expectedResult)
		{
			var tokenStream = new TokenStream(testString);
			var parser = new Parser(tokenStream);
			var evaluator = new Evaluator();

			Statement expression = parser.Handle();

			object value = evaluator.EvaluateStatement(expression);

			Assert.Equal(expectedResult, value);
		}
	}
}
