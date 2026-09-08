using UnityEngine;

namespace MathWizard.Time
{
    /// <summary>
    /// Default IGameplayClock: Gameplay Time = Real Time (Unity's Time.deltaTime), unscaled
    /// by any gameplay-level slow/stop effect. There is no time manipulation implemented yet.
    ///
    /// When time slow/stop is introduced, this is the class that changes (or gets swapped
    /// for one that consults a future time-manipulation system) — no gameplay system that
    /// depends on IGameplayClock needs to change.
    ///
    /// Deliberately NOT a MonoBehaviour: it has no per-frame Unity lifecycle needs of its own,
    /// it's just a thin read of Time.deltaTime, so a plain class keeps it trivially
    /// constructible for both runtime wiring and (if ever needed) tests.
    /// </summary>
    public sealed class UnityGameplayClock : IGameplayClock
    {
        public float DeltaTime => UnityEngine.Time.deltaTime;
    }
}
