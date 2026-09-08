using System.Collections.Generic;
using UnityEngine;

namespace MathWizard.Input
{
    /// <summary>
    /// Central authority for touch ownership.
    ///
    /// Core rule: a touch's STARTING position is tested against every registered
    /// InputRegion exactly once, when the touch begins. The highest-priority region
    /// containing that point claims the finger. That ownership does not change for
    /// the remaining lifetime of the touch, no matter where the finger drags -
    /// there is no re-evaluation, no negotiation, and no claim window.
    ///
    /// This router deliberately knows nothing about movement, camera rotation,
    /// dash physics, spells, mana, health, or networking. It only turns raw
    /// Input.touches into claim / move / release / cancel calls on ITouchOwner.
    /// Everything downstream of that is a separate, later concern.
    /// </summary>
    public class TouchRouter : MonoBehaviour
    {
        [Header("Regions")]
        [Tooltip("Drag every InputRegion in the scene here. If left empty, the router " +
                 "auto-discovers all InputRegions at Awake - but discovery order isn't " +
                 "guaranteed, so prefer setting this list explicitly once you have more " +
                 "than one same-priority region.")]
        [SerializeField] private List<InputRegion> regions = new List<InputRegion>();

        [Header("Debugging")]
        [Tooltip("Logs every ownership claim, release, and cancellation to the Console.")]
        [SerializeField] private bool logOwnershipEvents = true;

        // fingerId -> the owner that currently holds it. This dictionary IS the
        // ownership state. A finger is either in here (owned) or not (unowned).
        private readonly Dictionary<int, ITouchOwner> _activeOwners = new Dictionary<int, ITouchOwner>();
        private readonly Dictionary<int, Vector2> _lastPositions = new Dictionary<int, Vector2>();

        /// <summary>
        /// When false, new touches are never claimed, and all previously-owned
        /// touches have already been cancelled. Toggle via SetGameplayInputEnabled.
        /// </summary>
        public bool GameplayInputEnabled { get; private set; } = true;


        private void Awake()
        {
            if (regions.Count == 0)
            {
                regions.AddRange(
                    FindObjectsByType<InputRegion>(FindObjectsSortMode.None)
                );

                Log($"No regions assigned in the Inspector - auto-discovered {regions.Count}.");
            }
        }

        private void Update()
        {
            int touchCount = UnityEngine.Input.touchCount;
            for (int i = 0; i < touchCount; i++)
            {
                ProcessTouch(UnityEngine.Input.GetTouch(i));
            }
        }

        private void ProcessTouch(Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleBegan(touch);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    HandleMoved(touch);
                    break;

                case TouchPhase.Ended:
                    HandleEnded(touch);
                    break;

                case TouchPhase.Canceled:
                    HandleOsCancelled(touch);
                    break;
            }
        }

        private void HandleBegan(Touch touch)
        {
            if (!GameplayInputEnabled)
            {
                // Gameplay input is off - new touches simply are not looked at.
                return;
            }

            InputRegion best = ResolveRegion(touch.position);
            if (best == null || best.Owner == null)
            {
                return; // Didn't start inside any known region - not this router's concern.
            }

            _activeOwners[touch.fingerId] = best.Owner;
            _lastPositions[touch.fingerId] = touch.position;
            best.Owner.OnTouchClaimed(touch.fingerId, touch.position);

            Log($"Finger {touch.fingerId} -> {((MonoBehaviour)best.Owner).name}");
        }

        /// <summary>
        /// Finds the highest-priority region whose area contains the given point.
        /// Only ever called for a touch's starting position - never called again
        /// for a touch that already has an owner.
        /// </summary>
        private InputRegion ResolveRegion(Vector2 screenPosition)
        {
            InputRegion best = null;
            for (int i = 0; i < regions.Count; i++)
            {
                InputRegion region = regions[i];
                if (region == null || !region.isActiveAndEnabled) continue;
                if (!region.ContainsScreenPoint(screenPosition)) continue;

                if (best == null || region.Priority > best.Priority)
                {
                    best = region;
                }
            }
            return best;
        }

        private void HandleMoved(Touch touch)
        {
            // No region test here - only a dictionary lookup. This is what makes
            // ownership stable: a finger that started on Dash and drags over Look
            // still only ever reaches Dash's owner, because we never re-resolve.
            if (!_activeOwners.TryGetValue(touch.fingerId, out ITouchOwner owner)) return;

            Vector2 last = _lastPositions[touch.fingerId];
            Vector2 delta = touch.position - last;
            _lastPositions[touch.fingerId] = touch.position;

            owner.OnTouchMoved(touch.fingerId, touch.position, delta);
        }

        private void HandleEnded(Touch touch)
        {
            if (!_activeOwners.TryGetValue(touch.fingerId, out ITouchOwner owner)) return;

            owner.OnTouchReleased(touch.fingerId);
            _activeOwners.Remove(touch.fingerId);
            _lastPositions.Remove(touch.fingerId);

            Log($"Finger {touch.fingerId} Released");
        }

        private void HandleOsCancelled(Touch touch)
        {
            if (!_activeOwners.TryGetValue(touch.fingerId, out ITouchOwner owner)) return;

            owner.OnTouchCancelled(touch.fingerId);
            _activeOwners.Remove(touch.fingerId);
            _lastPositions.Remove(touch.fingerId);

            Log($"Finger {touch.fingerId} Cancelled (OS)");
        }

        /// <summary>
        /// Central gate for modal UI (Pause, the Solve interface, etc). Call this
        /// instead of teaching every individual control about menus.
        ///
        /// Disabling immediately cancels every currently-owned finger and stops new
        /// touches from being claimed. Re-enabling does NOT restore ownership to
        /// fingers still physically touching the screen - a fresh touch-down is
        /// required before gameplay input resumes.
        /// </summary>
        public void SetGameplayInputEnabled(bool enabled)
        {
            if (GameplayInputEnabled == enabled) return;

            GameplayInputEnabled = enabled;

            if (!enabled)
            {
                CancelAllActiveTouches();
                Log("Gameplay Input Disabled - all gameplay touches cancelled.");
            }
            else
            {
                Log("Gameplay Input Enabled - existing touches remain ignored until released and re-touched.");
            }
        }

        private void CancelAllActiveTouches()
        {
            if (_activeOwners.Count == 0) return;

            // Copy the keys first - we mutate _activeOwners while iterating.
            var fingerIds = new List<int>(_activeOwners.Keys);
            foreach (int fingerId in fingerIds)
            {
                _activeOwners[fingerId].OnTouchCancelled(fingerId);
                Log($"Finger {fingerId} Cancelled (gameplay input disabled)");
            }

            _activeOwners.Clear();
            _lastPositions.Clear();
        }

        private void Log(string message)
        {
            if (logOwnershipEvents)
            {
                Debug.Log($"[TouchRouter] {message}");
            }
        }
    }
}
