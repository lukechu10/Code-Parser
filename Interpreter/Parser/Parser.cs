using Interpreter.AST;
using Interpreter.Lexer;
using System.Collections.Generic;
using System.Diagnostics;

namespace Interpreter.Parser
{
	/// <summary>
	/// Represents a parser instance that parses source code
	/// </summary>
	public sealed class Parser
	{
		private static readonly Dictionary<string, int> _binOpPrecedence;

		private readonly TokenStream _tokenStream;

		static Parser()
		{
			// smaller value = lower precedence
			_binOpPrecedence = new Dictionary<string, int>()
			{
				["="] = 3, // assignment operator
				["=="] = 5, // equality operator
				["<"] = 10,
				[">"] = 10,
				["+"] = 20,
				["-"] = 20,
				["*"] = 40,
				["/"] = 40,
			};
		}

		/// <summary>
		/// Create a new Parser instance
		/// </summary>
		/// <param name="tokenStream">The TokenStream for the parser to read from</param>
		public Parser(TokenStream tokenStream)
		{
			this._tokenStream = tokenStream;
		}

		/// <summary>
		/// Looks up the operator precedence of the current <c>Token</c>
		/// </summary>
		/// <returns>A number representing the operator precedence of the current <c>Token</c> or <c>-1</c> if precedence not found</returns>
		private int GetCurrentTokenPrecedence()
		{
			if (!(this._tokenStream.CurrentToken is OperatorToken operatorToken))
			{
				return -1; // not an OperatorToken
			}

			string operatorLexeme = operatorToken.Operator;
			if (_binOpPrecedence.TryGetValue(operatorLexeme, out int precedence))
			{
				return precedence;
			}

			// operator not found in _binOpPrecendence
			return -1;
		}

		/// <summary>
		/// numberexpr ::= number
		/// </summary>
		/// <returns>An <c>ExprAST</c> node representing the number literal</returns>
		private ExprAST ParseNumberExpr()
		{
			var numberToken = this._tokenStream.CurrentToken as NumberToken;
			Debug.Assert(numberToken != null);

			ExprAST result = new NumberExprAST(numberToken.Value);
			this._tokenStream.Read(); // eat number
			return result;
		}

		/// <summary>
		/// Parses an expression in parenthesis (e.g. "<c>(1 + 2)</c>")
		/// <code>
		/// parenexpr ::= '(' expression ')'
		/// </code>
		/// </summary>
		/// <returns>An <c>ExprAST</c> node representing the parenthesis expression</returns>
		private ExprAST ParseParenExpr()
		{
			var operatorToken = this._tokenStream.CurrentToken as OperatorToken;
			Debug.Assert(operatorToken != null && operatorToken.Operator == "(");

			this._tokenStream.Read(); // eat opening parenthesis '('

			ExprAST expression = this.ParseExpression(); // parse expression between parenthesis
			if (expression == null)
			{
				return null;
			}

			if ((this._tokenStream.CurrentToken as OperatorToken)?.Operator != ")")
			{
				Log.Error("Expected ')' after expression");
				return null;
			}

			Debug.Assert((this._tokenStream.CurrentToken as OperatorToken).Operator == ")");
			this._tokenStream.Read(); // eat closing parenthesis ')'

			return expression;
		}

		/// <summary>
		/// Parses variable references and function calls
		/// <code>
		/// identifierexpr
		///		::= identifier
		///		::= identifier '(' expression* ')'
		///		::= identifier '=' expression
		/// </code>
		/// </summary>
		/// <returns>A <c>VariableExprAST</c> if identifier is a variable reference, a <c>CallExprAST</c> if identifier is a function call or a <c>VariableAssignmentExprAST</c> if identifier is a variable assignment statement</returns>
		private ExprAST ParseIdentifierExpr()
		{
			Debug.Assert(this._tokenStream.CurrentToken is IdentifierToken);

			string identifierName = (this._tokenStream.CurrentToken as IdentifierToken).Identifier;
			this._tokenStream.Read(); // eat identifier token

			if (this._tokenStream.CurrentToken is OperatorToken openParnethesisToken && openParnethesisToken.Operator == "(")
			{
				// parse function call expression
				this._tokenStream.Read(); // eat opening parenthesis '('

				List<ExprAST> arguments = new List<ExprAST>();

				#region Read function call arguments
				// if at least 1 argument
				if (this._tokenStream.CurrentToken is OperatorToken closeParanthesisToken && closeParanthesisToken.Operator != ")")
				{

					while (true)
					{
						ExprAST arg = this.ParseExpression(); // get argument
						if (arg == null)
						{
							return null; // forward error
						}

						arguments.Add(arg);

						if (this._tokenStream.CurrentToken is OperatorToken operatorToken)
						{
							if (operatorToken.Operator == ")")
							{
								break; // end of argument list reached
							}
							else if (operatorToken.Operator != ",")
							{
								Debug.Assert(operatorToken.Operator != ")" && operatorToken.Operator != ",");
								Log.Error("Expected ')' or ',' in argument list");
							}
						}

						else
						{
							// not ")" or "," after expression
							Log.Error("Expected ')' or ',' after expression in argument list");
						}

						this._tokenStream.Read(); // eat ','
					}
				}
				#endregion

				this._tokenStream.Read(); // eat closing parenthesis ')'

				return new CallExprAST(identifierName, arguments);
			}
			else if ((this._tokenStream.CurrentToken is OperatorToken equalsToken) && equalsToken.Operator == "=")
			{
				// parse variable assignment expression
				this._tokenStream.Read(); // eat '=' character

				ExprAST expression = this.ParseExpression(); // parse expression after '='
				return new VariableAssignmentExprAST(identifierName, expression);
			}
			else
			{
				return new VariableExprAST(identifierName); // parsed variable reference
			}
		}

