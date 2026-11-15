using EquipmentManager.Domain.Equipment;
using EquipmentManager.Domain.Exceptions;
using EquipmentManager.Domain.Rentals;
using EquipmentManager.Domain.Users;
using EquipmentManager.Policies;

namespace EquipmentManager.Services;

public class RentalService : IRentalService
{
    private readonly List<User> _users = new();
    private readonly List<Equipment> _equipment = new();
    private readonly List<Rental> _rentals = new();
    private readonly IRentalPolicy _rentalPolicy;

    private int _userSeq = 1;
    private int _equipmentSeq = 1;
    private int _rentalSeq = 1;

    public RentalService(IRentalPolicy rentalPolicy)
    {
        _rentalPolicy = rentalPolicy;
    }

    public User RegisterUser(string firstName, string lastName, UserType userType)
    {
        var user = new User($"USR-{_userSeq++:D3}", firstName, lastName, userType);
        _users.Add(user);
        return user;
    }

    public Laptop AddLaptop(string name, string serialNumber, int ramGb, string processor)
    {
        var laptop = new Laptop($"EQ-{_equipmentSeq++:D3}", name, serialNumber, ramGb, processor);
        _equipment.Add(laptop);
        return laptop;
    }

    public Projector AddProjector(string name, string serialNumber, int lumens, string resolution)
    {
        var projector = new Projector($"EQ-{_equipmentSeq++:D3}", name, serialNumber, lumens, resolution);
        _equipment.Add(projector);
        return projector;
    }

    public Camera AddCamera(string name, string serialNumber, double megapixels, bool includesLensKit)
    {
        var camera = new Camera($"EQ-{_equipmentSeq++:D3}", name, serialNumber, megapixels, includesLensKit);
        _equipment.Add(camera);
        return camera;
    }

    public IReadOnlyList<Equipment> GetAllEquipment() => _equipment.AsReadOnly();

    public IReadOnlyList<Equipment> GetAvailableEquipment() =>
        _equipment.Where(e => e.Status == EquipmentStatus.Available).ToList().AsReadOnly();

    public void MarkEquipmentUnavailable(string equipmentId)
    {
        var item = GetEquipmentById(equipmentId);
        if (item.Status == EquipmentStatus.Rented)
        {
            throw new RentalDomainException($"Cannot mark '{item.Name}' ({item.Id}) as unavailable while it is actively rented.");
        }
        item.Status = EquipmentStatus.Unavailable;
    }

    public Rental RentEquipment(string userId, string equipmentId, DateTime rentalDate, int durationDays)
    {
        var user = GetUserById(userId);
        var item = GetEquipmentById(equipmentId);

        if (item.Status != EquipmentStatus.Available)
        {
            throw new RentalDomainException($"Equipment '{item.Name}' ({item.Id}) cannot be rented because its status is {item.Status}.");
        }

        int currentActiveRentals = _rentals.Count(r => r.User.Id == user.Id && r.IsActive);
        int maxAllowed = _rentalPolicy.GetMaxActiveRentals(user.Type);

        if (currentActiveRentals >= maxAllowed)
        {
            throw new RentalDomainException(
                $"User '{user.FullName}' ({user.Type}) has reached the maximum limit of {maxAllowed} active rentals.");
        }

        var rental = new Rental($"RNT-{_rentalSeq++:D3}", user, item, rentalDate, durationDays);
        item.Status = EquipmentStatus.Rented;
        _rentals.Add(rental);

        return rental;
    }

    public Rental ReturnEquipment(string rentalId, DateTime returnDate)
    {
        var rental = _rentals.FirstOrDefault(r => r.Id == rentalId)
            ?? throw new RentalDomainException($"Rental with ID '{rentalId}' was not found.");

        if (!rental.IsActive)
        {
            throw new RentalDomainException($"Rental '{rentalId}' has already been returned.");
        }

        if (returnDate.Date < rental.RentalDate.Date)
        {
            throw new RentalDomainException("Return date cannot be earlier than the rental date.");
        }

        decimal penalty = _rentalPolicy.CalculateLatePenalty(rental.DueDate, returnDate);
        rental.CompleteReturn(returnDate, penalty);
        rental.Equipment.Status = EquipmentStatus.Available;

        return rental;
    }

    public IReadOnlyList<Rental> GetActiveRentalsForUser(string userId)
    {
        GetUserById(userId);
        return _rentals.Where(r => r.User.Id == userId && r.IsActive).ToList().AsReadOnly();
    }

    public IReadOnlyList<Rental> GetOverdueRentals(DateTime currentDate) =>
        _rentals.Where(r => r.IsActive && r.IsOverdue(currentDate)).ToList().AsReadOnly();

    public ServiceSummaryReport GenerateSummaryReport(DateTime currentDate)
    {
        return new ServiceSummaryReport(
            TotalUsers: _users.Count,
            TotalEquipment: _equipment.Count,
            AvailableEquipment: _equipment.Count(e => e.Status == EquipmentStatus.Available),
            RentedEquipment: _equipment.Count(e => e.Status == EquipmentStatus.Rented),
            UnavailableEquipment: _equipment.Count(e => e.Status == EquipmentStatus.Unavailable),
            ActiveRentalsCount: _rentals.Count(r => r.IsActive),
            OverdueActiveRentalsCount: _rentals.Count(r => r.IsActive && r.IsOverdue(currentDate)),
            TotalCollectedPenalties: _rentals.Sum(r => r.PenaltyFee)
        );
    }

    private User GetUserById(string userId) =>
        _users.FirstOrDefault(u => u.Id == userId)
        ?? throw new RentalDomainException($"User with ID '{userId}' was not found.");

    private Equipment GetEquipmentById(string equipmentId) =>
        _equipment.FirstOrDefault(e => e.Id == equipmentId)
        ?? throw new RentalDomainException($"Equipment with ID '{equipmentId}' was not found.");
}