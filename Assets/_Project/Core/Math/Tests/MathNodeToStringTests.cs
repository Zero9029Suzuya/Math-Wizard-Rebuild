using NUnit.Framework;
using MathWizard.Math.AST;

namespace MathWizard.Math.Tests.AST
{
    /// <summary>
    /// Testing and Debugging.
    /// </summary>
    public class MathNodeToStringTests
    {
        [Test]
        public void Constant_RendersPlainNumber()
        {
            Assert.AreEqual("3", new ConstantNode(3).ToString());
        }

        [Test]
        public void Variable_RendersName()
        {
            Assert.AreEqual("x", new VariableNode("x").ToString());
        }

        [Test]
        public void Negate_RendersMinusPrefix()
        {
            var node = new UnaryNode(UnaryOperator.Negate, new VariableNode("x"));

            Assert.AreEqual("-x", node.ToString());
        }

        [Test]
        public void Add_RendersWithPlus()
        {
            var node = new BinaryNode(BinaryOperator.Add, new ConstantNode(3), new ConstantNode(4));

            Assert.AreEqual("3 + 4", node.ToString());
        }

        [Test]
        public void Multiply_ConstantByVariable_RendersAsImplicitCoefficient()
        {
            var node = new BinaryNode(BinaryOperator.Multiply, new ConstantNode(3), new VariableNode("x"));

            Assert.AreEqual("3x", node.ToString());
        }

        [Test]
        public void Power_RendersWithCaret()
        {
            var node = new BinaryNode(BinaryOperator.Power, new VariableNode("x"), new ConstantNode(2));

            Assert.AreEqual("x^2", node.ToString());
        }

        [Test]
        public void Polynomial_3xSquaredPlus4xPlus1_RendersWithoutRedundantParens()
        {
            var threeXSquared = new BinaryNode(
                BinaryOperator.Multiply,
                new ConstantNode(3),
                new BinaryNode(BinaryOperator.Power, new VariableNode("x"), new ConstantNode(2)));

            var fourX = new BinaryNode(BinaryOperator.Multiply, new ConstantNode(4), new VariableNode("x"));
            var innerSum = new BinaryNode(BinaryOperator.Add, threeXSquared, fourX);
            var expression = new BinaryNode(BinaryOperator.Add, innerSum, new ConstantNode(1));

            Assert.AreEqual("3x^2 + 4x + 1", expression.ToString());
        }

        [Test]
        public void Multiply_OfAddition_AddsParenthesesAroundLowerPrecedenceChild()
        {
            // (3 + 4) * 5
            var sum = new BinaryNode(BinaryOperator.Add, new ConstantNode(3), new ConstantNode(4));
            var node = new BinaryNode(BinaryOperator.Multiply, sum, new ConstantNode(5));

            Assert.AreEqual("(3 + 4) * 5", node.ToString());
        }

        [Test]
        public void Subtract_OfSubtraction_AddsParenthesesOnRightToPreserveMeaning()
        {
            // 3 - (4 - 5)
            var innerSubtract = new BinaryNode(BinaryOperator.Subtract, new ConstantNode(4), new ConstantNode(5));
            var node = new BinaryNode(BinaryOperator.Subtract, new ConstantNode(3), innerSubtract);

            Assert.AreEqual("3 - (4 - 5)", node.ToString());
        }
    }
}
