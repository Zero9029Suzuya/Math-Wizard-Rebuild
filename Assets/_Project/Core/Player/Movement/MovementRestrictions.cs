using MathWizard.Player.Common;

namespace MathWizard.Player.Movement
{
    /// <summary>
    /// Owns whether each MoveCapability is currently allowed. Any number of independent
    /// sources (status effects, spell casts, the math-input UI) can block a capability at once;
    /// it becomes allowed again only once every source has unblocked it.
    ///
    /// This class owns nothing about WHY something is blocked, WHAT the resulting speed is, or
    /// HOW an action is actually executed — MovementController asks it "is this allowed?" and
    /// nothing more. It is a thin, deliberately dumb wrapper: the actual refcount logic lives in
    /// the shared CapabilityGate so it isn't duplicated when CastRestrictions needs the same
    /// mechanism in a later phase.
    /// </summary>
    public sealed class MovementRestrictions
    {
        private readonly CapabilityGate<MoveCapability> _gate = new CapabilityGate<MoveCapability>();

        public void Block(MoveCapability capability, object source) => _gate.Block(capability, source);

        public void Unblock(MoveCapability capability, object source) => _gate.Unblock(capability, source);

        public bool IsAllowed(MoveCapability capability) => _gate.IsAllowed(capability);
    }
}
