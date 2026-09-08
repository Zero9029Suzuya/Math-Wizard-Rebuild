using System.Collections.Generic;
using System.Globalization;
using MathWizard.Math.AST;

namespace MathWizard.Math.Parsing
{
    /// <summary>
    /// NOTE: Should be finished needs more testing.
    /// 
    /// TODO: Add differentiation string equivalent... as it would not detect d/dx
    /// Parses a math expression string into a MathNode tree.
    ///
    /// Grammar (lowest to highest precedence):
    ///   Expression := Term (('+' | '-') Term)*
    ///   Term       := Unary ( ('*' | '/') Unary | ImplicitFactor )*
    ///   Unary      := '-' Unary | Power
    ///   Power      := Atom ('^' Unary)?                           (right-associative 3^2 will never be 2^3).
    ///   Atom       := Number | Identifier | '(' Expression ')'
    ///
    /// "ImplicitFactor" lets "3x" and "3x^2" parse as multiplication without an explicit '*' or multiplier.
    /// </summary>
    public class MathParser
    {
        private List<Token> tokens;
        private int position;

        public ParseResult Parse(string expression)
        {
            try
            {
                tokens = new MathTokenizer(expression).Tokenize();
                position = 0;

                MathNode root = ParseExpression();

                if (Current.Type != TokenType.End)
                {
                    return ParseResult.Fail($"Unexpected token '{Current.Text}' after expression.");
                }

                return ParseResult.Ok(root);
            }
            catch (MathParseException ex)
            {
                return ParseResult.Fail(ex.Message);
            }
        }

        private Token Current => tokens[position];

        private Token Advance()
        {
            Token token = tokens[position];
            if (position < tokens.Count - 1)
            {
                position++;
            }

            return token;
        }

        private bool Check(TokenType type) => Current.Type == type;

        private Token Expect(TokenType type, string message)
        {
            if (!Check(type))
            {
                throw new MathParseException(message);
            }

            return Advance();
        }

        private MathNode ParseExpression()
        {
            MathNode left = ParseTerm();

            while (Check(TokenType.Plus) || Check(TokenType.Minus))
            {
                BinaryOperator op = Check(TokenType.Plus) ? BinaryOperator.Add : BinaryOperator.Subtract;
                Advance();
                MathNode right = ParseTerm();
                left = new BinaryNode(op, left, right);
            }

            return left;
        }

        private MathNode ParseTerm()
        {
            MathNode left = ParseUnary();

            while (true)
            {
                if (Check(TokenType.Star) || Check(TokenType.Slash))
                {
                    BinaryOperator op = Check(TokenType.Star) ? BinaryOperator.Multiply : BinaryOperator.Divide;
                    Advance();
                    MathNode right = ParseUnary();
                    left = new BinaryNode(op, left, right);
                    continue;
                }

                if (StartsFactor(Current))
                {
                    // e.g. "3x", "3x^2", "2(x + 1)" — no explicit operator between factors.
                    MathNode right = ParseUnary();
                    left = new BinaryNode(BinaryOperator.Multiply, left, right);
                    continue;
                }

                break;
            }

            return left;
        }

        private static bool StartsFactor(Token token)
        {
            return token.Type == TokenType.Number
                || token.Type == TokenType.Identifier
                || token.Type == TokenType.LParen;
        }

        private MathNode ParseUnary()
        {
            if (Check(TokenType.Minus))
            {
                Advance();
                MathNode operand = ParseUnary();
                return new UnaryNode(UnaryOperator.Negate, operand);
            }

            return ParsePower();
        }

        private MathNode ParsePower()
        {
            MathNode baseNode = ParseAtom();

            if (Check(TokenType.Caret))
            {
                Advance();
                MathNode exponent = ParseUnary(); // allows x^-2
                return new BinaryNode(BinaryOperator.Power, baseNode, exponent);
            }

            return baseNode;
        }

        private MathNode ParseAtom()
        {
            if (Check(TokenType.Number))
            {
                Token token = Advance();
                return new ConstantNode(double.Parse(token.Text, CultureInfo.InvariantCulture));
            }

            if (Check(TokenType.Identifier))
            {
                Token token = Advance();
                return new VariableNode(token.Text);
            }

            if (Check(TokenType.LParen))
            {
                Advance();
                MathNode inner = ParseExpression();
                Expect(TokenType.RParen, "Expected ')'.");
                return inner;
            }

            throw new MathParseException($"Unexpected token '{Current.Text}'.");
        }
    }
}
