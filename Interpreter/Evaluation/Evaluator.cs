using Interpreter.AST;
using System;
using System.Collections.Generic;
using System.Data;

namespace Interpreter.Evaluation {
	public sealed class Evaluator {
		public Dictionary<string, double> Variables { get; private set; } = new Dictionary<string, double>();

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">the expression to evaluate</param>
		/// <returns>A <c>double</c> with the value of the expression or a <c>string</c> with an error message</returns>
		public object EvaluateExpression(ExprAST expression) {
			try {
				return this.Evaluate(expression);
			}
			catch (Exception err) {
				return $"Error: {err.Message}";
			}
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">the expression to evaluate</param>
		private double Evaluate(ExprAST expression) {
			return expression switch
			{
				NumberExprAST numberExpression => numberExpression.Value,
				BinaryExprAST binaryExpression => this.EvaluateBinOpExpression(binaryExpression),
				VariableExprAST variableExpression => this.EvaluateVariableExpression(variableExpression),
				VariableAssignmentExprAST assignmentExpr => this.EvaluateVariableAssignmentExpression(assignmentExpr),
				VariableDeclarationExprAST variableDeclarationExpression => this.EvaluateVariableDeclarationExpression(variableDeclarationExpression),
				_ => throw new NotImplementedException($"Handling of {expression.GetType()} has not been implemented")
			};
		}

		private double EvaluateVariableExpression(VariableExprAST expression) {
			double variableValue;

			if (this.Variables.TryGetValue(expression.Name, out variableValue)) {
				return variableValue;
			}
			else throw new Exception($"Variable {expression.Name} does not exist in current scope");
		}

		private double EvaluateVariableAssignmentExpression(VariableAssignmentExprAST expression) {
			if (this.Variables.ContainsKey(expression.Name)) {
				this.Variables[expression.Name] = this.Evaluate(expression.AssignmentValue);
				return this.Variables[expression.Name];
			}
			else throw new Exception($"Variable {expression.Name} does not exist in current scope");
		}

		private double EvaluateVariableDeclarationExpression(VariableDeclarationExprAST expression) {
			string identifier = expression.Name;
			double initializerValue = this.Evaluate(expression.InitializerExpression);

			if (this.Variables.ContainsKey(identifier)) {
				throw new Exception($"Variable {identifier} already exists in current scope");
			}
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
