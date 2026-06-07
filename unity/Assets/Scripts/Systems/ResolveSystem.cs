using System;
using UnityEngine;
using Hearthshade.Core;

namespace Hearthshade.Systems
{
    /// <summary>
    /// Resolve — the player's nerve (GDD §5.2). A *soft* survival meter, never a combat health bar.
    /// Drains in darkness at night; restores in daylight, by the hearth, and after Rituals. At zero the
    /// player "loses the thread" (melancholy soft-fail) rather than dying.
    /// </summary>
    public class ResolveSystem : MonoBehaviour, IGameSystem
    {
        public int InitOrder => 40;

        [SerializeField] private float _max = 100f;
        [SerializeField] private float _darkDrainPerSec = 12f;
        [SerializeField] private float _nightAmbientDrain = 1.5f;
        [SerializeField] private float _dayRegenPerSec = 3f;

        public float Value { get; private set; }
        public float Max => _max;
        public float Normalized => Mathf.Clamp01(Value / _max);

        /// <summary>Raised when Resolve hits zero — GameManager/PlayerController handle the soft-fail.</summary>
        public event Action OnLostThread;
        public event Action<float> OnChanged;

        private GameClock _clock;
        private GameManager _gm;

        public void Init()
        {
            Value = _max;
            _clock = GameBootstrap.Instance.Get<GameClock>();
            _gm = GameBootstrap.Instance.Get<GameManager>();
        }

        /// <summary>Called each frame by PlayerController with the light level at the player's tile.</summary>
        public void Sample(float lightAtPlayer, float dt)
        {
            float before = Value;
            if (_clock.Current.IsNight())
            {
                float mult = _gm != null ? _gm.NightDrainMultiplier : 1f;
                if (lightAtPlayer < 0.2f) Value -= _darkDrainPerSec * mult * dt;
                else Value -= _nightAmbientDrain * mult * dt;
            }
            else if (_clock.Current != Phase.Dusk)
            {
                Value = Mathf.Min(_max, Value + _dayRegenPerSec * dt);
            }

            Value = Mathf.Clamp(Value, 0f, _max);
            if (!Mathf.Approximately(before, Value)) OnChanged?.Invoke(Value);
            if (Value <= 0f) OnLostThread?.Invoke();
        }

        public void Restore(float amount)
        {
            Value = Mathf.Clamp(Value + amount, 0f, _max);
            OnChanged?.Invoke(Value);
        }

        public void SetTo(float v) { Value = Mathf.Clamp(v, 0f, _max); OnChanged?.Invoke(Value); }
    }
}
