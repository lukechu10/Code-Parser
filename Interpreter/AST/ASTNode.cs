namespace Interpreter.AST
{
	public abstract class ASTNode
	{
		/// <summary>
		/// The NodeType of the node in the abstract syntax tree
		/// </summary>
		public abstract ExpressionType NodeType { get; protected set; }

		#region code gen visitor
		protected internal virtual ASTNode VisitChildren(ExprVisitor visitor)
		{
			return visitor.Visit(this);
		}

		protected internal virtual ASTNode Accept(ExprVisitor visitor)
		{
			return visitor.VisitExtension(this);
		}

		#endregion
	}
}