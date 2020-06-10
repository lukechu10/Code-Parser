using Interpreter.AST;
using System;

namespace Interpreter.Evaluation {
	internal class Evaluator {
		public static double Evaluate(ExprAST expression) {
			return expression switch
			{
				NumberExprAST _ => (expression as NumberExprAST).Value,
				BinaryExprAST _ => EvaluateBinOpExpression(expression as BinaryExprAST),
				VariableDeclarationExprAST _ => Evaluate((expression as VariableDeclarationExprAST).InitializerExpression),
				_ => 0
			};
		}

		private static double EvaluateBinOpExpression(BinaryExprAST expression) {
			double left = Evaluate(expression.LeftExpression);
			double right = Evaluate(expression.RightExpression);

			return expression.NodeType switch
			{
				ExpressionType.Add => left + right,
				ExpressionType.Substract => left - right,
				ExpressionType.Multiply => left * right,
				ExpressionType.Divide => left / right,
				ExpressionType.LessThan => left < right ? 1.0 : 0.0,
				ExpressionType.GreaterThan => left > right ? 1.0 : 0.0,
				_ => throw new Exception("expression is not a valid BinaryExprAST node")
			};
		}
	}
}
