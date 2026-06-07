using UnityEngine;
using Hearthshade.Core;
using Hearthshade.Systems;

namespace Hearthshade.Gameplay
{
    /// <summary>
    /// A villager (GDD §5.4). Cozy by day (schedule, gifts, questlines), vulnerable by night (Fear rises in
    /// the Gloam). On the Hollow difficulty, sustained max Fear can lead to Fade() — opt-in permadeath that
    /// echoes the stakes reviewers responded to in Grimshire. Befriended villagers help tend the light.
    /// </summary>
    public class Villager : MonoBehaviour, ILightEmitter
    {
        [SerializeField] private string _displayName = "Mira";
        [SerializeField] private Vector2Int _cell;
        [SerializeField] private float _fadeAtFearSeconds = 25f;

        [Range(0f, 1f)] public float Trust;   // befriend to unlock help (night watch, rituals)
        [Range(0f, 1f)] public float Fear;     // rises in darkness/gloam, falls near light + after rituals

        private float _fearedSeconds;
        private GloamSystem _gloam;
        private LightingRegistry _lighting;
        private GameManager _gm;
        private GameClock _clock;
        private bool _faded;

        // A trusted villager on watch contributes a small light (helping tend the homestead).
        public Vector2Int Cell => _cell;
        public float Radius => 1.2f;
        public float Intensity => (Trust > 0.5f && _clock != null && _clock.Current.IsNight()) ? 0.4f : 0f;
        public bool Active => Intensity > 0f && !_faded;

        public string DisplayName => _displayName;
        public bool Faded => _faded;

        private void Start()
        {
            var boot = GameBootstrap.Instance;
            _gloam = boot.Get<GloamSystem>();
            _lighting = boot.Get<LightingRegistry>();
            _gm = boot.Get<GameManager>();
            _clock = boot.Get<GameClock>();
            _lighting.Register(this);
        }

        private void OnDestroy() { if (_lighting != null) _lighting.Unregister(this); }

        private void Update()
        {
            if (_faded) return;
            float g = _gloam.GloamAt(_cell);
            float lit = _lighting.LightAt(_cell);

            // Fear tracks local dread minus comfort from light.
            float target = Mathf.Clamp01(g - lit * 0.8f);
            Fear = Mathf.MoveTowards(Fear, target, 0.25f * Time.deltaTime);

            if (_gm.PermadeathEnabled && Fear > 0.95f)
            {
                _fearedSeconds += Time.deltaTime;
                if (_fearedSeconds >= _fadeAtFearSeconds) Fade();
            }
            else _fearedSeconds = Mathf.Max(0f, _fearedSeconds - Time.deltaTime);
        }

        /// <summary>Comfort a villager via a Ritual (GDD §5.5) — care, not power.</summary>
        public void Comfort(float amount)
        {
            Fear = Mathf.Clamp01(Fear - amount);
            Trust = Mathf.Clamp01(Trust + amount * 0.25f);
        }

        private void Fade()
        {
            _faded = true;
            Debug.Log($"[Villager] {_displayName} has Faded into the Gloam. (Hollow difficulty only.)");
            // Hook: trigger an empty-chair memorial at the hearth; raise community grief beat.
            gameObject.SetActive(false);
        }

        public void SetCell(Vector2Int c) => _cell = c;
    }
}