		/// <summary>
		/// Parses a variable declaration statement ("let" keyword)
		/// <code>
		/// declarationstatement
		///		::= 'let' identifier
		///		::= 'let' identifier '=' expression
		/// </code>
		/// </summary>
		/// <returns>A <c>VariableDeclarationExprAST</c> node representing the variable declaration statement</returns>
		private VariableDeclarationExprAST ParseVariableDeclarationStatement()
		{
			Debug.Assert(this._tokenStream.CurrentToken.TokenType == TokenType.Keyword_LET);
			this._tokenStream.Read(); // eat "let" keyword

			if (this._tokenStream.CurrentToken.TokenType != TokenType.Identifier)
			{
				Log.Error("Expected an identifier after 'let' keyword");
				return null;
			}
			Debug.Assert(this._tokenStream.CurrentToken is IdentifierToken);

			string identifier = (this._tokenStream.CurrentToken as IdentifierToken).Identifier;
			this._tokenStream.Read(); // eat identifier token

			ExprAST initializerExpression;
			if (this._tokenStream.CurrentToken is OperatorToken operatorToken && operatorToken.Operator == "=")
			{
				this._tokenStream.Read(); // eat '=' character

				// found an initializer after variable declaration
				initializerExpression = this.ParseExpression();
			}
			else
			{
				// no initializer, use default value = 0
				initializerExpression = new NumberExprAST(0);
			}

			// construct VariableDeclarationExprAST
			return new VariableDeclarationExprAST(identifier, initializerExpression);
		}

		/// <summary>
		/// Parses a prototype expression
		/// <code>
		/// prototype
		///		::= identifier? '(' identifier* ')'
		/// </code>
		/// </summary>
		/// <returns>A <c>PrototypeAST</c> representing the prototype expression</returns>
		private PrototypeAST ParsePrototype()
		{
			string functionIdentifier;
			if (this._tokenStream.CurrentToken is IdentifierToken identifierToken)
			{
				functionIdentifier = identifierToken.Identifier;
				this._tokenStream.Read(); // eat identifier Token
			}
			else
			{
				functionIdentifier = ""; // anonymous function
			}

			if (!(this._tokenStream.CurrentToken is OperatorToken openParenthesisToken && openParenthesisToken.Operator == "("))
			{
				Log.Error("Expected '(' in prototype");
				return null;
			}
			this._tokenStream.Read(); // eat "(" Token

			var arguments = new List<string>();

			while (this._tokenStream.CurrentToken is IdentifierToken identifier)
			{
				arguments.Add(identifier.Identifier); // add identifier to arguments list
				this._tokenStream.Read(); // eat identifier Token

				if (this._tokenStream.CurrentToken is OperatorToken operatorToken)
				{
					if (operatorToken.Operator == ",")
					{
						this._tokenStream.Read(); // eat ',' operator
					}
					else if (operatorToken.Operator == ")")
					{
						break; // end of prototype found
					}
					else
					{
						Log.Error($"Unknown operator {operatorToken.Operator} in prototype");
						return null;
					}
				}
				else
				{
					Log.Error("Expected an operator after identifier in prototype");
					return null;
				}
			}

			Debug.Assert((this._tokenStream.CurrentToken as OperatorToken)?.Operator == ")");
			this._tokenStream.Read(); // eat ")" Token

			return new PrototypeAST(functionIdentifier, arguments); // construct PrototypeAST
		}

