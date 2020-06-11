namespace Interpreter.AST {
	public sealed class VariableDeclarationExprAST : ExprAST {
		/// <summary>
		/// The identifier of the new variable
		/// </summary>
		public string Name { get; private set; }
		public ExprAST InitializerExpression { get; private set; }
		public override ExpressionType NodeType { get; protected set; } = ExpressionType.VariableDeclaration;

		public VariableDeclarationExprAST(string identifier, ExprAST expression) {
			this.Name = identifier;
			this.InitializerExpression = expression;
		}
	}
}
