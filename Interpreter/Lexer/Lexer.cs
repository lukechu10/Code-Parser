using System.IO;
using System.Text;

namespace Interpreter.Lexer {
	public sealed class Lexer {
		/// <summary>
		/// The reader for the Lexer to read from
		/// </summary>
		public TextReader Reader { get; private set; }

		#region token values
		/// <summary>
		/// Temp variable to store last variable identifier
		/// </summary>
		/// <value>String identifying the identifier</value>
		public string LastIdentifier { get; private set; }
		/// <summary>
		/// Temp variable to store last double literal
		/// </summary>
		public double LastNumber { get; private set; }
		/// <summary>
		/// Temp variable to store last char token
		/// </summary>
		public char LastCharacter { get; private set; }
		#endregion
		private readonly StringBuilder _identifierBuilder = new StringBuilder();
		private readonly StringBuilder _numberBuilder = new StringBuilder();

		/// <summary>
		/// Creates a new Lexer instance which tokenizes the string str into <c>Token</c>s
		/// </summary>
		/// <param name="str">the string to be tokenized</param>
		public Lexer(TextReader reader) {
			this.Reader = reader;
		}

		/// <summary>
		/// Tokenizes _str and saves array of tokens
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

				this.LastIdentifier = this._identifierBuilder.ToString();
				this._identifierBuilder.Clear(); // clear _identifierBuilder for next token

				// token type
				Token token = this.LastIdentifier switch
				{
					"def" => Token.Definition,
					"extern" => Token.Extern,
					_ => Token.Identifier // identifier is not a keyword, identifier is a variable identifier
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

				this.LastNumber = double.Parse(this._numberBuilder.ToString());
				this._numberBuilder.Clear(); // clear _numberBuilder for next token

				return Token.Number;
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
				return Token.EndOfFile;
			}

			// return a character
			this.LastCharacter = (char)c;
			return Token.Character;
		}
	}
}
