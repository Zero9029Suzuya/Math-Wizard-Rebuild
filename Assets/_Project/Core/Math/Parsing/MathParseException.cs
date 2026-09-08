using System;

namespace MathWizard.Math.Parsing
{
    public class MathParseException : Exception
    {
        public MathParseException(string message) : base(message)
        {
        }
    }
}
