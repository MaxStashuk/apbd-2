using EquipmentManager.Domain.Users;

namespace EquipmentManager.Policies;

public class StandardUniversityRentalPolicy : IRentalPolicy
{
    private const int StudentLimit = 2;
    private const int EmployeeLimit = 5;
    private const decimal DailyLatePenaltyRate = 15.00m; // 15 PLN per overdue day

    public int GetMaxActiveRentals(UserType userType) => userType switch
    {
        UserType.Student => StudentLimit,
        UserType.Employee => EmployeeLimit,
        _ => throw new ArgumentOutOfRangeException(nameof(userType), "Unsupported user type.")
    };

    public decimal CalculateLatePenalty(DateTime dueDate, DateTime returnDate)
    {
        int overdueDays = (returnDate.Date - dueDate.Date).Days;
        return overdueDays > 0 ? overdueDays * DailyLatePenaltyRate : 0m;
    }
}