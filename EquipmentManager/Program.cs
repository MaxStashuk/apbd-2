using EquipmentManager.Policies;
using EquipmentManager.Services;
using EquipmentManager.UI;

namespace EquipmentManager;

class Program
{
    static void Main()
    {
        IRentalPolicy policy = new StandardUniversityRentalPolicy();
        IRentalService rentalService = new RentalService(policy);

        var menu = new ConsoleMenu(rentalService);

        // Run the mandatory 7-step demonstration scenario first (also seeds initial data)
        menu.RunAutomatedScenario();

        // Enter the interactive console menu loop
        menu.Run();
    }
}