using NUnit.Framework;
using MathWizard.Math.AST;
using MathWizard.Math.Parsing;

namespace MathWizard.Math.Tests.Parsing
{
    public class MathParserTests
    {
        private MathParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new MathParser();
        }

        [Test]
        public void Parses_Constant()
        {
            ParseResult result = parser.Parse("3");

            Assert.IsTrue(result.Success);
            var node = result.Root as ConstantNode;
            Assert.IsNotNull(node);
            Assert.AreEqual(3, node.Value);
        }

        [Test]
        public void Parses_Variable()
        {
            ParseResult result = parser.Parse("x");

            Assert.IsTrue(result.Success);
            var node = result.Root as VariableNode;
            Assert.IsNotNull(node);
            Assert.AreEqual("x", node.Name);
        }

        [Test]
        public void Parses_NegatedVariable()
        {
            ParseResult result = parser.Parse("-x");

            Assert.IsTrue(result.Success);
            var node = result.Root as UnaryNode;
            Assert.IsNotNull(node);
            Assert.AreEqual(UnaryOperator.Negate, node.Operator);
            Assert.IsInstanceOf<VariableNode>(node.Operand);
        }

        [Test]
        public void Parses_Addition()
        {
            ParseResult result = parser.Parse("3 + 4");

            Assert.IsTrue(result.Success);
            var node = result.Root as BinaryNode;
            Assert.IsNotNull(node);
            Assert.AreEqual(BinaryOperator.Add, node.Operator);
            Assert.AreEqual(3, ((ConstantNode)node.Left).Value);
            Assert.AreEqual(4, ((ConstantNode)node.Right).Value);
        }

        [Test]
        public void Parses_ImplicitMultiplication()
        {
            ParseResult result = parser.Parse("3x");

            Assert.IsTrue(result.Success);
            var node = result.Root as BinaryNode;
            Assert.IsNotNull(node);
            Assert.AreEqual(BinaryOperator.Multiply, node.Operator);
            Assert.AreEqual(3, ((ConstantNode)node.Left).Value);
            Assert.AreEqual("x", ((VariableNode)node.Right).Name);
        }

        [Test]
        public void Parses_Power()
        {
            ParseResult result = parser.Parse("x^2");

            Assert.IsTrue(result.Success);
            var node = result.Root as BinaryNode;
            Assert.IsNotNull(node);
            Assert.AreEqual(BinaryOperator.Power, node.Operator);
            Assert.AreEqual("x", ((VariableNode)node.Left).Name);
            Assert.AreEqual(2, ((ConstantNode)node.Right).Value);
        }

        [Test]
        public void Parses_Polynomial_MatchingHandBuiltTree()
        {
            ParseResult result = parser.Parse("3x^2 + 4x + 1");

            Assert.IsTrue(result.Success);

            // Same shape as the hand-built tree from Milestone 1:
            // Add(Add(Multiply(3, Power(x, 2)), Multiply(4, x)), 1)
            var expression = result.Root as BinaryNode;
            Assert.IsNotNull(expression);
            Assert.AreEqual(BinaryOperator.Add, expression.Operator);
            Assert.AreEqual(1, ((ConstantNode)expression.Right).Value);

            var inner = expression.Left as BinaryNode;
            Assert.IsNotNull(inner);
            Assert.AreEqual(BinaryOperator.Add, inner.Operator);

            var threeXSquared = inner.Left as BinaryNode;
            Assert.IsNotNull(threeXSquared);
            Assert.AreEqual(BinaryOperator.Multiply, threeXSquared.Operator);
            Assert.AreEqual(3, ((ConstantNode)threeXSquared.Left).Value);

            var power = threeXSquared.Right as BinaryNode;
            Assert.IsNotNull(power);
            Assert.AreEqual(BinaryOperator.Power, power.Operator);
            Assert.AreEqual("x", ((VariableNode)power.Left).Name);
            Assert.AreEqual(2, ((ConstantNode)power.Right).Value);

            var fourX = inner.Right as BinaryNode;
            Assert.IsNotNull(fourX);
            Assert.AreEqual(BinaryOperator.Multiply, fourX.Operator);
            Assert.AreEqual(4, ((ConstantNode)fourX.Left).Value);
            Assert.AreEqual("x", ((VariableNode)fourX.Right).Name);
        }

        [Test]
        public void Parses_Parentheses()
        {
            ParseResult result = parser.Parse("2 * (x + 1)");

            Assert.IsTrue(result.Success);
            var node = result.Root as BinaryNode;
            Assert.IsNotNull(node);
            Assert.AreEqual(BinaryOperator.Multiply, node.Operator);
            Assert.IsInstanceOf<BinaryNode>(node.Right);
            Assert.AreEqual(BinaryOperator.Add, ((BinaryNode)node.Right).Operator);
        }

        [Test]
        public void RoundTrip_ParseThenToString_ReproducesEquivalentText()
        {
            ParseResult first = parser.Parse("3x^2 + 4x + 1");
            string rendered = first.Root.ToString();

            ParseResult second = parser.Parse(rendered);

            Assert.IsTrue(second.Success);
            Assert.AreEqual(rendered, second.Root.ToString());
        }

        [Test]
        public void Fails_OnIncompleteExpression()
        {
            ParseResult result = parser.Parse("3 + ");

            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Root);
            Assert.IsNotEmpty(result.Error);
        }

        [Test]
        public void Fails_OnUnknownCharacter()
        {
            ParseResult result = parser.Parse("3 $ 4");

            Assert.IsFalse(result.Success);
        }
    }
}
