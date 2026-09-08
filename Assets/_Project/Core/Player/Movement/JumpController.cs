using MathWizard.Time;

namespace MathWizard.Player.Movement
{
    /// <summary>
    /// Ground jump (no cooldown) and air jump (its own cooldown), each independently gated by
    /// MovementRestrictions. Plain C#, no UnityEngine dependency.
    /// </summary>
    public sealed class JumpController
    {
        private readonly IGameplayClock _clock;
        private readonly MovementRestrictions _restrictions;
        private readonly float _airJumpCooldownDuration;

        private float _airJumpCooldownRemaining;

        public JumpController(IGameplayClock clock, MovementRestrictions restrictions, float airJumpCooldownDuration)
        {
            _clock = clock;
            _restrictions = restrictions;
            _airJumpCooldownDuration = airJumpCooldownDuration;
        }

        public bool IsAirJumpOnCooldown => _airJumpCooldownRemaining > 0f;

        public bool CanJump(bool isAirborne)
        {
            return isAirborne
                ? _restrictions.IsAllowed(MoveCapability.AirJump) && !IsAirJumpOnCooldown
                : _restrictions.IsAllowed(MoveCapability.Jump);
        }

        /// <summary>Attempts a (air-)jump. Starts the air-jump cooldown only when airborne.</summary>
        public bool TryJump(bool isAirborne)
        {
            if (!CanJump(isAirborne))
            {
                return false;
            }

            if (isAirborne)
            {
                _airJumpCooldownRemaining = _airJumpCooldownDuration;
            }
            return true;
        }

        public void Tick()
        {
            if (_airJumpCooldownRemaining > 0f)
            {
                _airJumpCooldownRemaining -= _clock.DeltaTime;
            }
        }
    }
}
