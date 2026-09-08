using NUnit.Framework;
using MathWizard.Math.AST;
using MathWizard.Math.Parsing;
using MathWizard.Math.Solving;

namespace MathWizard.Math.Tests.Solving
{
    public class DerivativeSolverTests
    {
        private DerivativeSolver solver;
        private MathParser parser;

        [SetUp]
        public void SetUp()
        {
            solver = new DerivativeSolver();
            parser = new MathParser();
        }

        private static void AssertStep(SolverStep step, SolverRule expectedRule, string expectedText)
        {
            Assert.AreEqual(expectedRule, step.Rule);
            Assert.AreEqual(expectedText, step.Text);
        }

        [Test]
        public void Solve_Polynomial_ResolvesOneTermAtATime()
        {
            MathNode expression = parser.Parse("3x^2 + 4x + 1").Root;

            SolverResult result = solver.Solve(expression, "x");

            Assert.IsTrue(result.Success);
            Assert.AreEqual(5, result.Steps.Count);

            AssertStep(result.Steps[0], SolverRule.Initial, "3x^2 + 4x + 1");
            AssertStep(result.Steps[1], SolverRule.ConstantMultipleRule, "3 * 2x^1 + 4x + 1");
            AssertStep(result.Steps[2], SolverRule.ConstantMultipleRule, "3 * 2x^1 + 4 * 1 + 1");
            AssertStep(result.Steps[3], SolverRule.ConstantRule, "3 * 2x^1 + 4 * 1 + 0");
            AssertStep(result.Steps[4], SolverRule.Simplification, "6x + 4");
        }

        [Test]
        public void Solve_Constant_ProducesConstantRuleThenSimplification()
        {
            MathNode expression = parser.Parse("5").Root;

            SolverResult result = solver.Solve(expression, "x");

            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, result.Steps.Count);

            AssertStep(result.Steps[0], SolverRule.Initial, "5");
            AssertStep(result.Steps[1], SolverRule.ConstantRule, "0");
            AssertStep(result.Steps[2], SolverRule.Simplification, "0");
        }

        [Test]
        public void Solve_BareVariable_ProducesVariableRuleThenSimplification()
        {
            MathNode expression = parser.Parse("x").Root;

            SolverResult result = solver.Solve(expression, "x");

            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, result.Steps.Count);

            AssertStep(result.Steps[0], SolverRule.Initial, "x");
            AssertStep(result.Steps[1], SolverRule.VariableRule, "1");
            AssertStep(result.Steps[2], SolverRule.Simplification, "1");
        }

        [Test]
        public void Solve_SimpleSum_ResolvesEachTermInOrder()
        {
            MathNode expression = parser.Parse("x + 3").Root;

            SolverResult result = solver.Solve(expression, "x");

            Assert.IsTrue(result.Success);
            Assert.AreEqual(4, result.Steps.Count);

            AssertStep(result.Steps[0], SolverRule.Initial, "x + 3");
            AssertStep(result.Steps[1], SolverRule.VariableRule, "1 + 3");
            AssertStep(result.Steps[2], SolverRule.ConstantRule, "1 + 0");
            AssertStep(result.Steps[3], SolverRule.Simplification, "1");
        }

        [Test]
        public void Solve_ConstantMultiple_ProducesConstantMultipleRuleThenSimplification()
        {
            MathNode expression = parser.Parse("4x").Root;

            SolverResult result = solver.Solve(expression, "x");

            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, result.Steps.Count);

            AssertStep(result.Steps[0], SolverRule.Initial, "4x");
            AssertStep(result.Steps[1], SolverRule.ConstantMultipleRule, "4 * 1");
            AssertStep(result.Steps[2], SolverRule.Simplification, "4");
        }

        [Test]
        public void Solve_UnsupportedExpression_FailsWithReason()
        {
            MathNode expression = parser.Parse("x * x").Root;

            SolverResult result = solver.Solve(expression, "x");

            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Steps);
            Assert.IsNotEmpty(result.Error);
        }

        [Test]
        public void EachStep_HasNonEmptyTokens()
        {
            MathNode expression = parser.Parse("3x^2 + 4x + 1").Root;

            SolverResult result = solver.Solve(expression, "x");

            foreach (SolverStep step in result.Steps)
            {
                Assert.IsNotEmpty(step.Tokens);
            }
        }
    }
}
