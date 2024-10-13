using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace SpikysLib.Extensions;

[Obsolete($"use {nameof(SpikysLib)}.{nameof(global::SpikysLib.TooltipLineID)} instead", true)]
public enum TooltipLineID {
    ItemName,
    Favorite,
    FavoriteDesc,
    NoTransfer,
    Social,
    SocialDesc,
    Damage,
    CritChance,
    Speed,
    Knockback,
    FishingPower,
    NeedsBait,
    BaitPower,
    Equipable,
    WandConsumes,
    Quest,
    Vanity,
    VanityLegal,
    Defense,
    PickPower,
    AxePower,
    HammerPower,
    TileBoost,
    HealLife,
    HealMana,
    UseMana,
    Placeable,
    Ammo,
    Consumable,
    Material,
    Tooltip,
    EtherianManaWarning,
    WellFedExpert,
    BuffTime,
    OneDropLogo,
    PrefixDamage,
    PrefixSpeed,
    PrefixCritChance,
    PrefixUseMana,
    PrefixSize,
    PrefixShootSpeed,
    PrefixKnockback,
    PrefixAccDefense,
    PrefixAccMaxMana,
    PrefixAccCritChance,
    PrefixAccDamage,
    PrefixAccMoveSpeed,
    PrefixAccMeleeSpeed,
    SetBonus,
    Expert,
    Master,
    JourneyResearch,
    BestiaryNotes,
    SpecialPrice,
    Price,
    Modded
}

[Obsolete($"use {nameof(TooltipHelper)} instead", true)]
public static class TooltipsExtensions {
    public static TooltipLineID FromString(string? value) => (TooltipLineID)TooltipHelper.FromString(value);
    
    [Obsolete($"use {nameof(TooltipHelper)}.{nameof(TooltipHelper.FindLine)} instead", true)]
    public static TooltipLine? FindLine(this List<TooltipLine> tooltips, string name) => TooltipHelper.FindLine(tooltips, name);
    
    [Obsolete($"use {nameof(TooltipHelper)}.{nameof(TooltipHelper.AddLine)} instead", true)]
    public static TooltipLine AddLine(this List<TooltipLine> tooltips, TooltipLine line, TooltipLineID? after = null) => TooltipHelper.AddLine(tooltips, line, (global::SpikysLib.TooltipLineID?)after);
    
    [Obsolete($"use {nameof(TooltipHelper)}.{nameof(TooltipHelper.FindOrAddLine)} instead", true)]
    public static TooltipLine FindorAddLine(this List<TooltipLine> tooltips, TooltipLine line, out bool addedLine, TooltipLineID? after = null) => TooltipHelper.FindOrAddLine(tooltips, line, out addedLine, (global::SpikysLib.TooltipLineID?)after);
    [Obsolete($"use {nameof(TooltipHelper)}.{nameof(TooltipHelper.FindOrAddLine)} instead", true)]
    public static TooltipLine FindorAddLine(this List<TooltipLine> tooltips, TooltipLine line, TooltipLineID after = TooltipLineID.Modded) => TooltipHelper.FindOrAddLine(tooltips, line, (global::SpikysLib.TooltipLineID)after);
}
