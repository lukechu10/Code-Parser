using System.Collections.Generic;

namespace Interpreter.AST
{
	public sealed class PrototypeAST : ExprAST
	{
		public string Name { get; private set; }
		public List<string> Arguments { get; private set; }
		public override ExpressionType NodeType { get; protected set; } = ExpressionType.Prototype;

		public PrototypeAST(string name, List<string> args)
		{
			this.Name = name;
			this.Arguments = args;
		}

		protected internal override ExprAST Accept(ExprVisitor visitor)
		{
			return visitor.VisitPrototypeAST(this);
		}
	}
}
