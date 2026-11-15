# University Equipment Rental Service (`EquipmentManager`)

**Course:** APBD – Tutorial 2 (Object-Oriented Project in C#)  
**Repository:** [https://github.com/MaxStashuk/apbd-2](https://github.com/MaxStashuk/apbd-2)

## 1. Project Description & Scope
`EquipmentManager` is a C# console application that manages a university equipment rental service. It handles the complete lifecycle of equipment and rentals while enforcing university business rules:

* **Domain Scope:**
  * **Equipment Hierarchy:** An abstract `Equipment` base class with auto-generated unique IDs (`EQ-xxx`), names, serial numbers, and availability statuses (`Available`, `Rented`, `Unavailable`), extended by three specialized device types with type-specific attributes:
    * `Laptop` (`RamGb`, `Processor`)
    * `Projector` (`Lumens`, `NativeResolution`)
    * `Camera` (`Megapixels`, `IncludesLensKit`)
  * **Users:** A `User` model (`USR-xxx`) storing first name, last name, and user category (`Student` or `Employee`).
  * **Rentals:** A `Rental` model (`RNT-xxx`) tracking who rented which item, the rental date, due date, actual return date, on-time status, and any calculated late fee.
* **Functional Operations Supported:**
  1. Register new users (`Student` or `Employee`).
  2. Add new equipment items (`Laptop`, `Projector`, `Camera`) and remove non-rented items.
  3. Display all registered equipment with their current status and hardware specifications.
  4. Filter and display only equipment currently available for rental.
  5. Rent available equipment to a user while validating active rental limits and item availability.
  6. Return equipment, automatically calculating late penalties if returned past the due date.
  7. Mark equipment as unavailable (e.g., due to damage or maintenance).
  8. Display active rentals for a specific user.
  9. Display all currently overdue rentals across the university.
  10. Generate a summary report of the entire rental service state (user counts, equipment breakdown by status, active/overdue rental counts, and total penalties collected).
* **Business Rules Enforced:**
  * Students may have at most **2** active rentals simultaneously.
  * Employees may have at most **5** active rentals simultaneously.
  * Equipment marked as `Unavailable` or `Rented` cannot be rented.
  * Late returns incur a configurable daily penalty (**15.00 PLN** per overdue day).
* **Execution Modes:**
  * **Automated 7-Step Demonstration Scenario:** Runs automatically on startup to demonstrate adding items, adding users, valid rentals, blocked invalid operations (exceeding student limit and renting unavailable equipment), an on-time return, a delayed return with penalty calculation, and a final system summary report.
  * **Interactive Console Menu (Optional Extension):** Launches immediately after the initial scenario, allowing interactive execution of all operations and simulated date advancement.

---

## 2. How to Run the Application

### Prerequisites
* **.NET SDK** (8.0 or newer) installed on your machine.

### Steps
1. Clone the repository and navigate to the project directory:
   ```bash
   git clone [https://github.com/MaxStashuk/apbd-2.git](https://github.com/MaxStashuk/apbd-2.git)
   cd apbd-2/EquipmentManager
   ```
2. Build and run the application using the .NET CLI:
   ```bash
   dotnet run
   ```
   *(Alternatively, open `EquipmentManager.sln` in JetBrains Rider and press `Shift + F10` / Run).*

---

## 3. Organization of Classes, Files, and Project Layers

Rather than placing business rules, data storage, and `Console.WriteLine` calls inside `Program.cs` or a single monolithic class, the solution is structured into four distinct folders/namespaces:

```text
EquipmentManager/
├── Domain/
│   ├── Equipment/
│   │   ├── Equipment.cs            (Abstract base class for all devices)
│   │   ├── EquipmentStatus.cs      (Enum: Available, Rented, Unavailable)
│   │   ├── Laptop.cs               (Concrete device with RAM & Processor)
│   │   ├── Projector.cs            (Concrete device with Lumens & Resolution)
│   │   └── Camera.cs               (Concrete device with Megapixels & LensKit)
│   ├── Users/
│   │   ├── User.cs                 (System user entity)
│   │   └── UserType.cs             (Enum: Student, Employee)
│   ├── Rentals/
│   │   └── Rental.cs               (Rental lifecycle & return state entity)
│   └── Exceptions/
│       └── RentalDomainException.cs(Explicit exception for business rule violations)
├── Policies/
│   ├── IRentalPolicy.cs            (Interface for rental limits & penalty rules)
│   └── StandardUniversityRentalPolicy.cs (Concrete implementation of limits & fees)
├── Services/
│   ├── IRentalService.cs           (Contract for rental service operations)
│   ├── RentalService.cs            (Core business workflow & state management)
│   └── ServiceSummaryReport.cs     (Immutable record holding report metrics)
├── UI/
│   ├── ConsoleMenu.cs              (Interactive text menu & demo scenario runner)
│   └── ConsoleReportPrinter.cs     (Console formatting helper methods)
└── Program.cs                      (Dependency wiring and application entry point)
```

### Why This Division Makes Sense
* **`Domain` Layer:** Contains only pure data structures and local state transitions (such as `Rental.CompleteReturn(...)`). Domain classes have zero knowledge of where data is stored, how the UI works, or what the university's current fee rates are.
* **`Policies` Layer:** Isolates volatile business numbers (the 2-item student limit, 5-item employee limit, and 15 PLN daily penalty rate) into a dedicated class so they are never hardcoded across multiple service methods or entities.
* **`Services` Layer:** Coordinates operations across users, equipment, and rentals while consulting `IRentalPolicy` and throwing `RentalDomainException` when an operation violates a rule.
* **`UI` Layer & `Program.cs`:** Handles all `Console.ReadLine` and `Console.WriteLine` interactions. `Program.cs` only wires up dependencies and starts the menu.

---

## 4. Addressing Cohesion, Coupling, and Class Responsibilities

### Clear Class Responsibilities & High Cohesion
Each class in the project is designed around a single, well-defined area of responsibility:
* **`RentalService` vs. `StandardUniversityRentalPolicy`:** `RentalService` is responsible for workflow coordination (finding entities by ID, checking availability, creating rentals, and updating statuses). It does *not* decide how much a late fee costs or how many items a student can rent—that decision belongs exclusively to `StandardUniversityRentalPolicy`. If the university changes the penalty calculation formula (e.g., introducing weekend grace periods) or raises employee limits, only `StandardUniversityRentalPolicy` needs to be edited.
* **`RentalService` vs. `ConsoleReportPrinter` / `ConsoleMenu`:** `RentalService` returns strongly-typed domain collections (`IReadOnlyList<Equipment>`, `IReadOnlyList<Rental>`) and an immutable `ServiceSummaryReport` record instead of printing text directly. `ConsoleReportPrinter` focuses strictly on string formatting for the terminal, while `ConsoleMenu` handles user input parsing and menu navigation.
* **Explicit Error Handling (`RentalDomainException`):** When a business rule fails (e.g., renting an unavailable camera or exceeding a student's active rental limit), `RentalService` throws a dedicated `RentalDomainException`. The service does not silently ignore invalid actions, nor does it print console warnings itself; the UI layer catches `RentalDomainException` and displays a clear message to the user.

### Low Coupling
* **Interface-Based Dependency Injection:** `RentalService` accepts an `IRentalPolicy` interface via its constructor (`public RentalService(IRentalPolicy rentalPolicy)`), and `ConsoleMenu` accepts an `IRentalService` interface (`public ConsoleMenu(IRentalService rentalService)`). Neither class is tightly coupled to a concrete implementation.
* **UI-Independent Core:** Because `Domain`, `Policies`, and `Services` never reference `System.Console` or the `UI` namespace, the entire backend logic could be moved into an ASP.NET Core Web API, a desktop GUI, or a unit test suite without modifying a single line of domain or service code.
* **Encapsulated Collections:** `RentalService` exposes `IReadOnlyList<T>` (`AsReadOnly()`) rather than raw mutable `List<T>` fields, preventing external classes in the `UI` layer from bypassing business rules by directly adding or deleting items from internal lists.

### Justification of Inheritance vs. Composition/Services
* **Why Inheritance Was Used for `Equipment`:** Every piece of university equipment shares core identity and lifecycle attributes (`Id`, `Name`, `SerialNumber`, `Status`), yet each hardware category requires distinct data (`RamGb` and `Processor` for `Laptop`, `Lumens` and `NativeResolution` for `Projector`, `Megapixels` and `IncludesLensKit` for `Camera`). Using an abstract `Equipment` class with an abstract `GetSpecifications()` method allows `RentalService` and `ConsoleReportPrinter` to treat all devices polymorphically without `if/switch` type-checking blocks.
* **Why Inheritance Was Avoided for `User`:** Although the system distinguishes between `Student` and `Employee`, their attributes (`Id`, `FirstName`, `LastName`) are identical. Their only difference in the problem domain is a business rule (maximum concurrent active rentals). Creating `Student : User` and `Employee : User` subclasses just to hardcode a `MaxRentals` integer inside the user entity would mix volatile business policy into the data model. Instead, `UserType` is modeled as a property on `User` and evaluated by `IRentalPolicy`, keeping user data decoupled from rental policy rules.

---

## 5. UI Overview & Interactive Workflow

The presentation layer (`UI/` namespace) is split into two focused components so that input handling and output rendering remain clean:
1. **`ConsoleReportPrinter`:** A static formatting utility that renders lists of `User`, `Equipment`, `Rental`, and `ServiceSummaryReport` objects into structured console output.
2. **`ConsoleMenu`:** An interactive command loop that prompts the user for input, parses primitives, invokes `IRentalService` methods, and catches domain/input exceptions.

### Startup & Automated 7-Step Scenario
When the application starts, `Program.cs` automatically runs `RunAutomatedScenario()`, which prints all 7 required demonstration steps to the terminal and seeds the system with initial users, equipment, and rentals. Immediately afterward, the interactive menu loop begins.

### Interactive Menu Structure
```text
=======================================================
 UNIVERSITY EQUIPMENT RENTAL SYSTEM (Date: 2026-03-11)
=======================================================
 1. Add a new user
 2. List all users
 3. Add new equipment (Laptop / Projector / Camera)
 4. Remove equipment
 5. Display ALL equipment with status
 6. Display AVAILABLE equipment only
 7. Rent equipment to a user
 8. Return equipment (calculates late penalty)
 9. Mark equipment as unavailable (maintenance/damage)
10. Display active rentals for a selected user
11. Display overdue rentals
12. Generate summary report
13. Advance simulated date (to test overdue/penalties)
14. Run automated 7-step assignment demonstration
 0. Exit
-------------------------------------------------------
Select an option:
```

### Key UI Features
* **Context-Aware Prompts:** Before asking for an ID (e.g., when renting, returning, or marking equipment unavailable), the UI automatically displays the relevant list of users, available equipment, or active rentals so the user does not have to memorize IDs like `USR-001` or `EQ-003`.
* **Simulated System Clock (Option 13):** To make testing overdue rentals and late return penalties effortless during live demonstrations, the menu maintains a `_simulatedDate` displayed in the header. Advancing the date by *N* days immediately allows testing overdue detection (Option 11) and late fee calculation upon return (Option 8).
* **Graceful Error Feedback:** Invalid inputs or violated business rules never crash the application. `ConsoleMenu` catches `RentalDomainException` and displays clear feedback (e.g., `[BUSINESS RULE ERROR]: User 'Jan Kowalski' (Student) has reached the maximum limit of 2 active rentals.`), returning smoothly to the main menu.
