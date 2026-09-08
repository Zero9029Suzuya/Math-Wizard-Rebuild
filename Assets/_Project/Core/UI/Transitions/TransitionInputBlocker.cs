using UnityEngine;
using MathWizard.UI.Overlays;

namespace MathWizard.UI.Transitions
{
    public class TransitionInputBlocker : MonoBehaviour
    {
        [Header("Main Controller")]
        [SerializeField]
        private OverlayController overlayController;

        public void SkipTransition()
        {
            overlayController.ActiveTransition?.Skip();
        }
    }
}