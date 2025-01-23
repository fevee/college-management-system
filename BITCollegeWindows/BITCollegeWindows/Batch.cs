/*
 * Name: Faye Vaquilar
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-12-09
 * Updated: 2023-12-14
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Data.Entity;
using BITCollege_FV.Data;
using System.Runtime.Remoting.Contexts;
using BITCollege_FV.Models;
using System.ServiceModel.Configuration;
using System.Globalization;
using Utility;
using BITCollegeWindows.CollegeRegistrationService;
using System.Diagnostics;

namespace BITCollegeWindows
{
    /// <summary>
    /// Batch:  This class provides functionality that will validate
    /// and process incoming xml files.
    /// </summary>
    public class Batch
    {

        private String inputFileName;
        private String logFileName;
        private String logData;

        private BITCollege_FVContext db = new BITCollege_FVContext();


        /// <summary>
        /// Process errors and append relevant information to logData.
        /// </summary>
        /// <param name="beforeQuery">Records before the round of validation.</param>
        /// <param name="afterQuery">Records after the round of validation.</param>
        /// <param name="message">Error message associated with the validation round.</param>
        private void ProcessErrors(IEnumerable<XElement> beforeQuery, IEnumerable<XElement> afterQuery, String message)
        {
            IEnumerable<XElement> errors = beforeQuery.Except(afterQuery);

            foreach (XElement item in errors)
            {
                logData += "\r\n------------------ERROR-------------------";
                logData += "\r\nFile: " + inputFileName;
                logData += "\r\nProgram: " + item.Element("program");
                logData += "\r\nStudent Number: " + item.Element("student_no");
                logData += "\r\nCourse Number: " + item.Element("course_no");
                logData += "\r\nRegistration Number: " + item.Element("registration_no");
                logData += "\r\nType: " + item.Element("type");
                logData += "\r\nGrade: " + item.Element("grade");
                logData += "\r\nNotes: " + item.Element("notes");
                logData += "\r\nNode Count: " + item.Nodes().Count();
                logData += "\r\nError Message: " + message;
                logData += "\r\n-------------------------------------------------";
            }
        }

        /// <summary>
        /// Reads and validates attributes of the input file.
        /// </summary>
        private void ProcessHeader()
        {
            XDocument xDocument = XDocument.Load(inputFileName);

            XElement root = xDocument.Element("student_update");

            string dateString = root.Attribute("date").Value;
            DateTime xmlDate = DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime todayDate = DateTime.Today;

            string programAcronym = root.Attribute("program").Value;
            AcademicProgram matchingProgram = db.AcademicPrograms.FirstOrDefault(x => x.ProgramAcronym == programAcronym);

            XAttribute checksum = root.Attribute("checksum");
            int expectedChecksum = int.Parse(checksum.Value);
            int actualChecksum = root.Elements("transaction")
                .Elements("student_no")
                .Select(y => int.Parse(y.Value))
                .Sum();

            if (root.Attributes().Count() != 3)
            {
                throw new Exception($"Incorrect number of root attributes for file {inputFileName}");
            }

            if (xmlDate.Date != todayDate.Date)
            {
                Console.WriteLine($"xmlDate: {xmlDate}");
                Console.WriteLine($"todayDate: {todayDate}");
                throw new Exception($"Incorrect date for file {inputFileName}");
            }

            if (matchingProgram == null)
            {
                throw new Exception($"Program acronym not found for file {inputFileName}");
            }

            if (actualChecksum != expectedChecksum)
            {
                throw new Exception($"Incorrect checksum value for file {inputFileName}");
            }

        }

        /// <summary>
        /// Reads and validates elements' values from input file.
        /// </summary>
        private void ProcessDetails()
        {
            XDocument xDocument = XDocument.Load(inputFileName);

            IEnumerable<XElement> totalTransactions = xDocument.Descendants().Elements("transaction");

            IEnumerable<XElement> childElements = totalTransactions.Where(x => x.Nodes().Count() == 7);
            ProcessErrors(totalTransactions, childElements, "Incorrect number of child elements.");

            IEnumerable<XElement> programQuery = childElements
                .Where(x => x.Element("program").Value
                .Equals(xDocument.Root.Attribute("program").Value));
            ProcessErrors(childElements, programQuery, "Mismatch between program element and root attribute.");

            IEnumerable<XElement> typeQuery = programQuery
                .Where(x => Numeric.IsNumeric(x.Element("type").Value, NumberStyles.Number)
                    && x.Element("type").Value.Equals("1") || x.Element("type").Value.Equals("2"));
            ProcessErrors(programQuery, typeQuery, "Invalid Type.");

            IEnumerable<XElement> gradeQuery = typeQuery
                .Where(x => x.Element("type").Value.Equals("1") && x.Element("grade").Value.Equals("*")
                    || (x.Element("type").Value.Equals("2") 
                            && double.TryParse(x.Element("grade").Value, out double gradeValue)
                            && gradeValue >= 0 && gradeValue <= 100));
            ProcessErrors(typeQuery, gradeQuery, "Invalid grade. If type 1, enter *");

            IEnumerable<long> allStudentNumbers = db.Students.Select(x => x.StudentNumber);
            IEnumerable<XElement> studentQuery = gradeQuery
                .Where(x => long.TryParse(x.Element("student_no").Value, out long studentNo)
                    && allStudentNumbers.Contains(studentNo));
            ProcessErrors(gradeQuery, studentQuery, "Invalid student number.");

            IEnumerable<string> allCourses = db.Courses.Select(x => x.CourseNumber);
            IEnumerable<XElement> courseQuery = studentQuery
                .Where(x => x.Element("type").Value.Equals("2") && x.Element("course_no").Value.Equals("*")
                    || (x.Element("type").Value.Equals("1") && allCourses.Contains(x.Element("course_no").Value)));
            ProcessErrors(studentQuery, courseQuery, "Invalid course number. If type 2, enter *");

            IEnumerable<long> allRegistrationNumbers = db.Registrations.Select(x => x.RegistrationNumber);
            IEnumerable<XElement> registrationQuery = courseQuery
                .Where(x =>(x.Element("type").Value.Equals("1") && x.Element("registration_no").Value.Equals("*")) 
                    || (x.Element("type").Value.Equals("2") 
                        && long.TryParse(x.Element("registration_no").Value, out long registrationNo)
                            && allRegistrationNumbers.Contains(registrationNo)));
            ProcessErrors(courseQuery, registrationQuery, "Invalid registration number.");

            // Process transactions for the validated query
            ProcessTransactions(registrationQuery);

        }

        // <summary>
        /// Process valid transaction records.
        /// </summary>
        /// <param name="transactionRecords">Collection of transaction elements to process.</param>
        private void ProcessTransactions(IEnumerable<XElement> transactionRecords)
        { 
            CollegeRegistrationClient registrationClient = new CollegeRegistrationClient();

            foreach (XElement transaction in transactionRecords)
            {
                int type = int.Parse(transaction.Element("type").Value);

                string notes = transaction.Element("notes").Value;

                long studentNumber = long.Parse(transaction.Element("student_no").Value);
                int studentId = db.Students
                                .Where(x => x.StudentNumber == studentNumber)
                                .Select(x => x.StudentId)
                                .FirstOrDefault();

                string courseNumber = transaction.Element("course_no").Value;
                int courseId = db.Courses
                               .Where(x => x.CourseNumber == courseNumber)
                               .Select(x => x.CourseId)
                               .FirstOrDefault();

                if (type == 1)
                {
                    int returnCode = registrationClient.RegisterCourse(studentId, courseId, notes);
                    if (returnCode == 0)
                    {

                        logData += $"\r\nStudent: {studentNumber} has successfully registered for course: {courseNumber}";
                    }
                    else
                    {
                        string error = BusinessRules.RegisterError(returnCode);
                        logData += $"\r\nREGISTRATION ERROR: {error}";
                    }
                }
                if (type == 2)
                {
                    double gradeInput = double.Parse(transaction.Element("grade").Value);
                    double validGrade = gradeInput / 100;
                    long registrationNumber = long.Parse(transaction.Element("registration_no").Value);
                    int registrationId = db.Registrations
                                   .Where(x => x.RegistrationNumber == registrationNumber)
                                   .Select(x => x.RegistrationId)
                                   .FirstOrDefault();
                    try
                    {
                        // Call WCF Service to update the student's grade
                        double? calculatedGradePointAverage = registrationClient.UpdateGrade(validGrade, registrationId, notes);

                        if (calculatedGradePointAverage.HasValue)
                        {
                            logData += $"\r\nA grade of: {gradeInput} has been successfully applied to registration {registrationId}.";
                        }
                        else
                        {
                            logData += $"\r\nGrade update failed for Registration ID: {registrationId}.";
                        }
                    }
                    catch (Exception ex)
                    {
                        logData += $"\r\nGrade update failed for Registration ID: {registrationId}. Exception: {ex.Message}";
                    }
                }
            }
        }

        /// <summary>
        /// Writes log data of the input file.
        /// </summary>
        /// <returns>The log data information.</returns>
        public String WriteLogData()
        {
            StreamWriter writer = new StreamWriter(logFileName, false);

            writer.Write(logData);
            writer.Close();

            String capturedData = logData;

            logData = string.Empty;
            logFileName = string.Empty;

            return capturedData;
        }

        /// <summary>
        /// Determines the appropriate filename and then proceeds with the header and detail processing.
        /// </summary>
        /// <param name="programAcronym">The program acronym</param>
        public void ProcessTransmission(String programAcronym)
        {
            string fileName = FormulateFileName(programAcronym);
            inputFileName = fileName + ".xml";
            logFileName = "LOG " + fileName + ".txt";

            // Check if the file exists
            if (File.Exists(inputFileName))
            {
                try
                {
                    ProcessHeader();
                    ProcessDetails();
                }
                catch (Exception ex)
                {
                    logData += $"\r\nException in ProcessTransmission: {ex.Message}";
                }
            }
            else
            {
                logData += $"\r\nFile does not exist: {inputFileName}";
            }

        }

        /// <summary>
        /// Formulates the file name with the correct date and program acronym.
        /// </summary>
        /// <param name="programAcronym">The appropriate program acronym</param>
        /// <returns>The input file name</returns>
        public string FormulateFileName(string programAcronym) =>
            $"{DateTime.Now.Year}-{DateTime.Now.DayOfYear:D3}-{programAcronym}";
    }
}
