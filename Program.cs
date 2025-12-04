using System;
using System.Collections.Generic;

namespace EducationCentreDIS
{
    // Education Centre Information System
    // Console application for managing in-memory records for Teachers, Admins and Students.
    // The code demonstrates basic OOP (inheritance + polymorphism), input validation,
    // and a single-screen console UI where each operation clears the previous content.
    
    /// <summary>
    /// Abstract base class for all person types in the system.
    /// Holds common properties and a small helper used by derived types.
    /// Each concrete type implements its own EditDetails workflow.
    /// </summary>
    abstract class Person
    {
        // Simple auto-incrementing identifier used for lookup and display.
        private static int _nextId = 1;

        // Public properties shared across all person types.
        public int Id { get; private set; }             // Unique id assigned at construction
        public string Name { get; set; }                // Full name
        public string Telephone { get; set; }           // Telephone (digits-only validation elsewhere)
        public string Email { get; set; }               // Email (validated to end with @gmail.com)
        public string Role { get; protected set; }      // Role string: "Teacher", "Admin" or "Student"

        /// <summary>
        /// Protected constructor: used by derived classes to initialize the shared state.
        /// </summary>
        protected Person(string name, string telephone, string email, string role)
        {
            Id = _nextId++;
            Name = name;
            Telephone = telephone;
            Email = email;
            Role = role;
        }

        /// <summary>
        /// Each derived class implements its own interactive edit flow.
        /// Called polymorphically from Program.EditPerson.
        /// </summary>
        public abstract void EditDetails();

        /// <summary>
        /// Returns a single-line, human-readable representation of the person record.
        /// Used when listing records.
        /// </summary>
        public override string ToString()
        {
            return $"ID: {Id} | Role: {Role} | Name: {Name} | Tel: {Telephone} | Email: {Email}";
        }

        /// <summary>
        /// Small helper used by derived classes to read a non-empty string.
        /// If currentValue is provided and the user presses Enter, the current value is kept.
        /// </summary>
        protected string ReadNonEmpty(string prompt, string currentValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                // Keep current value when editing and user presses Enter.
                if (!string.IsNullOrWhiteSpace(currentValue) && string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine("Value cannot be empty. Please try again.");
            }
        }
    }

    /// <summary>
    /// Teacher record: stores integer Salary and two subject fields.
    /// Implements its own EditDetails flow which validates telephone, email and salary.
    /// </summary>
    class Teacher : Person
    {
        // Salary is an integer per requirements.
        public int Salary { get; set; }
        public string Subject1 { get; set; }
        public string Subject2 { get; set; }

        public Teacher(string name, string telephone, string email,
                       int salary, string subject1, string subject2)
            : base(name, telephone, email, "Teacher")
        {
            Salary = salary;
            Subject1 = subject1;
            Subject2 = subject2;
        }

        /// <summary>
        /// Adds teacher-specific fields to the base ToString representation.
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + $" | Salary: {Salary:C} | Subjects: {Subject1}, {Subject2}";
        }

