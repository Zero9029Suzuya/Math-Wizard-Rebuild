using System;
using System.Collections.Generic;
using UnityEngine;
using MathWizard.UI.Transitions;

namespace MathWizard.UI.Overlays
{
    public class OverlayController : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup overlayCanvasGroup;

        private ScrollTransition activeTransition;

        public ScrollTransition ActiveTransition => activeTransition;

        public enum OverlayType
        {
            Campaign,
            SpellLocker,
            Login,
            Settings
        }

        [Serializable]
        private class OverlayEntry
        {
            [SerializeField]
            private OverlayType type;

            [SerializeField]
            private ScrollTransition transition;

            public OverlayType Type => type;
            public ScrollTransition Transition => transition;
        }

        [SerializeField]
        private List<OverlayEntry> overlays = new();

        private void Awake()
        {
            UpdateOverlay(false);
        }


        public void OpenOverlay(OverlayType type)
        {
            OverlayEntry entry = FindOverlay(type);

            if (entry == null)
            {
                Debug.LogError(
                    $"Overlay '{type}' is not registered.",
                    this
                );

                return;
            }
            entry.Transition.Opened += OnTransitionOpened;
            activeTransition = entry.Transition;

            UpdateOverlay(true);

            entry.Transition.Open();
        }

        public void CloseOverlay(OverlayType type)
        {
            OverlayEntry entry = FindOverlay(type);

            if (entry == null)
            {
                Debug.LogError(
                    $"Overlay '{type}' is not registered.",
                    this
                );

                return;
            }
            entry.Transition.Closed += OnTransitionClosed;
            activeTransition = entry.Transition;

            entry.Transition.Close();
            

        }

        private OverlayEntry FindOverlay(
            OverlayType type)
        {
            foreach (OverlayEntry entry in overlays)
            {
                if (entry.Type == type)
                {
                    return entry;
                }
            }

            return null;
        }

        private void OnTransitionOpened(ScrollTransition transition)
        {
            transition.Opened -= OnTransitionOpened;

            if (activeTransition == transition){
                activeTransition = null;
            }
        }

        private void OnTransitionClosed(ScrollTransition transition)
        {
            transition.Closed -= OnTransitionClosed;

            if (activeTransition == transition){
                activeTransition = null;
            }
            
            UpdateOverlay(false);
        }

        private void UpdateOverlay(bool open)
        {
            overlayCanvasGroup.interactable = open ? true : false;
            overlayCanvasGroup.blocksRaycasts = open ? true : false;
            overlayCanvasGroup.alpha = open ? 1f : 0f;
        }

        // --------------------------------------------------
        // Unity UI Button API
        // --------------------------------------------------

        public void OpenCampaignOverlay()
        {
            OpenOverlay(OverlayType.Campaign);
        }

        public void OpenSpellLockerOverlay()
        {
            OpenOverlay(OverlayType.SpellLocker);
        }

        public void OpenSettingsOverlay()
        {
            OpenOverlay(OverlayType.Settings);
        }

        public void CloseCampaignOverlay()
        {
            CloseOverlay(OverlayType.Campaign);
        }

        public void CloseSpellLockerOverlay()
        {
            CloseOverlay(OverlayType.SpellLocker);
        }

        public void CloseSettingsOverlay()
        {
            CloseOverlay(OverlayType.Settings);
        }
    }
}