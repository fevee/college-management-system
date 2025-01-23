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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class StudentData : Form
    {
        BITCollege_FVContext db = new BITCollege_FVContext();

        ///Given: Student and Registration data will be retrieved
        ///in this form and passed throughout application
        ///These variables will be used to store the current
        ///Student and selected Registration
        ConstructorData constructorData = new ConstructorData();

        /// <summary>
        /// This constructor will be used when this form is opened from
        /// the MDI Frame.
        /// </summary>
        public StudentData()
        {
            InitializeComponent();
            studentNumberMaskedTextBox.Leave += StudentNumberMaskedTextBox_Leave;
        }

        /// <summary>
        /// given:  This constructor will be used when returning to StudentData
        /// from another form.  This constructor will pass back
        /// specific information about the student and registration
        /// based on activites taking place in another form.
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public StudentData (ConstructorData constructor)
        {
            InitializeComponent();
            constructorData = constructor;
            studentNumberMaskedTextBox.Text = constructorData.RegistrationData.Student.StudentNumber.ToString();
            StudentNumberMaskedTextBox_Leave(null, null);

        }

        /// <summary>
        /// Handles the Leave event of the StudentNumberTextBox control.
        /// </summary>
        private void StudentNumberMaskedTextBox_Leave(object sender, EventArgs e)
        {
            if (studentNumberMaskedTextBox.MaskFull)
            {
                long.TryParse(studentNumberMaskedTextBox.Text, out long studentNumber);
                Student matchingStudent = db.Students.Where(x => x.StudentNumber == studentNumber).SingleOrDefault();

                if (matchingStudent != null)
                {
                    studentBindingSource.DataSource = matchingStudent;
                    IQueryable<Registration> registrations = db.Registrations.Where(x => x.StudentId == matchingStudent.StudentId);

                    if (registrations.Any())
                    {
                        registrationBindingSource.DataSource = registrations.ToList();
                        lnkUpdateGrade.Enabled = true;
                        lnkViewDetails.Enabled = true;
                    }
                    else
                    {
                        lnkUpdateGrade.Enabled = false;
                        lnkViewDetails.Enabled = false;
                        registrationBindingSource.DataSource = typeof(Registration);
                    }
                }
                else
                {
                    lnkUpdateGrade.Enabled = false;
                    lnkViewDetails.Enabled = false;

                    studentNumberMaskedTextBox.Focus();

                    studentBindingSource.DataSource = typeof(Student);

                    registrationBindingSource.DataSource = typeof(Registration);

                    MessageBox.Show($"Student {studentNumber} does not exist.", "Invalid Student Number", MessageBoxButtons.OK);
                }
            }

            if(constructorData.RegistrationData != null)
            {
                registrationNumberComboBox.Text = constructorData.RegistrationData.RegistrationNumber.ToString();
            }
        }

        /// <summary>
        /// given: Open grading form passing constructor data.
        /// </summary>
        private void lnkUpdateGrade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData((Student)studentBindingSource.Current, (Registration)registrationBindingSource.Current);
            Grading grading = new Grading(constructorData);
            grading.MdiParent = this.MdiParent;
            grading.Show();
            this.Close();
        }


        /// <summary>
        /// given: Open history form passing constructor data.
        /// </summary>
        private void lnkViewDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData((Student)studentBindingSource.Current, (Registration)registrationBindingSource.Current);
            History history = new History(constructorData);
            history.MdiParent = this.MdiParent;
            history.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Opens the form in top right corner of the frame.
        /// </summary>
        private void StudentData_Load(object sender, EventArgs e)
        {
            //keeps location of form static when opened and closed
            this.Location = new Point(0, 0);
        }

        /// <summary>
        /// Populates data for the constructor.
        /// </summary>
        /// <param name="currentStudent">The current instance of the Student class</param>
        /// <param name="currentRegistration">The current instance of the Registration class</param>
        private void PopulateConstructorData(Student currentStudent, Registration currentRegistration)
        {
             constructorData.StudentData = currentStudent;
             constructorData.RegistrationData = currentRegistration;
        }
    }
}
