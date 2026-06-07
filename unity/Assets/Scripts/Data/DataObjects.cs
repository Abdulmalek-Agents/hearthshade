using UnityEngine;
using Hearthshade.Core;

namespace Hearthshade.Data
{
    /// <summary>Per-phase definition that drives GameClock and the day→night color grade (GDD §11).</summary>
    [CreateAssetMenu(menuName = "Hearthshade/Phase Data", fileName = "PhaseData")]
    public class PhaseData : ScriptableObject
    {
        public Phase Phase;
        [Min(0.1f)] public float DurationSeconds = 8f;
        [Tooltip("Global Volume color grade target for this phase (warm day → bruised night).")]
        public Color AmbientGrade = Color.white;
    }

    /// <summary>Crop tuning (GDD §5.1). Withers when local Gloam exceeds WitherThreshold.</summary>
    [CreateAssetMenu(menuName = "Hearthshade/Crop Data", fileName = "CropData")]
    public class CropData : ScriptableObject
    {
        public string DisplayName = "Hollowroot";
        [Min(1)] public int Stages = 3;
        [Min(0.1f)] public float GrowSeconds = 3f;
        [Min(0)] public int SellValue = 4;
        [Range(0f, 1f)] public float WitherThreshold = 0.6f;
    }

    public enum ItemKind { Seed, Oil, Lantern, Food, Ward }

    /// <summary>Generic inventory item (GDD §5.3 economy).</summary>
    [CreateAssetMenu(menuName = "Hearthshade/Item Data", fileName = "ItemData")]
    public class ItemData : ScriptableObject
    {
        public string Id;
        public string Display;
        public ItemKind Kind;
        public int Value;
        public Sprite Icon;
    }

    /// <summary>Serializable save payload (Newtonsoft). Tile grids are not saved (regenerated each dawn).</summary>
    [System.Serializable]
    public class SaveData
    {
        public int Day;
        public Phase Phase;
        public int Oil, Seeds, Coin, SpareLanterns;
        public Difficulty Difficulty;
    }
}
