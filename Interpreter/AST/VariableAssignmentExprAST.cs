namespace Interpreter.AST {
	public sealed class VariableAssignmentExprAST : ExprAST {
		/// <summary>
		/// The identifier of the variable
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// The assignment of the value
		/// </summary>
		public ExprAST AssignmentValue { get; private set; }
		public override ExpressionType NodeType { get; protected set; } = ExpressionType.VariableAssignment;
	
		public VariableAssignmentExprAST(string identifier, ExprAST expression) {
			this.Name = identifier;
			this.AssignmentValue = expression;
		}
	}
}
