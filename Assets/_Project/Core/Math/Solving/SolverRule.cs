namespace MathWizard.Math.Solving
{
    /// <summary>
    /// Identifies which mathematical rule produced a SolverStep, so the game can
    /// eventually look up an explanation for it instead of the solver hard-coding
    /// English text. Extend this enum as new rules are implemented — do not
    /// hard-code rule names as strings elsewhere.
    /// </summary>
    public enum SolverRule
    {
        Initial,
        ConstantRule,
        VariableRule,
        PowerRule,
        ConstantMultipleRule,
        SumRule,
        DifferenceRule,
        Differentiation,
        Simplification
    }
}
