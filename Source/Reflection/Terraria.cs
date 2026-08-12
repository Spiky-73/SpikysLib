using System.Reflection;
using System.Collections.Generic;

using TPlayer = Terraria.Player;
using TMod = Terraria.ModLoader.Mod;
using TRecipe = Terraria.Recipe;
using TCustomCurrencyManager = Terraria.GameContent.UI.CustomCurrencyManager;
using TCustomCurrencySystem = Terraria.GameContent.UI.CustomCurrencySystem;
using System;

namespace SpikysLib.Reflection;

[Obsolete("use an Assembly publiciser instead", true)] // v1.4
public static class Main {
    public static readonly Assembly tModLoader = Assembly.Load("tModLoader");
}

[Obsolete("use an Assembly publiciser instead", true)] // v1.4
public static class Mod {
    public static readonly Method<TMod, object?> AutoloadConfig = new(nameof(AutoloadConfig));
}

[Obsolete("use an Assembly publiciser instead", true)] // v1.4
public static class Recipe {
    public static readonly StaticField<Dictionary<int, int>> _ownedItems = new(typeof(TRecipe), nameof(_ownedItems));
    public static readonly StaticMethod<object?> CollectItemsToCraftWithFrom = new(typeof(TRecipe), nameof(CollectItemsToCraftWithFrom), typeof(TPlayer));
}

[Obsolete("use an Assembly publiciser instead", true)] // v1.4
public static class CustomCurrencyManager {
    public static readonly StaticField<Dictionary<int, TCustomCurrencySystem>> _currencies = new(typeof(TCustomCurrencyManager), "_currencies");
}

[Obsolete("use an Assembly publiciser instead", true)] // v1.4
public static class CustomCurrencySystem {
    public static readonly Field<TCustomCurrencySystem, Dictionary<int, int>> _valuePerUnit = new("_valuePerUnit");
}