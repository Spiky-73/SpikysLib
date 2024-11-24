using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.UI;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SpikysLib;

public static class CurrencyHelper {
    public const int None = -2;
    public const int Coins = -1;

    public static bool IsPartOfACurrency(int item, out int currency) => (currency = CurrencyType(item)) != None;
    public static int CurrencyType(int item) {
        if (CoinValues.ContainsKey(item)) return Coins;
        foreach ((int key, CustomCurrencySystem system) in CustomCurrencies()) {
            if (system.ValuePerUnit().ContainsKey(item)) return key;
        }
        return None;
    }
    public static int CurrencyValue(int item) => CurrencyType(item) switch {
        None => 0,
        Coins => CoinValues[item],
        int t => CustomCurrencyManager.TryGetCurrencySystem(t, out var system) ? system.ValuePerUnit(item) : 0
    };

    public static int LowestValueType(int currency) => currency switch {
        None => ItemID.None,
        Coins => ItemID.CopperCoin,
        _ => CustomCurrencyManager.TryGetCurrencySystem(currency, out CustomCurrencySystem system) ? system.ValuePerUnit().MinBy(i => i.Value).Key : 0
    };

    public static List<KeyValuePair<int, int>> CurrencyCountToItems(int currency, long amount) {
        List<KeyValuePair<int, int>> values = [];
        switch (currency) {
        case None: return [];
        case Coins:
            values = new(CoinValues);
            break;
        default:
            values = CustomCurrencyManager.TryGetCurrencySystem(currency, out CustomCurrencySystem system) ? new(system.ValuePerUnit()) : [];
            break;
        }
        values.Sort((a, b) => -a.Value.CompareTo(b.Value));

        List<KeyValuePair<int, int>> stacks = [];
        foreach (var coin in values) {
            int count = (int)(amount / coin.Value);
            if (count == 0) continue;
            amount -= count * coin.Value;
            stacks.Add(new(coin.Key, count));
        }
        return stacks;
    }

    public static string PriceText(int currency, long price) {
        if (price == 0 || currency == None) return string.Empty;

        switch (currency) {
        case Coins:
            List<KeyValuePair<int, int>> coins = CurrencyCountToItems(currency, price);
            List<string> parts = [];
            foreach (KeyValuePair<int, int> coin in coins) parts.Add($"{coin.Value} {Lang.inter[18 - coin.Key + ItemID.CopperCoin].Value}");
            return $"[c/{(CoinColors[coins[0].Key]*(Main.mouseTextColor/255f)).Hex3()}:{string.Join(' ', parts)}]";
        default:
            string[] lines = [string.Empty];
            int num = 0;
            CustomCurrencyManager.GetPriceText(currency, lines, ref num, price);
            lines[0] = lines[0].Replace(Lang.tip[50] + " ", string.Empty);
            return lines[0];
        }
    }

    public static Dictionary<int, CustomCurrencySystem> CustomCurrencies() => Reflection.CustomCurrencyManager._currencies.GetValue();
    public static Dictionary<int, int> ValuePerUnit(this CustomCurrencySystem system) => Reflection.CustomCurrencySystem._valuePerUnit.GetValue(system);
    public static int ValuePerUnit(this CustomCurrencySystem system, int type) => Reflection.CustomCurrencySystem._valuePerUnit.GetValue(system)[type];

    public static readonly Dictionary<int, int> CoinValues = new() {
        {ItemID.CopperCoin,   1},
        {ItemID.SilverCoin,   100},
        {ItemID.GoldCoin,     10000},
        {ItemID.PlatinumCoin, 1000000},
    };
    public static readonly Dictionary<int, Color> CoinColors = new() {
        {ItemID.CopperCoin,   Colors.CoinCopper},
        {ItemID.SilverCoin,   Colors.CoinSilver},
        {ItemID.GoldCoin,     Colors.CoinGold},
        {ItemID.PlatinumCoin, Colors.CoinPlatinum},
    };
}