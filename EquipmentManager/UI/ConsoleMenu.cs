using EquipmentManager.Domain.Exceptions;
using EquipmentManager.Domain.Users;
using EquipmentManager.Services;

namespace EquipmentManager.UI;

public class ConsoleMenu
{
    private readonly IRentalService _rentalService;
    private DateTime _simulatedDate = DateTime.Today;

    public ConsoleMenu(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    public void Run()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n=======================================================");
            Console.WriteLine($" UNIVERSITY EQUIPMENT RENTAL SYSTEM (Date: {_simulatedDate:yyyy-MM-dd})");
            Console.WriteLine("=======================================================");
            Console.WriteLine(" 1. Add a new user");
            Console.WriteLine(" 2. List all users");
            Console.WriteLine(" 3. Add new equipment (Laptop / Projector / Camera)");
            Console.WriteLine(" 4. Remove equipment");
            Console.WriteLine(" 5. Display ALL equipment with status");
            Console.WriteLine(" 6. Display AVAILABLE equipment only");
            Console.WriteLine(" 7. Rent equipment to a user");
            Console.WriteLine(" 8. Return equipment (calculates late penalty)");
            Console.WriteLine(" 9. Mark equipment as unavailable (maintenance/damage)");
            Console.WriteLine("10. Display active rentals for a selected user");
            Console.WriteLine("11. Display overdue rentals");
            Console.WriteLine("12. Generate summary report");
            Console.WriteLine("13. Advance simulated date (to test overdue/penalties)");
            Console.WriteLine("14. Run automated 7-step assignment demonstration");
            Console.WriteLine(" 0. Exit");
            Console.WriteLine("-------------------------------------------------------");
            Console.Write("Select an option: ");

            string? choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1": AddUserInteractive(); break;
                    case "2":
                        ConsoleReportPrinter.PrintHeader("Registered Users");
                        ConsoleReportPrinter.PrintUsers(_rentalService.GetAllUsers());
                        break;
                    case "3": AddEquipmentInteractive(); break;
                    case "4": RemoveEquipmentInteractive(); break;
                    case "5":
                        ConsoleReportPrinter.PrintHeader("All Equipment");
                        ConsoleReportPrinter.PrintEquipmentList(_rentalService.GetAllEquipment());
                        break;
                    case "6":
                        ConsoleReportPrinter.PrintHeader("Available Equipment");
                        ConsoleReportPrinter.PrintEquipmentList(_rentalService.GetAvailableEquipment());
                        break;
                    case "7": RentEquipmentInteractive(); break;
                    case "8": ReturnEquipmentInteractive(); break;
                    case "9": MarkUnavailableInteractive(); break;
                    case "10": ShowUserRentalsInteractive(); break;
                    case "11":
                        ConsoleReportPrinter.PrintHeader($"Overdue Rentals (as of {_simulatedDate:yyyy-MM-dd})");
                        ConsoleReportPrinter.PrintRentals(_rentalService.GetOverdueRentals(_simulatedDate));
                        break;
                    case "12":
                        ConsoleReportPrinter.PrintHeader("System Summary Report");
                        ConsoleReportPrinter.PrintSummary(_rentalService.GenerateSummaryReport(_simulatedDate));
                        break;
                    case "13": AdvanceDateInteractive(); break;
                    case "14": RunAutomatedScenario(); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("  [!] Invalid option. Please choose 0-14."); break;
                }
            }
            catch (RentalDomainException ex)
            {
                Console.WriteLine($"\n  [BUSINESS RULE ERROR]: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n  [INPUT ERROR]: {ex.Message}");
            }
        }
    }

    private void AddUserInteractive()
    {
        ConsoleReportPrinter.PrintHeader("Add New User");
        string firstName = ReadRequiredString("First Name: ");
        string lastName = ReadRequiredString("Last Name: ");
        Console.Write("User Type (1 = Student, 2 = Employee): ");
        string typeChoice = Console.ReadLine()?.Trim() ?? "";

        UserType userType = typeChoice switch
        {
            "1" => UserType.Student,
            "2" => UserType.Employee,
            _ => throw new RentalDomainException("Invalid user type selection. Choose 1 or 2.")
        };

        var user = _rentalService.RegisterUser(firstName, lastName, userType);
        Console.WriteLine($"  [SUCCESS] Registered {user}");
    }

    private void AddEquipmentInteractive()
    {
        ConsoleReportPrinter.PrintHeader("Add New Equipment");
        Console.WriteLine("Select Equipment Type: 1 = Laptop, 2 = Projector, 3 = Camera");
        Console.Write("Choice: ");
        string typeChoice = Console.ReadLine()?.Trim() ?? "";

        string name = ReadRequiredString("Model/Name: ");
        string serial = ReadRequiredString("Serial Number: ");

        switch (typeChoice)
        {
            case "1":
                int ram = ReadInt("RAM (GB): ");
                string cpu = ReadRequiredString("Processor: ");
                var laptop = _rentalService.AddLaptop(name, serial, ram, cpu);
                Console.WriteLine($"  [SUCCESS] Added {laptop}");
                break;
            case "2":
                int lumens = ReadInt("Brightness (ANSI Lumens): ");
                string res = ReadRequiredString("Native Resolution (e.g. 1920x1080): ");
                var projector = _rentalService.AddProjector(name, serial, lumens, res);
                Console.WriteLine($"  [SUCCESS] Added {projector}");
                break;
            case "3":
                double mp = ReadDouble("Megapixels: ");
                Console.Write("Includes Lens Kit? (y/n): ");
                bool lens = (Console.ReadLine()?.Trim().ToLower() == "y");
                var camera = _rentalService.AddCamera(name, serial, mp, lens);
                Console.WriteLine($"  [SUCCESS] Added {camera}");
                break;
            default:
                throw new RentalDomainException("Invalid equipment type selected.");
        }
    }

    private void RemoveEquipmentInteractive()
    {
        ConsoleReportPrinter.PrintHeader("Remove Equipment");
        ConsoleReportPrinter.PrintEquipmentList(_rentalService.GetAllEquipment());
        string eqId = ReadRequiredString("Enter Equipment ID to remove (e.g. EQ-001): ");
        _rentalService.RemoveEquipment(eqId);
        Console.WriteLine($"  [SUCCESS] Equipment {eqId.ToUpper()} removed.");
    }

    private void RentEquipmentInteractive()
    {
        ConsoleReportPrinter.PrintHeader("Rent Equipment");
        Console.WriteLine("Registered Users:");
        ConsoleReportPrinter.PrintUsers(_rentalService.GetAllUsers());
        Console.WriteLine("\nAvailable Equipment:");
        ConsoleReportPrinter.PrintEquipmentList(_rentalService.GetAvailableEquipment());

        string userId = ReadRequiredString("\nEnter User ID (e.g. USR-001): ");
        string eqId = ReadRequiredString("Enter Equipment ID (e.g. EQ-001): ");
        int days = ReadInt("Rental duration (days): ");

        var rental = _rentalService.RentEquipment(userId, eqId, _simulatedDate, days);
        Console.WriteLine($"  [SUCCESS] Created rental: {rental}");
    }

    private void ReturnEquipmentInteractive()
    {
        ConsoleReportPrinter.PrintHeader("Return Equipment");
        var activeRentals = _rentalService.GetAllRentals().Where(r => r.IsActive).ToList();
        ConsoleReportPrinter.PrintRentals(activeRentals);

        if (activeRentals.Count == 0) return;

        string rentalId = ReadRequiredString("\nEnter Rental ID to return (e.g. RNT-001): ");
        var completed = _rentalService.ReturnEquipment(rentalId, _simulatedDate);
        Console.WriteLine($"  [SUCCESS] Returned: {completed}");
    }

    private void MarkUnavailableInteractive()
    {
        ConsoleReportPrinter.PrintHeader("Mark Equipment Unavailable");
        ConsoleReportPrinter.PrintEquipmentList(_rentalService.GetAllEquipment());
        string eqId = ReadRequiredString("\nEnter Equipment ID (e.g. EQ-001): ");
        _rentalService.MarkEquipmentUnavailable(eqId);
        Console.WriteLine($"  [SUCCESS] Marked {eqId.ToUpper()} as Unavailable.");
    }

    private void ShowUserRentalsInteractive()
    {
        ConsoleReportPrinter.PrintHeader("Active Rentals For User");
        ConsoleReportPrinter.PrintUsers(_rentalService.GetAllUsers());
        string userId = ReadRequiredString("\nEnter User ID (e.g. USR-001): ");
        var rentals = _rentalService.GetActiveRentalsForUser(userId);
        ConsoleReportPrinter.PrintRentals(rentals);
    }

    private void AdvanceDateInteractive()
    {
        int days = ReadInt("Enter number of days to advance (can be negative to go back): ");
        _simulatedDate = _simulatedDate.AddDays(days);
        Console.WriteLine($"  [SUCCESS] Current simulated date is now {_simulatedDate:yyyy-MM-dd}");
    }

    public void RunAutomatedScenario()
    {
        ConsoleReportPrinter.PrintHeader("Running 7-Step Demonstration Scenario");

        // 1. Adding several equipment items of different types
        ConsoleReportPrinter.PrintHeader("Step 1: Adding Equipment Items");
        var laptop1 = _rentalService.AddLaptop("Dell Latitude 5540", "SN-DL-101", 16, "Intel i7-1355U");
        var laptop2 = _rentalService.AddLaptop("MacBook Pro 14", "SN-MB-102", 32, "Apple M3 Pro");
        var projector1 = _rentalService.AddProjector("Epson EB-FH06", "SN-EP-201", 3500, "1920x1080");
        var camera1 = _rentalService.AddCamera("Sony Alpha a7 IV", "SN-SN-301", 33.0, true);
        var camera2 = _rentalService.AddCamera("Canon EOS R6", "SN-CN-302", 20.1, false);

        _rentalService.MarkEquipmentUnavailable(camera2.Id);
        ConsoleReportPrinter.PrintEquipmentList(_rentalService.GetAllEquipment());

        // 2. Adding several users of different types
        ConsoleReportPrinter.PrintHeader("Step 2: Adding Users");
        var student = _rentalService.RegisterUser("Jan", "Kowalski", UserType.Student);
        var employee = _rentalService.RegisterUser("Anna", "Nowak", UserType.Employee);
        ConsoleReportPrinter.PrintUsers(new[] { student, employee });

        // 3. A correct rental operation
        ConsoleReportPrinter.PrintHeader("Step 3: Performing Valid Rentals");
        var rental1 = _rentalService.RentEquipment(student.Id, laptop1.Id, _simulatedDate, durationDays: 7);
        var rental2 = _rentalService.RentEquipment(student.Id, projector1.Id, _simulatedDate, durationDays: 3);
        var rental3 = _rentalService.RentEquipment(employee.Id, laptop2.Id, _simulatedDate, durationDays: 5);
        ConsoleReportPrinter.PrintRentals(new[] { rental1, rental2, rental3 });

        // 4. Attempted invalid operations
        ConsoleReportPrinter.PrintHeader("Step 4: Attempting Invalid Operations");
        try
        {
            Console.WriteLine("  -> Attempting 3rd rental for Student (Limit is 2)...");
            _rentalService.RentEquipment(student.Id, camera1.Id, _simulatedDate, durationDays: 2);
        }
        catch (RentalDomainException ex)
        {
            Console.WriteLine($"  [BLOCKED BY BUSINESS RULE]: {ex.Message}");
        }

        try
        {
            Console.WriteLine("  -> Attempting to rent equipment marked as Unavailable...");
            _rentalService.RentEquipment(employee.Id, camera2.Id, _simulatedDate, durationDays: 2);
        }
        catch (RentalDomainException ex)
        {
            Console.WriteLine($"  [BLOCKED BY BUSINESS RULE]: {ex.Message}");
        }

        // 5. A return completed on time
        ConsoleReportPrinter.PrintHeader("Step 5: Return Completed On Time");
        var onTimeReturn = _rentalService.ReturnEquipment(rental1.Id, _simulatedDate.AddDays(5));
        Console.WriteLine($"  {onTimeReturn}");

        // 6. A delayed return that leads to a penalty
        ConsoleReportPrinter.PrintHeader("Step 6: Delayed Return With Penalty");
        _simulatedDate = _simulatedDate.AddDays(10);
        Console.WriteLine($"  Simulated date advanced to {_simulatedDate:yyyy-MM-dd}. Overdue rentals:");
        ConsoleReportPrinter.PrintRentals(_rentalService.GetOverdueRentals(_simulatedDate));

        var lateReturn = _rentalService.ReturnEquipment(rental2.Id, _simulatedDate);
        Console.WriteLine($"  Returned late: {lateReturn}");

        // 7. Displaying a final report of the system state
        ConsoleReportPrinter.PrintHeader("Step 7: Final System State Report");
        ConsoleReportPrinter.PrintSummary(_rentalService.GenerateSummaryReport(_simulatedDate));
    }

    private static string ReadRequiredString(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
            throw new RentalDomainException("Value cannot be empty.");
        return input;
    }

    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int value))
            throw new RentalDomainException("Please enter a valid integer number.");
        return value;
    }

    private static double ReadDouble(string prompt)
    {
        Console.Write(prompt);
        if (!double.TryParse(Console.ReadLine()?.Trim(), out double value))
            throw new RentalDomainException("Please enter a valid number.");
        return value;
    }
}