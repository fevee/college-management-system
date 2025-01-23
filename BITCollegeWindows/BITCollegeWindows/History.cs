/*
 * Name: Faye Vaquilar
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-11-24
 * Updated: 2023-11-27
 */

using BITCollege_FV.Data;
using BITCollege_FV.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class History : Form
    {
        BITCollege_FVContext db = new BITCollege_FVContext();

        ///given:  student and registration data will passed throughout 
        ///application. This object will be used to store the current
        ///student and selected registration
        ConstructorData constructorData;

        /// <summary>
        /// given:  This constructor will be used when called from the
        /// Student form.  This constructor will receive 
        /// specific information about the student and registration
        /// further code required:  
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public History(ConstructorData constructor)
        {
            InitializeComponent();

            constructorData = constructor;

            studentNumberMaskedLabel.Text = constructorData.RegistrationData.Student.StudentNumber.ToString();
            fullNameLabel1.Text = constructorData.RegistrationData.Student.FullName;
            descriptionLabel1.Text = constructorData.RegistrationData.Student.AcademicProgram.Description;

        }

        /// <summary>
        /// given: This code will navigate back to the Student form with
        /// the specific student and registration data that launched
        /// this form.
        /// </summary>
        private void lnkReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //return to student with the data selected for this form
            StudentData student = new StudentData(constructorData);
            student.MdiParent = this.MdiParent;
            student.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Open this form in top right corner of the frame.
        /// further code required:
        /// </summary>
        private void History_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            Student currentStudent = constructorData.StudentData;

            try
            {
                var registrationsQuery = db.Registrations.Include(x => x.Student)
                    .Join(db.Courses, x => x.CourseId, y => y.CourseId, (x, y) => new
                    {
                        x.RegistrationId,
                        x.RegistrationNumber,
                        x.RegistrationDate,
                        y.Title,
                        x.Grade,
                        x.Notes,
                        x.Student.StudentId
                    }
                    )
                .Where(x => x.StudentId == currentStudent.StudentId)
                .ToList();

                registrationBindingSource.DataSource = registrationsQuery;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
