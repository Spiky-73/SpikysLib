using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SpikysLib.Extensions;

public static class PlayerExtensions {

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
        for (int i = 54; i < 58; i++) {
            if (!player.inventory[i].IsAir && predicate(player.inventory[i])) return player.inventory[i];
        }
        for (int i = 0; i < 54; i++) {
            if (!player.inventory[i].IsAir && predicate(player.inventory[i])) return player.inventory[i];
        }

        return null;
    }
    public static Item? PickPaint(this Player player) => player.Pick(i => i.PaintOrCoating);
    public static Item? PickBait(this Player player) => player.Pick(i => i.bait > 0);

    public static bool InChest(this Player player, [MaybeNullWhen(false)] out Item[] chest) => (chest = player.Chest()) is not null;
    [return: NotNullIfNotNull("chest")]
    public static Item[]? Chest(this Player player, int? chest = null) {
        int c = chest ?? player.chest;
        return c switch {
            >= 0 => Main.chest[c].item,
            -2 => player.bank.item,
            -3 => player.bank2.item,
            -4 => player.bank3.item,
            -5 => player.bank4.item,
            _ => null
        };
    }

    public static int CountItems(this Player player, int type, bool includeChest = false) {
        int total = player.inventory.CountItems(type, 58) + new[] { Main.mouseItem }.CountItems(type) + new[] { Main.CreativeMenu.GetItemByIndex(0) }.CountItems(type);
        if (includeChest) {
            if (CrossMod.MagicStorageIntegration.Enabled && CrossMod.MagicStorageIntegration.InMagicStorage) total += CrossMod.MagicStorageIntegration.CountItems(type);
            else if (player.InChest(out Item[]? chest)) total += chest.CountItems(type);
            if (player.chest != -5 && player.useVoidBag()) total += player.bank4.item.CountItems(type);
        }
        return total;
    }

    public static long CountCurrency(this Player player, int currency, bool includeBanks = true, bool includeChest = false) {
        long count = player.inventory.CountCurrency(currency, 58);
        count += new Item[] { Main.mouseItem }.CountCurrency(currency);
        if (includeBanks) count += player.bank.item.CountCurrency(currency)
                                + player.bank2.item.CountCurrency(currency)
                                + player.bank3.item.CountCurrency(currency)
                                + player.bank4.item.CountCurrency(currency);
        if (includeChest && player.chest >= 0) count += player.Chest()!.CountCurrency(currency);

        return count;
    }


    public static ReadOnlyDictionary<int, int> OwnedItems => Data.ownedItems;

    public static readonly int[] InventoryContexts = new int[] { ItemSlot.Context.InventoryItem, ItemSlot.Context.InventoryAmmo, ItemSlot.Context.InventoryCoin };

    private class Data : ILoadable {
        public void Load(Mod mod) => ownedItems = new(Reflection.Recipe._ownedItems.GetValue());
        public void Unload() => ownedItems = null!;
        public static ReadOnlyDictionary<int, int> ownedItems = null!;
    }
}