using MathWizard.Player.Common;
using MathWizard.Player.Movement;

namespace MathWizard.Player.Effects
{
    /// <summary>
    /// The narrow set of plain-C# handles a status effect is allowed to touch. Deliberately NOT
    /// a reference to the whole Player or to any MonoBehaviour — effects push changes through
    /// these specific mutation APIs only, never by reaching into private state. This is what
    /// keeps effects (and Player) from becoming a God Object as more effects are added later.
    ///
    /// CameraRestrictions is the plain RestrictionStack exposed by CameraController, not the
    /// CameraController itself — same idea for ForcedMovement (the plain ForcedMovementGate, not
    /// the ForcedMovementController MonoBehaviour). This is also why effects can be unit tested
    /// without instantiating any Unity object.
    /// </summary>
    public sealed class PlayerMovementContext
    {
        public MovementRestrictions Restrictions { get; }
        public MovementModifiers Modifiers { get; }
        public ForcedMovementGate ForcedMovement { get; }
        public RestrictionStack CameraRestrictions { get; }

        public PlayerMovementContext(
            MovementRestrictions restrictions,
            MovementModifiers modifiers,
            ForcedMovementGate forcedMovement,
            RestrictionStack cameraRestrictions)
        {
            Restrictions = restrictions;
            Modifiers = modifiers;
            ForcedMovement = forcedMovement;
            CameraRestrictions = cameraRestrictions;
        }
    }

    /// <summary>
    /// Minimal status-effect contract. OnTick is intentionally omitted from this phase — none of
    /// Root/Grounded/Stun need a per-frame effect beyond the one-shot restrictions they apply on
    /// entry, so adding a tick hook now would be speculative. Add it when an effect actually
    /// needs one (e.g. a future damage-over-time effect).
    /// </summary>
    public interface IStatusEffect
    {
        void OnApply(PlayerMovementContext context);
        void OnExpire(PlayerMovementContext context);
    }
}
