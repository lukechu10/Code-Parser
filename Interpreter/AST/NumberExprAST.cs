namespace Interpreter.AST {
	/// <summary>
	/// Represents a double literal in the AST
	/// </summary>
	public sealed class NumberExprAST : ExprAST {
		/// <summary>
		/// Value of double literal
		/// </summary>
		public double Value { get; private set; }
		public override ExpressionType NodeType { get; protected set; }

		public NumberExprAST(double value) {
			this.Value = value;
			this.NodeType = ExpressionType.Number;
		}

		protected internal override ExprAST Accept(ExprVisitor visitor) {
			return visitor.VisitNumberExprAST(this);
		}
	}
}
