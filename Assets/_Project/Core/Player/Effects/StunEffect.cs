using MathWizard.Player.Movement;

namespace MathWizard.Player.Effects
{
    /// <summary>
    /// Stun: blocks Move/Jump/AirJump/Dash/Teleport and restricts camera sensitivity to a
    /// configurable ceiling. Deliberately does NOT block knockback (that's Root's unique trait)
    /// and does NOT touch casting (casting isn't implemented yet, and per the resolved design,
    /// Stun never blocks it even once casting exists — that's Silence's job in a later phase).
    ///
    /// cameraSensitivityCeiling is a constructor parameter, not a hard-coded value, so a
    /// "Minor Stun" (partial camera slow) and "Major Stun" (full lock, ceiling 0) can share this
    /// same class with different data — matching the requirement that camera behavior be
    /// configurable rather than hard-coded to one implementation.
    /// </summary>
    public sealed class StunEffect : IStatusEffect
    {
        private readonly float _cameraSensitivityCeiling;

        public StunEffect(float cameraSensitivityCeiling)
        {
            _cameraSensitivityCeiling = cameraSensitivityCeiling;
        }

        public void OnApply(PlayerMovementContext context)
        {
            context.Restrictions.Block(MoveCapability.Move, this);
            context.Restrictions.Block(MoveCapability.Jump, this);
            context.Restrictions.Block(MoveCapability.AirJump, this);
            context.Restrictions.Block(MoveCapability.Dash, this);
            context.Restrictions.Block(MoveCapability.Teleport, this);
            context.CameraRestrictions.Add(this, _cameraSensitivityCeiling);
        }

        public void OnExpire(PlayerMovementContext context)
        {
            context.Restrictions.Unblock(MoveCapability.Move, this);
            context.Restrictions.Unblock(MoveCapability.Jump, this);
            context.Restrictions.Unblock(MoveCapability.AirJump, this);
            context.Restrictions.Unblock(MoveCapability.Dash, this);
            context.Restrictions.Unblock(MoveCapability.Teleport, this);
            context.CameraRestrictions.Remove(this);
        }
    }
}
