using NUnit.Framework;
using MathWizard.Math.AST;
using MathWizard.Math.Simplification;

namespace MathWizard.Math.Tests.Simplification
{
    public class ExpressionSimplifierTests
    {
        private ExpressionSimplifier simplifier;

        [SetUp]
        public void SetUp()
        {
            simplifier = new ExpressionSimplifier();
        }

        [Test]
        public void FoldsConstantAddition()
        {
            var node = new BinaryNode(BinaryOperator.Add, new ConstantNode(3), new ConstantNode(4));

            var result = simplifier.Simplify(node) as ConstantNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.Value);
        }

        [Test]
        public void DropsAdditiveZero_OnEitherSide()
        {
            var rightZero = new BinaryNode(BinaryOperator.Add, new VariableNode("x"), new ConstantNode(0));
            var leftZero = new BinaryNode(BinaryOperator.Add, new ConstantNode(0), new VariableNode("x"));

            Assert.AreEqual("x", simplifier.Simplify(rightZero).ToString());
            Assert.AreEqual("x", simplifier.Simplify(leftZero).ToString());
        }

        [Test]
        public void PowerOfOne_ReturnsBase()
        {
            var node = new BinaryNode(BinaryOperator.Power, new VariableNode("x"), new ConstantNode(1));

            Assert.AreEqual("x", simplifier.Simplify(node).ToString());
        }

        [Test]
        public void PowerOfZero_ReturnsOne()
        {
            var node = new BinaryNode(BinaryOperator.Power, new VariableNode("x"), new ConstantNode(0));

            var result = simplifier.Simplify(node) as ConstantNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void MultiplyByOne_ReturnsOtherFactor()
        {
            var node = new BinaryNode(BinaryOperator.Multiply, new ConstantNode(1), new VariableNode("x"));

            Assert.AreEqual("x", simplifier.Simplify(node).ToString());
        }

        [Test]
        public void MultiplyByZero_ReturnsZero()
        {
            var node = new BinaryNode(BinaryOperator.Multiply, new ConstantNode(0), new VariableNode("x"));

            var result = simplifier.Simplify(node) as ConstantNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value);
        }

        [Test]
        public void CollectsNestedConstantFactors_AcrossMultiplication()
        {
            // 3 * (2 * x) -> 6x
            var inner = new BinaryNode(BinaryOperator.Multiply, new ConstantNode(2), new VariableNode("x"));
            var node = new BinaryNode(BinaryOperator.Multiply, new ConstantNode(3), inner);

            Assert.AreEqual("6x", simplifier.Simplify(node).ToString());
        }

        [Test]
        public void DoubleNegation_Cancels()
        {
            var node = new UnaryNode(UnaryOperator.Negate, new UnaryNode(UnaryOperator.Negate, new VariableNode("x")));

            Assert.AreEqual("x", simplifier.Simplify(node).ToString());
        }

        [Test]
        public void Simplify_OfRawPolynomialDerivative_MatchesExpectedAnswer()
        {
            // Raw (unsimplified) derivative of 3x^2 + 4x + 1:
            // (3 * (2 * x^1)) + (4 * 1) + 0
            var term1 = new BinaryNode(
                BinaryOperator.Multiply,
                new ConstantNode(3),
                new BinaryNode(
                    BinaryOperator.Multiply,
                    new ConstantNode(2),
                    new BinaryNode(BinaryOperator.Power, new VariableNode("x"), new ConstantNode(1))));

            var term2 = new BinaryNode(BinaryOperator.Multiply, new ConstantNode(4), new ConstantNode(1));

            var raw = new BinaryNode(
                BinaryOperator.Add,
                new BinaryNode(BinaryOperator.Add, term1, term2),
                new ConstantNode(0));

            Assert.AreEqual("6x + 4", simplifier.Simplify(raw).ToString());
        }
    }
}
