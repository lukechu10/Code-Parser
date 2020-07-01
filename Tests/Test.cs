using System;
using System.Collections.Generic;
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
		private void Evaluate(string testString, object expectedResult)
		{
			var tokenStream = new TokenStream(testString);
			var parser = new Parser(tokenStream);
			var evaluator = new Evaluator();

			Statement expression = parser.Handle();

			object result = evaluator.EvaluateStatement(expression);

			Assert.Equal(expectedResult, result);
		}

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
			this.Evaluate(testString, expectedResult);
		}

		[Theory]
		[InlineData("let x;", null)]
		[InlineData("let x = 1;", null)]
		[InlineData("let x = 1 + 1;", null)]
		public void Variables(string testString, object expectedResult)
		{
			this.Evaluate(testString, expectedResult);
		}

		[Theory]
		[MemberData(nameof(VariableAssignmentData))]
		public void VariableAssignment((string, object)[] statements)
		{
			var evaluator = new Evaluator();
			foreach ((string, object) statement in statements)
			{
				var tokenStream = new TokenStream(statement.Item1);
				var parser = new Parser(tokenStream);

				Statement expression = parser.Handle();

				object result = evaluator.EvaluateStatement(expression);

				Assert.Equal(statement.Item2, result);
			}
		}

		public static IEnumerable<object[]> VariableAssignmentData =>
			new List<object[]>
			{
				new object[] {new[]{("let x = 1", (object)null), ("x", 1.0)}},
				new object[] {new[]{("let x = 1", (object)null), ("x = 2", 2.0), ("x = x + x", 4.0)}},
				new object[] {new[]{("let x = 1", (object)null), ("let y = 2", (object)null), ("x + y", 3.0)}}
			};
	}
}
