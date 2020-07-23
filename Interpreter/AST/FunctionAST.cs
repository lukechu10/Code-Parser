namespace Interpreter.AST
{
	/// <summary>
	/// Represents a function definition
	/// </summary>
	public sealed class FunctionAST : ExprAST
	{
		/// <summary>
		/// Prototype of the function
		/// </summary>
		public PrototypeAST Prototype { get; private set; }
		/// <summary>
		/// The body of the function
		/// </summary>
		public ExprAST Body { get; private set; }
		public override ExpressionType NodeType { get; protected set; } = ExpressionType.Function;

		/// <summary>
		/// Create a new function definition node
		/// </summary>
		/// <param name="proto">The prototype of the function</param>
		/// <param name="body">The body of the function</param>
		public FunctionAST(PrototypeAST proto, ExprAST body)
		{
			this.Prototype = proto;
			this.Body = body;
		}

		protected internal override ExprAST Accept(ExprVisitor visitor)
		{
			return visitor.VisitFunctionAST(this);
		}
	}
}
