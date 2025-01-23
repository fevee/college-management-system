# BITCollege Academic Management System

A comprehensive C# ASP.NET MVC project designed for managing academic entities and operations in a college environment. This system facilitates the management of students, academic programs, course registrations, grade point tracking, and state-based tuition adjustments.  


## Technologies Used

- **Languages:** C#, HTML, CSS
- **Frameworks:** ASP.NET MVC, Entity Framework, Windows Forms
- **Database:** Microsoft SQL Server
- **Design Patterns:** State Pattern, Singleton Pattern
- **Testing Frameworks:** NUnit


## Features

### 1. **Entity Management**
   - **Students:** 
     - Create, view, edit, and delete student records.
     - Automatically generate unique student numbers using stored procedures.
     - Track student grade point averages (GPA), outstanding fees, and full contact information.
   - **Academic Programs:** 
     - Manage program details including program acronyms and descriptions.
     - Associate students and courses with specific programs.
   - **Courses:**
     - Support for different course types: Graded, Audit, and Mastery.
     - Automatic course number generation and credit hour tracking.
   - **Registrations:** 
     - Manage course registrations for students with tracking of registration numbers, grades, and notes.

### 2. **Grade Point State System**
   - Implements a **State Design Pattern** for GPA-based tuition adjustments and student status:
     - `RegularState`, `HonoursState`, `ProbationState`, and `SuspendedState`.
   - Automatically adjusts the state of students based on their GPA and course activity.
   - Calculates tuition rate adjustments dynamically based on state rules.

### 3. **Integration with SQL Server**
   - Uses stored procedures for generating unique identifiers (`NextNumber`) for students, registrations, and courses.
   - Implements SQL Server connections for efficient database interactions.

### 4. **Windows Forms Integration**
   - Includes a `StudentData` Windows Form application for displaying and interacting with student and registration data.
   - Offers features like grade updates and detailed student information display.

### 5. **WCF Service**
   - Provides an extensible **WCF service** (`ICollegeRegistration`) for operations like:
     - Dropping courses.
     - Registering students in courses with validation and return codes.
     - Updating grades for specific registrations.

### 6. **Validation and Error Handling**
   - Comprehensive use of ASP.NET Data Annotations for input validation.
   - Enforces validation rules for:
     - Canadian provinces (via regex).
     - GPA ranges (0 to 4.5).
     - Outstanding fees and date formatting.

### 7. **Navigation and User Interface**
   - Fully functional MVC controllers and Razor views for managing and displaying data.
   - Implements dropdowns, masked inputs, and validation messages in forms.

### 8. **Unit Testing**
   - Comprehensive unit tests were created and executed to verify the functionality of key components, including:
     - State transitions in the `GradePointState` system.
     - Stored procedure outputs for generating unique numbers.
     - CRUD operations for students, courses, and registrations.
   - Ensures reliability and correctness of the system across various edge cases.


