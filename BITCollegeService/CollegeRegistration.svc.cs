/*
 * Name: Faye Vaquilar
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-11-10
 * Updated: 2023-11-20
 */

using BITCollege_FV.Data;
using BITCollege_FV.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Utility;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CollegeRegistration" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CollegeRegistration.svc or CollegeRegistration.svc.cs at the Solution Explorer and start debugging.
    public class CollegeRegistration : ICollegeRegistration
    {
        BITCollege_FVContext db = new BITCollege_FVContext();

        public void DoWork()
        {
        }

        /// <summary>
        /// Implementation will remove the argument's record from the database and persist the change.
        /// </summary>
        /// <param name="registrationId">Represents the registration ID.</param>
        /// <returns>Returns a value of false if an exception occurs, otherwise returns true.</returns>
        public bool DropCourse(int registrationId)
        {
            Registration registrationToRemove = db.Registrations.FirstOrDefault(x => x.RegistrationId == registrationId);
            try
            {
                db.Registrations.Remove(registrationToRemove);
                db.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Implementation will make use of various return codes to indicate success or failure of a course registration.
        /// </summary>
        /// <param name="studentId">Represents the Student ID</param>
        /// <param name="courseId">Represents the Course ID</param>
        /// <param name="notes">Represents registration notes.</param>
        /// <returns>Returns the code indicating success or reason of failure.</returns>
        public int RegisterCourse(int studentId, int courseId, String notes)
        {
            IQueryable<Registration> registrationQuery = db.Registrations
                .Where(x => x.StudentId == studentId && x.CourseId == courseId);

            Course courseQuery = db.Courses.FirstOrDefault(x => x.CourseId == courseId);

            Student studentQuery = db.Students.FirstOrDefault(x => x.StudentId == studentId);

            bool hasIncompleteRegistration = registrationQuery.Any(x => x.Grade == null);

            int returnCode = 0;

            if (hasIncompleteRegistration)
            {
                returnCode =  -100;
            }
            else if(courseQuery != null && courseQuery.CourseType == "Mastery")
            {

                MasteryCourse masteryCourse = (MasteryCourse)courseQuery;
                int maximumAttempts = masteryCourse.MaximumAttempts;

                int numberOfRegistrations = registrationQuery.Count();

                if(numberOfRegistrations >= maximumAttempts)
                {
                    returnCode = -200;
                }
            }
            else
            {
                try
                {
                    Registration newRegistration = new Registration
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        Notes = notes,
                        RegistrationDate = DateTime.Today,
                        RegistrationNumber = NextRegistration.GetInstance().NextAvailableNumber,
                    };

                    db.Registrations.Add(newRegistration);
                    db.SaveChanges();

                    double tuitionAmount = courseQuery.TuitionAmount;

                    double rateAdjustment = studentQuery.GradePointState.TuitionRateAdjustment(studentQuery);

                    double adjustedTuition = tuitionAmount * rateAdjustment;

                    studentQuery.OutstandingFees += adjustedTuition;

                    db.SaveChanges();
                }
                catch (Exception)
                {
                    returnCode = -300;
                }
            }
            return returnCode;
        }

        /// <summary>
        /// Sets the Grade property of the Registration record to value of the grade argument.
        /// </summary>
        /// <param name="grade">Represents the course grade.</param>
        /// <param name="registrationId">Represents the Registration ID.</param>
        /// <param name="notes">Represents the notes.</param>
        /// <returns>Returns the calculated grade point average</returns>
        public double? UpdateGrade(double grade, int registrationId, String notes)
        {
            Registration registrationUpdateQuery = db.Registrations.FirstOrDefault(x => x.RegistrationId == registrationId);

            registrationUpdateQuery.Grade = grade;

            registrationUpdateQuery.Notes = notes;

            db.SaveChanges();

            double? calculatedGradePointAverage = CalculateGradePointAverage(registrationUpdateQuery.StudentId);

            return calculatedGradePointAverage;
        }

        /// <summary>
        /// Calculates the student's grade point average.
        /// </summary>
        /// <param name="studentId">Represents the Student ID.</param>
        /// <returns>Returns the students calculated grade point average.</returns>
        private double? CalculateGradePointAverage(int studentId)
        {
            double totalCreditHours = 0;
            double totalGradePointValue = 0;
            double? calculatedGradePointAverage;

            IQueryable<Registration> registrationsQuery = db.Registrations
                .Where(x => x.StudentId == studentId && x.Grade != null);

            foreach(Registration record in registrationsQuery.ToList())
            {
                double grade = record.Grade.Value;

                CourseType courseType = BusinessRules.CourseTypeLookup(record.Course.CourseType);

                double gradePoint = BusinessRules.GradeLookup(grade, courseType);
                double gradePointValue = gradePoint * record.Course.CreditHours;

                if (courseType != CourseType.AUDIT)
                {
                    totalGradePointValue += gradePointValue;
                    totalCreditHours += record.Course.CreditHours;
                }
            }

            calculatedGradePointAverage = totalCreditHours == 0 ? (double?)null : totalGradePointValue / totalCreditHours;

            Student studentUpdateQuery = db.Students.FirstOrDefault(x => x.StudentId == studentId);
            studentUpdateQuery.GradePointAverage = calculatedGradePointAverage;
            studentUpdateQuery.GradePointState.StateChangeCheck(studentUpdateQuery);

            db.SaveChanges();

            return calculatedGradePointAverage;
        }
    }
}
