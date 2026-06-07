using System;

namespace Hearthshade.Core
{
    /// <summary>The five beats of the Hearthshade day. Implements GDD §4 (the day → dusk → night → dawn loop).</summary>
    public enum Phase
    {
        Morning, // plan the day; review how far the Gloam crept overnight
        Day,     // cozy verbs: farm, cook, craft, befriend
        Dusk,    // telegraphed "the light is going" beat
        Night,   // Gloam advances; maintain the light (no combat)
        Dawn     // the dark is burned back; cleanup
    }

    public static class PhaseExtensions
    {
        public static bool IsNight(this Phase p) => p == Phase.Night;

        /// <summary>Daylight and dawn actively burn the Gloam back (GDD §5.1).</summary>
        public static bool BurnsGloam(this Phase p) => p == Phase.Day || p == Phase.Dawn || p == Phase.Morning;
    }
}
