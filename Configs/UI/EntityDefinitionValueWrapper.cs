using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Configs.UI;

public class EntityDefinitionValueWrapper<TDefinition, T> : ValueWrapper<TDefinition, T> where TDefinition : EntityDefinition<TDefinition> {
    public override T Value { get; set; } = default!;
    public override void OnBind(ConfigElement element) => Reflection.ConfigElement.TooltipFunction.SetValue(element, () => Key.Tooltip ?? string.Empty);
}