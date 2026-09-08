using System.Collections.Generic;

namespace MathWizard.Player.Common
{
    /// <summary>
    /// Tracks whether each of a fixed set of capabilities (TKey) is currently blocked, where
    /// multiple independent sources can block the same capability at once. A capability is
    /// allowed only when nothing is blocking it.
    ///
    /// This is a refcount-by-source-key design, not a plain bool per capability, specifically
    /// because two independent effects can block the same thing at the same time (e.g. an
    /// Immobile spell AND Root both blocking Move). If either ended and used a plain bool,
    /// clearing it would incorrectly re-enable the capability while the other blocker is still
    /// active. Keying blocks by source object also makes "who is blocking this?" answerable,
    /// which is worth the (trivial) extra cost over a bare counter.
    ///
    /// Generic over TKey so the same implementation backs both MovementRestrictions
    /// (MoveCapability) and, in a later phase, CastRestrictions (CastCapability) — two real
    /// consumers, which is why this is a shared generic rather than being duplicated or
    /// hard-coded into MovementRestrictions.
    /// </summary>
    public sealed class CapabilityGate<TKey>
    {
        private readonly Dictionary<TKey, HashSet<object>> _blockers = new Dictionary<TKey, HashSet<object>>();

        /// <summary>Marks <paramref name="capability"/> as blocked by <paramref name="source"/>.</summary>
        public void Block(TKey capability, object source)
        {
            if (!_blockers.TryGetValue(capability, out var set))
            {
                set = new HashSet<object>();
                _blockers[capability] = set;
            }
            set.Add(source);
        }

        /// <summary>
        /// Removes <paramref name="source"/>'s block on <paramref name="capability"/>. The
        /// capability remains blocked if any other source is still blocking it.
        /// </summary>
        public void Unblock(TKey capability, object source)
        {
            if (_blockers.TryGetValue(capability, out var set))
            {
                set.Remove(source);
                if (set.Count == 0)
                {
                    _blockers.Remove(capability);
                }
            }
        }

        /// <summary>True when no source is currently blocking this capability.</summary>
        public bool IsAllowed(TKey capability)
        {
            return !_blockers.TryGetValue(capability, out var set) || set.Count == 0;
        }

        /// <summary>Debug/inspection helper: who is currently blocking this capability.</summary>
        public IReadOnlyCollection<object> GetBlockers(TKey capability)
        {
            if (_blockers.TryGetValue(capability, out var set))
            {
                return set;
            }
            return System.Array.Empty<object>();
        }
    }
}
