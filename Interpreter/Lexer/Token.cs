namespace Interpreter.Lexer
{
	public enum TokenType : ushort
	{
		/// <summary>
		/// End of file token
		/// </summary>
		EndOfFile,
		/// <summary>
		/// Identifier for a variable
		/// </summary>
		Identifier,
		/// <summary>
		/// Number literal expression
		/// </summary>
		Number,
		/// <summary>
		/// A Token represented by its character
		/// </summary>
		Operator,
		/// <summary>
		/// A Token representing the "let" keyword for variable declarations
		/// </summary>
		Keyword_LET,
		/// <summary>
		/// Function declaration keyword
		/// </summary>
		Keyword_FUNCTION,
		/// <summary>
		/// External reference keyword
		/// </summary>
		Keyword_EXTERN
	}

	public class Token
	{
		/// <summary>
		/// The type of the <c>Token</c>
		/// </summary>
		public TokenType TokenType { get; private set; }

		public Token(TokenType tokenType) => this.TokenType = tokenType;
	}

	public sealed class IdentifierToken : Token
	{
		public readonly string Identifier;

		public IdentifierToken(string identifier) : base(TokenType.Identifier)
		{
			this.Identifier = identifier;
		}
	}

	public sealed class NumberToken : Token
	{
		public readonly double Value;

		public NumberToken(double value) : base(TokenType.Number)
		{
			this.Value = value;
		}
	}

	public sealed class OperatorToken : Token
	{
		/// <summary>
		/// The lexeme of the operator represented by the <c>OperatorToken</c>
		/// </summary>
		public readonly string Operator;

		public OperatorToken(string operatorLexeme) : base(TokenType.Operator)
		{
			this.Operator = operatorLexeme;
		}
	}
}
