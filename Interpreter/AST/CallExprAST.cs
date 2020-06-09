using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.AST {
	public sealed class CallExprAST : ExprAST {
		public string Callee { get; private set; }
		public List<ExprAST> Arguments { get; private set; }
		public override ExpressionType NodeType { get; protected set; } = ExpressionType.Call;

		public CallExprAST(string callee, List<ExprAST> args) {
			this.Callee = callee;
			this.Arguments = args;
		}

		protected internal override ExprAST Accept(ExprVisitor visitor) {
			return visitor.VisitCallExprAST(this);
		}
	}
}
