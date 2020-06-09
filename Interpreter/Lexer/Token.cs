namespace Interpreter.Lexer {
	public enum Token : ushort {
		/// <summary>
		/// End of file token
		/// </summary>
		EndOfFile,
		/// <summary>
		/// Function definition keyword
		/// </summary>
		Definition,
		/// <summary>
		/// External reference
		/// </summary>
		Extern,
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
		Character
	}
}
