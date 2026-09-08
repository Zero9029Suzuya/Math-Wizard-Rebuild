using UnityEngine;
using UnityEngine.EventSystems;
using UnityCamera = UnityEngine.Camera;

namespace MathWizard.Player.Input
{
    public sealed class MobileJoystick :
        MonoBehaviour,
        IPointerDownHandler,
        IDragHandler,
        IPointerUpHandler
    {
        [SerializeField] private RectTransform handle;
        [SerializeField] private float range = 50f;

        private RectTransform _rectTransform;
        private int _activePointerId = -1;

        public Vector2 Value { get; private set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            if (handle == null && transform.childCount > 0)
            {
                handle = transform.GetChild(0).GetComponent<RectTransform>();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_activePointerId != -1)
                return;

            _activePointerId = eventData.pointerId;
            UpdateJoystick(eventData.position, eventData.pressEventCamera);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
                return;

            UpdateJoystick(eventData.position, eventData.pressEventCamera);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
                return;

            _activePointerId = -1;
            Value = Vector2.zero;

            if (handle != null)
                handle.anchoredPosition = Vector2.zero;
        }

        private void UpdateJoystick(
            Vector2 screenPosition,
            UnityCamera eventCamera)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform,
                    screenPosition,
                    eventCamera,
                    out Vector2 localPosition))
            {
                return;
            }

            Vector2 clampedPosition =
                Vector2.ClampMagnitude(localPosition, range);

            if (handle != null)
                handle.anchoredPosition = clampedPosition;

            Value = clampedPosition / range;
        }
    }
}