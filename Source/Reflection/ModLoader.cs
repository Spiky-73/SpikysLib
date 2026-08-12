using System;

using TItem = Terraria.Item;
using TItemLoader = Terraria.ModLoader.ItemLoader;

namespace SpikysLib.Reflection;

[Obsolete("use an Assembly publiciser instead", true)] // v1.4
public static class ItemLoader {
    public delegate TItem TransferWithLimitFn(TItem source, int limit);
    public static readonly StaticMethod<TItem> TransferWithLimit = new(typeof(TItemLoader), nameof(TItemLoader.TransferWithLimit), typeof(TItem), typeof(int));
}