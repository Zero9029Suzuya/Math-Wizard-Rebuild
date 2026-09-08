using System.Collections.Generic;

namespace MathWizard.Player.Common
{
    /// <summary>
    /// A "strongest restriction wins" stack: each active source registers a sensitivity
    /// ceiling in [0, 1] (1 = no restriction, 0 = fully locked), and the effective value is the
    /// MINIMUM of all active ceilings — never summed, never multiplied. With no active sources,
    /// the effective value is 1 (fully unrestricted).
    ///
    /// This is a separate class from AdditiveModifierStack on purpose: camera restriction and
    /// movement speed have deliberately different aggregation rules in this game, and collapsing
    /// them into one "percentage stack with a stacking-mode flag" would hide that real semantic
    /// difference behind a shared interface that doesn't actually fit both cases well.
    /// </summary>
    public sealed class RestrictionStack
    {
        private readonly Dictionary<object, float> _ceilings = new Dictionary<object, float>();

        /// <summary>Registers or replaces this source's sensitivity ceiling (clamped to [0, 1]).</summary>
        public void Add(object source, float sensitivityCeiling01)
        {
            _ceilings[source] = Clamp01(sensitivityCeiling01);
        }

        public void Remove(object source)
        {
            _ceilings.Remove(source);
        }

        /// <summary>The strongest (lowest) active ceiling, or 1 (unrestricted) if none are active.</summary>
        public float GetEffectiveValue()
        {
            if (_ceilings.Count == 0)
            {
                return 1f;
            }

            float min = 1f;
            foreach (var ceiling in _ceilings.Values)
            {
                if (ceiling < min)
                {
                    min = ceiling;
                }
            }
            return min;
        }

        private static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }
    }
}
