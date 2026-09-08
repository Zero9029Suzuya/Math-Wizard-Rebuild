using System.Collections.Generic;
using MathWizard.Time;

namespace MathWizard.Player.Effects
{
    /// <summary>
    /// Tracks active status effects and expires them after their duration. This is intentionally
    /// NOT a general-purpose status-effect framework (no priority resolution, no cross-effect
    /// interaction rules, no stacking policy) — it's the minimum needed to apply/expire
    /// Root/Grounded/Stun correctly, including holding several at once. A richer framework can
    /// be built later once more effects (Silence, buffs, DoTs, etc.) exist to design it against;
    /// building one now would be guessing at requirements this phase doesn't have.
    ///
    /// Every mutation of Restrictions/Modifiers/ForcedMovement/CameraRestrictions happens inside
    /// an effect's own OnApply/OnExpire — this controller only decides *when* those fire.
    /// </summary>
    public sealed class StatusEffectController
    {
        private sealed class ActiveEffect
        {
            public IStatusEffect Effect;
            public float RemainingDuration;
        }

        private readonly List<ActiveEffect> _active = new List<ActiveEffect>();
        private readonly PlayerMovementContext _context;
        private readonly IGameplayClock _clock;

        public StatusEffectController(PlayerMovementContext context, IGameplayClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public IReadOnlyList<IStatusEffect> ActiveEffects
        {
            get
            {
                var list = new List<IStatusEffect>(_active.Count);
                foreach (var active in _active) list.Add(active.Effect);
                return list;
            }
        }

        /// <summary>Applies an effect immediately and schedules its expiry after duration seconds.</summary>
        public void Apply(IStatusEffect effect, float duration)
        {
            effect.OnApply(_context);
            _active.Add(new ActiveEffect { Effect = effect, RemainingDuration = duration });
        }

        /// <summary>Expires a specific effect instance immediately, e.g. if cleansed early.</summary>
        public void ExpireImmediately(IStatusEffect effect)
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(_active[i].Effect, effect))
                {
                    _active[i].Effect.OnExpire(_context);
                    _active.RemoveAt(i);
                }
            }
        }

        /// <summary>Advances every active effect's remaining duration and expires any that hit zero.</summary>
        public void Tick()
        {
            float dt = _clock.DeltaTime;

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                _active[i].RemainingDuration -= dt;
                if (_active[i].RemainingDuration <= 0f)
                {
                    _active[i].Effect.OnExpire(_context);
                    _active.RemoveAt(i);
                }
            }
        }
    }
}
