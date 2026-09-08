using UnityEngine;

namespace MathWizard.Input
{
    /// <summary>
    /// Minimal stand-in for the real MovementOwner / LookOwner / ButtonOwner to be written later (LMAO). 
    /// It does nothing except remember what it was told and optionally log/post it, 
    /// so you can verify the routing logic in isolation before any real gameplay behaviour is attached.
    ///
    /// Attach one of these per region (Movement, Look, Dash, ...) and rename the
    /// GameObject to something identifiable - the name shows up in the router's
    /// Console logs (e.g. "Finger 3 -> Movement").
    /// </summary>
    public class DebugTouchOwner : MonoBehaviour, ITouchOwner
    {
        [Tooltip("Logs every OnTouchMoved call with its delta. Off by default - the router's own logs are usually enough.")]
        [SerializeField] private bool logMovement = false;

        public bool IsOwningTouch { get; private set; }
        public Vector2 CurrentPosition { get; private set; }
        public Vector2 LastDelta { get; private set; }

        public void OnTouchClaimed(int fingerId, Vector2 screenPosition)
        {
            IsOwningTouch = true;
            CurrentPosition = screenPosition;
            LastDelta = Vector2.zero;
        }

        public void OnTouchMoved(int fingerId, Vector2 screenPosition, Vector2 delta)
        {
            CurrentPosition = screenPosition;
            LastDelta = delta;

            if (logMovement)
            {
                Debug.Log($"[{name}] finger {fingerId} delta {delta}");
            }
        }

        public void OnTouchReleased(int fingerId)
        {
            IsOwningTouch = false;
            LastDelta = Vector2.zero;
        }

        public void OnTouchCancelled(int fingerId)
        {
            IsOwningTouch = false;
            LastDelta = Vector2.zero;
        }
    }
}
