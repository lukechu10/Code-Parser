using Interpreter.Lexer;
using System.Collections.Generic;
using System.IO;

namespace Interpreter
{
	public sealed class TokenStream
	{
		private readonly string _str;
		private Lexer.Lexer _lexer;
		/// <summary>
		/// Current index of value in <c>List<Token> Tokens<c>
		/// </summary>
		private int _index = 0;
		public List<Token> Tokens { get; private set; } = new List<Token>();

		/// <summary>
		/// Tokenizes a string and creates a <c>TokenStream<c>
		/// </summary>
		/// <param name="str">The string for the <c>TokenStream</c> to read from</param>
		public TokenStream(string str)
		{
			this._str = str;
			this.TokenizeString();
		}

		/// <summary>
		/// Tokenizes the entire string <c>this._str</c>
		/// </summary>
		private void TokenizeString()
		{
			// create string reader
			var stringReader = new StringReader(this._str);
			this._lexer = new Lexer.Lexer(stringReader);

			// tokenize entire string
			Token nextToken;
			do
			{
				nextToken = this._lexer.GetNextToken();
				this.Tokens.Add(nextToken);
			} while (nextToken.TokenType != TokenType.EndOfFile);
		}

		/// <summary>
		/// Reads the next token in the <c>TokenStream<c> and increments the index (eats current token)
		/// </summary>
		/// <returns>The next <c>Token<c></returns>
		public Token Read()
		{
			return this.Tokens[_index++];
		}

		/// <summary>
		/// Peeks at the next token in the <c>TokenStream<c>. This method has no side effects and does not eat the current token.
		/// </summary>
		/// <returns>The next <c>Token<c></returns>
		public Token Peek()
		{
			return this.Tokens[_index + 1];
		}

		/// <summary>
		/// Peeks at the <c>x</c>th token after the current token. Passing a paramater of 1 is equivalent to calling Peek() without arguments.
		/// </summary>
		/// <param name="x">The number of tokens to look ahead</param>
		/// <returns>The <c>x</c>th token after the current token</returns>
		public Token Peek(int x)
		{
			return this.Tokens[_index + x];
		}

		public Token CurrentToken => this.Tokens[_index];
	}
}