        /// <summary>
        /// Interactive edit flow for Teacher records.
        /// - Clears the console to ensure a single visible screen.
        /// - Prompts for editable values. Pressing Enter keeps the current value.
        /// - Uses Program helper methods to validate telephone and email formats.
        /// - Ensures salary is an integer.
        /// - Waits for Enter at the end before returning to the main menu.
        /// </summary>
        public override void EditDetails()
        {
            Console.Clear();
            Console.WriteLine("Editing Teacher (press Enter to keep current value).");

            Name = ReadNonEmpty($"Name ({Name}): ", Name);
            Telephone = Program.ReadTelephoneNumeric($"Telephone ({Telephone}): ", Telephone);
            Email = Program.ReadValidatedGmail($"Email ({Email}): ", Email);

            // Salary: allow keeping current by pressing Enter, otherwise require an integer.
            while (true)
            {
                Console.Write($"Salary ({Salary}): ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    break; // keep current

                if (int.TryParse(input, out int newSalary))
                {
                    Salary = newSalary;
                    break;
                }

                Console.WriteLine("Invalid salary. Please enter a valid integer number.");
            }

            Subject1 = ReadNonEmpty($"Subject 1 ({Subject1}): ", Subject1);
            Subject2 = ReadNonEmpty($"Subject 2 ({Subject2}): ", Subject2);

            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Admin record: stores integer Salary, EmploymentType (Full-time/Part-time) and WorkingHours.
    /// EmploymentType is validated and normalized by Program.ReadEmploymentType.
    /// </summary>
    class Admin : Person
    {
        public int Salary { get; set; }                // integer salary
        public string EmploymentType { get; set; }     // "Full-time" or "Part-time"
        public double WorkingHours { get; set; }       // hours per week

        public Admin(string name, string telephone, string email,
                     int salary, string employmentType, double workingHours)
            : base(name, telephone, email, "Admin")
        {
            Salary = salary;
            EmploymentType = employmentType;
            WorkingHours = workingHours;
        }

        /// <summary>
        /// Adds Admin-specific fields to the base string for display.
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + $" | Salary: {Salary:C} | Type: {EmploymentType} | Hours: {WorkingHours}";
        }

        /// <summary>
        /// Interactive edit flow for Admin records:
        /// - Clears console to show only the edit screen.
        /// - Validates telephone and gmail address.
        /// - Ensures salary is integer and working hours are numeric.
        /// - Enforces EmploymentType to be Full-time or Part-time.
        /// </summary>
        public override void EditDetails()
        {
            Console.Clear();
            Console.WriteLine("Editing Admin (press Enter to keep current value).");

            Name = ReadNonEmpty($"Name ({Name}): ", Name);
            Telephone = Program.ReadTelephoneNumeric($"Telephone ({Telephone}): ", Telephone);
            Email = Program.ReadValidatedGmail($"Email ({Email}): ", Email);

            // Salary (integer)
            while (true)
            {
                Console.Write($"Salary ({Salary}): ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    break;

                if (int.TryParse(input, out int newSalary))
                {
                    Salary = newSalary;
                    break;
                }

                Console.WriteLine("Invalid salary. Please enter a valid integer number.");
            }

            // Employment type must be exactly "Full-time" or "Part-time" (case-insensitive).
            EmploymentType = Program.ReadEmploymentType($"Employment type ({EmploymentType}): ", EmploymentType);

            // Working hours (double)
            while (true)
            {
                Console.Write($"Working hours ({WorkingHours}): ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    break;

                if (double.TryParse(input, out double newHours))
                {
                    WorkingHours = newHours;
                    break;
                }

                Console.WriteLine("Invalid hours. Please enter a valid number.");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Student record: stores three subjects. Editing validates telephone and email.
    /// </summary>
    class Student : Person
    {
        public string Subject1 { get; set; }   // first subject
        public string Subject2 { get; set; }   // second subject
        public string Subject3 { get; set; }   // third subject

        public Student(string name, string telephone, string email,
                       string subject1, string subject2, string subject3)
            : base(name, telephone, email, "Student")
        {
            Subject1 = subject1;
            Subject2 = subject2;
            Subject3 = subject3;
        }

        /// <summary>
        /// Appends subjects to the base representation.
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + $" | Subjects: {Subject1}, {Subject2}, {Subject3}";
        }

        /// <summary>
        /// Interactive edit flow for Student records:
        /// - Clears the screen, validates telephone and gmail address,
        /// - Allows pressing Enter to keep current values.
        /// </summary>
        public override void EditDetails()
        {
            Console.Clear();
            Console.WriteLine("Editing Student (press Enter to keep current value).");

            Name = ReadNonEmpty($"Name ({Name}): ", Name);
            Telephone = Program.ReadTelephoneNumeric($"Telephone ({Telephone}): ", Telephone);
            Email = Program.ReadValidatedGmail($"Email ({Email}): ", Email);

            Subject1 = ReadNonEmpty($"Subject 1 ({Subject1}): ", Subject1);
            Subject2 = ReadNonEmpty($"Subject 2 ({Subject2}): ", Subject2);
            Subject3 = ReadNonEmpty($"Subject 3 ({Subject3}): ", Subject3);

            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Main program: contains the menu loop, CRUD-like operations and validation helpers.
    /// Data is stored only in memory (no persistence).
    /// Every top-level operation clears the console so only a single "screen" is visible.
    /// </summary>
    class Program
    {
        // In-memory list storing all created person records.
        private static List<Person> people = new List<Person>();

        /// <summary>
        /// Application entry point. Runs a continuous menu loop until the user exits.
        /// Each iteration redraws the main menu (single-screen behavior enforced by Console.Clear).
        /// </summary>
        static void Main(string[] args)
        {
            while (true)
            {
                ShowMenu();

                Console.Write("Enter option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddNewPerson();
                        break;
                    case "2":
                        ViewAllPeople();
                        break;
                    case "3":
                        ViewByRole();
                        break;
                    case "4":
                        EditPerson();
                        break;
                    case "5":
                        DeletePerson();
                        break;
                    case "0":
                        Console.WriteLine("Exiting application. Goodbye!");
                        return;
                    default:
                        // Invalid selection: inform the user, wait for Enter, then redraw menu.
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        /// <summary>
        /// Renders the main menu header and options.
        /// Clears the console first so previous screens are hidden.
        /// </summary>
        static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=======================================");
            Console.WriteLine("   EDUCATION CENTRE INFORMATION SYSTEM ");
            Console.WriteLine("=======================================");
            Console.WriteLine("1. Add new record");
            Console.WriteLine("2. View all records");
            Console.WriteLine("3. View records by role");
            Console.WriteLine("4. Edit existing record");
            Console.WriteLine("5. Delete existing record");
            Console.WriteLine("0. Exit");
            Console.WriteLine("=======================================");
        }

        /// <summary>
        /// Top-level flow for adding a new person record.
        /// - Shows role selection, clears the role menu, then collects common fields
        ///   (Name/Telephone/Email) before delegating to a role-specific creation method.
        /// - All input validation for common fields is performed by helper methods.
        /// </summary>
        static void AddNewPerson()
        {
            Console.Clear();
            Console.WriteLine("Select role to add:");
            Console.WriteLine("1. Teacher");
            Console.WriteLine("2. Admin");
            Console.WriteLine("3. Student");
            Console.Write("Your choice: ");
            string roleChoice = Console.ReadLine();

            // Remove role menu from the console so only the data-entry prompts remain visible.
            Console.Clear();

            // Collect common fields with validation helpers (these re-prompt until valid).
            string name = ReadNonEmptyString("Name: ");
            string telephone = ReadTelephoneNumeric("Telephone: ");
            string email = ReadValidatedGmail("Email: ");

            switch (roleChoice)
            {
                case "1":
                    AddTeacher(name, telephone, email);
                    break;
                case "2":
                    AddAdmin(name, telephone, email);
                    break;
                case "3":
                    AddStudent(name, telephone, email);
                    break;
                default:
                    // Invalid role selection: inform and return to main menu after Enter.
                    Console.WriteLine("Invalid role selection.");
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to return to main menu...");
                    Console.ReadLine();
                    break;
            }
        }

        /// <summary>
        /// Prompts for teacher-specific fields, constructs a Teacher object and stores it.
        /// </summary>
        static void AddTeacher(string name, string telephone, string email)
        {
            int salary = ReadInt("Salary: ");
            Console.Write("Subject 1: ");
            string subject1 = Console.ReadLine();
            Console.Write("Subject 2: ");
            string subject2 = Console.ReadLine();

            Person t = new Teacher(name, telephone, email, salary, subject1, subject2);
            people.Add(t);

            Console.WriteLine("Teacher added successfully.");
            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }

        /// <summary>
        /// Prompts for admin-specific fields, constructs an Admin object and stores it.
        /// Employment type is validated by ReadEmploymentType helper.
        /// </summary>
        static void AddAdmin(string name, string telephone, string email)
        {
            int salary = ReadInt("Salary: ");
            string type = ReadEmploymentType("Employment type (Full-time/Part-time): ");
            double hours = ReadDouble("Working hours per week: ");

            Person a = new Admin(name, telephone, email, salary, type, hours);
            people.Add(a);

            Console.WriteLine("Admin added successfully.");
            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }

        /// <summary>
        /// Prompts for student-specific fields, constructs a Student object and stores it.
        /// </summary>
        static void AddStudent(string name, string telephone, string email)
        {
            Console.Write("Subject 1: ");
            string subject1 = Console.ReadLine();
            Console.Write("Subject 2: ");
            string subject2 = Console.ReadLine();
            Console.Write("Subject 3: ");
            string subject3 = Console.ReadLine();

            Person s = new Student(name, telephone, email, subject1, subject2, subject3);
            people.Add(s);

            Console.WriteLine("Student added successfully.");
            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }

        /// <summary>
        /// Displays all stored records. If none exist, shows a friendly message.
        /// Waits for Enter before returning to menu.
        /// </summary>
        static void ViewAllPeople()
        {
            Console.Clear();

            if (people.Count == 0)
            {
                Console.WriteLine("No records found.");
            }
            else
            {
                Console.WriteLine("All records:");
                foreach (Person p in people)
                {
                    Console.WriteLine(p.ToString());
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }

        /// <summary>
        /// Prompts the user for a role and displays matching records (case-insensitive).
        /// </summary>
        static void ViewByRole()
        {
            Console.Clear();
            Console.Write("Enter role to filter (Teacher/Admin/Student): ");
            string role = Console.ReadLine();

            bool found = false;
            foreach (Person p in people)
            {
                if (p.Role.Equals(role, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(p.ToString());
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No records found for the given role.");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }

        /// <summary>
        /// Initiates the edit flow for a selected record:
        /// - Shows all records, asks for ID, finds the person and calls the polymorphic EditDetails.
        /// - Each concrete EditDetails implementation handles its own validations and end-of-edit prompt.
        /// </summary>
        static void EditPerson()
        {
            if (people.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("No records to edit.");
                Console.WriteLine();
                Console.WriteLine("Press Enter to return to main menu...");
                Console.ReadLine();
                return;
            }

            Console.Clear();
            ViewAllPeopleForEdit();

            int id = ReadInt("Enter ID of record to edit: ");

            Person person = people.Find(p => p.Id == id);
            if (person == null)
            {
                Console.WriteLine("Record not found.");
                Console.WriteLine();
                Console.WriteLine("Press Enter to return to main menu...");
                Console.ReadLine();
                return;
            }

            // Polymorphic call - the concrete object handles its own edit UI and validation.
            person.EditDetails();
        }

        /// <summary>
        /// Utility that prints the list of people without adding a "Press Enter" pause.
        /// Used by edit and delete flows where the caller manages the prompt.
        /// </summary>
        static void ViewAllPeopleForEdit()
        {
            if (people.Count == 0)
            {
                Console.WriteLine("No records found.");
                return;
            }

            Console.WriteLine("All records:");
            foreach (Person p in people)
            {
                Console.WriteLine(p.ToString());
            }
        }

        /// <summary>
        /// Delete flow:
        /// - Shows all records, asks for the Id to delete, shows the selected record,
        ///   asks for confirmation and removes the record if confirmed.
        /// - Waits for Enter before returning to menu.
        /// </summary>
        static void DeletePerson()
        {
            if (people.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("No records to delete.");
                Console.WriteLine();
                Console.WriteLine("Press Enter to return to main menu...");
                Console.ReadLine();
                return;
            }

            Console.Clear();
            ViewAllPeopleForEdit();

            int id = ReadInt("Enter ID of record to delete: ");

            Person person = people.Find(p => p.Id == id);
            if (person == null)
            {
                Console.WriteLine("Record not found.");
                Console.WriteLine();
                Console.WriteLine("Press Enter to return to main menu...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Selected record:");
            Console.WriteLine(person.ToString());
            Console.Write("Are you sure you want to delete this record? (y/n): ");
            string confirm = Console.ReadLine();

            if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                people.Remove(person);
                Console.WriteLine("Record deleted.");
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }

        // ----------------------------
        // Input / validation helpers
        // ----------------------------

        /// <summary>
        /// Reads a decimal value from the console. Keeps prompting until a valid decimal is entered.
        /// </summary>
        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (decimal.TryParse(input, out decimal value))
                {
                    return value;
                }
                Console.WriteLine("Invalid number. Please try again.");
            }
        }

        /// <summary>
        /// Reads a double from the console with validation.
        /// </summary>
        static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (double.TryParse(input, out double value))
                {
                    return value;
                }
                Console.WriteLine("Invalid number. Please try again.");
            }
        }

        /// <summary>
        /// Reads an integer value from the console with validation.
        /// Used for salary and id input.
        /// </summary>
        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int value))
                {
                    return value;
                }
                Console.WriteLine("Invalid integer. Please try again.");
            }
        }

        /// <summary>
        /// Reads a non-empty string. Keeps prompting until the user provides a non-blank value.
        /// </summary>
        static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();
                Console.WriteLine("Value cannot be empty. Please try again.");
            }
        }

        /// <summary>
        /// Validates that the entered email ends with "@gmail.com" (case-insensitive).
        /// When currentValue is provided (editing scenario), pressing Enter returns currentValue.
        /// </summary>
        public static string ReadValidatedGmail(string prompt, string currentValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                // If editing and Enter pressed, keep existing value.
                if (!string.IsNullOrWhiteSpace(currentValue) && string.IsNullOrWhiteSpace(input))
                    return currentValue;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    string trimmed = input.Trim();
                    if (trimmed.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                        return trimmed;
                }

                Console.WriteLine("Invalid email. Email must end with '@gmail.com'. Please try again.");
            }
        }

        /// <summary>
        /// Ensures telephone is digits-only. When currentValue provided, Enter keeps the current telephone.
        /// Uses long.TryParse to accept large numeric strings.
        /// </summary>
        public static string ReadTelephoneNumeric(string prompt, string currentValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                // Keep current value on Enter (editing).
                if (!string.IsNullOrWhiteSpace(currentValue) && string.IsNullOrWhiteSpace(input))
                    return currentValue;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    string trimmed = input.Trim();
                    if (long.TryParse(trimmed, out _))
                        return trimmed;
                }

                Console.WriteLine("Invalid telephone. Please enter digits only (no letters or symbols).");
            }
        }

        /// <summary>
        /// Accepts only "Full-time" or "Part-time" (case-insensitive).
        /// Returns normalized casing ("Full-time" or "Part-time").
        /// When currentValue provided, Enter keeps it.
        /// </summary>
        public static string ReadEmploymentType(string prompt, string currentValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                // Keep current when editing and pressing Enter.
                if (!string.IsNullOrWhiteSpace(currentValue) && string.IsNullOrWhiteSpace(input))
                    return currentValue;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    string trimmed = input.Trim();
                    if (trimmed.Equals("Full-time", StringComparison.OrdinalIgnoreCase) ||
                        trimmed.Equals("Part-time", StringComparison.OrdinalIgnoreCase))
                    {
                        // Normalize returned string for consistent storage/display.
                        return trimmed.Equals("Full-time", StringComparison.OrdinalIgnoreCase) ? "Full-time" : "Part-time";
                    }
                }

                Console.WriteLine("Invalid employment type. Enter exactly 'Full-time' or 'Part-time'.");
            }
        }
    }
}
