using MathWizard.Player.Common;

namespace MathWizard.Player.Movement
{
    /// <summary>
    /// Owns the additive movement-speed modifier stack. MovementController reads
    /// GetSpeedMultiplier() and multiplies it against base speed; it does not know or care how
    /// many modifiers are active or where they came from.
    /// </summary>
    public sealed class MovementModifiers
    {
        private readonly AdditiveModifierStack _stack = new AdditiveModifierStack();

        /// <summary>
        /// While true (set by Grounded), positive (buff) modifiers are ignored but negative
        /// (debuff) modifiers still apply — per the explicit Grounded rule.
        /// </summary>
        public bool SuppressPositive
        {
            get => _stack.SuppressPositive;
            set => _stack.SuppressPositive = value;
        }

        public void Add(object source, float percent) => _stack.Add(source, percent);

        public void Remove(object source) => _stack.Remove(source);

        /// <summary>Final speed multiplier: 1 + sum(applicable modifiers), floored at 0.</summary>
        public float GetSpeedMultiplier() => _stack.GetTotalMultiplier();
    }
}
