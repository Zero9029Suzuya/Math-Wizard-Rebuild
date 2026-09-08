using System;
using System.Collections.Generic;
using MathWizard.Math.AST;
using MathWizard.Math.Calculus;
using MathWizard.Math.Simplification;

namespace MathWizard.Math.Solving
{
    /// <summary>
    /// Solves "differentiate this expression" problems as a sequence of real AST
    /// transformations. At each step, exactly one not-yet-differentiated additive
    /// term is resolved — replaced in place by its true derivative, computed by
    /// Differentiator — and the resulting whole tree is recorded as a SolverStep.
    /// Once every term is resolved, a final ExpressionSimplifier pass produces the
    /// answer step.
    ///
    /// This walks Add/Subtract structure generically; it is not specific to any one
    /// expression. Everything below Add/Subtract (a bare constant, variable, unary
    /// negation, or non-sum binary expression such as 3x^2) is resolved atomically
    /// in one step via Differentiator — this milestone does not further decompose
    /// a single term's own rule chain (e.g. showing Constant Multiple Rule and
    /// Power Rule as two separate steps within one term). Doing that would require
    /// representing "an unresolved sub-operation" in the tree itself (something
    /// like a DerivativeNode) which does not exist yet — deliberately deferred
    /// until a later milestone where more than one capability needs it.
    /// </summary>
    public class DerivativeSolver
    {
        private readonly Differentiator differentiator = new Differentiator();
        private readonly ExpressionSimplifier simplifier = new ExpressionSimplifier();

        public SolverResult Solve(MathNode expression, string variable)
        {
            var steps = new List<SolverStep> { new SolverStep(expression, SolverRule.Initial) };
            var resolved = new HashSet<MathNode>();
            MathNode current = expression;

            while (true)
            {
                MathNode target = FindNextUnresolvedTerm(current, resolved);
                if (target == null)
                {
                    break;
                }

                MathNode derivative;
                try
                {
                    derivative = differentiator.Differentiate(target, variable);
                }
                catch (NotSupportedException ex)
                {
                    return SolverResult.Fail(ex.Message);
                }

                resolved.Add(derivative);
                current = ReplaceSubtree(current, target, derivative);
                steps.Add(new SolverStep(current, DescribeRuleForNode(target)));
            }

            MathNode simplified = simplifier.Simplify(current);
            steps.Add(new SolverStep(simplified, SolverRule.Simplification));

            return SolverResult.Ok(steps);
        }

        /// <summary>
        /// Finds the leftmost additive term in <paramref name="node"/> that has not
        /// yet been resolved. Recurses through Add/Subtract structure (which is
        /// purely organizational — the Sum/Difference Rule says "differentiate each
        /// side independently," so the operator itself never changes). Anything
        /// else is a leaf term: resolved if it's an object we previously produced
        /// as a derivative (tracked by reference, since MathNode has no overridden
        /// equality), otherwise it's our next target.
        /// </summary>
        private static MathNode FindNextUnresolvedTerm(MathNode node, HashSet<MathNode> resolved)
        {
            if (node is BinaryNode binary && (binary.Operator == BinaryOperator.Add || binary.Operator == BinaryOperator.Subtract))
            {
                return FindNextUnresolvedTerm(binary.Left, resolved) ?? FindNextUnresolvedTerm(binary.Right, resolved);
            }

            return resolved.Contains(node) ? null : node;
        }

        /// <summary>
        /// Rebuilds <paramref name="root"/> with the specific node instance
        /// <paramref name="target"/> replaced by <paramref name="replacement"/>,
        /// leaving every other subtree exactly as it was — including returning the
        /// very same object reference for any branch that didn't change, so
        /// resolved-term tracking (above) keeps working across repeated calls.
        /// </summary>
        private static MathNode ReplaceSubtree(MathNode root, MathNode target, MathNode replacement)
        {
            if (ReferenceEquals(root, target))
            {
                return replacement;
            }

            switch (root)
            {
                case BinaryNode binary:
                    MathNode left = ReplaceSubtree(binary.Left, target, replacement);
                    MathNode right = ReplaceSubtree(binary.Right, target, replacement);
                    if (ReferenceEquals(left, binary.Left) && ReferenceEquals(right, binary.Right))
                    {
                        return root;
                    }
                    return new BinaryNode(binary.Operator, left, right);

                case UnaryNode unary:
                    MathNode operand = ReplaceSubtree(unary.Operand, target, replacement);
                    return ReferenceEquals(operand, unary.Operand) ? root : new UnaryNode(unary.Operator, operand);

                default:
                    return root; // ConstantNode / VariableNode have no children.
            }
        }

        /// <summary>
        /// Tags a step with the rule that resolved the term just replaced.
        /// </summary>
        private static SolverRule DescribeRuleForNode(MathNode originalTerm)
        {
            switch (originalTerm)
            {
                case BinaryNode binary:
                    switch (binary.Operator)
                    {
                        case BinaryOperator.Multiply:
                            return SolverRule.ConstantMultipleRule;
                        case BinaryOperator.Power:
                            return SolverRule.PowerRule;
                        default:
                            return SolverRule.Differentiation;
                    }

                case VariableNode _:
                    return SolverRule.VariableRule;

                case ConstantNode _:
                    return SolverRule.ConstantRule;

                default:
                    return SolverRule.Differentiation;
            }
        }
    }
}
