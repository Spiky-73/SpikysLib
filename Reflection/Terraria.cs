using System.Reflection;
using System.Collections.Generic;

using TRecipe = Terraria.Recipe;
using TCustomCurrencyManager = Terraria.GameContent.UI.CustomCurrencyManager;
using TCustomCurrencySystem = Terraria.GameContent.UI.CustomCurrencySystem;

namespace SpikysLib.Reflection;

public static class Main {
    public static readonly Assembly tModLoader = Assembly.Load("tModLoader");
}

public static class Recipe {
    public static readonly StaticField<Dictionary<int, int>> _ownedItems = new(typeof(TRecipe), nameof(_ownedItems));
}

public static class CustomCurrencyManager {
    public static readonly StaticField<Dictionary<int, TCustomCurrencySystem>> _currencies = new(typeof(TCustomCurrencyManager), "_currencies");
}

public static class CustomCurrencySystem {
    public static readonly Field<TCustomCurrencySystem, Dictionary<int, int>> _valuePerUnit = new("_valuePerUnit");
}