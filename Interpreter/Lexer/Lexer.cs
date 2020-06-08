using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Interpreter.Lexer {
	class Lexer {
		/// <summary>
		/// The reader for the Lexer to read from
		/// </summary>
		private readonly TextReader _reader;

		#region token values
		/// <summary>
		/// Temp variable to store last variable identifier
		/// </summary>
		/// <value>String identifying the identifier</value>
		private string _lastIdentifier;
		/// <summary>
		/// Temp variable to store last double literal
		/// </summary>
		private double _lastNumber;
		/// <summary>
		/// Temp variable to store last char token
		/// </summary>
		private char _lastCharacter;
		#endregion
		private readonly StringBuilder _identifierBuilder = new StringBuilder();
		private readonly StringBuilder _numberBuilder = new StringBuilder();

		/// <summary>
		/// Creates a new Lexer instance which tokenizes the string str into <c>Token</c>s
		/// </summary>
		/// <param name="str">the string to be tokenized</param>
		public Lexer(TextReader reader) {
			this._reader = reader;
		}

		/// <summary>
		/// Tokenizes _str and saves array of tokens
		/// </summary>
		public Token GetNextToken() {
			const int EOF = -1; // end of file
			int c = ' ';

			// skip any whitespace characters
			while (char.IsWhiteSpace((char)c)) {
				c = this._reader.Read();
			}

			// c is a letter: token can be a keyword or an identifier
			if (char.IsLetter((char)c)) {
				this._identifierBuilder.Append((char)c);

				// get the entire identifier
				while (char.IsLetterOrDigit((char)this._reader.Peek())) {
					this._identifierBuilder.Append((char)this._reader.Read());
				}

				this._lastIdentifier = _identifierBuilder.ToString();
				this._identifierBuilder.Clear(); // clear _identifierBuilder for next token

				// token type
				Token token = this._lastIdentifier switch
				{
					"def" => Token.Definition,
					"extern" => Token.Extern,
					_ => Token.Identifier // identifier is not a keyword, identifier is a variable identifier
				};
				return token;
			}

			// c is a number literal
			else if (char.IsDigit((char)c)) {
				do {
					this._numberBuilder.Append(c);
					c = this._reader.Read();
				} while (char.IsDigit((char)c) || c == '.');

				this._lastNumber = double.Parse(this._numberBuilder.ToString());
				this._numberBuilder.Clear(); // clear _numberBuilder for next token

				return Token.Number;
			}

			// comment until end of line
			else if (c == '#') {
				do {
					c = this._reader.Read();
				} while (c != EOF && c != '\n' && c != '\r');

				if (c != EOF) return this.GetNextToken(); // recursively get next token after comment
			}

			// found end of file
			else if (c == EOF) {
				return Token.EndOfFile;
			}

			// return character
			this._lastCharacter = (char)c;
			return Token.Character;
		}
	}
}
