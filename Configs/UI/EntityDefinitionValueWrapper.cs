using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Configs.UI;

public class EntityDefinitionValueWrapper<TDefinition, T> : KeyValueWrapper<TDefinition, T> where TDefinition : EntityDefinition<TDefinition>, new() where T: new() {
    public override void OnBind(ConfigElement element) => Reflection.ConfigElement.TooltipFunction.SetValue(element, () => Key.Tooltip ?? string.Empty);
}