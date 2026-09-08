using UnityEngine;
using MathWizard.Player.Common;

namespace MathWizard.Player.Camera
{
    /// <summary>
    /// Owns camera look and its sensitivity restriction — entirely independent of
    /// MovementController, per the explicit requirement that camera restriction is never
    /// implemented through movement code. The camera is physically parented to the player rig
    /// in the scene, but that's a scene-setup fact, not a code dependency: this class has no
    /// reference to MovementController and MovementController has no reference to this class.
    ///
    /// Aggregation uses RestrictionStack (strongest-restriction-wins), not the additive stack
    /// movement speed uses — see RestrictionStack's own doc comment for why they're separate.
    /// </summary>
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform yawPivot;   // rotates left/right (player body or a dedicated yaw node)
        [SerializeField] private Transform pitchPivot;  // rotates up/down (the camera itself)
        [SerializeField] private float baseSensitivity = 1f;
        [SerializeField] private float minPitchDegrees = -80f;
        [SerializeField] private float maxPitchDegrees = 80f;

        private readonly RestrictionStack _restrictionStack = new RestrictionStack();
        private float _pitchDegrees;

        /// <summary>Exposed so Player's composition root can hand this stack to status effects
        /// via PlayerMovementContext without exposing the whole CameraController.</summary>
        public RestrictionStack RestrictionStack => _restrictionStack;

        /// <summary>Current sensitivity after restriction: baseSensitivity * strongest active ceiling.</summary>
        public float CurrentSensitivity => baseSensitivity * _restrictionStack.GetEffectiveValue();

        /// <summary>
        /// Applies an abstract look delta (already independent of physical input layout — the
        /// Input Router decides which physical input feeds this call, this class doesn't care).
        /// </summary>
        public void ApplyLookDelta(Vector2 lookDelta)
        {
            Vector2 scaled = lookDelta * CurrentSensitivity;

            if (yawPivot != null)
            {
                yawPivot.Rotate(Vector3.up, scaled.x, Space.World);
            }

            if (pitchPivot != null)
            {
                _pitchDegrees = Mathf.Clamp(_pitchDegrees - scaled.y, minPitchDegrees, maxPitchDegrees);
                Vector3 euler = pitchPivot.localEulerAngles;
                pitchPivot.localEulerAngles = new Vector3(_pitchDegrees, euler.y, euler.z);
            }
        }
    }
}
