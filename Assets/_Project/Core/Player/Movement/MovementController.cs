using UnityEngine;
using MathWizard.Time;

namespace MathWizard.Player.Movement
{
    /// <summary>
    /// Executes voluntary movement only (joystick-driven move, jump, air jump, dash). Forced
    /// movement (knockback, downward pull) lives in ForcedMovementController instead — this
    /// class never applies a force the player didn't request.
    ///
    /// Every request (RequestMove/RequestJump/RequestAirJump/RequestDash) is checked against
    /// MovementRestrictions before anything happens. A rejected request is simply a no-op: the
    /// caller (Input layer, or a spell's own logic) is never told to cancel whatever caused the
    /// request — e.g. an Immobile spell keeps casting even though its own joystick-move requests
    /// keep getting rejected here.
    ///
    /// Input is abstract by design: this class only ever receives a normalized Vector2 direction
    /// or a bare "jump/dash was pressed" call. It has no idea whether that came from a joystick,
    /// a keyboard, or a different future control scheme — that mapping is the Input layer's job,
    /// not this class's.
    ///
    /// Movement execution re-checks MoveCapability.Move every frame in Update() rather than
    /// trusting the last value RequestMove() computed — a status effect applied while the
    /// joystick is held steady (and RequestMove isn't necessarily called again that frame) must
    /// still stop the player immediately, not on the next joystick event.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class MovementController : MonoBehaviour
    {
        [SerializeField] private float baseMoveSpeed = 6f;
        [SerializeField] private float jumpVelocity = 8f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float airJumpCooldownDuration = 1.5f;
        [SerializeField] private float dashStrength = 3f; // multiplier applied to baseMoveSpeed for the dash's duration
        [SerializeField] private float dashDuration = 0.15f;

        private CharacterController _characterController;
        private MovementRestrictions _restrictions;
        private MovementModifiers _modifiers;
        private DashController _dashController;
        private JumpController _jumpController;
        private IGameplayClock _clock;
        private readonly DashMotionState _dashMotion = new DashMotionState();

        private Vector3 _verticalVelocity;
        private Vector2 _pendingMoveInput;

        public bool IsAirborne => _characterController != null && !_characterController.isGrounded;
        public DashController DashController => _dashController;
        public JumpController JumpController => _jumpController;

        /// <summary>
        /// Wires this controller to the shared restriction/modifier instances owned by the
        /// Player composition root, and creates its own DashController/JumpController (these are
        /// movement-internal resources, not shared with anything else).
        /// </summary>
        public void Initialize(MovementRestrictions restrictions, MovementModifiers modifiers, IGameplayClock clock)
        {
            _characterController = GetComponent<CharacterController>();
            _restrictions = restrictions;
            _modifiers = modifiers;
            _clock = clock;
            _dashController = new DashController(clock, restrictions);
            _jumpController = new JumpController(clock, restrictions, airJumpCooldownDuration);
        }

        /// <summary>
        /// Abstract movement input — a normalized direction in the player's local XZ plane. Just
        /// stores the value; Update() is the sole place that decides whether it's actually
        /// allowed to move the player this frame, so it stays correct even if RequestMove isn't
        /// called again on the exact frame a restriction changes.
        /// </summary>
        public void RequestMove(Vector2 direction)
        {
            _pendingMoveInput = direction;
        }

        public void RequestJump()
        {
            if (_jumpController.TryJump(isAirborne: IsAirborne))
            {
                _verticalVelocity.y = jumpVelocity;
            }
        }

        // Parameterless wrapper for Inspector/UnityEvent hookup, per project convention.
        public void Jump() => RequestJump();

        /// <summary>
        /// Requests a dash. DashController remains the sole authority on whether a charge can be
        /// consumed (charges, usage cooldown, MoveCapability.Dash). This method only decides the
        /// dash's direction and speed (baseMoveSpeed * dashStrength) and hands the physical burst
        /// off to DashMotionState, which stays active — overriding normal movement — until
        /// dashDuration elapses. It never calls CharacterController.Move directly, so Update()
        /// stays the single owner of continuous CharacterController motion for voluntary
        /// movement (see the ownership note in the integration report).
        /// </summary>
        public void RequestDash()
        {
            if (_dashController.TryConsumeDash())
            {
                Vector3 dashDirection = transform.TransformDirection(new Vector3(_pendingMoveInput.x, 0f, _pendingMoveInput.y));
                if (dashDirection.sqrMagnitude < 0.001f)
                {
                    dashDirection = transform.forward;
                }
                _dashMotion.Begin(dashDirection.normalized, baseMoveSpeed * dashStrength, dashDuration);
            }
        }

        public void Dash() => RequestDash();

        private void Update()
        {
            float dt = _clock.DeltaTime;
            _dashController?.Tick();
            _jumpController?.Tick();
            _dashMotion.Tick(dt);

            

            _verticalVelocity.y += gravity * dt;
            if (_characterController.isGrounded && _verticalVelocity.y < 0f)
            {
                _verticalVelocity.y = -2f; // keeps the controller grounded
            }

            float speedMultiplier = _modifiers.GetSpeedMultiplier();

            Vector3 horizontal;
            if (_dashMotion.IsActive)
            {
                // A dash in progress overrides normal movement input for its duration. Not
                // re-gated by MoveCapability.Move here — DashController already checked every
                // relevant restriction before the dash was allowed to start, and letting a
                // started burst run to completion is the simplest behavior absent a stated
                // requirement for mid-dash interruption (e.g. Root landing mid-burst). Flagged as
                // an open question in the integration report, not decided silently as permanent.
                horizontal = _dashMotion.Direction * _dashMotion.Speed;
            }
            else if (_restrictions.IsAllowed(MoveCapability.Move))
            {
                horizontal = transform.TransformDirection(new Vector3(_pendingMoveInput.x, 0f, _pendingMoveInput.y))
                             * baseMoveSpeed * speedMultiplier;
            }
            else
            {
                horizontal = Vector3.zero;
            }

            _characterController.Move((horizontal + _verticalVelocity) * dt);
        }
    }
}