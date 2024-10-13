using System;
using Microsoft.Xna.Framework;

namespace SpikysLib.Extensions;

[Obsolete($"use {nameof(GraphicsHelper)} instead", true)]
public static class GraphicsExtensions {
    [Obsolete($"use {nameof(GraphicsHelper)}.{nameof(GraphicsHelper.ApplyRGB)} instead", true)]
    public static void ApplyRGB(ref this Color color, float mult) => GraphicsHelper.ApplyRGB(ref color, mult);
}