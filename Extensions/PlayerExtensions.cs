using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Terraria;

namespace SpikysLib.Extensions;

[Obsolete($"use {nameof(PlayerHelper)} instead", true)]
public static class PlayerExtensions {
    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.GetDropItem)} instead", true)]
    public static void GetDropItem(this Player player, ref Item item, GetItemSettings? settings = null) => PlayerHelper.GetDropItem(player, ref item, settings);

    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.RemoveFromInventory)} instead", true)]
    public static void RemoveFromInventory(this Player player, int type, int count = 1) => PlayerHelper.RemoveFromInventory(player, type, count);

    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.FindItemRaw)} instead", true)]
    public static Item? FindItemRaw(this Player player, int type) => PlayerHelper.FindItemRaw(player, type);

    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.Pick)} instead", true)]
    public static Item? Pick(this Player player, Predicate<Item> predicate) => PlayerHelper.Pick(player, predicate);
    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.PickPaint)} instead", true)]
    public static Item? PickPaint(this Player player) => PlayerHelper.PickPaint(player);
    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.PickBait)} instead", true)]
    public static Item? PickBait(this Player player) => PlayerHelper.PickBait(player);

    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.InChest)} instead", true)]
    public static bool InChest(this Player player, [MaybeNullWhen(false)] out Item[] chest) => PlayerHelper.InChest(player, out chest);
    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.Chest)} instead", true)]
    [return: NotNullIfNotNull("chest")]
    public static Item[]? Chest(this Player player, int? chest = null) => PlayerHelper.Chest(player);

    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.CountItems)} instead", true)]
    public static int CountItems(this Player player, int type, bool includeChest = false) => PlayerHelper.CountItems(player, type, includeChest);
    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.CountCurrency)} instead", true)]
    public static long CountCurrency(this Player player, int currency, bool includeBanks = true, bool includeChest = false) => PlayerHelper.CountCurrency(player, currency, includeBanks, includeChest);

    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.OwnedItems)} instead", true)]
    public static ReadOnlyDictionary<int, int> OwnedItems => PlayerHelper.OwnedItems;

    [Obsolete($"use {nameof(PlayerHelper)}.{nameof(PlayerHelper.InventoryContexts)} instead", true)]
    public static readonly int[] InventoryContexts = PlayerHelper.InventoryContexts;
}