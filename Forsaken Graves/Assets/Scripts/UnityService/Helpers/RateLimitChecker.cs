using UnityEngine;

namespace ForsakenGraves.UnityService.Helpers
{
    public class RateLimitChecker
    {
        public float CooldownDurationInSeconds => _cooldownPeriodInSeconds;

        readonly float _cooldownPeriodInSeconds;
        private float _lastCooldownFinishTime;

        public RateLimitChecker(float cooldownPeriodInSeconds)
        {
            _cooldownPeriodInSeconds = cooldownPeriodInSeconds;
            _lastCooldownFinishTime = -1f;
        }

        public bool CanCall => Time.unscaledTime > _lastCooldownFinishTime;

        public void PutOnCooldown()
        {
            _lastCooldownFinishTime = Time.unscaledTime + _cooldownPeriodInSeconds;
        }
    }
}