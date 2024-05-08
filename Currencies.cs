using System.Reflection;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.UI;

namespace SpikysLib;

public readonly record struct CustomCurrencyData(CustomCurrencySystem System, Dictionary<int, int> Values);

public static class Currencies {

    public const int None = -2;
    public const int Coins = -1;

    public static int LowestValueType(int currency) {
        if (currency == None) return ItemID.None;
        if (currency == Coins) return ItemID.CopperCoin;

        int minType = 0;
        int minValue = 0;
        foreach ((int key, int value) in CurrencySystems(currency).Values) {
            if (minValue == 0 || value < minValue) (minType, minValue) = (key, value);
        }
        return minType;
    }

    public static List<KeyValuePair<int, long>> CurrencyCountToItems(int currency, long amount) {
        List<KeyValuePair<int, int>> values = new();
        switch (currency) {
        case None: return new();
        case Coins:
            values = new() {
                        new(ItemID.PlatinumCoin, 1000000),
                        new(ItemID.GoldCoin, 10000),
                        new(ItemID.SilverCoin, 100),
                        new(ItemID.CopperCoin, 1)
                    };
            break;
        default:
            foreach (var v in _customCurrencies[currency].Values) values.Add(v);
            values.Sort((a, b) => a.Value < b.Value ? 0 : 1);
            break;
        }

        List<KeyValuePair<int, long>> stacks = new();
        foreach (var coin in values) {
            int count = (int)(amount / coin.Value);
            if (count == 0) continue;
            amount -= count * coin.Value;
            stacks.Add(new(coin.Key, count));
        }
        return stacks;
    }


    public static string PriceText(int currency, long count) {
        if (count == 0 || currency == None) return string.Empty;

        List<KeyValuePair<int, long>> coins = CurrencyCountToItems(currency, count);
        List<string> parts = new();
        switch (currency) {
        case Coins:
            foreach (KeyValuePair<int, long> coin in coins) parts.Add($"{coin.Value} {Lang.inter[18 - coin.Key + ItemID.CopperCoin].Value}");
            break;
        default:
            foreach (KeyValuePair<int, long> coin in coins) parts.Add($"{coin.Value} {Lang.GetItemNameValue(coin.Key)}");
            break;
        }
        return string.Join(' ', parts);
    }


    public static CustomCurrencyData CurrencySystems(int currency) => _customCurrencies[currency];
    public static List<int> CustomCurrencies => new(_customCurrencies.Keys);


    internal static void GetCurrencies() {
        FieldInfo curField = typeof(CustomCurrencyManager).GetField("_currencies", BindingFlags.NonPublic | BindingFlags.Static)!;
        FieldInfo valuesField = typeof(CustomCurrencySystem).GetField("_valuePerUnit", BindingFlags.NonPublic | BindingFlags.Instance)!;
        Dictionary<int, CustomCurrencySystem> currencies = (Dictionary<int, CustomCurrencySystem>)curField.GetValue(null)!;
        _customCurrencies = new();
        foreach (var (key, system) in currencies) _customCurrencies[key] = new(system, (Dictionary<int, int>)valuesField.GetValue(system)!);
    }
    internal static void ClearCurrencies() => _customCurrencies = null!;

    private static Dictionary<int, CustomCurrencyData> _customCurrencies = null!;
}