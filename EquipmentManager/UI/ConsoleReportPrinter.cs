using EquipmentManager.Domain.Equipment;
using EquipmentManager.Domain.Rentals;
using EquipmentManager.Domain.Users;
using EquipmentManager.Services;

namespace EquipmentManager.UI;

public static class ConsoleReportPrinter
{
    public static void PrintHeader(string title)
    {
        Console.WriteLine($"\n=== {title.ToUpper()} ===");
    }

    public static void PrintUsers(IEnumerable<User> users)
    {
        var list = users.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("  (No users registered)");
            return;
        }
        foreach (var user in list)
        {
            Console.WriteLine($"  {user}");
        }
    }

    public static void PrintEquipmentList(IEnumerable<Equipment> equipmentList)
    {
        var list = equipmentList.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("  (No equipment found)");
            return;
        }
        foreach (var item in list)
        {
            Console.WriteLine($"  {item}");
        }
    }

    public static void PrintRentals(IEnumerable<Rental> rentals)
    {
        var list = rentals.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("  (None)");
            return;
        }
        foreach (var rental in list)
        {
            Console.WriteLine($"  {rental}");
        }
    }

    public static void PrintSummary(ServiceSummaryReport report)
    {
        Console.WriteLine($"  Total Registered Users     : {report.TotalUsers}");
        Console.WriteLine($"  Total Equipment Items      : {report.TotalEquipment}");
        Console.WriteLine($"    - Available              : {report.AvailableEquipment}");
        Console.WriteLine($"    - Currently Rented       : {report.RentedEquipment}");
        Console.WriteLine($"    - Unavailable/Maintenance: {report.UnavailableEquipment}");
        Console.WriteLine($"  Active Rentals             : {report.ActiveRentalsCount}");
        Console.WriteLine($"  Currently Overdue Rentals  : {report.OverdueActiveRentalsCount}");
        Console.WriteLine($"  Total Penalties Collected  : {report.TotalCollectedPenalties:C}");
    }
}