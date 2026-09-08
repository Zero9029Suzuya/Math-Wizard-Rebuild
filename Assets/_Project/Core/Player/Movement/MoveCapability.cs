namespace MathWizard.Player.Movement
{
    /// <summary>
    /// The independently-blockable voluntary-movement capabilities. Deliberately NOT a single
    /// "CanMove" bool — Root, Grounded, and Stun each block a different subset of these.
    ///
    /// Teleport is included even though no spell uses it yet, per the requirement that the
    /// architecture must not make adding it later difficult — it costs nothing to reserve the
    /// enum value now, and every status effect that should block it already can.
    ///
    /// Knockback is NOT here — it is forced movement, not a voluntary capability, and is gated
    /// separately by ForcedMovementCapability so that Root (blocks it) and Grounded/Stun (allow
    /// it) can differ from their voluntary-movement rules.
    /// </summary>
    public enum MoveCapability
    {
        Move,
        Jump,
        AirJump,
        Dash,
        Teleport
    }
}
