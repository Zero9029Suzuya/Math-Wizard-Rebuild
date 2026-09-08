using UnityEngine;
using UnityEngine.EventSystems;

namespace MathWizard.Player.Input
{
    /// <summary>
    /// The right-side touch region that produces a look delta. Same pointer-id-tracking pattern
    /// as MobileJoystick, for the same reason: a second finger landing on this region mid-drag
    /// must not steal the delta from the finger that's actually looking around. Because this and
    /// MobileJoystick each track their own independent pointerId and live on separate
    /// RectTransforms, Unity's EventSystem naturally dispatches left-thumb and right-thumb
    /// touches to each independently — no central touch router is needed for two coexisting
    /// regions like this.
    ///
    /// Exposes ConsumeDelta() (drain-on-read) rather than a running Value, since a look delta is
    /// inherently a per-frame "how far did the touch move since last frame" quantity, not a
    /// standing position like the joystick's.
    /// </summary>
    public sealed class LookInputArea :
        MonoBehaviour,
        IPointerDownHandler,
        IDragHandler,
        IPointerUpHandler
    {
        private int _activePointerId = -1;
        private Vector2 _accumulatedDelta;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_activePointerId != -1)
            {
                return;
            }

            _activePointerId = eventData.pointerId;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
            {
                return;
            }

            _accumulatedDelta += eventData.delta;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
            {
                return;
            }

            _activePointerId = -1;
        }

        /// <summary>Returns the delta accumulated since the last call, then resets it to zero.</summary>
        public Vector2 ConsumeDelta()
        {
            Vector2 delta = _accumulatedDelta;
            _accumulatedDelta = Vector2.zero;
            return delta;
        }
    }
}
