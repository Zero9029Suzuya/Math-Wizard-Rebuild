using UnityEngine;

namespace MathWizard.Player.Movement
{
    public sealed class DashMotionState
    {
        public bool IsActive { get; private set; }
        public Vector3 Direction { get; private set; }
        public float Speed { get; private set; }

        private float _remainingDuration;

        public void Begin(Vector3 direction, float speed, float duration)
        {
            Direction = direction;
            Speed = speed;
            _remainingDuration = duration;
            IsActive = true;
        }

        public void Tick(float deltaTime)
        {
            if (!IsActive)
                return;

            _remainingDuration -= deltaTime;

            if (_remainingDuration <= 0f)
            {
                _remainingDuration = 0f;
                IsActive = false;
                Direction = Vector3.zero;
                Speed = 0f;
            }
        }
    }
}