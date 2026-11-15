using EquipmentManager.Domain.Equipment;
using EquipmentManager.Domain.Rentals;
using EquipmentManager.Domain.Users;

namespace EquipmentManager.Services;

public interface IRentalService
{
    User RegisterUser(string firstName, string lastName, UserType userType);
    IReadOnlyList<User> GetAllUsers();

    Laptop AddLaptop(string name, string serialNumber, int ramGb, string processor);
    Projector AddProjector(string name, string serialNumber, int lumens, string resolution);
    Camera AddCamera(string name, string serialNumber, double megapixels, bool includesLensKit);
    void RemoveEquipment(string equipmentId);

    IReadOnlyList<Equipment> GetAllEquipment();
    IReadOnlyList<Equipment> GetAvailableEquipment();
    void MarkEquipmentUnavailable(string equipmentId);

    Rental RentEquipment(string userId, string equipmentId, DateTime rentalDate, int durationDays);
    Rental ReturnEquipment(string rentalId, DateTime returnDate);

    IReadOnlyList<Rental> GetAllRentals();
    IReadOnlyList<Rental> GetActiveRentalsForUser(string userId);
    IReadOnlyList<Rental> GetOverdueRentals(DateTime currentDate);
    ServiceSummaryReport GenerateSummaryReport(DateTime currentDate);
}