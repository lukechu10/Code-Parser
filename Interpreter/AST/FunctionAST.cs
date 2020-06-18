namespace Interpreter.AST
{
	public sealed class FunctionAST : ExprAST
	{
		public PrototypeAST Prototype { get; private set; }
		public ExprAST Body { get; private set; }
		public override ExpressionType NodeType { get; protected set; } = ExpressionType.Function;

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
