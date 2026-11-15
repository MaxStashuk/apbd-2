namespace EquipmentManager.Domain.Equipment;

public class Camera : Equipment
{
    public double Megapixels { get; set; }
    public bool IncludesLensKit { get; set; }

    public Camera(string id, string name, string serialNumber, double megapixels, bool includesLensKit)
        : base(id, name, serialNumber)
    {
        Megapixels = megapixels;
        IncludesLensKit = includesLensKit;
    }

    public override string GetSpecifications() => $"{Megapixels}MP, Lens Kit: {(IncludesLensKit ? "Yes" : "No")}";
}