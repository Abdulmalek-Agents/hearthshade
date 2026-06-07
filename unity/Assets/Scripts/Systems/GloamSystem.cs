using System;
using UnityEngine;
using Hearthshade.Core;

namespace Hearthshade.Systems
{
    /// <summary>
    /// The Gloam — the environmental antagonist (GDD §5.1). A tile-based "fluid" that, at night, seeps from
    /// the map edges toward unlit tiles and is pushed back by light. It never deals combat damage; instead it
    /// withers crops, frightens villagers, and erodes the homestead. Advances only on GameClock night ticks.
    /// </summary>
    public class GloamSystem : MonoBehaviour, IGameSystem
    {
        public int InitOrder => 20;

        [Range(0f, 1f)] [SerializeField] private float _spreadRate = 0.22f;
        [Range(0f, 1f)] [SerializeField] private float _pushBackRate = 0.6f;
        [SerializeField] private float _witherThreshold = 0.6f;

        private LightingRegistry _lighting;
        private GameClock _clock;
        private float[,] _gloam;
        private float[,] _next;
        private Vector2Int _size;

        /// <summary>Raised per tile when it crosses the wither threshold (FarmSystem & Villager listen).</summary>
        public event Action<Vector2Int, float> OnTileGloamed;

        public void Init()
        {
            var boot = GameBootstrap.Instance;
            _lighting = boot.Get<LightingRegistry>();
            _clock = boot.Get<GameClock>();
            _size = _lighting.GridSize;
            _gloam = new float[_size.x, _size.y];
            _next = new float[_size.x, _size.y];

            _clock.OnNightTick += Tick;
            _clock.OnPhaseChanged += OnPhase;
        }

        private void OnDestroy()
        {
            if (_clock == null) return;
            _clock.OnNightTick -= Tick;
            _clock.OnPhaseChanged -= OnPhase;
        }

        private void OnPhase(Phase p)
        {
            // Daylight / dawn burn the Gloam back across the whole grid (GDD §5.1).
            if (!p.BurnsGloam()) return;
            for (int x = 0; x < _size.x; x++)
                for (int y = 0; y < _size.y; y++)
                    _gloam[x, y] = Mathf.Max(0f, _gloam[x, y] - 0.5f);
        }

        private void Tick()
        {
            _lighting.Recompute();

            for (int x = 0; x < _size.x; x++)
                for (int y = 0; y < _size.y; y++)
                {
                    var cell = new Vector2Int(x, y);
                    float lit = _lighting.LightAt(cell);

                    // strongest neighbour Gloam; map edges count as a permanent source (the treeline).
                    float neigh = 0f;
                    neigh = Mathf.Max(neigh, Sample(x + 1, y));
                    neigh = Mathf.Max(neigh, Sample(x - 1, y));
                    neigh = Mathf.Max(neigh, Sample(x, y + 1));
                    neigh = Mathf.Max(neigh, Sample(x, y - 1));

                    float darkness = Mathf.Max(0f, 1f - lit * 1.4f);
                    float current = _gloam[x, y];
                    float result;

                    if (lit > 0.25f)
                        result = Mathf.Max(0f, current - _pushBackRate * lit);
                    else if (neigh > 0.25f)
                        result = Mathf.Min(1f, current + _spreadRate * darkness * neigh + 0.05f);
                    else
                        result = current;

                    _next[x, y] = result;
                    if (current < _witherThreshold && result >= _witherThreshold)
                        OnTileGloamed?.Invoke(cell, result);
                }

            (_gloam, _next) = (_next, _gloam);
        }

        /// <summary>Out-of-bounds samples return 1 so the dark always pours in from the edges.</summary>
        private float Sample(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _size.x || y >= _size.y) return 1f;
            return _gloam[x, y];
        }

        public float GloamAt(Vector2Int c) =>
            (_lighting.InBounds(c)) ? _gloam[c.x, c.y] : 1f;

        /// <summary>Fraction of interior tiles still free of deep Gloam — the HUD "homestead held" %.</summary>
        public float HomesteadHeld01()
        {
            int interior = 0, held = 0;
            for (int x = 1; x < _size.x - 1; x++)
                for (int y = 1; y < _size.y - 1; y++)
                { interior++; if (_gloam[x, y] < _witherThreshold) held++; }
            return interior == 0 ? 1f : (float)held / interior;
        }

        public void ClearAll() { Array.Clear(_gloam, 0, _gloam.Length); }
    }
}
