using UnityEngine;
using UnityEngine.Rendering.Universal;
using Hearthshade.Core;
using Hearthshade.Systems;

namespace Hearthshade.Gameplay
{
    /// <summary>
    /// The hearth: the homestead's permanent heart-light and rest point (GDD §4, §5.3). Always-on, highest
    /// priority emitter — the Gloam can never fully claim the hearth tile. Resting here (day only) restores
    /// Resolve. This is the emotional and mechanical center of "make a home worth protecting."
    /// </summary>
    [RequireComponent(typeof(Light2D))]
    public class Hearth : MonoBehaviour, ILightEmitter, IInteractable
    {
        [SerializeField] private Vector2Int _cell = new(7, 5);
        [SerializeField] private float _radius = 2.6f;
        [SerializeField] private float _restoreOnRest = 18f;

        private LightingRegistry _registry;
        private ResolveSystem _resolve;
        private GameClock _clock;

        public Vector2Int Cell => _cell;
        public float Radius => _radius;
        public float Intensity => 1f;      // always full
        public bool Active => true;
        public string Prompt => "Rest";

        private void Start()
        {
            var boot = GameBootstrap.Instance;
            _registry = boot.Get<LightingRegistry>();
            _resolve = boot.Get<ResolveSystem>();
            _clock = boot.Get<GameClock>();
            _registry.Register(this);
        }

        private void OnDestroy() { if (_registry != null) _registry.Unregister(this); }

        public bool Interact(PlayerController player)
        {
            if (_clock.Current.IsNight()) return false;   // too tense to rest at night (GDD §4.3)
            _resolve.Restore(_restoreOnRest);
            return true;
        }
    }
}
