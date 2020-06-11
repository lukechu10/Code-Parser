using Interpreter.AST;
using System;
using System.Collections.Generic;

namespace Interpreter.Evaluation {
	public sealed class Evaluator {
		public Dictionary<string, double> Variables { get; private set; } = new Dictionary<string, double>();

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">the expression to evaluate</param>
		/// <returns></returns>
		public double Evaluate(ExprAST expression) {
			return expression switch
			{
				NumberExprAST _ => (expression as NumberExprAST).Value,
				BinaryExprAST _ => this.EvaluateBinOpExpression(expression as BinaryExprAST),
				VariableExprAST _ => this.Variables[(expression as VariableExprAST).Name],
				VariableDeclarationExprAST _ => this.EvaluateVariableDeclarationExpression(expression as VariableDeclarationExprAST),
				_ => 0
			};
		}

		private double EvaluateVariableDeclarationExpression(VariableDeclarationExprAST expression) {
			string identifier = expression.Name;
			double initializerValue = this.Evaluate(expression.InitializerExpression);
			this.Variables[identifier] = initializerValue;

			return initializerValue;
		}

		private double EvaluateBinOpExpression(BinaryExprAST expression) {
			double left = this.Evaluate(expression.LeftExpression);
			double right = this.Evaluate(expression.RightExpression);

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
