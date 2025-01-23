/*
 * Name: Faye Vaquilar
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-09-04
 * Updated: 2023-10-29
 */

using BITCollege_FV.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using Utility;
using BITCollege_FV.Migrations;

namespace BITCollege_FV.Models
{
    /// <summary>
    /// AcademicProgram Model. Represents the AcademicProgram table in the database.
    /// </summary>
    public class AcademicProgram
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AcademicProgramId { get; set; }

        [Required]
        [Display(Name = "Program")]
        public string ProgramAcronym { get; set; }

        [Required]
        [Display(Name = "Program\nName")]
        public string Description { get; set; }

        // Navigation Properties
        public virtual ICollection<Course> Course { get; set; }

        public virtual ICollection<Student> Student { get; set; }
    }

    /// <summary>
    /// Student Model. Represents the Student table in the database.
    /// </summary>
    public class Student
    {
        private BITCollege_FVContext db = new BITCollege_FVContext();

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("GradePointState")]
        public int GradePointStateId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name = "Student\nNumber")]
        public long StudentNumber { get; set; }

        [Required]
        [Display(Name = "First\nName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last\nName")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required (ErrorMessage = "A valid Canadian province code is required.")]
        [RegularExpression("^(N[BLSTU]|[AMN]B|[BQ]C|ON|PE|SK|YT)")]
        public string Province { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Grade Point\nAverage")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Range(0, 4.5)]
        public double? GradePointAverage { get; set; }

        [Required]
        [Display(Name = "Fees")]
        [DisplayFormat(DataFormatString = "{0:c2}")]
        public double OutstandingFees { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        [Display(Name = "Address")]
        public string FullAddress
        {
            get
            {
                return String.Format("{0} {1} {2}",
                    Address, City, Province);
            }
        }

        // Navigation Properties
        public virtual ICollection<Registration> Registration { get; set; }

        public  virtual GradePointState GradePointState { get; set; }

        public virtual AcademicProgram AcademicProgram { get; set; }


        /// <summary>
        /// Ensures that a student is always associated with the correct grade point state.
        /// </summary>
        public void ChangeState()
        {
            int currentGradePointStateId = this.GradePointStateId;
            GradePointState before;
            do
            {
                before = db.GradePointStates.Find(currentGradePointStateId);

                before.StateChangeCheck(this);

                currentGradePointStateId = this.GradePointStateId;

            } while (currentGradePointStateId != before.GradePointStateId);
        }

        /// <summary>
        /// Sets the StudentNumber property to the appropriate value returned from the NextNumber method. 
        /// </summary>
        public void SetNextStudentNumber()
        {
            string discriminator = "NextStudent";
            long? nextNumber = StoredProcedure.NextNumber(discriminator);

            if (nextNumber.HasValue)
            {
                this.StudentNumber = (long)nextNumber;
            }
        }
    }

    /// <summary>
    /// Registration Model. Represents the Registration table in the database.
    /// </summary>
    public class Registration
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RegistrationId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Display(Name = "Registration\nNumber")]
        public long RegistrationNumber { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime RegistrationDate { get; set; }

        [DisplayFormat(NullDisplayText = "Ungraded")]
        [Range(0, 1)]
        public double? Grade { get; set; }

        public string Notes { get; set; }

        // Navigation Properties
        public virtual Course Course { get; set; }

        public virtual Student Student { get; set; }

        /// <summary>
        /// Sets the RegistrationNumber property to the appropriate value returned from the NextNumber method.
        /// </summary>
        public void SetNextRegistrationNumber()
        {
            string discriminator = "NextRegistration";
            long? nextNumber = StoredProcedure.NextNumber(discriminator);

            if (nextNumber.HasValue)
            {
                this.RegistrationNumber = (long)nextNumber;
            }
        }
    }

    /// <summary>
    /// CourseModel. Represents the Course table in the database.
    /// </summary>
    public abstract class Course
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CourseId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name  = "Course\nNumber")]
        public string CourseNumber { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Credit\nHours")]
        public double CreditHours { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Tuition")]
        public double TuitionAmount { get; set; }

        [Display(Name = "Course\nType")]
        public string CourseType 
        { 
            get
            {
                return BusinessRules.ParseString(GetType().Name, "Course");
            }
        }

        public string Notes { get; set; }

        // Navigation properties
        public virtual AcademicProgram AcademicProgram { get; set; }

        public virtual ICollection<Registration> Registration { get; set; }

        /// <summary>
        /// Sets the CourseNumber property to the appropriate value.
        /// </summary>
        public abstract void SetNextCourseNumber();

    }

    /// <summary>
    /// GradedCourse Model. Represents the GradedCourse table in the database.
    /// </summary>
    public class GradedCourse : Course
    {
        [Required]
        [Display(Name = "Assignments")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double AssignmentWeight { get; set; }

        [Required]
        [Display(Name = "Exams")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double ExamWeight { get; set; }

        /// <summary>
        /// Sets the CourseNumber property to the appropriate value.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            string discriminator = "NextGradedCourse";
            long? nextNumber = StoredProcedure.NextNumber(discriminator);

            if(nextNumber.HasValue)
            {
                this.CourseNumber = $"G-{nextNumber}";
            }
        }
    }

    /// <summary>
    /// AuditCourse Model. Represents the AuditCourse table in the database.
    /// </summary>
    public class AuditCourse : Course
    {
        /// <summary>
        /// Sets the CourseNumber property to the appropriate value.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            string discriminator = "NextAuditCourse";
            long? nextNumber = StoredProcedure.NextNumber(discriminator);

            if (nextNumber.HasValue)
            {
                this.CourseNumber = $"A-{nextNumber}";
            }
        }
    }

    /// <summary>
    /// MasteryCourse Model. Represents the MasteryCourse table in the database.
    /// </summary>
    public class MasteryCourse : Course
    {
        [Required]
        [Display(Name = "Maximum\nAttempts")]
        public int MaximumAttempts { get; set; }

        /// <summary>
        /// Sets the CourseNumber property to the appropriate value.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            string discriminator = "NextMasteryCourse";
            long? nextNumber = StoredProcedure.NextNumber(discriminator);

            if (nextNumber.HasValue)
            {
                this.CourseNumber = $"M-{nextNumber}";
            }
        }
    }

    /// <summary>
    /// GradePointState Model. Represents the GradePointState table in the database.
    /// </summary>
    public abstract class GradePointState
    {
        protected static BITCollege_FVContext db = new BITCollege_FVContext();

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int GradePointStateId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Lower\nLimit")]
        public double LowerLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Upper\nLimit")]
        public double UpperLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Tuition Rate\nFactor")]
        public double TuitionRateFactor { get; set; }

        [Display(Name = "State")]
        public string Description 
        { 
            get
            {
                return BusinessRules.ParseString(GetType().Name, "State");
            }
        }

        // Navigation Properties
        public virtual ICollection<Student> student { get; set; }

        /// <summary>
        /// Adjusts the tuition rate of a student according to their grade point state and average.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        /// <returns>The tuition rate factor.</returns>
        public abstract double TuitionRateAdjustment(Student student);

        /// <summary>
        /// Checks when the grade point state of a student is updated to the state below or above it.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        public abstract void StateChangeCheck(Student student);

    }

    /// <summary>
    /// RegularState Model. Represents the RegularState table in the database.
    /// </summary>
    public class RegularState : GradePointState
    {
        private static RegularState regularState;

        private const double RegularLowerLimit = 2.00;
        private const double RegularUpperLimit = 3.70;
        private const double RegularTuitionRateFactor = 1.0;

        private RegularState()
        {
            this.LowerLimit = RegularLowerLimit;
            this.UpperLimit = RegularUpperLimit;
            this.TuitionRateFactor = RegularTuitionRateFactor;
        }

        /// <summary>
        /// Returns the only object of the RegularState class.
        /// </summary>
        /// <returns>The private instance of the RegularState class.</returns>
        public static RegularState GetInstance()
        {
            if (regularState == null)
            {
                BITCollege_FVContext db = new BITCollege_FVContext();

                regularState = db.RegularStates.SingleOrDefault();

                if (regularState == null)
                {
                    regularState = new RegularState();

                    db.RegularStates.Add(regularState);

                    db.SaveChanges();
                }
            }
            return regularState;
        }

        /// <summary>
        /// Realizes a change from one Grade Point State to the next.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < RegularLowerLimit )
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            }
            else if (student.GradePointAverage > RegularUpperLimit)
            {
                student.GradePointStateId = HonoursState.GetInstance().GradePointStateId;
            }
        }

        /// <summary>
        /// Determines the discount or premium to the actual cost of tuition
        /// based on the grade point state of a student.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        /// <returns>the tuition rate factor.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localRateValue = RegularTuitionRateFactor;

            return localRateValue;
        }
        
    }

    /// <summary>
    /// HonoursState Model. Represents the HonoursState table in the database.
    /// </summary>
    public class HonoursState : GradePointState
    {
        private static HonoursState honoursState;

        private const double HonoursLowerLimit = 3.70;
        private const double HonoursUpperLimit = 4.50;
        private const double HonoursTuitionRateFactor = 0.9;

        /// <summary>
        /// Initializes a new instance of the HonoursState class.
        /// </summary>
        private HonoursState()
        {
            this.LowerLimit = HonoursLowerLimit;
            this.UpperLimit = HonoursUpperLimit;
            this.TuitionRateFactor = HonoursTuitionRateFactor;
        }

        /// <summary>
        /// Returns the only object of the HonoursState class.
        /// </summary>
        /// <returns>The private instance of the HonoursState class.</returns>
        public static HonoursState GetInstance()
        {
            if (honoursState == null)
            {
                BITCollege_FVContext db = new BITCollege_FVContext();

                honoursState = db.HonoursStates.SingleOrDefault();

                if (honoursState == null)
                {
                    honoursState = new HonoursState();

                    db.HonoursStates.Add(honoursState);

                    db.SaveChanges();
                }
            }
            return honoursState;
        }

        /// <summary>
        /// Realizes a change from one Grade Point State to the next.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < HonoursLowerLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
            }

        }

        /// <summary>
        /// Determines the discount or premium to the actual cost of tuition
        /// based on the grade point state of a student.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        /// <returns></returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localRateValue = HonoursTuitionRateFactor;

            IQueryable<Registration> studentCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                                      && x.Grade != null);
            int courseCount = studentCourses.Count();

            if (courseCount >= 5)
            {
                IQueryable<Registration> newCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                                     && x.Grade == null);

                int newCoursesCount = newCourses.Count();

                localRateValue -= newCoursesCount * 0.05;
            }

            if (student.GradePointAverage > 4.25)
            {
                localRateValue -= 0.02;
            }

            return localRateValue;
        }
    }

    /// <summary>
    /// SuspendedState Model. Represents the SuspendedState model in the database.
    /// </summary>
    public class SuspendedState : GradePointState
    {
        private static SuspendedState suspendedState;

        private const double SuspendedLowerLimit = 0.00;
        private const double SuspendedUpperLimit = 1.00;
        private const double SuspendedTuitionRateFactor = 1.1;

        /// <summary>
        /// Initializes a new instance of the SuspendedState class.
        /// </summary>
        private SuspendedState()
        {
            this.LowerLimit = SuspendedLowerLimit;
            this.UpperLimit = SuspendedUpperLimit;
            this.TuitionRateFactor = SuspendedTuitionRateFactor;
        }

        /// <summary>
        /// Returns the only object of the SuspendedState class.
        /// </summary>
        /// <returns>The private instance of the SuspendedState class.</returns>
        public static SuspendedState GetInstance()
        {
            if (suspendedState == null)
            {
                BITCollege_FVContext db = new BITCollege_FVContext();

                suspendedState = db.SuspendedStates.SingleOrDefault();

                if (suspendedState == null)
                {
                    suspendedState = new SuspendedState();

                    db.SuspendedStates.Add(suspendedState);

                    db.SaveChanges();
                }
            }
            return suspendedState;
        }

        /// <summary>
        /// Realizes a change from one Grade Point State to the next.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage > SuspendedUpperLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            }
        }

        /// <summary>
        /// Determines the discount or premium to the actual cost of tuition
        /// based on the grade point state of a student.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        /// <returns>The tuition rate factor.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localRateValue = SuspendedTuitionRateFactor;

            IQueryable<Registration> newCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                     && x.Grade == null);

            int newCoursesCount = newCourses.Count();

            if (student.GradePointAverage < 0.50)
            {

                localRateValue = SuspendedTuitionRateFactor + (newCoursesCount * 0.05);
            }
            else if (student.GradePointAverage < 0.75)
            {
                localRateValue = SuspendedTuitionRateFactor + (newCoursesCount * 0.02);
            }

            return localRateValue;
        }
    }

    /// <summary>
    /// ProbationState Model. Represents the ProbationState table in the database.
    /// </summary>
    public class ProbationState : GradePointState
    {
        private static ProbationState probationState;

        private const double ProbationLowerLimit = 1.00;
        private const double ProbationUpperLimit = 2.00;
        private const double ProbationTuitionRateFactor = 1.075;

        /// <summary>
        /// Initializes a new instance of the ProbationState class.
        /// </summary>
        private ProbationState()
        {
            this.LowerLimit = ProbationLowerLimit;
            this.UpperLimit = ProbationUpperLimit;
            this.TuitionRateFactor = ProbationTuitionRateFactor;
        }

        /// <summary>
        /// Returns the only object of the ProbationState class.
        /// </summary>
        /// <returns>The private instance of the SuspendedState class.</returns>
        public static ProbationState GetInstance()
        {
            if (probationState == null)
            {
                BITCollege_FVContext db = new BITCollege_FVContext();

                probationState = db.ProbationStates.SingleOrDefault();

                if (probationState == null)
                {
                    probationState = new ProbationState();

                    db.ProbationStates.Add(probationState);

                    db.SaveChanges();
                }
            }
            return probationState;
        }

        /// <summary>
        /// Realizes a change from one Grade Point State to another.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < ProbationLowerLimit)
            {
                student.GradePointStateId = SuspendedState.GetInstance().GradePointStateId;
            }
            else if (student.GradePointAverage > ProbationUpperLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
            }
        }

        /// <summary>
        /// Determines the discount or premium to the actual cost of tuition
        /// based on the grade point state of a student.
        /// </summary>
        /// <param name="student">An instance of the Student class.</param>
        /// <returns>The tuition rate factor.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localRateValue = ProbationTuitionRateFactor;

            IQueryable<Registration> studentCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                     && x.Grade != null);

            int coursesCount = studentCourses.Count();

            if (coursesCount >= 5)
            {
                localRateValue = 1.035;
            }
            return localRateValue;
        }

    }

    /// <summary>
    /// NextUniqueNumber model. Represents the NextUniqueNumber table in the database.
    /// </summary>
    public abstract class NextUniqueNumber
    {
        protected static BITCollege_FVContext db = new BITCollege_FVContext();

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int NextUniqueNumberId { get; set; }

        [Required]
        public long NextAvailableNumber { get; set; }
    }
    
    /// <summary>
    /// NextStudent Model. Represents the NextStudent table in the database.
    /// </summary>
    public class NextStudent : NextUniqueNumber
    {
        private static NextStudent nextStudent;

        /// <summary>
        /// Initializes a new instance of the NextStudent class.
        /// </summary>
        private NextStudent()
        {
            this.NextAvailableNumber = 20000000;
        }

        /// <summary>
        /// Returns the only object of the NextStudent class.
        /// </summary>
        /// <returns>The private instance of the NextStudent class.</returns>
        public static NextStudent GetInstance()
        {
            if (nextStudent == null)
            {
                BITCollege_FVContext db = new BITCollege_FVContext();

                nextStudent = db.NextStudents.SingleOrDefault();

                if (nextStudent == null)
                {
                    nextStudent = new NextStudent();

                    db.NextStudents.Add(nextStudent);

                    db.SaveChanges();
                }
            }
            return nextStudent;
        }

    }

    /// <summary>
    /// NextRegistration Model. Represents the NextRegistation table in the database.
    /// </summary>
    public class NextRegistration : NextUniqueNumber
    {
        private static NextRegistration nextRegistration;

        /// <summary>
        /// Initializes a new instance of the NextRegistration class.
        /// </summary>
        private NextRegistration()
        {
            this.NextAvailableNumber = 700;
        }

        /// <summary>
        /// Returns the only object of the NextRegistration class.
        /// </summary>
        /// <returns>The private instance of the NextRegistration class.</returns>
        public static NextRegistration GetInstance()
        {
            if (nextRegistration == null)
            {
                BITCollege_FVContext db = new BITCollege_FVContext();

                nextRegistration = db.NextRegistrations.SingleOrDefault();

                if (nextRegistration == null)
                {
                    nextRegistration = new NextRegistration();

                    db.NextRegistrations.Add(nextRegistration);

                    db.SaveChanges();
                }
            }
            return nextRegistration;
        }
    }

    /// <summary>
    /// NextGradedCourse Model. Represents the NextGradedCourse table in the database.
    /// </summary>
    public class NextGradedCourse : NextUniqueNumber
    {
        private static NextGradedCourse nextGradedCourse;

        /// <summary>
        /// Initializes a new instance of the NextGradedCourse class.
        /// </summary>
        private NextGradedCourse()
        {
            this.NextAvailableNumber = 200000;
        }

        /// <summary>
        /// Returns the only object of the NextGradedCourse class.
        /// </summary>
        /// <returns>The private instance of the NextGradedCourse class.</returns>
        public static NextGradedCourse GetInstance()
        {
            if (nextGradedCourse == null)
            {
                BITCollege_FVContext db = new BITCollege_FVContext();

                nextGradedCourse = db.NextGradedCourses.SingleOrDefault();

                if (nextGradedCourse == null)
                {
                    nextGradedCourse = new NextGradedCourse();

                    db.NextGradedCourses.Add(nextGradedCourse);

                    db.SaveChanges();
                }
            }
            return nextGradedCourse;
        }
    }

    /// <summary>
    /// NextAuditCourse Model. Represents the NextAuditCourse table in the database.
    /// </summary>
    public class NextAuditCourse : NextUniqueNumber
    {
        private static NextAuditCourse nextAuditCourse;

        /// <summary>
        /// Initializes a new instance of the NextAuditCourse class.
        /// </summary>
        private NextAuditCourse()
        {
            this.NextAvailableNumber = 2000;
        }

        /// <summary>
        /// Returns the only object of the NextAuditCourse class.
        /// </summary>
        /// <returns>The private instance of the NextAuditCourse class.</returns>
        public static NextAuditCourse GetInstance()
        {
            if (nextAuditCourse == null)
            {
                BITCollege_FVContext db = new BITCollege_FVContext();

                nextAuditCourse = db.NextAuditCourses.SingleOrDefault();

                if (nextAuditCourse == null)
                {
                    nextAuditCourse = new NextAuditCourse();

                    db.NextAuditCourses.Add(nextAuditCourse);

                    db.SaveChanges();
                }
            }
            return nextAuditCourse;
        }
    }

    /// <summary>
    /// NextMasteryCourse Model. Represents the NextMasteryCourse table in the database.
    /// </summary>
    public class NextMasteryCourse : NextUniqueNumber
    {
        private static NextMasteryCourse nextMasteryCourse;

        /// <summary>
        /// Initializes a new instance of the NextAuditCourse class.
        /// </summary>
        private NextMasteryCourse()
        {
            this.NextAvailableNumber = 20000;
        }

        /// <summary>
        /// Returns the only object of the NextMasteryCourse class.
        /// </summary>
        /// <returns>The private instance of the NextMasteryCourse class.</returns>
        public static NextMasteryCourse GetInstance()
        {
            if (nextMasteryCourse == null)
            {
                BITCollege_FVContext db = new BITCollege_FVContext();

                nextMasteryCourse = db.NextMasteryCourses.SingleOrDefault();

                if (nextMasteryCourse == null)
                {
                    nextMasteryCourse = new NextMasteryCourse();

                    db.NextMasteryCourses.Add(nextMasteryCourse);

                    db.SaveChanges();
                }
            }
            return nextMasteryCourse;
        }
    }

    /// <summary>
    /// StoredProcedure Model. Used to execute SQL Server stored procedures.
    /// </summary>
    public static class StoredProcedure
    {
        public static long? NextNumber(string discriminator)
        {
            try
            {
                long? returnValue = 0;
                SqlConnection connection = new SqlConnection("Data Source=LAPTOP-KC5UKCA1\\VENUS; " +
                "Initial Catalog=BITCollege_FVContext;Integrated Security=True");
                SqlCommand storedProcedure = new SqlCommand("next_number", connection);
                storedProcedure.CommandType = CommandType.StoredProcedure;
                storedProcedure.Parameters.AddWithValue("@Discriminator", discriminator);
                SqlParameter outputParameter = new SqlParameter("@NewVal", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                storedProcedure.Parameters.Add(outputParameter);
                connection.Open();
                storedProcedure.ExecuteNonQuery();
                connection.Close();
                returnValue = (long?)outputParameter.Value;
                return returnValue;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}