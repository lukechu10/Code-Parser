using System;

namespace Interpreter.AST {
	public sealed class BinaryExprAST : ExprAST {
		public ExprAST LeftExpression { get; private set; }
		public ExprAST RightExpression { get; private set; }

		public override ExpressionType NodeType { get; protected set; }

		public BinaryExprAST(char op, ExprAST leftExpression, ExprAST rightExpression) {
			this.NodeType = op switch
			{
				'+' => ExpressionType.Add,
				'-' => ExpressionType.Substract,
				'*' => ExpressionType.Multiply,
				'<' => ExpressionType.LessThan,
				_ => throw new ArgumentException($"op {op} is not a valid operator")
			};

			this.LeftExpression = leftExpression;
			this.RightExpression = rightExpression;
		}

		protected internal override ExprAST Accept(ExprVisitor visitor) {
			return visitor.VisitBinaryExprAST(this);
		}
	}
}
