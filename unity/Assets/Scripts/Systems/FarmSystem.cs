using System.Collections.Generic;
using UnityEngine;
using Hearthshade.Core;
using Hearthshade.Data;

namespace Hearthshade.Systems
{
    /// <summary>
    /// Grid-based crop management (GDD §4.1, §5.1). Crops grow only in daylight and wither when the local
    /// Gloam crosses the threshold — the literal expression of "cozy progression == survival progression":
    /// a bigger, prettier, better-lit farm is also a more defensible one.
    /// </summary>
    public class FarmSystem : MonoBehaviour, IGameSystem
    {
        public int InitOrder => 30;

        [SerializeField] private CropData _defaultCrop;

        public class Crop
        {
            public int Stage;        // 0..3 (3 = ripe)
            public float GrowTimer;
            public bool Withered;
        }

        private readonly Dictionary<Vector2Int, Crop> _crops = new();
        private GameClock _clock;
        private GloamSystem _gloam;

        public void Init()
        {
            _clock = GameBootstrap.Instance.Get<GameClock>();
            _gloam = GameBootstrap.Instance.Get<GloamSystem>();
            _gloam.OnTileGloamed += OnTileGloamed;
        }

        private void OnDestroy() { if (_gloam != null) _gloam.OnTileGloamed -= OnTileGloamed; }

        private void OnTileGloamed(Vector2Int cell, float g)
        {
            if (_crops.TryGetValue(cell, out var c) && !c.Withered) c.Withered = true;
        }

        public bool Plant(Vector2Int cell)
        {
            if (_crops.ContainsKey(cell)) return false;
            _crops[cell] = new Crop();
            return true;
        }

        /// <summary>Returns sell value if a ripe crop was harvested, else 0. Clears withered crops for free.</summary>
        public int Harvest(Vector2Int cell)
        {
            if (!_crops.TryGetValue(cell, out var c)) return 0;
            if (c.Withered) { _crops.Remove(cell); return 0; }
            if (c.Stage < 3) return 0;
            _crops.Remove(cell);
            return _defaultCrop != null ? _defaultCrop.SellValue : 4;
        }

        private void Update()
        {
            if (!_clock.Current.BurnsGloam() && _clock.Current != Phase.Day) return; // grow in daylight only
            float grow = _defaultCrop != null ? _defaultCrop.GrowSeconds : 3f;
            foreach (var c in _crops.Values)
            {
                if (c.Withered || c.Stage >= 3) continue;
                c.GrowTimer += Time.deltaTime;
                if (c.GrowTimer >= grow) { c.GrowTimer = 0f; c.Stage++; }
            }
        }

        public IReadOnlyDictionary<Vector2Int, Crop> Crops => _crops;
        public void ClearWithered()
        {
            var keys = new List<Vector2Int>();
            foreach (var kv in _crops) if (kv.Value.Withered) keys.Add(kv.Key);
            foreach (var k in keys) _crops.Remove(k);
        }
    }
}
