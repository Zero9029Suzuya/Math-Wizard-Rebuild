namespace MathWizard.Time
{
    /// <summary>
    /// The single seam gameplay systems use to ask "how much gameplay time has passed?"
    /// instead of reading Unity's Time.deltaTime directly.
    ///
    /// This phase intentionally does NOT implement time slow/stop. It exists so that a
    /// future time-manipulation system has one place to plug into (by swapping or wrapping
    /// the implementation gameplay systems are given) without every timer in the game
    /// needing to change. Gameplay systems must never depend on UnityEngine.Time directly
    /// once they take a dependency on this interface.
    /// </summary>
    public interface IGameplayClock
    {
        /// <summary>Gameplay-time seconds elapsed since the last tick.</summary>
        float DeltaTime { get; }
    }
}
