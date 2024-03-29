using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace SpikysLib.Extensions;

public static class ItemExtensions {

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

    public static void RunWithHiddenItems(Item[] chest, Action action, Predicate<Item> hidden) {
    Dictionary<int, Item> hiddenItems = new();
        for (int i = 0; i < chest.Length; i++) {
            if (!hidden(chest[i])) continue;
            hiddenItems[i] = chest[i];
            chest[i] = new();
        }
        action();
        foreach ((int slot, Item item) in hiddenItems) {
            chest[slot] = item;
        }
    }

    public static int CountItems(this Item[] container, int type, params int[] ignoreSots) {
        int total = 0;
        for (int i = 0; i < container.Length; i++) {
            if (Array.IndexOf(ignoreSots, i) == -1 && container[i].type == type)
                total += container[i].stack;
        }
        return total;
    }

    public static int CurrencyType(this Item item) {
        if (item.IsACoin) return Currencies.Coins;
        foreach (int key in Currencies.CustomCurrencies) {
            if (Currencies.CurrencySystems(key).System.Accepts(item)) return key;
        }
        return Currencies.None;
    }
    public static bool IsPartOfACurrency(this Item item, out int currency) => (currency = item.CurrencyType()) != Currencies.None;

    public static long CurrencyValue(this Item item) => item.CurrencyType() switch {
        Currencies.None => 0,
        Currencies.Coins => item.value / 5,
        int t => Currencies.CurrencySystems(t).Values[item.type]
    };

    public static long CountCurrency(this Item[] container, int currency, params int[] ignoreSlots) {
        long count;
        switch (currency) {
        case Currencies.None: return 0L;
        case Currencies.Coins:
            count = 0L;

            for (int i = 0; i < container.Length; i++) {
                if (System.Array.IndexOf(ignoreSlots, i) == -1 && container[i].IsACoin)
                    count += (long)container[i].value / 5 * container[i].stack;
            }
            return count;
        default:
            CustomCurrencySystem system = Currencies.CurrencySystems(currency).System;
            long cap = system.CurrencyCap;
            system.SetCurrencyCap(long.MaxValue);
            count = system.CountCurrency(out _, container, ignoreSlots);
            system.SetCurrencyCap(cap);
            return count;
        }
    }

}