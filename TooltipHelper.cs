﻿using System.Collections.Generic;
using Terraria.ModLoader;

namespace SpikysLib;

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

public static class TooltipHelper {

    public static TooltipLineID FromString(string? value) {
        if(value is null) return TooltipLineID.Modded;
        if (value.StartsWith("Tooltip")) return TooltipLineID.Tooltip;
        if (System.Enum.TryParse(value, out TooltipLineID id)) return id;
        return TooltipLineID.Modded;
    }

    public static TooltipLine? FindLine(this List<TooltipLine> tooltips, TooltipLineID line) => tooltips.FindLine(line.ToString());
    public static TooltipLine? FindLine(this List<TooltipLine> tooltips, string name) => tooltips.Find(l => l.Name == name);
    public static void AddLine(this List<TooltipLine> tooltips, TooltipLine line, TooltipLineID? after = null) {
        after ??= FromString(line.Name);
        for (int i = 0; i < tooltips.Count; i++) {
            TooltipLineID lookingAt = FromString(tooltips[i].Name);
            if (lookingAt <= after) continue;
            tooltips.Insert(i, line);
            return;
        }
        tooltips.Add(line);
    }

    public static TooltipLine FindOrAddLine(this List<TooltipLine> tooltips, TooltipLine line, TooltipLineID? after = null) => FindOrAddLine(tooltips, line, out _, after);
    public static TooltipLine FindOrAddLine(this List<TooltipLine> tooltips, TooltipLine line, out bool addedLine, TooltipLineID? after = null) {
        TooltipLine? target = tooltips.FindLine(line.Name);
        if (addedLine = target is not null) return target!;
        tooltips.AddLine(line, after);
        return line!;
    }
}
