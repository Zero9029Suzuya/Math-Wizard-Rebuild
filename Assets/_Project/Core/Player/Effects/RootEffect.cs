using MathWizard.Player.Movement;

namespace MathWizard.Player.Effects
{
    /// <summary>
    /// Root: blocks Move/Jump/AirJump/Dash/Teleport AND knockback. Camera and casting are
    /// untouched — Root's defining trait versus Stun is specifically that it also blocks forced
    /// displacement.
    ///
    /// A sibling of GroundedEffect/StunEffect, not a base class for either — they block
    /// different capability sets (Grounded allows Move; Stun allows knockback), so a shared base
    /// would need to disable half of whatever it defined for at least one of them.
    /// </summary>
    public sealed class RootEffect : IStatusEffect
    {
        public void OnApply(PlayerMovementContext context)
        {
            context.Restrictions.Block(MoveCapability.Move, this);
            context.Restrictions.Block(MoveCapability.Jump, this);
            context.Restrictions.Block(MoveCapability.AirJump, this);
            context.Restrictions.Block(MoveCapability.Dash, this);
            context.Restrictions.Block(MoveCapability.Teleport, this);
            context.ForcedMovement.BlockKnockback(this);
        }

        public void OnExpire(PlayerMovementContext context)
        {
            context.Restrictions.Unblock(MoveCapability.Move, this);
            context.Restrictions.Unblock(MoveCapability.Jump, this);
            context.Restrictions.Unblock(MoveCapability.AirJump, this);
            context.Restrictions.Unblock(MoveCapability.Dash, this);
            context.Restrictions.Unblock(MoveCapability.Teleport, this);
            context.ForcedMovement.UnblockKnockback(this);
        }
    }
}
