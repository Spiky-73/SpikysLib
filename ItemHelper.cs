using System;
using System.Collections.Generic;
using Terraria;
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


}