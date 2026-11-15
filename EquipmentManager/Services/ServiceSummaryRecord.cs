namespace EquipmentManager.Services;

public record ServiceSummaryReport(
    int TotalUsers,
    int TotalEquipment,
    int AvailableEquipment,
    int RentedEquipment,
    int UnavailableEquipment,
    int ActiveRentalsCount,
    int OverdueActiveRentalsCount,
    decimal TotalCollectedPenalties
);