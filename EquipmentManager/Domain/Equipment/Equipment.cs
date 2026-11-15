namespace EquipmentManager.Domain.Equipment;

public abstract class Equipment
{
    public string Id { get; }
    public string Name { get; set; }
    public string SerialNumber { get; set; }
    public EquipmentStatus Status { get; set; }

    protected Equipment(string id, string name, string serialNumber)
    {
        Id = id;
        Name = name;
        SerialNumber = serialNumber;
        Status = EquipmentStatus.Available;
    }

    public abstract string GetSpecifications();

    public override string ToString() =>
        $"[{Id}] {Name} (S/N: {SerialNumber}) | Status: {Status} | Specs: {GetSpecifications()}";
}