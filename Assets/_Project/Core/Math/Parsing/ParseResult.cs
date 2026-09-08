using MathWizard.Math.AST;

namespace MathWizard.Math.Parsing
{
    /// <summary>
    /// Outcome of parsing a string into a MathNode: 
    /// either a successfully built tree,
    /// or an error message describing what went wrong.
    /// </summary>
    public readonly struct ParseResult
    {
        public bool Success { get; }
        public MathNode Root { get; }
        public string Error { get; }

        private ParseResult(bool success, MathNode root, string error)
        {
            Success = success;
            Root = root;
            Error = error;
        }

        public static ParseResult Ok(MathNode root) => new ParseResult(true, root, null);

        public static ParseResult Fail(string error) => new ParseResult(false, null, error);
    }
}
