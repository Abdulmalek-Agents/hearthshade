using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Hearthshade.Core;
using Hearthshade.Systems;

namespace Hearthshade.Gameplay
{
    /// <summary>
    /// Grid-stepped player using the new Input System (README §5). Drives Resolve sampling each frame and
    /// routes Interact/Place/Rest to the right systems. Mirrors the HTML prototype's verbs in 3D/2.5D space.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _stepCooldown = 0.12f;
        [SerializeField] private Transform _visual;

        public Vector2Int Cell { get; private set; }

        private LightingRegistry _lighting;
        private ResolveSystem _resolve;
        private GameManager _gm;
        private GameClock _clock;
        private FarmSystem _farm;

        private Vector2 _moveInput;
        private float _cooldown;
        private readonly List<IInteractable> _interactables = new();

        private void Start()
        {
            var boot = GameBootstrap.Instance;
            _lighting = boot.Get<LightingRegistry>();
            _resolve  = boot.Get<ResolveSystem>();
            _gm       = boot.Get<GameManager>();
            _clock    = boot.Get<GameClock>();
            _farm     = boot.Get<FarmSystem>();

            _resolve.OnLostThread += HandleLostThread;
        }

        private void OnDestroy() { if (_resolve != null) _resolve.OnLostThread -= HandleLostThread; }

        // ---- Input System message callbacks (Send Messages / Unity Events) -------------------
        public void OnMove(InputValue v) => _moveInput = v.Get<Vector2>();
        public void OnInteract(InputValue v) { if (v.isPressed) TryInteract(); }
        public void OnPlaceLantern(InputValue v) { if (v.isPressed) TryPlaceLantern(); }
        public void OnRest(InputValue v) { if (v.isPressed) TryRest(); }

        private void Update()
        {
            // grid-stepped movement
            _cooldown -= Time.deltaTime;
            if (_cooldown <= 0f && _moveInput.sqrMagnitude > 0.2f)
            {
                Vector2Int dir = Mathf.Abs(_moveInput.x) > Mathf.Abs(_moveInput.y)
                    ? new Vector2Int((int)Mathf.Sign(_moveInput.x), 0)
                    : new Vector2Int(0, (int)Mathf.Sign(_moveInput.y));
                var target = Cell + dir;
                if (_lighting.InBounds(target)) { Cell = target; _cooldown = _stepCooldown; SyncVisual(); }
            }

            // Resolve responds to the light at the player's feet (GDD §5.2)
            float lit = _lighting.LightAt(Cell);
            _resolve.Sample(lit, Time.deltaTime);
        }

        private void SyncVisual()
        {
            if (_visual != null) _visual.position = new Vector3(Cell.x, Cell.y, 0f);
        }

        // ---- registration so we know what's nearby ------------------------------------------
        public void RegisterInteractable(IInteractable i) { if (!_interactables.Contains(i)) _interactables.Add(i); }
        public void UnregisterInteractable(IInteractable i) => _interactables.Remove(i);

        private IInteractable Nearest()
        {
            IInteractable best = null; int bestDist = int.MaxValue;
            foreach (var i in _interactables)
            {
                int d = Mathf.Abs(i.Cell.x - Cell.x) + Mathf.Abs(i.Cell.y - Cell.y);
                if (d <= 1 && d < bestDist) { best = i; bestDist = d; }
            }
            return best;
        }

        private void TryInteract()
        {
            var i = Nearest();
            if (i != null) { i.Interact(this); return; }
            // fallback: farm the current tile
            int v = _farm.Harvest(Cell);
            if (v > 0) { _gm.AddCoin(v); return; }
            if (_gm.SpendSeed()) _farm.Plant(Cell);
        }

        private void TryPlaceLantern()
        {
            if (_gm.SpareLanterns <= 0 || _gm.Oil <= 0) return;
            // Spawn handled by a LanternSpawner in scene; omitted here for brevity.
            _gm.SpendLantern(); _gm.SpendOil(1);
        }

        private void TryRest()
        {
            if (_clock.Current.IsNight()) return; // can't rest at night (GDD §4.3)
            // RestPoint (hearth) does the actual restore when within range.
        }

        private void HandleLostThread()
        {
            // Soft-fail: melancholy reset to morning, keep coins, lose the night (GDD §5.2).
            _clock.Restore(_clock.Day, Phase.Morning);
            GameBootstrap.Instance.Get<GloamSystem>().ClearAll();
            _farm.ClearWithered();
            _resolve.SetTo(_resolve.Max * 0.7f);
        }
    }
}
