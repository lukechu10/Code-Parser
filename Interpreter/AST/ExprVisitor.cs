namespace Interpreter.AST {
	public abstract class ExprVisitor {
		protected ExprVisitor() { }

		public virtual ExprAST Visit(ExprAST node) {
			return node?.Accept(this);
		}

		protected internal virtual ExprAST VisitExtension(ExprAST node) {
			return node.VisitChildren(this);
		}
		protected internal virtual ExprAST VisitBinaryExprAST(BinaryExprAST node) {
			this.Visit(node.LeftExpression);
			this.Visit(node.RightExpression);

			return node;
		}
		protected internal virtual ExprAST VisitCallExprAST(CallExprAST node) {
			foreach (var argument in node.Arguments) {
				this.Visit(argument);
			}

			return node;
		}
		protected internal virtual ExprAST VisitFunctionAST(FunctionAST node) {
			this.Visit(node.Prototype);
			this.Visit(node.Body);

			return node;
		}
		protected internal virtual ExprAST VisitVariableExprAST(VariableExprAST node) => node;
		protected internal virtual ExprAST VisitPrototypeAST(PrototypeAST node) => node;
		protected internal virtual ExprAST VisitNumberExprAST(NumberExprAST node) => node;
	}
}