		/// <summary>
		/// Parses a function declaration statement
		/// <code>
		/// functiondeclaration
		/// 	::= 'function' prototype '=>' expression
		/// </code>
		/// </summary>
		/// <returns>An <c>FunctionAST</c> node representing the function declaration statement</returns>
		private FunctionAST ParseFunctionStatement()
		{
			Debug.Assert(this._tokenStream.CurrentToken.TokenType == TokenType.Keyword_FUNCTION);

			this._tokenStream.Read(); // eat "function" keyword
			PrototypeAST prototype = this.ParsePrototype();

			if (prototype == null)
			{
				return null;
			}

			if ((this._tokenStream.CurrentToken as OperatorToken)?.Operator != "=>")
			{
				Log.Error("Expected '=>' after prototype");
				return null;
			}
			this._tokenStream.Read(); // eat "=>" operator

			ExprAST body = this.ParseExpression();
			if (body == null)
			{
				return null;
			}

			return new FunctionAST(prototype, body);
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
		private ExprAST ParsePrimaryExpr()
		{
			switch (this._tokenStream.CurrentToken)
			{
				case IdentifierToken _:
					return this.ParseIdentifierExpr();
				case NumberToken _:
					return this.ParseNumberExpr();
				case OperatorToken operatorToken:
					if (operatorToken.Operator == "(")
					{
						return this.ParseParenExpr();
					}
					else
					{
						Log.Error($"Unexpected operator {operatorToken.Operator} when expecting an expression");
						return null;
					}
				default:
					// keyword token
					// switch (this._tokenStream.CurrentToken.TokenType)
					// {
					// 	case TokenType.Keyword_LET: return this.ParseVariableDeclarationExpr();
					// 	default: return null;
					// }
					return null;
			}
		}

		/// <summary>
		/// Parses pairs of binary operator and primary expressions
		/// <code>
		/// binoprhs
		///		::= (operator primary)*
		///	</code>
		/// </summary>
		/// <param name="exprPrecedence">The minimal operator precedence the function is allowed to eat</param>
		/// <param name="lhs">The left hand side of the expression</param>
		/// <returns>An <c>ExprAST</c> node</returns>
		private ExprAST ParseBinOpRHS(int exprPrecedence, ExprAST lhs)
		{
			while (true)
			{
				int tokenPrecedence = this.GetCurrentTokenPrecedence();

				/* if this is a binary operator that binds at least as tightly as the current binary operator, consume it, otherwise we are done
				 * Example: "1 * 2 + 3"
				 * current binary operator = '+'
				 * previous binary operator = '*'
				 * '+' does not bind as tight as '*' (has lower precedence)
				 */
				if (tokenPrecedence < exprPrecedence)
				{
					return lhs;
				}

				Debug.Assert(this._tokenStream.CurrentToken is OperatorToken);
				string binaryOperator = (this._tokenStream.CurrentToken as OperatorToken).Operator;
				this._tokenStream.Read(); // eat binary operator

				ExprAST rhs = this.ParsePrimaryExpr(); // parse expression after binary operator
				if (rhs == null)
				{
					return null; // forward error
				}

				// if binary operator binds less tightly with RHS than the operator after RHS, let the pending operator take RHS as its LHS.
				int nextPrecedence = this.GetCurrentTokenPrecedence();
				if (tokenPrecedence < nextPrecedence)
				{
					rhs = this.ParseBinOpRHS(tokenPrecedence + 1, rhs);
					if (rhs == null)
					{
						return null;
					}
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
		private ExprAST ParseExpression()
		{
			ExprAST lhs = this.ParsePrimaryExpr(); // left hand side of expression
			if (lhs == null)
			{
				return null; // forward error
			}

			return this.ParseBinOpRHS(0, lhs);
		}

		/// <summary>
		/// Parses a top level expression and wraps it into an anonymous function
		/// <code>
		/// toplevelexpr ::= expression
		/// </code>
		/// </summary>
		/// <returns>A <c>FunctionAST</c> representing the anonymous function</returns>
		// private FunctionAST ParseTopLevelExpr()
		// {
		// 	ExprAST expression = this.ParseExpression();
		// 	if (expression == null) return null;

		// 	// make an anonymous function prototype
		// 	PrototypeAST prototype = new PrototypeAST(string.Empty, new List<string>());
		// 	return new FunctionAST(prototype, expression);
		// }

		/// <summary>
		/// Parses a statement
		/// <code>
		/// statement
		/// 	::= toplevelexpr
		/// 	::= declarationstatement
		/// </code>
		/// </summary>
		/// <returns></returns>
		private ASTNode ParseStatement()
		{
			if (this._tokenStream.CurrentToken.TokenType == TokenType.Keyword_LET)
			{
				return this.ParseVariableDeclarationStatement();
			}
			else if (this._tokenStream.CurrentToken.TokenType == TokenType.Keyword_FUNCTION)
			{
				return this.ParseFunctionStatement();
			}
			else
			{
				return this.ParseExpression();
			}
		}

		/// <summary>
		/// Parses a statement
		/// </summary>
		/// <returns>The parsed statement</returns>
		public ASTNode HandleStatement()
		{
			ASTNode statement = this.ParseStatement();

			if (statement == null)
			{
				this._tokenStream.Read(); // eat next token for error recovery
				return null;
			}
			else
			{
				return statement;
			}
		}
	}
}

