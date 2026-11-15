namespace EquipmentManager.Domain.Equipment;

public class Laptop : Equipment
{
    public int RamGb { get; set; }
    public string Processor { get; set; }

    public Laptop(string id, string name, string serialNumber, int ramGb, string processor)
        : base(id, name, serialNumber)
    {
        RamGb = ramGb;
        Processor = processor;
    }

    public override string GetSpecifications() => $"CPU: {Processor}, RAM: {RamGb}GB";
}