using System.IO;
using System.Text;

namespace Interpreter.Lexer {
	public sealed class Lexer {
		/// <summary>
		/// The reader for the Lexer to read from
		/// </summary>
		public TextReader Reader { get; private set; }

		private readonly StringBuilder _identifierBuilder = new StringBuilder();
		private readonly StringBuilder _numberBuilder = new StringBuilder();

		/// <summary>
		/// Creates a new Lexer instance which reads from the <c>TextReader</c>
		/// </summary>
		/// <param name="reader">The reader to read input from</param>
		public Lexer(TextReader reader) {
			this.Reader = reader;
		}

		/// <summary>
		/// Reads the next <c>Token</c>
		/// </summary>
		public Token GetNextToken() {
			const int EOF = -1; // end of file
			int c = ' ';

			// skip any whitespace characters
			while (char.IsWhiteSpace((char)c)) {
				c = this.Reader.Read();
			}

			// c is a letter or '_': token can be a keyword or an identifier
			if (char.IsLetter((char)c) || c == '_') {
				this._identifierBuilder.Append((char)c);

				// get the entire identifier
				while (char.IsLetterOrDigit((char)this.Reader.Peek()) || this.Reader.Peek() == '_') {
					this._identifierBuilder.Append((char)this.Reader.Read());
				}

				string identifier = this._identifierBuilder.ToString(); // save identifier string
				this._identifierBuilder.Clear(); // clear _identifierBuilder for next token

				// determine if identifier is a keyword
				Token token = identifier switch
				{
					"function" => new Token(TokenType.Keyword_FUNCTION),
					"extern" => new Token(TokenType.Keyword_EXTERN),
					"let" => new Token(TokenType.Keyword_LET),
					_ => new IdentifierToken(identifier) // identifier is not a keyword, identifier is a variable identifier
				};

				return token;
			}

			// c is a number literal
			else if (char.IsDigit((char)c)) {
				this._numberBuilder.Append((char)c);
				// while next char is a number or '.', add to _numberBuilder
				while (char.IsDigit((char)this.Reader.Peek()) || this.Reader.Peek() == '.') {
					this._numberBuilder.Append((char)this.Reader.Read());
				}

				double value = double.Parse(this._numberBuilder.ToString());
				this._numberBuilder.Clear(); // clear _numberBuilder for next token

				return new NumberToken(value);
			}

			// comment until end of line
			else if (c == '#') {
				while (c != EOF && c != '\n' && c != '\r') {
					c = this.Reader.Read(); // throw away next char
				}

				// end of file not reached, continue reading stream to get next token
				if (c != EOF) {
					return this.GetNextToken(); // recursively get next token after comment
				}
			}

			// found end of file
			else if (c == EOF) {
				return new Token(TokenType.EndOfFile);
			}

			// return a character
			string operatorLexeme = ((char)c).ToString();
			return new OperatorToken(operatorLexeme);
		}
	}
}
