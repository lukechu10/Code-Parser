using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.AST
{
	/// <summary>
	/// Represents a call to a function
	/// </summary>
	public sealed class CallExprAST : ExprAST
	{
		/// <summary>
		/// Name of function to be called
		/// </summary>
		/// <value>A string representing the name of the function to be called</value>
		public string Callee { get; private set; }
		/// <summary>
		/// Arguments sent to the function
		/// </summary>
		public List<ExprAST> Arguments { get; private set; }
		public override ExpressionType NodeType { get; protected set; } = ExpressionType.Call;

		/// <summary>
		/// Create a new function call expression node
		/// </summary>
		/// <param name="callee"></param>
		/// <param name="args"></param>
		public CallExprAST(string callee, List<ExprAST> args)
		{
			this.Callee = callee;
			this.Arguments = args;
		}

		protected internal override ASTNode Accept(ExprVisitor visitor)
		{
			return visitor.VisitCallExprAST(this);
		}
	}
}
