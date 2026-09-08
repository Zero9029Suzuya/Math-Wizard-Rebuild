using System.Collections.Generic;
using MathWizard.Math.AST;
using MathWizard.Math.Parsing;

namespace MathWizard.Math.Solving
{
    /// <summary>
    /// AI:
    /// One intermediate state produced while solving a problem: the expression at
    /// this point, its rendered text, a token breakdown for display/interaction, and
    /// which rule produced it. Storing the MathNode (not just text) means later
    /// steps, validation, or hints can inspect the actual structure instead of
    /// re-parsing rendered strings.
    /// </summary>
    public class SolverStep
    {
        public MathNode Expression { get; }
        public string Text { get; }
        public IReadOnlyList<SolverToken> Tokens { get; }
        public SolverRule Rule { get; }

        public SolverStep(MathNode expression, SolverRule rule)
        {
            Expression = expression;
            Rule = rule;
            Text = expression.ToString();
            Tokens = Tokenize(Text);
        }

        private static IReadOnlyList<SolverToken> Tokenize(string text)
        {
            // Reuses the parser's tokenizer purely to split rendered text into display
            // units. MathTokenizer/Token remain internal to Parsing — only SolverToken,
            // defined in this subsystem, ever leaves this method.
            List<Token> tokens = new MathTokenizer(text).Tokenize();
            var result = new List<SolverToken>(tokens.Count);

            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.End)
                {
                    continue;
                }

                result.Add(new SolverToken(token.Text));
            }

            return result;
        }
    }
}
