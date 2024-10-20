using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace SpikysLib;

public static class ItemHelper {

    public static Item MoveInto(Item item, Item toMove, out int transferred, int? maxStack = null, bool canFavorite = true) {
        transferred = 0;
        if (toMove.IsAir) return item;
        if (item.IsAir) {
            transferred = maxStack.HasValue ? Math.Min(maxStack.Value, toMove.stack) : toMove.stack;
            item = toMove.Clone();
            item.stack = 0;
            ItemLoader.SplitStack(item, toMove, transferred);
        } else if (item.type == toMove.type && item.stack < (maxStack ?? item.maxStack)) {
            int oldStack = item.maxStack;
            if (maxStack.HasValue) item.maxStack = maxStack.Value;
            ItemLoader.TryStackItems(item, toMove, out transferred);
            item.maxStack = oldStack;
        }
        if (transferred != 0) {
            item.favorited = item.favorited || canFavorite && toMove.favorited;
            if (toMove.IsAir) toMove.TurnToAir(true);
        }
        return item;
    }

    public static void RunWithHiddenItems(Item[] items, Action action, Predicate<Item> hidden) {
    Dictionary<int, Item> hiddenItems = new();
        for (int i = 0; i < items.Length; i++) {
            if (!hidden(items[i])) continue;
            hiddenItems[i] = items[i];
            items[i] = new();
        }
        action();
        foreach ((int slot, Item item) in hiddenItems) {
            items[slot] = item;
        }
    }

    public static int CountItems(this Item[] items, int type, params int[] ignoreSots) {
        int total = 0;
        for (int i = 0; i < items.Length; i++) {
            if (Array.IndexOf(ignoreSots, i) == -1 && items[i].type == type)
                total += items[i].stack;
        }
        return total;
    }
    public static long CountCurrency(this Item[] items, int currency, params int[] ignoreSlots) {
        long count;
        switch (currency) {
        case CurrencyHelper.None: return 0L;
        case CurrencyHelper.Coins:
            count = 0L;

            for (int i = 0; i < items.Length; i++) {
                if (Array.IndexOf(ignoreSlots, i) == -1 && items[i].IsACoin)
                    count += (long)items[i].value / 5 * items[i].stack;
            }
            return count;
        default:
            if (!CustomCurrencyManager.TryGetCurrencySystem(currency, out CustomCurrencySystem system)) return 0;
            long cap = system.CurrencyCap;
            system.SetCurrencyCap(long.MaxValue);
            count = system.CountCurrency(out _, items, ignoreSlots);
            system.SetCurrencyCap(cap);
            return count;
        }
    }

    public static bool IsPartOfACurrency(this Item item, out int currency) => (currency = item.CurrencyType()) != CurrencyHelper.None;
    public static int CurrencyType(this Item item) {
        if (item.IsACoin) return CurrencyHelper.Coins;
        foreach ((int key, CustomCurrencySystem system) in CurrencyHelper.CustomCurrencies()) {
            if (system.Accepts(item)) return key;
        }
        return CurrencyHelper.None;
    }
    public static long CurrencyValue(this Item item) => item.CurrencyType() switch {
        CurrencyHelper.None => 0,
        CurrencyHelper.Coins => item.value / 5,
        int t => CustomCurrencyManager.TryGetCurrencySystem(t, out var system) ? system.ValuePerUnit(item.type) : 0
    };
}