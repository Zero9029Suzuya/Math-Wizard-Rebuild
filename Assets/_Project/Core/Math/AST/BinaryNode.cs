namespace MathWizard.Math.AST
{
    /// <summary>
    /// A node with two operands joined by a binary operator, e.g. (3 + 4), (x ^ 2), (3 * x).
    /// </summary>
    public class BinaryNode : MathNode
    {
        public BinaryOperator Operator { get; }
        public MathNode Left { get; }
        public MathNode Right { get; }

        public BinaryNode(BinaryOperator op, MathNode left, MathNode right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public override string ToString()
        {
            int precedence = MathNodeFormatting.Precedence(this);

            switch (Operator)
            {
                case BinaryOperator.Add:
                    return $"{MathNodeFormatting.RenderChild(Left, precedence, false)} + {MathNodeFormatting.RenderChild(Right, precedence, false)}";

                case BinaryOperator.Subtract:
                    return $"{MathNodeFormatting.RenderChild(Left, precedence, false)} - {MathNodeFormatting.RenderChild(Right, precedence, true)}";

                case BinaryOperator.Multiply:
                    return RenderMultiply(precedence);

                case BinaryOperator.Divide:
                    return $"{MathNodeFormatting.RenderChild(Left, precedence, false)} / {MathNodeFormatting.RenderChild(Right, precedence, true)}";

                case BinaryOperator.Power:
                    return $"{MathNodeFormatting.RenderChild(Left, precedence, true)}^{MathNodeFormatting.RenderChild(Right, precedence, false)}";

                default:
                    return base.ToString();
            }
        }

        private string RenderMultiply(int precedence)
        {
            string left = MathNodeFormatting.RenderChild(Left, precedence, false);
            string right = MathNodeFormatting.RenderChild(Right, precedence, false);

            // Prefer "3x" / "3x^2" over "3 * x" / "3 * x^2" for a constant coefficient
            // directly followed by a variable (optionally raised to a power).
            return IsImplicitCoefficient() ? $"{left}{right}" : $"{left} * {right}";
        }

        private bool IsImplicitCoefficient()
        {
            if (!(Left is ConstantNode))
            {
                return false;
            }

            if (Right is VariableNode)
            {
                return true;
            }

            return Right is BinaryNode rightBinary
                && rightBinary.Operator == BinaryOperator.Power
                && rightBinary.Left is VariableNode;
        }
    }
}
