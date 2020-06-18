namespace Interpreter.AST
{
	/// <summary>
	/// Base class for AST expression nodes
	/// </summary>
	public abstract class ExprAST : Statement
	{
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