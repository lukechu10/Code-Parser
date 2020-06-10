using Interpreter.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Interpreter.Parser {
	public sealed class Parser {
		private static readonly Dictionary<char, int> _binOpPrecedence;

		public Lexer.Token CurrentToken { get; private set; }
		private Lexer.Lexer _scanner;

		static Parser() {
			// smaller value = lower precedence
			_binOpPrecedence = new Dictionary<char, int>() {
				['<'] = 10,
				['>'] = 10,
				['+'] = 20,
				['-'] = 20,
				['*'] = 40,
				['/'] = 40
			};
		}

		public Parser(Lexer.Lexer scanner) {
			this._scanner = scanner;
		}

		/// <summary>
		/// Gets the next Token from _scanner and saves the result into _currentToken
		/// </summary>
		/// <returns>The new Token</returns>
		public Lexer.Token GetNextToken() {
			return this.CurrentToken = this._scanner.GetNextToken();
		}

		/// <summary>
		/// Looks up the operator precedence of the current <c>Token</c>
		/// </summary>
		/// <returns>A number representing the operator precedence of the current <c>Token</c> or <c>-1</c> if precedence not found</returns>
		private int GetCurrentTokenPrecedence() {
			if (this.CurrentToken != Lexer.Token.Character) return -1;

			int precedence;
			if (_binOpPrecedence.TryGetValue(this._scanner.LastCharacter, out precedence)) {
				return precedence;
			}

			// operator not found in _binOpPrecendence
			return -1;
		}

		/// <summary>
		/// numberexpr ::= number
		/// </summary>
		/// <returns>An <c>ExprAST</c> node representing the number literal</returns>
		private ExprAST ParseNumberExpr() {
			Debug.Assert(this.CurrentToken == Lexer.Token.Number);

			ExprAST result = new NumberExprAST(this._scanner.LastNumber);
			this.GetNextToken(); // eat number
			return result;
		}

		/// <summary>
		/// Parses an expression in parenthesis (e.g. "<c>(1 + 2)</c>")
		/// <code>
		/// parenexpr ::= '(' expression ')'
		/// </code>
		/// </summary>
		/// <returns>An <c>ExprAST</c> node representing the parenthesis expression</returns>
		private ExprAST ParseParenExpr() {
			Debug.Assert(this._scanner.LastCharacter == '(');

			this.GetNextToken(); // eat opening parenthesis '('

			ExprAST expression = this.ParseExpr(); // parse expression between parenthesis
			if (expression == null) return null;

			if (this._scanner.LastCharacter != ')') {
				Console.WriteLine("Expected ')'");
				return null;
			}

			Debug.Assert(this._scanner.LastCharacter == ')');
			this.GetNextToken(); // eat closing parenthesis ')'

			return expression;
		}

		/// <summary>
		/// Parses variable references and function calls
		/// <code>
		///  identifierexpr
		///		::= identifier
		///		::= identifier '(' expression ')'
		/// </code>
		/// </summary>
		/// <returns>A <c>VariableExprAST</c> if identifier is a variable reference, a <c>CallExprAST</c> if identifier is a function call</returns>
		private ExprAST ParseIdentifierExpr() {
			Debug.Assert(this.CurrentToken == Lexer.Token.Identifier);

			string identifierName = this._scanner.LastIdentifier;
			this.GetNextToken(); // eat identifier token

			if (this._scanner.LastCharacter != '(') {
				return new VariableExprAST(identifierName); // parsed variable reference
			}

			Debug.Assert(this._scanner.LastCharacter == '('); // parse function call expression
			this.GetNextToken(); // eat opening parenthesis '('

			List<ExprAST> arguments = new List<ExprAST>();

			#region Read function call arguments
			if (this._scanner.LastCharacter != ')') {
				while (true) {
					ExprAST arg = this.ParseExpr(); // get argument
					if (arg == null) return null; // forward error

					arguments.Add(arg);

					if (this._scanner.LastCharacter == ')') break; // end of argument list reached

					if (this._scanner.LastCharacter != ',') {
						Debug.Assert(this._scanner.LastCharacter != ')' && this._scanner.LastCharacter != ',');
						Console.WriteLine("Expected ')' or ',' in argument list");
					}

					this.GetNextToken(); // eat ','
				}
			}
			#endregion

			this.GetNextToken(); // eat closing parenthesis ')'

			return new CallExprAST(identifierName, arguments);
		}

		/// <summary>
		/// <code>
		/// primaryexpr
		///		::= identifierexpr
		///		::= numberexpr
		///		::=	parenexpr
		/// </code>
		/// </summary>
		/// <returns>An <c>ExprAST</c> node representing the primary expression</returns>
		private ExprAST ParsePrimaryExpr() {
			if (this.CurrentToken == Lexer.Token.Identifier) return this.ParseIdentifierExpr();
			else if (this.CurrentToken == Lexer.Token.Number) return this.ParseNumberExpr();
			else if (this._scanner.LastCharacter == '(') return this.ParseParenExpr();
			else {
				Console.WriteLine($"Unknown {this.CurrentToken} token when expecting an expression");
				return null;
			}
		}

		/// <summary>
		/// Parses pairs of binary operator and primary expressions
		/// <code>
		/// binoprhs
		///		::= ('+' primary)*
		///	</code>
		/// </summary>
		/// <param name="exprPrecedence">The minimal operator precedence the function is allowed to eat</param>
		/// <param name="lhs">The left hand side of the expression</param>
		/// <returns>An <c>ExprAST</c></returns>
		private ExprAST ParseBinOpRHS(int exprPrecedence, ExprAST lhs) {
			while (true) {
				int tokenPrecedence = this.GetCurrentTokenPrecedence();

				if (tokenPrecedence == -1) {
					// make sure this._scanner.LastCharacter is valid
					switch (this._scanner.LastCharacter) {
						case '(':
						case ')':
						case ';':
							break;
						default:
							Console.WriteLine($"Invalid operator {this._scanner.LastCharacter}");
							return null;
					}
				}

				/* if this is a binary operator that binds at least as tightly as the current binary operator, consume it, otherwise we are done
				 * Example: "1 * 2 + 3"
				 * current binary operator = '+'
				 * previous binary operator = '*'
				 * '+' does not bind as tight as '*' (has lower precedence)
				 */
				if (tokenPrecedence < exprPrecedence) {
					return lhs;
				}

				Debug.Assert(this.CurrentToken == Lexer.Token.Character);
				char binaryOperator = this._scanner.LastCharacter;
				this.GetNextToken(); // eat binary operator

				ExprAST rhs = this.ParsePrimaryExpr(); // parse expression after binary operator
				if (rhs == null) return null; // forward error

				// if binary operator binds less tightly with RHS than the operator after RHS, let the pending operator take RHS as its LHS.
				int nextPrecedence = this.GetCurrentTokenPrecedence();
				if (tokenPrecedence < nextPrecedence) {
					rhs = this.ParseBinOpRHS(tokenPrecedence + 1, rhs);
					if (rhs == null) return null;
				}

				lhs = new BinaryExprAST(binaryOperator, lhs, rhs);
			}
		}

		/// <summary>
		/// Parses a mathematical expression
		/// <code>
		/// expression
		///		::= primary binoprhs
		/// </code>
		/// </summary>
		/// <returns>An <c>ExprAST</c> node representing the expression</returns>
		private ExprAST ParseExpr() {
			ExprAST lhs = this.ParsePrimaryExpr(); // left hand side of expression
			if (lhs == null) return null; // forward error

			return this.ParseBinOpRHS(0, lhs);
		}

		/// <summary>
		/// Parses a top level expression and wraps it into an anonymous function
		/// <code>
		/// toplevelexpr ::= expression
		/// </code>
		/// </summary>
		/// <returns>A <c>FunctionAST</c> representing the anonymous function</returns>
		private FunctionAST ParseTopLevelExpr() {
			ExprAST expression = this.ParseExpr();
			if (expression == null) return null;

			// make an anonymous function prototype
			PrototypeAST prototype = new PrototypeAST(string.Empty, new List<string>());
			return new FunctionAST(prototype, expression);
		}

		public FunctionAST HandleTopLevelExpression() {
			FunctionAST functionAST = this.ParseTopLevelExpr();

			if(functionAST == null) {
				this._scanner.Reader.ReadLine(); // eat entire line for error recovery
				return null;
			}
			else {
				return functionAST;
			}
		}
	}
}

