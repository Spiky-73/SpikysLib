using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;
using SpikysLib.Collections;
using System;
using Terraria.ModLoader.IO;

namespace SpikysLib.CrossMod;

[JITWhenModsEnabled(ModName)]
public sealed class MagicStorageIntegration : ILoadable {
    public const string ModName = "MagicStorage";

    public static bool Enabled => ModLoader.HasMod(ModName);
    public static Version Version => ModLoader.GetMod(ModName).Version;
    public static bool InMagicStorage(Player player) => Main.worldID != 0 && player.GetModPlayer<MagicStorage.StoragePlayer>().GetStorageHeart() is not null;

    public void Load(Mod mod) {
        if (Enabled) ModLoader.GetMod(ModName).Call("Create Aggregator",
            mod, "UidAggregator",                     // Mod, Name
            (Item _) => false,                        // AppliesToItem
            null,                                     // CanAggregateItems
            null,                                     // SelectData
            (GlobalItem item, TagCompound tag) => { // SelectGlobalData
                if (item is ItemGuid) tag.Remove("guid");
            }
        );
    }

    public void Unload() { }

    [Obsolete("use CountItems(Player player, int type, int? prefix) instead"), MethodImpl(MethodImplOptions.NoInlining)] // v1.3
    public static int CountItems(int type, int? prefix = null) => CountItems(Main.LocalPlayer, type, prefix);
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int CountItems(Player player, int type, int? prefix = null) {
        if (!player.TryGetModPlayer(out MagicStorage.StoragePlayer? storagePlayer) || storagePlayer is null) return 0;
        var heart = storagePlayer.GetStorageHeart();
        if (heart is null) return 0;
        int count = 0;
        foreach (Item i in heart.GetStoredItems())
            if (i.type == type && (!prefix.HasValue || i.prefix == prefix)) count += i.stack;
        return count;
    }

    [Obsolete("use Contains(Player player, Item item) instead"), MethodImpl(MethodImplOptions.NoInlining)] // v1.3
    public static bool Contains(Item item) => Contains(Main.LocalPlayer, item);
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool Contains(Player player, Item item) {
        if (!player.TryGetModPlayer(out MagicStorage.StoragePlayer? storagePlayer) || storagePlayer is null) return false;
        var heart = storagePlayer.GetStorageHeart();
        if (heart is null) return false;
        return heart.GetStoredItems().Exist(i => i.type == item.type && i.prefix == item.prefix);
    }

    public static bool StackingFix => false;
}