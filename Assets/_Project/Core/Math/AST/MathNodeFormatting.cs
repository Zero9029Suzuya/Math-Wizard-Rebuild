namespace MathWizard.Math.AST
{
    /// <summary>
    /// AI 
    /// Shared precedence rules used by node ToString() implementations to decide
    /// when a child expression needs parentheses. Purely a rendering concern —
    /// it has no bearing on evaluation or parsing.
    /// </summary>
    internal static class MathNodeFormatting
    {
        private const int AddSubtractPrecedence = 0;
        private const int MultiplyDividePrecedence = 1;
        private const int UnaryPrecedence = 2;
        private const int PowerPrecedence = 3;
        private const int AtomPrecedence = 4;

        public static int Precedence(MathNode node)
        {
            switch (node)
            {
                case ConstantNode _:
                case VariableNode _:
                    return AtomPrecedence;

                case UnaryNode _:
                    return UnaryPrecedence;

                case BinaryNode binary:
                    switch (binary.Operator)
                    {
                        case BinaryOperator.Add:
                        case BinaryOperator.Subtract:
                            return AddSubtractPrecedence;
                        case BinaryOperator.Multiply:
                        case BinaryOperator.Divide:
                            return MultiplyDividePrecedence;
                        case BinaryOperator.Power:
                            return PowerPrecedence;
                        default:
                            return AtomPrecedence;
                    }

                default:
                    return AtomPrecedence;
            }
        }

        /// <summary>
        /// Renders a child node, wrapping it in parentheses if omitting them would
        /// change its meaning at the given parent precedence.
        /// </summary>
        /// <param name="requiresStrictlyHigher">
        /// True for positions where equal precedence still needs parentheses
        /// (e.g. the right side of subtraction/division, the left side of power).
        /// </param>
        public static string RenderChild(MathNode child, int parentPrecedence, bool requiresStrictlyHigher)
        {
            string text = child.ToString();
            int childPrecedence = Precedence(child);

            bool needsParens = requiresStrictlyHigher
                ? childPrecedence <= parentPrecedence
                : childPrecedence < parentPrecedence;

            return needsParens ? $"({text})" : text;
        }
    }
}
