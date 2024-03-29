using Microsoft.Xna.Framework;

namespace SpikysLib.Extensions;

public static class GraphicsExtensions {

    public static void ApplyRGB(ref this Color color, float mult) {
        color.R = (byte)(color.R * mult);
        color.G = (byte)(color.G * mult);
        color.B = (byte)(color.B * mult);
    }

}