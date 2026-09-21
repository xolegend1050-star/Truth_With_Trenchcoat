using System.Collections.Generic;
using UnityEngine;

namespace TruthWithTrenchcoat.Core
{
    /// <summary>
    /// Simple item-tag inventory. Since this is a VR game using XR grab
    /// interactables, physical items live in the player's hand (grabbed),
    /// but we ALSO record a lightweight "has collected" flag here so
    /// puzzle scripts (e.g. DrawerController checking for the wrench)
    /// don't need to search the player's hands directly.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }

        private readonly HashSet<string> collectedItemIds = new HashSet<string>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void AddItem(string itemId)
        {
            collectedItemIds.Add(itemId);
            Debug.Log($"[PlayerInventory] Collected: {itemId}");
        }

        public bool HasItem(string itemId) => collectedItemIds.Contains(itemId);

        public void RemoveItem(string itemId) => collectedItemIds.Remove(itemId);
    }
}
