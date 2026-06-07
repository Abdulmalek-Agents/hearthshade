namespace Hearthshade.Core
{
    /// <summary>
    /// Common contract for the persistent runtime systems spun up by <see cref="GameBootstrap"/>.
    /// Init() is called in a deterministic order so systems never read each other before they are built.
    /// </summary>
    public interface IGameSystem
    {
        /// <summary>Lower numbers initialize first. See GameBootstrap order table.</summary>
        int InitOrder { get; }

        /// <summary>Build internal state. Safe to reference other already-initialized systems.</summary>
        void Init();
    }
}
