using UnityEngine;
using MathWizard.Time;
using MathWizard.Player.Movement;
using MathWizard.Player.Camera;
using MathWizard.Player.Effects;

namespace MathWizard.Player
{
    /// <summary>
    /// Composition root only — holds references, wires the plain gameplay classes to the
    /// MonoBehaviour shells, and ticks the things that need per-frame ticking. Contains no
    /// gameplay rules of its own. This is what keeps it from becoming a God Object as Health,
    /// Mana, Scrolls, Spells, and Ultimate are added in later phases: each of those will be
    /// another owned subsystem wired here, not new logic written here.
    ///
    /// The three ApplyX methods exist only because this phase has no spell/ability system yet to
    /// be the real trigger for status effects — they're a temporary, explicit way to apply
    /// Root/Grounded/Stun for testing and for other systems (once they exist) to call. They are
    /// not a permanent "Player decides what CC does" API — once Abilities/Effects sourcing exists
    /// in full, whatever triggers a CC effect will call StatusEffectController.Apply directly
    /// with the right IStatusEffect instance.
    /// </summary>
    [RequireComponent(typeof(MovementController))]
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private MovementController movementController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ForcedMovementController forcedMovementController;
        [SerializeField] private float groundedDownwardPullForce = 25f;

        private MovementRestrictions _restrictions;
        private MovementModifiers _modifiers;
        private StatusEffectController _statusEffects;
        private IGameplayClock _clock;

        public MovementRestrictions Restrictions => _restrictions;
        public MovementModifiers Modifiers => _modifiers;
        public StatusEffectController StatusEffects => _statusEffects;

        private void Awake()
        {
            _clock = new UnityGameplayClock();
            _restrictions = new MovementRestrictions();
            _modifiers = new MovementModifiers();

            var context = new PlayerMovementContext(
                _restrictions,
                _modifiers,
                forcedMovementController.Gate,
                cameraController.RestrictionStack);

            _statusEffects = new StatusEffectController(context, _clock);

            movementController.Initialize(_restrictions, _modifiers, _clock);
        }

        private void Update()
        {
            _statusEffects.Tick();
        }

        public void ApplyRoot(float duration)
        {
            _statusEffects.Apply(new RootEffect(), duration);
        }

        public void ApplyGrounded(float duration)
        {
            bool wasAirborne = movementController.IsAirborne;
            _statusEffects.Apply(new GroundedEffect(), duration);
            if (wasAirborne)
            {
                forcedMovementController.ApplyDownwardPull(groundedDownwardPullForce);
            }
        }

        public void ApplyStun(float duration, float cameraSensitivityCeiling = 0f)
        {
            _statusEffects.Apply(new StunEffect(cameraSensitivityCeiling), duration);
        }
    }
}
