using System.Collections.Generic;

namespace MathWizard.Math.Solving
{
    /// <summary>
    /// Outcome of a solve attempt. Mirrors ParseResult's success/failure shape so a
    /// future ProblemGenerator can check "can the solver handle this?" without
    /// catching exceptions.
    /// </summary>
    public readonly struct SolverResult
    {
        public bool Success { get; }
        public IReadOnlyList<SolverStep> Steps { get; }
        public string Error { get; }

        private SolverResult(bool success, IReadOnlyList<SolverStep> steps, string error)
        {
            Success = success;
            Steps = steps;
            Error = error;
        }

        public static SolverResult Ok(IReadOnlyList<SolverStep> steps) => new SolverResult(true, steps, null);

        public static SolverResult Fail(string error) => new SolverResult(false, null, error);
    }
}
