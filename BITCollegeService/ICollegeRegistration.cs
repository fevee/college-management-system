/*
 * Name: Faye Vaquilar
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-11-10
 * Updated: 2023-11-20
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICollegeRegistration" in both code and config file together.
    [ServiceContract]
    public interface ICollegeRegistration
    {
        [OperationContract]
        void DoWork();

        /// <summary>
        /// Implementation will remove the argument's record from the database and persist the change.
        /// </summary>
        /// <param name="registrationId">Represents the registration ID.</param>
        /// <returns>Returns a value of false if an exception occurs, otherwise returns true.</returns>
        [OperationContract]
        bool DropCourse(int registrationId);

        /// <summary>
        /// Implementation will make use of various return codes to indicate success or failure of a course registration.
        /// </summary>
        /// <param name="studentId">Represents the Student ID</param>
        /// <param name="courseId">Represents the Course ID</param>
        /// <param name="notes">Represents registration notes.</param>
        /// <returns>Returns the code indicating success or reason of failure.</returns>
        [OperationContract]
        int RegisterCourse(int studentId, int courseId, String notes);

        /// <summary>
        /// Sets the Grade property of the Registration record to value of the grade argument.
        /// </summary>
        /// <param name="grade">Represents the course grade.</param>
        /// <param name="registrationId">Represents the Registration ID.</param>
        /// <param name="notes">Represents the notes.</param>
        /// <returns>Returns the calculated grade point average</returns>
        [OperationContract]
        double? UpdateGrade(double grade, int registrationId, String notes);
    }
}
