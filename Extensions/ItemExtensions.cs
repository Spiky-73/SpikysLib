using System;
using Terraria;

namespace SpikysLib.Extensions;

[Obsolete($"use {nameof(ItemHelper)} instead", true)]
public static class ItemExtensions {
    [Obsolete($"use {nameof(ItemHelper)}.{nameof(ItemHelper.MoveInto)} instead", true)]
    public static Item MoveInto(Item item, Item toMove, out int transferred, int? maxStack = null, bool canFavorite = true) => ItemHelper.MoveInto(item, toMove, out transferred, maxStack, canFavorite);

    [Obsolete($"use {nameof(ItemHelper)}.{nameof(ItemHelper.RunWithHiddenItems)} instead", true)]
    public static void RunWithHiddenItems(Item[] chest, Action action, Predicate<Item> hidden) => ItemHelper.RunWithHiddenItems(chest, action, hidden);

    [Obsolete($"use {nameof(ItemHelper)}.{nameof(ItemHelper.CountItems)} instead", true)]
    public static int CountItems(this Item[] container, int type, params int[] ignoreSots) => ItemHelper.CountItems(container, type, ignoreSots);
    
    [Obsolete($"use {nameof(ItemHelper)}.{nameof(ItemHelper.CurrencyType)} instead", true)]
    public static int CurrencyType(this Item item) => ItemHelper.CurrencyType(item);
    [Obsolete($"use {nameof(ItemHelper)}.{nameof(ItemHelper.IsPartOfACurrency)} instead", true)]
    public static bool IsPartOfACurrency(this Item item, out int currency) => ItemHelper.IsPartOfACurrency(item, out currency);
    [Obsolete($"use {nameof(ItemHelper)}.{nameof(ItemHelper.CurrencyValue)} instead", true)]
    public static long CurrencyValue(this Item item) => ItemHelper.CurrencyValue(item);
    
    [Obsolete($"use {nameof(ItemHelper)}.{nameof(ItemHelper.CountCurrency)} instead", true)]
    public static long CountCurrency(this Item[] container, int currency, params int[] ignoreSlots) => ItemHelper.CountCurrency(container, currency, ignoreSlots);
}