namespace EquipmentManager.Domain.Users;

public class User
{
    public string Id { get; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserType Type { get; set; }

    public User(string id, string firstName, string lastName, UserType type)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Type = type;
    }

    public string FullName => $"{FirstName} {LastName}";

    public override string ToString() => $"[{Id}] {FullName} ({Type})";
}