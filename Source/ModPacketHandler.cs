using System.IO;
using SpikysLib.Configs;
using Terraria.ModLoader;

namespace SpikysLib;

public abstract class ModPacketHandler : ModType {
    public byte Type { get; internal set; }
    
    public ModPacket GetPacket() {
        var packet = Mod.GetPacket();
        packet.Write(Type);        
        return packet;
    }

    public abstract void Handle(BinaryReader reader, int fromWho);

    protected override void InitTemplateInstance() => ConfigHelper.SetInstance(this);
    protected sealed override void Register() {
        ModTypeLookup<ModPacketHandler>.Register(this);
        PacketHandlerLoader.Register(this);
    }
    public sealed override void SetupContent() => SetStaticDefaults();
    public override void Unload() => ConfigHelper.SetInstance(this, true);
}