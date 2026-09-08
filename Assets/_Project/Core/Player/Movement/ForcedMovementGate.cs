using MathWizard.Player.Common;

namespace MathWizard.Player.Movement
{
    /// <summary>
    /// The kinds of externally-imposed (non-voluntary) movement the game currently
    /// distinguishes. Only Knockback is gated today — Root blocks it, Grounded and Stun allow
    /// it. DownwardPull (Grounded-while-airborne) and Teleport (future spell-based forced
    /// teleport, distinct from the voluntary-Teleport capability a spell might request to
    /// *start*) are reserved here so adding their own gating rules later doesn't require
    /// restructuring this enum.
    /// </summary>
    public enum ForcedMovementCapability
    {
        Knockback
    }

    /// <summary>
    /// Plain, testable permission gate for forced movement. This is deliberately separate from
    /// MovementRestrictions (which only knows about voluntary MoveCapability) because forced and
    /// voluntary movement have different rules for the same status effects — most notably, Root
    /// blocks both, but Grounded and Stun block only voluntary movement while still permitting
    /// knockback.
    ///
    /// This gate makes the decision only. Actually applying a knockback force (physics) is
    /// ForcedMovementController's job — kept separate so this decision logic can be unit tested
    /// without Unity.
    /// </summary>
    public sealed class ForcedMovementGate
    {
        private readonly CapabilityGate<ForcedMovementCapability> _gate = new CapabilityGate<ForcedMovementCapability>();

        public void BlockKnockback(object source) => _gate.Block(ForcedMovementCapability.Knockback, source);

        public void UnblockKnockback(object source) => _gate.Unblock(ForcedMovementCapability.Knockback, source);

        public bool CanReceiveKnockback => _gate.IsAllowed(ForcedMovementCapability.Knockback);
    }
}
