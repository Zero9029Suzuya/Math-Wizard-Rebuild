namespace MathWizard.Math.AST
{
    /// <summary>
    /// Holds Variable Names "x", "y", "c", "a", etc.
    /// </summary>
    public class VariableNode : MathNode
    {
        public string Name { get; }

        public VariableNode(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
