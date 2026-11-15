namespace EquipmentManager.Domain.Exceptions;

public class RentalDomainException : Exception
{
    public RentalDomainException(string message) : base(message) { }
}