using System;
using Xunit;
using System.IO;
using Interpreter.Lexer;
using Interpreter.Parser;
using Interpreter.Evaluation;
using Interpreter.AST;

namespace Tests {
	public class E2E {
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
		public void Expression(string testString, double expectedResult) {
			TextReader textReader = new StringReader(testString);

			Lexer lexer = new Lexer(textReader);
			Parser parser = new Parser(lexer);
			Evaluator evaluator = new Evaluator();

			parser.GetNextToken();
			ExprAST expression = parser.HandleTopLevelExpression().Body;

			object value = evaluator.EvaluateExpression(expression);

			Assert.Equal(expectedResult, value);
		}

		[Theory]
		[InlineData("let x;", 0)]
		[InlineData("let x = 1;", 1)]
		[InlineData("let x = 1 + 1;", 2)]
		public void Variables(string testString, double expectedResult) {
			TextReader textReader = new StringReader(testString);

			Lexer lexer = new Lexer(textReader);
			Parser parser = new Parser(lexer);
			Evaluator evaluator = new Evaluator();

			parser.GetNextToken();
			ExprAST expression = parser.HandleTopLevelExpression().Body;

			object value = evaluator.EvaluateExpression(expression);

			Assert.Equal(expectedResult, value);
		}
	}
}
