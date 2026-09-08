using MathWizard.Player.Movement;

namespace MathWizard.Player.Effects
{
    /// <summary>
    /// Grounded: allows Move, blocks Jump/AirJump/Dash/Teleport, suppresses positive
    /// (buff) speed modifiers while leaving negative (debuff) ones active, and allows knockback.
    ///
    /// The one-shot "strong downward pull if applied while airborne" rule is deliberately NOT
    /// implemented inside this effect's OnApply — it's a single physical event that needs the
    /// MonoBehaviour-level ForcedMovementController and an airborne check, both of which live
    /// outside the plain PlayerMovementContext this effect operates on. The composition root
    /// (Player.ApplyGrounded) performs that check and calls ForcedMovementController directly at
    /// the moment Grounded is applied. This keeps this class fully plain/testable at the cost of
    /// one physics side-effect living slightly outside the effect object itself — a deliberate,
    /// documented simplification for this phase (see the implementation review's tech-debt list).
    ///
    /// Known simplification: MovementModifiers.SuppressPositive is a single shared flag, not
    /// refcounted per source. This is fine as long as only one Grounded instance is ever active
    /// on a player at a time (the expected case for a single named CC effect); if a design later
    /// allows multiple simultaneous Grounded stacks, this flag needs to become a refcount like
    /// the capability gates.
    /// </summary>
    public sealed class GroundedEffect : IStatusEffect
    {
        public void OnApply(PlayerMovementContext context)
        {
            context.Restrictions.Block(MoveCapability.Jump, this);
            context.Restrictions.Block(MoveCapability.AirJump, this);
            context.Restrictions.Block(MoveCapability.Dash, this);
            context.Restrictions.Block(MoveCapability.Teleport, this);
            context.Modifiers.SuppressPositive = true;
        }

        public void OnExpire(PlayerMovementContext context)
        {
            context.Restrictions.Unblock(MoveCapability.Jump, this);
            context.Restrictions.Unblock(MoveCapability.AirJump, this);
            context.Restrictions.Unblock(MoveCapability.Dash, this);
            context.Restrictions.Unblock(MoveCapability.Teleport, this);
            context.Modifiers.SuppressPositive = false;
        }
    }
}
