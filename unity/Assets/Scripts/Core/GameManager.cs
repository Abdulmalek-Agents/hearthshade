using UnityEngine;

namespace Hearthshade.Core
{
    public enum Difficulty { Gentle, Tended, Hollow }

    /// <summary>
    /// Top-level game state: difficulty band, inventory totals, and the "lose the thread" soft-fail
    /// (GDD §5.2 — failure is melancholy, never death). Holds the player's run-level resources.
    /// </summary>
    public class GameManager : MonoBehaviour, IGameSystem
    {
        public int InitOrder => 100;

        [SerializeField] private Difficulty _difficulty = Difficulty.Tended;
        public Difficulty Difficulty => _difficulty;

        // Run inventory (mirrors the HTML prototype economy).
        public int Oil { get; private set; } = 6;
        public int Seeds { get; private set; } = 5;
        public int Coin { get; private set; } = 0;
        public int SpareLanterns { get; private set; } = 2;

        public void Init() { }

        public bool SpendOil(int n = 1) { if (Oil < n) return false; Oil -= n; return true; }
        public void AddOil(int n) => Oil += n;
        public bool SpendSeed() { if (Seeds <= 0) return false; Seeds--; return true; }
        public void AddCoin(int n) => Coin += n;
        public bool SpendLantern() { if (SpareLanterns <= 0) return false; SpareLanterns--; return true; }

        /// <summary>Permadeath only meaningful on Hollow; preventable on Tended; off on Gentle.</summary>
        public bool PermadeathEnabled => _difficulty == Difficulty.Hollow;
        public float NightDrainMultiplier => _difficulty switch
        {
            Difficulty.Gentle => 0.6f,
            Difficulty.Hollow => 1.5f,
            _ => 1f
        };

        public void SetDifficulty(Difficulty d) => _difficulty = d;
    }
}
