using UnityEngine;

namespace MathWizard.Input
{
    /// <summary>
    /// Anything that can take ownership of a finger once the TouchRouter assigns it.
    ///
    /// Implementations should NOT read Input.touches directly and should NOT try to
    /// decide "is this touch mine?" themselves - that decision belongs entirely to
    /// the TouchRouter, made once, at the moment the touch begins. An ITouchOwner
    /// simply reacts to whatever the router tells it.
    /// </summary>
    public interface ITouchOwner
    {
        /// <summary>
        /// Called exactly once, on the frame ownership is assigned (TouchPhase.Began).
        /// </summary>
        void OnTouchClaimed(int fingerId, Vector2 screenPosition);

        /// <summary>
        /// Called every frame the owned finger reports Moved or Stationary.
        /// </summary>
        void OnTouchMoved(int fingerId, Vector2 screenPosition, Vector2 delta);

        /// <summary>
        /// Called when the owned finger is lifted normally (TouchPhase.Ended).
        /// </summary>
        void OnTouchReleased(int fingerId);

        /// <summary>
        /// Called when ownership ends abnormally: an OS-level touch cancellation
        /// (TouchPhase.Canceled) or a gameplay-input-disable event (e.g. Pause,
        /// Solve interface opening). Implementations should reset to a neutral
        /// state exactly as if the touch had been released.
        /// </summary>
        void OnTouchCancelled(int fingerId);
    }
}
