using System;
using System.Collections.Generic;
using UnityEngine;
using Hearthshade.Data;

namespace Hearthshade.Core
{
    /// <summary>
    /// The heartbeat of Hearthshade. A phase state-machine that owns time-of-day and fires the events
    /// every other system listens to. Implements GDD §4 / §5.6. Nothing else advances time.
    /// </summary>
    public class GameClock : MonoBehaviour, IGameSystem
    {
        public int InitOrder => 50;

        [Tooltip("Ordered Morning→Day→Dusk→Night→Dawn phase definitions (duration + ambient grade).")]
        [SerializeField] private List<PhaseData> _phases = new();

        [Tooltip("Seconds between night ticks. The Gloam advances once per tick, not every frame.")]
        [SerializeField] private float _nightTickInterval = 0.45f;

        public Phase Current { get; private set; } = Phase.Morning;
        public int Day { get; private set; } = 1;
        public float PhaseProgress01 { get; private set; }     // 0..1 through the current phase

        /// <summary>Raised whenever the phase changes. Payload = the new phase.</summary>
        public event Action<Phase> OnPhaseChanged;
        /// <summary>Raised once per night tick. The Gloam sim subscribes to this.</summary>
        public event Action OnNightTick;
        /// <summary>Raised when a new in-game Day starts (after Dawn → Morning).</summary>
        public event Action<int> OnNewDay;

        private int _index;
        private float _elapsed;
        private float _tickAccum;
        private bool _running;

        public void Init()
        {
            _index = 0; _elapsed = 0f; _running = true;
            Current = _phases.Count > 0 ? _phases[0].Phase : Phase.Morning;
            OnPhaseChanged?.Invoke(Current);
        }

        public void SetRunning(bool running) => _running = running;

        private void Update()
        {
            if (!_running || _phases.Count == 0) return;

            float dur = Mathf.Max(0.01f, _phases[_index].DurationSeconds);
            _elapsed += Time.deltaTime;
            PhaseProgress01 = Mathf.Clamp01(_elapsed / dur);

            if (Current.IsNight())
            {
                _tickAccum += Time.deltaTime;
                while (_tickAccum >= _nightTickInterval)
                {
                    _tickAccum -= _nightTickInterval;
                    OnNightTick?.Invoke();
                }
            }

            if (_elapsed >= dur)
                Advance();
        }

        private void Advance()
        {
            _elapsed = 0f; _tickAccum = 0f;
            _index = (_index + 1) % _phases.Count;
            Current = _phases[_index].Phase;

            if (Current == Phase.Morning)
            {
                Day++;
                OnNewDay?.Invoke(Day);
            }
            OnPhaseChanged?.Invoke(Current);
        }

        /// <summary>Used by the save system to restore a session mid-arc.</summary>
        public void Restore(int day, Phase phase)
        {
            Day = day;
            _index = _phases.FindIndex(p => p.Phase == phase);
            if (_index < 0) _index = 0;
            Current = _phases[_index].Phase;
            _elapsed = 0f;
            OnPhaseChanged?.Invoke(Current);
        }
    }
}
