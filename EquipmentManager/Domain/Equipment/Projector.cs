namespace EquipmentManager.Domain.Equipment;

public class Projector : Equipment
{
    public int Lumens { get; set; }
    public string NativeResolution { get; set; }

    public Projector(string id, string name, string serialNumber, int lumens, string nativeResolution)
        : base(id, name, serialNumber)
    {
        Lumens = lumens;
        NativeResolution = nativeResolution;
    }

    public override string GetSpecifications() => $"Brightness: {Lumens} ANSI Lumens, Res: {NativeResolution}";
}