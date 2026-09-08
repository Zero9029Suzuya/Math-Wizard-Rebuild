namespace MathWizard.Math.Solving
{
    /// <summary>
    /// AI:
    /// A single display unit within a solver step's rendered text (e.g. "3", "x^2", "+").
    /// Deliberately separate from Parsing.Token, which stays internal to the parser —
    /// the solver subsystem should not depend on parsing internals, even though today
    /// it happens to reuse the tokenizer to build these.
    /// </summary>
    public readonly struct SolverToken
    {
        public string Text { get; }

        public SolverToken(string text)
        {
            Text = text;
        }
    }
}
