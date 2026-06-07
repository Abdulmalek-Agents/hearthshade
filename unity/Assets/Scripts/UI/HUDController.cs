using UnityEngine;
using UnityEngine.UIElements;
using Hearthshade.Core;
using Hearthshade.Systems;

namespace Hearthshade.UI
{
    /// <summary>
    /// Binds the runtime systems to a UI Toolkit HUD: phase tag, time bar, Resolve, "homestead held" %,
    /// and the inventory pills (oil/seeds/coin/lanterns). Mirrors the prototype HUD so play feels 1:1.
    /// Attach to a UIDocument whose UXML contains the named elements below.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class HUDController : MonoBehaviour
    {
        private Label _phase, _resolveVal, _holdVal, _day, _oil, _seeds, _coin, _lanterns;
        private VisualElement _timeFill, _resolveFill, _holdFill;

        private GameClock _clock;
        private ResolveSystem _resolve;
        private GloamSystem _gloam;
        private GameManager _gm;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _phase      = root.Q<Label>("phase");
            _resolveVal = root.Q<Label>("resolve-val");
            _holdVal    = root.Q<Label>("hold-val");
            _day        = root.Q<Label>("day");
            _oil        = root.Q<Label>("oil");
            _seeds      = root.Q<Label>("seeds");
            _coin       = root.Q<Label>("coin");
            _lanterns   = root.Q<Label>("lanterns");
            _timeFill    = root.Q<VisualElement>("time-fill");
            _resolveFill = root.Q<VisualElement>("resolve-fill");
            _holdFill    = root.Q<VisualElement>("hold-fill");

            var boot = GameBootstrap.Instance;
            _clock = boot.Get<GameClock>();
            _resolve = boot.Get<ResolveSystem>();
            _gloam = boot.Get<GloamSystem>();
            _gm = boot.Get<GameManager>();
        }

        private void Update()
        {
            if (_clock == null) return;

            bool night = _clock.Current.IsNight();
            if (_phase != null) { _phase.text = _clock.Current.ToString() + (night ? "  \u25CF" : "  \u2600");
                _phase.style.color = night ? new Color(0.70f,0.62f,0.86f) : new Color(0.91f,0.69f,0.42f); }

            SetFill(_timeFill, _clock.PhaseProgress01);
            SetFill(_resolveFill, _resolve.Normalized);
            if (_resolveVal != null) _resolveVal.text = Mathf.RoundToInt(_resolve.Value).ToString();

            float held = _gloam.HomesteadHeld01();
            SetFill(_holdFill, held);
            if (_holdVal != null) _holdVal.text = Mathf.RoundToInt(held * 100f) + "%";

            if (_day != null)      _day.text = _clock.Day.ToString();
            if (_oil != null)      _oil.text = _gm.Oil.ToString();
            if (_seeds != null)    _seeds.text = _gm.Seeds.ToString();
            if (_coin != null)     _coin.text = _gm.Coin.ToString();
            if (_lanterns != null) _lanterns.text = _gm.SpareLanterns.ToString();
        }

        private static void SetFill(VisualElement fill, float t01)
        {
            if (fill != null) fill.style.width = Length.Percent(Mathf.Clamp01(t01) * 100f);
        }
    }
}
