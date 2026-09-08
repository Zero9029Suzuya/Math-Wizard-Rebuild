using System.Collections.Generic;

namespace MathWizard.Math.Parsing
{
    /// <summary>
    /// Converts a raw expression string into tokens. Knows nothing about grammar or
    /// precedence — that is MathParser's job.
    /// </summary>
    internal class MathTokenizer
    {
        private readonly string source;
        private int position;

        public MathTokenizer(string source)
        {
            this.source = source ?? string.Empty;
            position = 0;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (true)
            {
                SkipWhitespace();

                if (position >= source.Length)
                {
                    tokens.Add(new Token(TokenType.End, string.Empty));
                    break;
                }

                char current = source[position];

                if (char.IsDigit(current) || current == '.')
                {
                    tokens.Add(ReadNumber());
                    continue;
                }

                if (char.IsLetter(current))
                {
                    tokens.Add(ReadIdentifier());
                    continue;
                }

                switch (current)
                {
                    case '+': tokens.Add(new Token(TokenType.Plus, "+")); position++; break;
                    case '-': tokens.Add(new Token(TokenType.Minus, "-")); position++; break;
                    case '*': tokens.Add(new Token(TokenType.Star, "*")); position++; break;
                    case '/': tokens.Add(new Token(TokenType.Slash, "/")); position++; break;
                    case '^': tokens.Add(new Token(TokenType.Caret, "^")); position++; break;
                    case '(': tokens.Add(new Token(TokenType.LParen, "(")); position++; break;
                    case ')': tokens.Add(new Token(TokenType.RParen, ")")); position++; break;
                    default:
                        throw new MathParseException($"Unexpected character '{current}' at position {position}.");
                }
            }

            return tokens;
        }

        private void SkipWhitespace()
        {
            while (position < source.Length && char.IsWhiteSpace(source[position]))
            {
                position++;
            }
        }

        private Token ReadNumber()
        {
            int start = position;
            bool sawDot = false;

            while (position < source.Length && (char.IsDigit(source[position]) || (source[position] == '.' && !sawDot)))
            {
                if (source[position] == '.')
                {
                    sawDot = true;
                }

                position++;
            }

            string text = source.Substring(start, position - start);
            return new Token(TokenType.Number, text);
        }

        private Token ReadIdentifier()
        {
            int start = position;

            while (position < source.Length && char.IsLetter(source[position]))
            {
                position++;
            }

            string text = source.Substring(start, position - start);
            return new Token(TokenType.Identifier, text);
        }
    }
}
