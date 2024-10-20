using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.UI;
using System.Linq;

namespace SpikysLib;

// TODO look into CustomCurrencySystem to reuse stuff
public static class CurrencyHelper {
    public const int None = -2;
    public const int Coins = -1;

    public static bool IsPartOfACurrency(int item, out int currency) => new Item(item).IsPartOfACurrency(out currency);

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
            values = [
                new(ItemID.PlatinumCoin, 1000000),
                new(ItemID.GoldCoin, 10000),
                new(ItemID.SilverCoin, 100),
                new(ItemID.CopperCoin, 1)
            ];
            break;
        default:
            Dictionary<int, int> valuesPerUnit = CustomCurrencyManager.TryGetCurrencySystem(currency, out CustomCurrencySystem system) ? system.ValuePerUnit() : [];
            foreach (var v in valuesPerUnit) values.Add(v);
            values.Sort((a, b) => a.Value < b.Value ? 0 : 1);
            break;
        }

        List<KeyValuePair<int, int>> stacks = [];
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

        List<KeyValuePair<int, int>> coins = CurrencyCountToItems(currency, count);
        List<string> parts = [];
        switch (currency) {
        case Coins:
            foreach (KeyValuePair<int, int> coin in coins) parts.Add($"{coin.Value} {Lang.inter[18 - coin.Key + ItemID.CopperCoin].Value}");
            break;
        default:
            foreach (KeyValuePair<int, int> coin in coins) parts.Add($"{coin.Value} {Lang.GetItemNameValue(coin.Key)}");
            break;
        }
        return string.Join(' ', parts);
    }

    public static Dictionary<int, CustomCurrencySystem> CustomCurrencies() => Reflection.CustomCurrencyManager._currencies.GetValue();
    public static Dictionary<int, int> ValuePerUnit(this CustomCurrencySystem system) => Reflection.CustomCurrencySystem._valuePerUnit.GetValue(system);
    public static int ValuePerUnit(this CustomCurrencySystem system, int type) => Reflection.CustomCurrencySystem._valuePerUnit.GetValue(system)[type];
}