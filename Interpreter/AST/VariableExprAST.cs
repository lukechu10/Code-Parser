namespace Interpreter.AST
{
	public sealed class VariableExprAST : ExprAST
	{
		public string Name { get; private set; }
		public override ExpressionType NodeType { get; protected set; } = ExpressionType.Variable;

		/// <summary>
		/// Create a new variable expression node in AST
		/// </summary>
		/// <param name="name">The identifier of the variable</param>
		public VariableExprAST(string name)
		{
			this.Name = name;
		}

		protected internal override ASTNode Accept(ExprVisitor visitor)
		{
			return visitor.VisitVariableExprAST(this);
		}
	}
}
