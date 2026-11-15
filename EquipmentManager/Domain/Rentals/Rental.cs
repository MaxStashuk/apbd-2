using EquipmentManager.Domain.Users;

namespace EquipmentManager.Domain.Rentals;

public class Rental
{
    public string Id { get; }
    public User User { get; }
    public Equipment.Equipment Equipment { get; }
    public DateTime RentalDate { get; }
    public DateTime DueDate { get; }
    public DateTime? ActualReturnDate { get; private set; }
    public decimal PenaltyFee { get; private set; }

    public bool IsActive => ActualReturnDate == null;
    public bool IsReturnedOnTime => ActualReturnDate.HasValue && ActualReturnDate.Value.Date <= DueDate.Date;

    public Rental(string id, User user, Equipment.Equipment equipment, DateTime rentalDate, int durationDays)
    {
        Id = id;
        User = user;
        Equipment = equipment;
        RentalDate = rentalDate;
        DueDate = rentalDate.AddDays(durationDays);
    }

    public bool IsOverdue(DateTime currentDate)
    {
        var checkDate = ActualReturnDate ?? currentDate;
        return checkDate.Date > DueDate.Date;
    }

    public void CompleteReturn(DateTime returnDate, decimal penaltyFee)
    {
        ActualReturnDate = returnDate;
        PenaltyFee = penaltyFee;
    }

    public override string ToString()
    {
        string status = IsActive
            ? $"ACTIVE (Due: {DueDate:yyyy-MM-dd})"
            : $"RETURNED on {ActualReturnDate:yyyy-MM-dd} (On time: {IsReturnedOnTime}, Penalty: {PenaltyFee:C})";

        return $"[{Id}] User: {User.FullName} | Item: {Equipment.Name} ({Equipment.Id}) | Rented: {RentalDate:yyyy-MM-dd} | {status}";
    }
}