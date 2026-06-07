using UnityEngine;

namespace Hearthshade.Gameplay
{
    /// <summary>Anything the player can act on with the Interact action (refuel, harvest, rest, market).</summary>
    public interface IInteractable
    {
        /// <summary>Grid cell this interactable occupies (player must be within 1 tile).</summary>
        Vector2Int Cell { get; }

        /// <summary>Short verb shown in the prompt, e.g. "Refuel", "Harvest", "Rest".</summary>
        string Prompt { get; }

        /// <summary>Perform the interaction. Returns true if something happened.</summary>
        bool Interact(PlayerController player);
    }
}
