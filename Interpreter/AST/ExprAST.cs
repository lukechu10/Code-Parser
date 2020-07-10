namespace Interpreter.AST
{
	/// <summary>
	/// Base class for AST expression nodes
	/// </summary>
	public abstract class ExprAST : Statement
	{
		/// <summary>
		/// The NodeType of the node in the abstract syntax tree
		/// </summary>
		public abstract ExpressionType NodeType { get; protected set; }

		protected internal virtual ExprAST VisitChildren(ExprVisitor visitor)
		{
			return visitor.Visit(this);
		}

		protected internal virtual ExprAST Accept(ExprVisitor visitor)
		{
			return visitor.VisitExtension(this);
		}
	}
}