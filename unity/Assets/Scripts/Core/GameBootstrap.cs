using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hearthshade.Core
{
    /// <summary>
    /// Single persistent entry point. Collects every <see cref="IGameSystem"/> in the bootstrap object,
    /// initializes them in deterministic order, then async-loads the Homestead scene.
    /// See README §4 for the ordering rationale (Gloam must read a built light grid, etc.).
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }

        [SerializeField] private string _firstSceneName = "Homestead";

        private readonly List<IGameSystem> _systems = new();

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _systems.AddRange(GetComponentsInChildren<IGameSystem>(true));
            foreach (var s in _systems.OrderBy(s => s.InitOrder))
            {
                s.Init();
                Debug.Log($"[Bootstrap] Initialized {s.GetType().Name} (order {s.InitOrder}).");
            }
        }

        public T Get<T>() where T : class, IGameSystem =>
            _systems.FirstOrDefault(s => s is T) as T;
    }
}
