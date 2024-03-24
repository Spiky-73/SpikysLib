using System.Reflection;
using TRecipe = Terraria.Recipe;

using System.Collections.Generic;

namespace SpikysLib.Reflection;

public static class Main {
    public static readonly Assembly tModLoader = Assembly.Load("tModLoader");
}

public static class Recipe {
    public static readonly StaticField<Dictionary<int, int>> _ownedItems = new(typeof(TRecipe), nameof(_ownedItems));
}