using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D
using Hearthshade.Core;
using Hearthshade.Systems;

namespace Hearthshade.Gameplay
{
    /// <summary>
    /// A lantern: cozy decoration by day, survival tool by night (GDD core pillar "one toolbox, two tones").
    /// Burns oil at night, dims as it empties, and can be refueled by the player. Emits to LightingRegistry.
    /// </summary>
    [RequireComponent(typeof(Light2D))]
    public class Lantern : MonoBehaviour, ILightEmitter, IInteractable
    {
        [SerializeField] private Vector2Int _cell;
        [SerializeField] private float _maxOil = 6f;
        [SerializeField] private float _burnPerSec = 0.35f;
        [SerializeField] private float _radius = 1.9f;

        private float _oil = 3f;
        private Light2D _light2D;
        private LightingRegistry _registry;
        private GameClock _clock;
        private GameManager _gm;

        public Vector2Int Cell => _cell;
        public float Radius => _radius;
        public float Intensity => Mathf.Clamp01(_oil / _maxOil) * 0.9f;
        public bool Active => _oil > 0f;
        public string Prompt => "Refuel";

        private void Awake() => _light2D = GetComponent<Light2D>();

        private void Start()
        {
            var boot = GameBootstrap.Instance;
            _registry = boot.Get<LightingRegistry>();
            _clock = boot.Get<GameClock>();
            _gm = boot.Get<GameManager>();
            _registry.Register(this);
        }

        private void OnDestroy() { if (_registry != null) _registry.Unregister(this); }

        private void Update()
        {
            if (_clock.Current.IsNight() && _oil > 0f)
                _oil = Mathf.Max(0f, _oil - _burnPerSec * Time.deltaTime);

            // visual light scales with remaining oil
            _light2D.intensity = Intensity;
            _light2D.pointLightOuterRadius = _radius;
            _light2D.enabled = Active;
        }

        public bool Interact(PlayerController player)
        {
            if (_oil >= _maxOil) return false;
            if (!_gm.SpendOil(1)) return false;
            _oil = Mathf.Min(_maxOil, _oil + 3f);
            return true;
        }

        public float Oil => _oil;
        public void SetCell(Vector2Int c) => _cell = c;
    }
}
