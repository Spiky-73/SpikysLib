using SpikysLib.DataStructures;

namespace SpikysLib.Constants;

public readonly record struct InventorySection(int Start, int Count);

public static class InventorySlots {
    public const int Count = 58;
    public static readonly Range Hotbar = Range.FromCount(0, 10);
    public static readonly Range Items = Range.FromCount(0, 50);
    public static readonly Range Coins = Range.FromCount(Items.End, 4);
    public static readonly Range Ammo = Range.FromCount(Coins.End, 4);
    public const int Mouse = Count;

    public const int PiggyBank = -2;
    public const int Safe = -3;
    public const int DefendersForge = -4;
    public const int VoidBag = -5;
}

public static class ArmorSlots {
    public const int Count = 3;
    public const int Head = 0;
    public const int Body = 1;
    public const int Leg = 2;
}

public static class EquipmentSlots {
    public const int Count = 5;
    public const int Pet = 0;
    public const int LightPet = 1; 
    public const int Minecart = 2; 
    public const int Mount = 3; 
    public const int Grapple = 4; 
}