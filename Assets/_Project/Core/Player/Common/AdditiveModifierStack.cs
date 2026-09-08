using System.Collections.Generic;

namespace MathWizard.Player.Common
{
    /// <summary>
    /// A linear/additive percentage stack: the final multiplier is 1 + the sum of all active
    /// modifiers, never compounded (never repeated multiplication of the running value).
    /// e.g. +10% and -30% active together produce 0.80, not 1.10 * 0.70.
    ///
    /// This is currently used for movement speed. It intentionally does NOT support a
    /// "strongest wins" mode — that is a materially different aggregation rule (see
    /// RestrictionStack, used for camera) and forcing both behaviors through one configurable
    /// class would hide a real semantic difference behind a shared interface. Two small classes
    /// beat one confusing flexible one here.
    ///
    /// SuppressPositive exists specifically for Grounded, which allows negative (debuff)
    /// modifiers to keep applying while ignoring positive (buff) modifiers. It is a simple flag
    /// rather than a per-modifier property because "which modifiers count as positive" is a
    /// property of the modifier's sign, not something each caller needs to declare.
    /// </summary>
    public sealed class AdditiveModifierStack
    {
        private readonly Dictionary<object, float> _modifiers = new Dictionary<object, float>();

        /// <summary>When true, only modifiers with a negative (debuff) value are summed.</summary>
        public bool SuppressPositive { get; set; }

        /// <summary>
        /// Adds or replaces this source's modifier. Value is a signed fraction, e.g. 0.10 for
        /// +10%, -0.30 for -30%.
        /// </summary>
        public void Add(object source, float percent)
        {
            _modifiers[source] = percent;
        }

        public void Remove(object source)
        {
            _modifiers.Remove(source);
        }

        /// <summary>
        /// Final multiplier to apply to the base value: 1 + sum(applicable modifiers), floored
        /// at 0 so that stacking enough debuffs can never invert movement into negative speed.
        /// There is deliberately no upper clamp yet — no buff in the current design needs one,
        /// and inventing a ceiling without a concrete reason would be exactly the kind of
        /// unrequested gameplay value this project avoids. Add one if/when a buff needs it.
        /// </summary>
        public float GetTotalMultiplier()
        {
            float sum = 0f;
            foreach (var modifier in _modifiers.Values)
            {
                if (SuppressPositive && modifier > 0f)
                {
                    continue;
                }
                sum += modifier;
            }

            float multiplier = 1f + sum;
            return multiplier < 0f ? 0f : multiplier;
        }
    }
}
