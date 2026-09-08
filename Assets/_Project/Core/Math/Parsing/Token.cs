namespace MathWizard.Math.Parsing
{
    /// <summary>
    /// Refer to TokenType for supported Tokens and Basic Symbols.
    /// </summary>
    internal readonly struct Token
    {
        public TokenType Type { get; }
        public string Text { get; }

        public Token(TokenType type, string text)
        {
            Type = type;
            Text = text;
        }
    }
}
