using System;
using MathWizard.Math.AST;

namespace MathWizard.Math.Calculus
{
    /// <summary>
    /// Supported: Constant Rule, Variable Rule, Sum Rule, Difference Rule, Constant Multiple Rule [ 3(x + 2)(d/dx) ], Power Rule (x^n) (only if n is constant).
    /// 
    /// TODO:
    /// Not Supported (Will throw NotSupportedException - not descriptive yet):
    /// Product Rule for two non-constant factors, quotient rule, chain rule, variable exponents - This will probably be a new AST Concept (FunctionNode for trig)
    /// </summary>
    public class Differentiator
    {
        public MathNode Differentiate(MathNode expression, string variable)
        {
            switch (expression)
            {
                case ConstantNode _:
                    return new ConstantNode(0);

                case VariableNode variableNode:
                    return new ConstantNode(variableNode.Name == variable ? 1 : 0);

                case UnaryNode unary when unary.Operator == UnaryOperator.Negate:
                    return new UnaryNode(UnaryOperator.Negate, Differentiate(unary.Operand, variable));

                case BinaryNode binary:
                    return DifferentiateBinary(binary, variable);

                default:
                    throw new NotSupportedException(
                        $"Differentiation of '{expression.GetType().Name}' is not supported yet.");
            }
        }

        private MathNode DifferentiateBinary(BinaryNode binary, string variable)
        {
            switch (binary.Operator)
            {
                case BinaryOperator.Add:
                    // Sum Rule: d/dx(f + g) = f' + g'
                    return new BinaryNode(
                        BinaryOperator.Add,
                        Differentiate(binary.Left, variable),
                        Differentiate(binary.Right, variable));

                case BinaryOperator.Subtract:
                    // Difference Rule: d/dx(f - g) = f' - g'
                    return new BinaryNode(
                        BinaryOperator.Subtract,
                        Differentiate(binary.Left, variable),
                        Differentiate(binary.Right, variable));

                case BinaryOperator.Multiply:
                    return DifferentiateMultiply(binary, variable);

                case BinaryOperator.Power:
                    return DifferentiatePower(binary, variable);

                case BinaryOperator.Divide:
                    throw new NotSupportedException("The quotient rule is not supported yet.");

                default:
                    throw new NotSupportedException(
                        $"Differentiation of operator '{binary.Operator}' is not supported yet.");
            }
        }

        private MathNode DifferentiateMultiply(BinaryNode binary, string variable)
        {
            bool leftIsConstant = !DependsOn(binary.Left, variable);
            bool rightIsConstant = !DependsOn(binary.Right, variable);

            if (leftIsConstant && rightIsConstant)
            {
                return new ConstantNode(0);
            }

            if (leftIsConstant)
            {
                // Constant Multiple Rule: d/dx(c * f) = c * f'
                return new BinaryNode(BinaryOperator.Multiply, binary.Left, Differentiate(binary.Right, variable));
            }

            if (rightIsConstant)
            {
                return new BinaryNode(BinaryOperator.Multiply, Differentiate(binary.Left, variable), binary.Right);
            }

            throw new NotSupportedException("The product rule (two non-constant factors) is not supported yet.");
        }

        private MathNode DifferentiatePower(BinaryNode binary, string variable)
        {
            bool exponentDependsOnVariable = DependsOn(binary.Right, variable);

            if (exponentDependsOnVariable)
            {
                throw new NotSupportedException("Variable exponents are not supported yet.");
            }

            bool baseDependsOnVariable = DependsOn(binary.Left, variable);

            if (!baseDependsOnVariable)
            {
                // A constant base raised to a fixed power is itself constant.
                return new ConstantNode(0);
            }

            if (!(binary.Left is VariableNode baseVariable) || baseVariable.Name != variable)
            {
                throw new NotSupportedException("The chain rule for composite bases is not supported yet.");
            }

            if (!(binary.Right is ConstantNode exponent))
            {
                throw new NotSupportedException("Non-constant exponents are not supported yet.");
            }

            // Power Rule: d/dx(x^n) = n * x^(n-1)
            var reducedExponent = new ConstantNode(exponent.Value - 1);
            return new BinaryNode(
                BinaryOperator.Multiply,
                new ConstantNode(exponent.Value),
                new BinaryNode(BinaryOperator.Power, binary.Left, reducedExponent));
        }

        private bool DependsOn(MathNode node, string variable)
        {
            switch (node)
            {
                case VariableNode variableNode:
                    return variableNode.Name == variable;
                case ConstantNode _:
                    return false;
                case UnaryNode unary:
                    return DependsOn(unary.Operand, variable);
                case BinaryNode binary:
                    return DependsOn(binary.Left, variable) || DependsOn(binary.Right, variable);
                default:
                    return true; // unknown node types treated conservatively as variable-dependent
            }
        }
    }
}
