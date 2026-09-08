using System.Collections;
using UnityEngine;
using MathWizard.UI.Overlays;
using System;

namespace MathWizard.UI.Transitions
{
    public abstract class ScrollTransition : MonoBehaviour
    {
        protected enum TransitionState
        {
            Closed,
            Opening,
            Open,
            Closing
        }

        [Header("Contents")]

        [SerializeField]
        private CanvasGroup contentsCanvasGroup;

        [Header("Animation")]

        [SerializeField]
        [Min(0.01f)]
        private float transitionDuration = 0.4f;

        

        [SerializeField]
        [Min(0f)]
        private float contentsFadeDuration = 0.15f;

        [SerializeField]
        [Min(0f)]
        private float contentsFadeDelay = 0.05f;

        protected TransitionState State { get; private set; }
        
        protected float TransitionDuration => transitionDuration;

        private Coroutine transitionCoroutine;

        public event Action<ScrollTransition> Opened;
        public event Action<ScrollTransition> Closed;

        protected virtual void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            State = TransitionState.Closed;

            ApplyClosedState();

            SetContentsInteraction(false);
        }

        // --------------------------------------------------
        // Public API
        // --------------------------------------------------

        public void Open()
        {
            if (State == TransitionState.Open ||
                State == TransitionState.Opening)
            {
                return;
            }

            StartTransition(true);
        }

        public void Close()
        {
            if (State == TransitionState.Closed ||
                State == TransitionState.Closing)
            {
                return;
            }

            StartTransition(false);
        }

        public void Skip()
        {
            if (State != TransitionState.Opening &&
                State != TransitionState.Closing)
            {
                return;
            }

            StopCurrentTransition();

            if (State == TransitionState.Opening)
            {
                SetOpenState();
            }
            else
            {
                SetClosedState();
            }
        }

        // --------------------------------------------------
        // Transition Control
        // --------------------------------------------------

        private void StartTransition(bool opening)
        {
            StopCurrentTransition();

            State = opening
                ? TransitionState.Opening
                : TransitionState.Closing;


            transitionCoroutine =
                StartCoroutine(
                    RunTransition(State)
                );
        }

        private IEnumerator RunTransition(TransitionState state)
        {
            State = state;

            if (State == TransitionState.Opening)
            {
                yield return OpenTransition();
            }
            else if (State == TransitionState.Closing)
            {
                yield return CloseTransition();
            }

            transitionCoroutine = null;
        }

        private IEnumerator OpenTransition()
        {
            SetContentsInteraction(false);

            yield return PlayOpenAnimation();

            if (contentsFadeDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    contentsFadeDelay
                );
            }

            yield return FadeContents(
                contentsCanvasGroup.alpha,
                1f
            );

            SetOpenState();
        }

        private IEnumerator CloseTransition()
        {
            SetContentsInteraction(false);

            yield return FadeContents(
                contentsCanvasGroup.alpha,
                0f
            );

            yield return PlayCloseAnimation();

            SetClosedState();
        }

        // --------------------------------------------------
        // Content Fade
        // --------------------------------------------------

        private IEnumerator FadeContents(
            float startAlpha,
            float targetAlpha)
        {
            if (contentsFadeDuration <= 0f)
            {
                contentsCanvasGroup.alpha =
                    targetAlpha;

                yield break;
            }

            float elapsed = 0f;

            while (elapsed < contentsFadeDuration)
            {
                elapsed += UnityEngine.Time.unscaledDeltaTime;

                float linearProgress =
                    Mathf.Clamp01(
                        elapsed / contentsFadeDuration
                    );

                float progress =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        linearProgress
                    );

                contentsCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        targetAlpha,
                        progress
                    );

                yield return null;
            }

            contentsCanvasGroup.alpha =
                targetAlpha;
        }

        // --------------------------------------------------
        // State
        // --------------------------------------------------

        protected void SetOpenState()
        {
            ApplyOpenState();

            contentsCanvasGroup.alpha = 1f;

            SetContentsInteraction(true);

            State = TransitionState.Open;

            Opened?.Invoke(this);
        }

        protected void SetClosedState()
        {
            ApplyClosedState();

            contentsCanvasGroup.alpha = 0f;

            SetContentsInteraction(false);

            State = TransitionState.Closed;

            Closed?.Invoke(this);
        }

        // --------------------------------------------------
        // Interaction
        // --------------------------------------------------

        private void SetContentsInteraction(bool enabled)
        {
            contentsCanvasGroup.interactable =
                enabled;

            contentsCanvasGroup.blocksRaycasts =
                enabled;
        }

        

        // --------------------------------------------------
        // Coroutine Control
        // --------------------------------------------------

        private void StopCurrentTransition()
        {
            if (transitionCoroutine == null)
            {
                return;
            }

            StopCoroutine(
                transitionCoroutine
            );

            transitionCoroutine = null;
        }

        // --------------------------------------------------
        // Implementation
        // --------------------------------------------------

        protected abstract IEnumerator PlayOpenAnimation();

        protected abstract IEnumerator PlayCloseAnimation();

        protected abstract void ApplyOpenState();

        protected abstract void ApplyClosedState();
    }
}