using System;
using NUnit.Framework;
using MathWizard.Math.AST;
using MathWizard.Math.Calculus;
using MathWizard.Math.Parsing;

namespace MathWizard.Math.Tests.Calculus
{
    public class DifferentiatorTests
    {
        private Differentiator differentiator;
        private MathParser parser;

        [SetUp]
        public void SetUp()
        {
            differentiator = new Differentiator();
            parser = new MathParser();
        }

        [Test]
        public void ConstantRule_DerivativeIsZero()
        {
            var result = differentiator.Differentiate(new ConstantNode(5), "x") as ConstantNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value);
        }

        [Test]
        public void VariableRule_DerivativeOfMatchingVariableIsOne()
        {
            var result = differentiator.Differentiate(new VariableNode("x"), "x") as ConstantNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void VariableRule_DerivativeOfOtherVariableIsZero()
        {
            var result = differentiator.Differentiate(new VariableNode("y"), "x") as ConstantNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value);
        }

        [Test]
        public void PowerRule_XCubed_MatchesThreeXSquared()
        {
            MathNode expression = parser.Parse("x^3").Root;

            MathNode derivative = differentiator.Differentiate(expression, "x");

            Assert.AreEqual("3x^2", derivative.ToString());
        }

        [Test]
        public void SumRule_And_DifferenceRule_DifferentiateEachTerm()
        {
            MathNode sum = parser.Parse("x + 3").Root;
            MathNode difference = parser.Parse("x - 3").Root;

            Assert.AreEqual(BinaryOperator.Add, ((BinaryNode)differentiator.Differentiate(sum, "x")).Operator);
            Assert.AreEqual(BinaryOperator.Subtract, ((BinaryNode)differentiator.Differentiate(difference, "x")).Operator);
        }

        [Test]
        public void ConstantMultipleRule_FourX_MatchesFour()
        {
            MathNode expression = parser.Parse("4x").Root;

            MathNode derivative = differentiator.Differentiate(expression, "x");

            // 4 * 1, not yet simplified — the solver's job to simplify, not the differentiator's.
            Assert.AreEqual("4 * 1", derivative.ToString());
        }

        [Test]
        public void ProductOfTwoNonConstantFactors_Throws()
        {
            MathNode expression = parser.Parse("x * x").Root;

            Assert.Throws<NotSupportedException>(() => differentiator.Differentiate(expression, "x"));
        }

        [Test]
        public void CompositeBase_RequiresChainRule_Throws()
        {
            MathNode expression = parser.Parse("(x + 1)^2").Root;

            Assert.Throws<NotSupportedException>(() => differentiator.Differentiate(expression, "x"));
        }

        [Test]
        public void VariableExponent_Throws()
        {
            MathNode expression = parser.Parse("2^x").Root;

            Assert.Throws<NotSupportedException>(() => differentiator.Differentiate(expression, "x"));
        }

        [Test]
        public void Division_Throws()
        {
            MathNode expression = parser.Parse("x / 2").Root;

            Assert.Throws<NotSupportedException>(() => differentiator.Differentiate(expression, "x"));
        }
    }
}
