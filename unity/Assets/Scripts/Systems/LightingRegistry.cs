using System.Collections.Generic;
using UnityEngine;
using Hearthshade.Core;

namespace Hearthshade.Systems
{
    /// <summary>A thing that pushes the Gloam back. Lanterns, the hearth, moon-wells.</summary>
    public interface ILightEmitter
    {
        Vector2Int Cell { get; }   // grid cell the emitter sits on
        float Radius { get; }      // light radius in tiles
        float Intensity { get; }   // 0..1, scales with oil for lanterns
        bool Active { get; }       // false when out of oil / dormant
    }

    /// <summary>
    /// Central registry of every light emitter plus a per-tile light field the Gloam reads each night tick.
    /// Implements the "light = safe" rule (GDD §5.1, §11). Grid mirrors the prototype (15×11 default).
    /// </summary>
    public class LightingRegistry : MonoBehaviour, IGameSystem
    {
        public int InitOrder => 10;

        [SerializeField] private Vector2Int _gridSize = new(15, 11);
        public Vector2Int GridSize => _gridSize;

        private readonly List<ILightEmitter> _emitters = new();
        private float[,] _light;

        public void Init() => _light = new float[_gridSize.x, _gridSize.y];

        public void Register(ILightEmitter e) { if (!_emitters.Contains(e)) _emitters.Add(e); }
        public void Unregister(ILightEmitter e) => _emitters.Remove(e);

        /// <summary>Recompute the light field. Called once per night tick before the Gloam advances.</summary>
        public void Recompute()
        {
            for (int x = 0; x < _gridSize.x; x++)
                for (int y = 0; y < _gridSize.y; y++)
                    _light[x, y] = 0f;

            foreach (var e in _emitters)
            {
                if (!e.Active) continue;
                int r = Mathf.CeilToInt(e.Radius);
                for (int dx = -r; dx <= r; dx++)
                    for (int dy = -r; dy <= r; dy++)
                    {
                        int x = e.Cell.x + dx, y = e.Cell.y + dy;
                        if (x < 0 || y < 0 || x >= _gridSize.x || y >= _gridSize.y) continue;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        if (d > e.Radius) continue;
                        float v = e.Intensity * (1f - d / e.Radius);
                        if (v > _light[x, y]) _light[x, y] = v;
                    }
            }
        }

        public float LightAt(Vector2Int c) =>
            (c.x < 0 || c.y < 0 || c.x >= _gridSize.x || c.y >= _gridSize.y) ? 0f : _light[c.x, c.y];

        public bool InBounds(Vector2Int c) =>
            c.x >= 0 && c.y >= 0 && c.x < _gridSize.x && c.y < _gridSize.y;
    }
}
