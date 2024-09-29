using System.Collections.Generic;
using System.IO;
using SpikysLib.Collections;
using Terraria.ModLoader;

namespace SpikysLib;


public static class PacketHandlerLoader {

    public static void Handle(Mod mod, BinaryReader reader, int fromWho) => s_handlers[mod][reader.ReadByte()-1].Handle(reader, fromWho);

    internal static void Register(ModPacketHandler handler) {
        List<ModPacketHandler> handlers = s_handlers.GetOrAdd(handler.Mod, _ => []);
        handlers.Add(handler);
        handler.Type = (byte)handlers.Count;
    }

    private static readonly Dictionary<Mod, List<ModPacketHandler>> s_handlers = [];
}