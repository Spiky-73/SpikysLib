using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using SpikysLib.Collections;
using SpikysLib.Constants;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.UI;

namespace SpikysLib;

public static class PlayerHelper {

    public static void GetDropItem(this Player player, ref Item item, GetItemSettings? settings = null) {
        if (item.IsAir) return;
        item.position = player.Center;
        Item rem = player.GetItem(player.whoAmI, item, settings ?? GetItemSettings.GetItemInDropItemCheck);
        if (rem.stack > 0) {
            int i = Item.NewItem(new EntitySource_OverfullInventory(player, null), (int)player.position.X, (int)player.position.Y, player.width, player.height, rem.type, rem.stack, false, rem.prefix, true, false);
            Main.item[i] = rem.Clone();
            Main.item[i].newAndShiny = false;
            if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f, 0f, 0f, 0, 0, 0);
        }
        item = new();
        Recipe.FindRecipes();
    }

    public static void RemoveFromInventory(this Player player, int type, int count = 1) {
        foreach (Item i in player.inventory) {
            if (i.type != type) continue;
            if (i.stack < count) {
                count -= i.stack;
                i.TurnToAir();
            } else {
                i.stack -= count;
                return;
            }
        }
    }

    public static Item? FindItemRaw(this Player player, int type) {
        int num = player.FindItem(type);
        return num == -1 ? null : player.inventory[num];
    }

    public static Item? Pick(this Player player, Predicate<Item> predicate) {
        for (int i = InventorySlots.Ammo.Start; i < InventorySlots.Ammo.End; i++) {
            if (!player.inventory[i].IsAir && predicate(player.inventory[i])) return player.inventory[i];
        }
        for (int i = 0; i < InventorySlots.Ammo.Start; i++) {
            if (!player.inventory[i].IsAir && predicate(player.inventory[i])) return player.inventory[i];
        }

        return null;
    }
    public static Item? PickPaint(this Player player) => player.Pick(i => i.PaintOrCoating);
    public static Item? PickBait(this Player player) => player.Pick(i => i.bait > 0);

    public static bool InChest(this Player player, [MaybeNullWhen(false)] out Item[] chest) => (chest = player.Chest()) is not null;
    [return: NotNullIfNotNull(nameof(chest))]
    public static Item[]? Chest(this Player player, int? chest = null) {
        int c = chest ?? player.chest;
        return c switch {
            >= 0 => Main.chest[c].item,
            InventorySlots.PiggyBank => player.bank.item,
            InventorySlots.Safe => player.bank2.item,
            InventorySlots.DefendersForge => player.bank3.item,
            InventorySlots.VoidBag => player.bank4.item,
            _ => null
        };
    }

    public static int CountItems(this Player player, int type, bool includeChests = false) {
        bool local = player.whoAmI == Main.myPlayer;

        if (!includeChests) return local ?
            (player.inventory.CountItems(type, InventorySlots.Mouse) + new Item[] { Main.mouseItem }.CountItems(type)) :
            player.inventory.CountItems(type);

        int count = 0;
        if(local){
            // Makes sure the held item is counted a single time
            if(!_mouseItemMaterial) count += new Item[] { Main.mouseItem }.CountItems(type);
            count += OwnedItems.GetValueOrDefault(type);
        } else {
            count += new Item[] { player.inventory[InventorySlots.Mouse] }.CountItems(type);
            (Dictionary<int, int> items, var ownedItems) = ([], Reflection.Recipe._ownedItems.GetValue());
            Reflection.Recipe._ownedItems.SetValue(items);
            Reflection.Recipe.CollectItemsToCraftWithFrom.Invoke(player);
            Reflection.Recipe._ownedItems.SetValue(ownedItems);
            count += items.GetValueOrDefault(type);
        }
        if (CrossMod.MagicStorageIntegration.Enabled) count += CrossMod.MagicStorageIntegration.CountItems(player, type);
        
        return count;
    }
    public static long CountCurrency(this Player player, int currency, bool includeBanks = true, bool includeChest = false) {
        long count = player.whoAmI == Main.myPlayer ?
            player.inventory.CountCurrency(currency, InventorySlots.Mouse) + new Item[] { Main.mouseItem }.CountCurrency(currency) :
            player.inventory.CountCurrency(currency);
        if (includeBanks) count += player.bank.item.CountCurrency(currency)
                                + player.bank2.item.CountCurrency(currency)
                                + player.bank3.item.CountCurrency(currency)
                                + player.bank4.item.CountCurrency(currency);
        if (includeChest && player.chest >= 0) count += player.Chest()!.CountCurrency(currency);

        return count;
    }


    public static ReadOnlyDictionary<int, int> OwnedItems { get; private set; } = null!;
    private static bool _mouseItemMaterial;

    public static readonly int[] InventoryContexts = [ItemSlot.Context.InventoryItem, ItemSlot.Context.InventoryAmmo, ItemSlot.Context.InventoryCoin];

    internal static void Load() {
        OwnedItems = new(Reflection.Recipe._ownedItems.GetValue());
        On_Recipe.CollectItemsToCraftWithFrom += HookCollectItemsToCraftWithFrom;
        On_Recipe.CollectItems_IEnumerable1 += HookDetectMouseItemMaterial;
    }


    private static void HookCollectItemsToCraftWithFrom(On_Recipe.orig_CollectItemsToCraftWithFrom orig, Player player) {
        if(player == Main.LocalPlayer) _mouseItemMaterial = false;
        orig(player);
    }

    private static void HookDetectMouseItemMaterial(On_Recipe.orig_CollectItems_IEnumerable1 orig, IEnumerable<Item> items) {
        if(items.Exist(i => i == Main.mouseItem)) _mouseItemMaterial = true;
        orig(items);
    }

    internal static void Unload() => OwnedItems = null!;
}