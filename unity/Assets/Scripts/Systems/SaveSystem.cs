using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Hearthshade.Core;
using Hearthshade.Data;

namespace Hearthshade.Systems
{
    /// <summary>
    /// JSON save/load using Unity 6 Awaitable for non-blocking disk writes. Persists the run inventory,
    /// clock state, and difficulty band. (Tile grids are regenerated/cleared each dawn so they are not saved.)
    /// </summary>
    public class SaveSystem : MonoBehaviour, IGameSystem
    {
        public int InitOrder => 5;

        private string Path => System.IO.Path.Combine(Application.persistentDataPath, "hearthshade.save.json");
        private GameManager _gm;
        private GameClock _clock;

        public void Init()
        {
            _gm = GameBootstrap.Instance.Get<GameManager>();
            _clock = GameBootstrap.Instance.Get<GameClock>();
        }

        public async Awaitable SaveAsync()
        {
            var data = new SaveData
            {
                Day = _clock != null ? _clock.Day : 1,
                Phase = _clock != null ? _clock.Current : Phase.Morning,
                Oil = _gm.Oil, Seeds = _gm.Seeds, Coin = _gm.Coin, SpareLanterns = _gm.SpareLanterns,
                Difficulty = _gm.Difficulty
            };
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await Awaitable.BackgroundThreadAsync();
            File.WriteAllText(Path, json);
            await Awaitable.MainThreadAsync();
            Debug.Log($"[Save] Wrote {Path}");
        }

        public SaveData Load()
        {
            if (!File.Exists(Path)) return null;
            return JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(Path));
        }

        public bool HasSave() => File.Exists(Path);
    }
}
