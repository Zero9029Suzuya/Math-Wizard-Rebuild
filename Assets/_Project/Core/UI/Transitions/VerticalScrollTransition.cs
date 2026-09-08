using System.Collections;
using UnityEngine;

namespace MathWizard.UI.Transitions
{
    public class VerticalScrollTransition : ScrollTransition
    {
        [Header("Vertical Scroll")]

        [SerializeField]
        private RectTransform rollerTop;

        [SerializeField]
        private RectTransform rollerBottom;

        [SerializeField]
        private RectTransform background;

        [Header("Closed Position")]

        [SerializeField]
        private float closedOffset = 0f;

        [Header("Closed Background")]

        [SerializeField]
        [Min(0f)]
        private float closedBackgroundHeight = 10f;

        private float backgroundOpenHeight;

        private Vector2 rollerTopOpenPosition;
        private Vector2 rollerBottomOpenPosition;

        private Vector2 rollerTopClosedPosition;
        private Vector2 rollerBottomClosedPosition;

        protected override void Awake()
        {
            CacheOpenState();
            CalculateClosedState();

            base.Awake();
        }

        private void CacheOpenState()
        {
            rollerTopOpenPosition =
                rollerTop.anchoredPosition;

            rollerBottomOpenPosition =
                rollerBottom.anchoredPosition;

            backgroundOpenHeight =
                background.sizeDelta.y;
        }

        private void CalculateClosedState()
        {
            Vector2 center =
                (rollerTopOpenPosition +
                rollerBottomOpenPosition) * 0.5f;

            rollerTopClosedPosition =
                center + Vector2.down * closedOffset;

            rollerBottomClosedPosition =
                center + Vector2.up * closedOffset;
        }

        protected override IEnumerator PlayOpenAnimation()
        {
            float elapsed = 0f;

            Vector2 startTop =
                rollerTop.anchoredPosition;

            Vector2 startBottom =
                rollerBottom.anchoredPosition;

            /*
             * The actual duration is currently owned
             * by the base class, so the vertical class
             * needs to receive the normalized animation
             * time rather than duplicate the duration.
             */

            while (elapsed < 1f)
            {
                elapsed +=
                    UnityEngine.Time.unscaledDeltaTime /
                    TransitionDuration;

                float progress =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        Mathf.Clamp01(elapsed)
                    );

                rollerTop.anchoredPosition =
                    Vector2.Lerp(
                        startTop,
                        rollerTopOpenPosition,
                        progress
                    );

                rollerBottom.anchoredPosition =
                    Vector2.Lerp(
                        startBottom,
                        rollerBottomOpenPosition,
                        progress
                    );

                float backgroundHeight =
                    Mathf.Lerp(
                        closedBackgroundHeight,
                        backgroundOpenHeight,
                        progress
                    );

                background.sizeDelta =
                    new Vector2(
                        background.sizeDelta.x,
                        backgroundHeight
                    );

                yield return null;
            }

            ApplyOpenState();
        }

        protected override IEnumerator PlayCloseAnimation()
        {
            float elapsed = 0f;

            Vector2 startTop =
                rollerTop.anchoredPosition;

            Vector2 startBottom =
                rollerBottom.anchoredPosition;

            while (elapsed < 1f)
            {
                elapsed +=
                    UnityEngine.Time.unscaledDeltaTime /
                    TransitionDuration;

                float progress =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        Mathf.Clamp01(elapsed)
                    );


                rollerTop.anchoredPosition =
                    Vector2.Lerp(
                        startTop,
                        rollerTopClosedPosition,
                        progress
                    );

                rollerBottom.anchoredPosition =
                    Vector2.Lerp(
                        startBottom,
                        rollerBottomClosedPosition,
                        progress
                    );

                float backgroundHeight =
                    Mathf.Lerp(
                        backgroundOpenHeight,
                        closedBackgroundHeight,
                        progress
                    );

                background.sizeDelta =
                    new Vector2(
                        background.sizeDelta.x,
                        backgroundHeight
                    );
                yield return null;
            }

            ApplyClosedState();
        }

        protected override void ApplyOpenState()
        {
            rollerTop.anchoredPosition =
                rollerTopOpenPosition;

            rollerBottom.anchoredPosition =
                rollerBottomOpenPosition;
        }

        protected override void ApplyClosedState()
        {
            rollerTop.anchoredPosition =
                rollerTopClosedPosition;

            rollerBottom.anchoredPosition =
                rollerBottomClosedPosition;
        }
    }
}