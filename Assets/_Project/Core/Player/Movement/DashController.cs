using System.Collections.Generic;
using MathWizard.Time;

namespace MathWizard.Player.Movement
{
    /// <summary>
    /// Dash's core-movement charge/cooldown/recharge logic. Plain C#, no UnityEngine dependency,
    /// so it can be unit tested directly.
    ///
    /// Design for multiple missing charges: rather than one big recharge state machine,
    /// each missing charge gets its own independent countdown in a small list. Consuming a
    /// charge appends a new 10s countdown; any countdown reaching zero increments the current
    /// charge count and is removed. This is the simplest representation that correctly handles
    /// "consume twice in a row" producing two charges recharging on staggered timers, without
    /// needing to model recharge as a single resumable/restartable timer with special-cased
    /// interruption logic. A single shared timer would have to answer awkward questions (does
    /// consuming a second charge restart the shared timer? extend it? run a second one anyway?)
    /// that this per-charge-timer design avoids by construction.
    ///
    /// Maximum charges (3) is a compile-time constant, per the explicit requirement that it
    /// never becomes a runtime-modifiable stat. Current charges can be increased directly (e.g.
    /// by a future buff) via AddCharge, independent of the recharge-timer list.
    /// </summary>
    public sealed class DashController
    {
        public const int MaxCharges = 3;
        private const float UsageCooldownDuration = 2f;
        private const float RechargeDuration = 10f;
        private const float movementMultiplier = 3.5f;

        private readonly IGameplayClock _clock;
        private readonly MovementRestrictions _restrictions;
        private readonly List<float> _rechargeTimersRemaining = new List<float>();

        private float _usageCooldownRemaining;

        public DashController(IGameplayClock clock, MovementRestrictions restrictions)
        {
            _clock = clock;
            _restrictions = restrictions;
            CurrentCharges = MaxCharges;
        }

        public int CurrentCharges { get; private set; }

        /// <summary>True while a dash was used too recently to use another, regardless of charges.</summary>
        public bool IsOnUsageCooldown => _usageCooldownRemaining > 0f;

        public bool CanDash =>
            CurrentCharges > 0
            && !IsOnUsageCooldown
            && _restrictions.IsAllowed(MoveCapability.Dash);

        /// <summary>
        /// Attempts to consume one charge. Returns false (no state change) if dash is currently
        /// unavailable for any reason — out of charges, on usage cooldown, or restricted.
        /// </summary>
        public bool TryConsumeDash()
        {
            if (!CanDash)
            {
                return false;
            }

            CurrentCharges -= 1;
            _usageCooldownRemaining = UsageCooldownDuration;
            _rechargeTimersRemaining.Add(RechargeDuration);
            return true;
        }

        /// <summary>Grants a charge directly (e.g. a future buff), clamped at MaxCharges.</summary>
        public void AddCharge(int amount = 1)
        {
            int next = CurrentCharges + amount;
            CurrentCharges = next > MaxCharges ? MaxCharges : next;
        }

        /// <summary>Advances the usage cooldown and every in-flight recharge timer by one tick.</summary>
        public void Tick()
        {
            float dt = _clock.DeltaTime;

            if (_usageCooldownRemaining > 0f)
            {
                _usageCooldownRemaining -= dt;
            }

            for (int i = _rechargeTimersRemaining.Count - 1; i >= 0; i--)
            {
                _rechargeTimersRemaining[i] -= dt;
                if (_rechargeTimersRemaining[i] <= 0f)
                {
                    _rechargeTimersRemaining.RemoveAt(i);
                    if (CurrentCharges < MaxCharges)
                    {
                        CurrentCharges += 1;
                    }
                }
            }
        }
    }
}
