using System.Collections.Generic;
using Terraria.GameContent.UI;
using System;
using System.Linq;

namespace SpikysLib;

public readonly record struct CustomCurrencyData(CustomCurrencySystem System, Dictionary<int, int> Values) {}

[Obsolete($"use {nameof(CurrencyHelper)} instead", true)]
public static class Currencies {
    public const int None = CurrencyHelper.None;
    public const int Coins = CurrencyHelper.Coins;

    public static int LowestValueType(int currency) => CurrencyHelper.LowestValueType(currency);

    public static List<KeyValuePair<int, long>> CurrencyCountToItems(int currency, long amount) => CurrencyHelper.CurrencyCountToItems(currency, amount).Select(i => new KeyValuePair<int, long>(i.Key, i.Value)).ToList();

    public static string PriceText(int currency, long count) => CurrencyHelper.PriceText(currency, count);

    [Obsolete($"use {nameof(CustomCurrencyManager)}.{nameof(CustomCurrencyManager.TryGetCurrencySystem)} instead", true)]
    public static CustomCurrencyData CurrencySystems(int currency) => CustomCurrencyManager.TryGetCurrencySystem(currency, out CustomCurrencySystem system) ? new(system, system.ValuePerUnit()) : throw new KeyNotFoundException();

    public static List<int> CustomCurrencies => new(Reflection.CustomCurrencyManager._currencies.GetValue().Keys);
}