using UnityEngine;
using MathWizard.Player.Movement;
using MathWizard.Player.Camera;

namespace MathWizard.Player.Input
{
    /// <summary>
    /// The single translation point between UI input (joystick, look area, jump/dash buttons)
    /// and the existing gameplay controllers. Owns no gameplay rules whatsoever — every request
    /// it forwards can be freely rejected downstream (MovementController/JumpController/
    /// DashController/MovementRestrictions), and this class never checks Root/Grounded/Stun,
    /// cooldowns, or charges itself. A button press or joystick value is a request, not a
    /// permission.
    ///
    /// References are explicit serialized fields, not FindObjectOfType/GameObject.Find — this
    /// class has no idea about Canvas hierarchy or GameObject names, it only knows the
    /// MobileJoystick/LookInputArea/MovementController/CameraController instances it was handed.
    /// </summary>
    public sealed class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private MovementController movementController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private MobileJoystick joystick;
        [SerializeField] private LookInputArea lookInputArea;

        private void Update()
        {
            if (joystick != null && movementController != null)
            {
                movementController.RequestMove(joystick.Value);
            }

            if (lookInputArea != null && cameraController != null)
            {
                Vector2 lookDelta = lookInputArea.ConsumeDelta();
                if (lookDelta.x != 0f || lookDelta.y != 0f)
                {
                    cameraController.ApplyLookDelta(lookDelta);
                }
            }
        }

        /// <summary>Wire to BTN_Jump's OnClick in the Inspector.</summary>
        public void OnJumpButtonPressed()
        {
            movementController.RequestJump();
        }

        /// <summary>Wire to the Dash button's OnClick in the Inspector.</summary>
        public void OnDashButtonPressed()
        {
            movementController.RequestDash();
        }
    }
}
