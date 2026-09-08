namespace MathWizard.Math.AST
{
    /// <summary>
    /// A literal numeric value, e.g. 3, -4, 0.5.
    /// </summary>
    public class ConstantNode : MathNode
    {
        public double Value { get; }

        public ConstantNode(double value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
