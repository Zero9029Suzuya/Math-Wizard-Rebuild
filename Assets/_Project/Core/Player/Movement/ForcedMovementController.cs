using UnityEngine;

namespace MathWizard.Player.Movement
{
    /// <summary>
    /// Executes externally-imposed movement (knockback, downward pull) — separate from
    /// MovementController, which only ever executes voluntary movement (joystick/jump/dash).
    /// Knockback is gated through ForcedMovementGate; downward pull currently is not gated by
    /// anything (no status effect in this phase blocks it), it's simply invoked only when
    /// Grounded is applied while airborne.
    ///
    /// The full knockback combat mechanic (damage sources, force curves, etc.) is out of scope
    /// for this phase — this is the minimal seam needed to prove Root/Grounded/Stun's differing
    /// knockback permission rules.
    /// </summary>
    public sealed class ForcedMovementController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;

        private readonly ForcedMovementGate _gate = new ForcedMovementGate();

        /// <summary>Exposed so Player's composition root can hand this to status effects via
        /// PlayerMovementContext without exposing the whole ForcedMovementController.</summary>
        public ForcedMovementGate Gate => _gate;

        /// <summary>Attempts to apply a knockback force. Returns false if currently blocked (Root).</summary>
        public bool TryApplyKnockback(Vector3 force)
        {
            if (!_gate.CanReceiveKnockback)
            {
                return false;
            }

            if (characterController != null)
            {
                characterController.Move(force * UnityEngine.Time.deltaTime);
            }
            return true;
        }

        /// <summary>
        /// Applies a one-shot downward pull. Not gated by anything currently — Grounded is the
        /// only source of this call, and it's invoked directly by the composition root only when
        /// the player is airborne at the moment Grounded is applied (see Player.ApplyGrounded).
        /// </summary>
        public void ApplyDownwardPull(float force)
        {
            if (characterController != null)
            {
                characterController.Move(Vector3.down * force * UnityEngine.Time.deltaTime);
            }
        }
    }
}
