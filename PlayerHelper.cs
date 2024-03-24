using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
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

    public static bool InChest(this Player player, [MaybeNullWhen(false)] out Item[] chest) => (chest = player.Chest()) is not null;
    [return: NotNullIfNotNull("chest")]
    public static Item[]? Chest(this Player player, int? chest = null) {
        int c = chest ?? player.chest;
        return c switch {
            > -1 => Main.chest[c].item,
            -2 => player.bank.item,
            -3 => player.bank2.item,
            -4 => player.bank3.item,
            -5 => player.bank4.item,
            _ => null
        };
    }

    public static long CountCurrency(this Player player, int currencyIndex = -1) {
        if (currencyIndex != -1) {
            CustomCurrencyManager.TryGetCurrencySystem(currencyIndex, out CustomCurrencySystem customCurrencySystem);
            return customCurrencySystem.CombineStacks(out _, new long[] {
                customCurrencySystem.CountCurrency(out _, player.inventory, new int[] { 58, 57, 56, 55, 54 }),
                customCurrencySystem.CountCurrency(out _, player.bank.item, Array.Empty<int>()),
                customCurrencySystem.CountCurrency(out _, player.bank2.item, Array.Empty<int>()),
                customCurrencySystem.CountCurrency(out _, player.bank3.item, Array.Empty<int>()),
                customCurrencySystem.CountCurrency(out _, player.bank4.item, Array.Empty<int>())
            });
        }
        return Utils.CoinsCombineStacks(out _, new long[] {
            Utils.CoinsCount(out _, player.inventory, new int[] { 58, 57, 56, 55, 54 }),
            Utils.CoinsCount(out _, player.bank.item, Array.Empty<int>()),
            Utils.CoinsCount(out _, player.bank2.item, Array.Empty<int>()),
            Utils.CoinsCount(out _, player.bank3.item, Array.Empty<int>()),
            Utils.CoinsCount(out _, player.bank4.item, Array.Empty<int>())
        });
    }

    public static ReadOnlyDictionary<int, int> OwnedItems => Data.ownedItems;

    public static readonly int[] InventoryContexts = new int[] { ItemSlot.Context.InventoryItem, ItemSlot.Context.InventoryAmmo, ItemSlot.Context.InventoryCoin };

    private class Data : ILoadable {
        public void Load(Mod mod) => ownedItems = new(Reflection.Recipe._ownedItems.GetValue());
        public void Unload() => ownedItems = null!;
        public static ReadOnlyDictionary<int, int> ownedItems = null!;
    }
}