using EquipmentManager.Domain.Users;

namespace EquipmentManager.Policies;

public interface IRentalPolicy
{
    int GetMaxActiveRentals(UserType userType);
    decimal CalculateLatePenalty(DateTime dueDate, DateTime returnDate);
}