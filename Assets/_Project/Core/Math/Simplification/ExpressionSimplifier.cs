using MathWizard.Math.AST;

namespace MathWizard.Math.Simplification
{
    /// <summary>
    /// Reduces a MathNode tree using a small, deliberately limited set of rules:
    /// constant folding, additive/multiplicative identities (0, 1), and collecting
    /// constant factors across nested multiplications (e.g. 3 * (2 * x) -> 6x).
    ///
    /// This is not a general computer algebra system — it implements only what the
    /// current solver steps need. Combining like variable terms (3x + 4x -> 7x) is
    /// intentionally not implemented yet; add it if/when a solver step actually
    /// produces that shape.
    /// </summary>
    public class ExpressionSimplifier
    {
        public MathNode Simplify(MathNode node)
        {
            switch (node)
            {
                case ConstantNode _:
                case VariableNode _:
                    return node;

                case UnaryNode unary:
                    return SimplifyUnary(unary);

                case BinaryNode binary:
                    return SimplifyBinary(binary);

                default:
                    return node;
            }
        }

        private MathNode SimplifyUnary(UnaryNode unary)
        {
            MathNode operand = Simplify(unary.Operand);

            if (unary.Operator == UnaryOperator.Negate)
            {
                if (operand is ConstantNode constant)
                {
                    return new ConstantNode(-constant.Value);
                }

                if (operand is UnaryNode innerUnary && innerUnary.Operator == UnaryOperator.Negate)
                {
                    return innerUnary.Operand; // double negation: -(-x) -> x
                }
            }

            return new UnaryNode(unary.Operator, operand);
        }

        private MathNode SimplifyBinary(BinaryNode binary)
        {
            MathNode left = Simplify(binary.Left);
            MathNode right = Simplify(binary.Right);

            switch (binary.Operator)
            {
                case BinaryOperator.Add:
                    return SimplifyAdd(left, right);
                case BinaryOperator.Subtract:
                    return SimplifySubtract(left, right);
                case BinaryOperator.Multiply:
                    return SimplifyMultiply(left, right);
                case BinaryOperator.Divide:
                    return SimplifyDivide(left, right);
                case BinaryOperator.Power:
                    return SimplifyPower(left, right);
                default:
                    return new BinaryNode(binary.Operator, left, right);
            }
        }

        private static MathNode SimplifyAdd(MathNode left, MathNode right)
        {
            if (left is ConstantNode leftConstant && right is ConstantNode rightConstant)
            {
                return new ConstantNode(leftConstant.Value + rightConstant.Value);
            }

            if (IsZero(left))
            {
                return right;
            }

            if (IsZero(right))
            {
                return left;
            }

            return new BinaryNode(BinaryOperator.Add, left, right);
        }

        private static MathNode SimplifySubtract(MathNode left, MathNode right)
        {
            if (left is ConstantNode leftConstant && right is ConstantNode rightConstant)
            {
                return new ConstantNode(leftConstant.Value - rightConstant.Value);
            }

            if (IsZero(right))
            {
                return left;
            }

            return new BinaryNode(BinaryOperator.Subtract, left, right);
        }

        private static MathNode SimplifyMultiply(MathNode left, MathNode right)
        {
            var (coefficient, remainder) = ExtractConstantFactor(new BinaryNode(BinaryOperator.Multiply, left, right));

            if (remainder == null)
            {
                return new ConstantNode(coefficient);
            }

            if (coefficient == 0)
            {
                return new ConstantNode(0);
            }

            if (coefficient == 1)
            {
                return remainder;
            }

            if (coefficient == -1)
            {
                return new UnaryNode(UnaryOperator.Negate, remainder);
            }

            return new BinaryNode(BinaryOperator.Multiply, new ConstantNode(coefficient), remainder);
        }

        private static MathNode SimplifyDivide(MathNode left, MathNode right)
        {
            if (left is ConstantNode leftConstant && right is ConstantNode rightConstant && rightConstant.Value != 0)
            {
                return new ConstantNode(leftConstant.Value / rightConstant.Value);
            }

            if (IsZero(left) && !IsZero(right))
            {
                return new ConstantNode(0);
            }

            if (IsOne(right))
            {
                return left;
            }

            return new BinaryNode(BinaryOperator.Divide, left, right);
        }

        private static MathNode SimplifyPower(MathNode left, MathNode right)
        {
            if (IsZero(right))
            {
                return new ConstantNode(1);
            }

            if (IsOne(right))
            {
                return left;
            }

            if (left is ConstantNode leftConstant && right is ConstantNode rightConstant)
            {
                return new ConstantNode(global::System.Math.Pow(leftConstant.Value, rightConstant.Value));
            }

            return new BinaryNode(BinaryOperator.Power, left, right);
        }

        private static bool IsZero(MathNode node) => node is ConstantNode c && c.Value == 0;

        private static bool IsOne(MathNode node) => node is ConstantNode c && c.Value == 1;

        /// <summary>
        /// Pulls the accumulated constant factor out of a (possibly nested) chain of
        /// multiplications, e.g. 3 * (2 * x) -> (6, x). A null remainder means the
        /// whole chain was constant.
        /// </summary>
        private static (double coefficient, MathNode remainder) ExtractConstantFactor(MathNode node)
        {
            if (node is ConstantNode constant)
            {
                return (constant.Value, null);
            }

            if (node is BinaryNode binary && binary.Operator == BinaryOperator.Multiply)
            {
                var (leftCoefficient, leftRemainder) = ExtractConstantFactor(binary.Left);
                var (rightCoefficient, rightRemainder) = ExtractConstantFactor(binary.Right);

                double coefficient = leftCoefficient * rightCoefficient;
                MathNode remainder = CombineFactors(leftRemainder, rightRemainder);

                return (coefficient, remainder);
            }

            return (1, node);
        }

        private static MathNode CombineFactors(MathNode left, MathNode right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            return new BinaryNode(BinaryOperator.Multiply, left, right);
        }
    }
}
