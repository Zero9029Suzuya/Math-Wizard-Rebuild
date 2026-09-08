namespace MathWizard.Math.AST
{
    /// <summary>
    /// For cases that the value can be negative, mostly used for Variable Node and Grouped Expression
    /// e.g. '(', ')', '[', ']' etc,
    /// </summary>
    /// MathNode can be anything, Variable Name, Contants, Etc. 
    public class UnaryNode : MathNode
    {
        public UnaryOperator Operator { get; }
        public MathNode Operand { get; }

        public UnaryNode(UnaryOperator op, MathNode operand)
        {
            Operator = op;
            Operand = operand;
        }

        public override string ToString()
        {
            int precedence = MathNodeFormatting.Precedence(this);
            string operand = MathNodeFormatting.RenderChild(Operand, precedence, false);

            switch (Operator)
            {
                case UnaryOperator.Negate:
                    return $"-{operand}";

                default:
                    return operand;
            }
        }
    }
}
