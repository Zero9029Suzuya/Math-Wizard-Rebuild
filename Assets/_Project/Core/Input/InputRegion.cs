using UnityEngine;

namespace MathWizard.Input
{
    /// <summary>
    /// Defines one screen-space area that can claim a touch when it begins inside it.
    ///
    /// Priority is used ONLY to resolve overlaps at the exact moment a touch begins
    /// (e.g. a small high-priority Dash button sitting inside a large low-priority
    /// Look region). Priority plays no role afterward - once a touch is claimed,
    /// this region has nothing further to do with it.
    ///
    /// Uses a RectTransform so regions can be laid out with normal Unity UI anchoring
    /// and will adapt correctly across resolutions/aspect ratios, the same way any
    /// other UI element does.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class InputRegion : MonoBehaviour
    {
        [Tooltip("Higher values win when a touch begins inside multiple overlapping regions.")]
        [SerializeField] private int priority = 0;

        [Tooltip("The object that receives this region's touches. Must implement ITouchOwner.")]
        [SerializeField] private MonoBehaviour ownerBehaviour;

        [Tooltip("Optional. Leave empty to auto-detect the correct camera from the parent Canvas's render mode.")]
        [SerializeField] private Camera overrideCamera;

        private RectTransform _rectTransform;
        private Canvas _parentCanvas;
        private ITouchOwner _owner;

        public int Priority => priority;
        public ITouchOwner Owner => _owner;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();

            _owner = ownerBehaviour as ITouchOwner;
            if (_owner == null)
            {
                Debug.LogError(
                    $"[InputRegion] '{name}' has no owner assigned that implements ITouchOwner. " +
                    "This region will never claim a touch.", this);
            }
        }

        /// <summary>
        /// Tests a raw screen-space point (e.g. Touch.position) against this region's
        /// RectTransform, using the correct camera for the Canvas's render mode.
        /// </summary>
        public bool ContainsScreenPoint(Vector2 screenPoint)
        {
            if (_rectTransform == null) return false;
            Camera cam = ResolveCamera();
            return RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, screenPoint, cam);
        }

        private Camera ResolveCamera()
        {
            if (overrideCamera != null) return overrideCamera;

            // Screen Space - Overlay canvases must use a null camera for correct
            // screen-point testing. Screen Space - Camera and World Space canvases
            // need their assigned worldCamera, or the test will be wrong.
            if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                return _parentCanvas.worldCamera;
            }

            return null;
        }
    }
}
