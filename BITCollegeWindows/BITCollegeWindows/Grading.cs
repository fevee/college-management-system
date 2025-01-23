/*
 * Name: Faye Vaquilar
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-11-24
 * Updated: 2023-11-27
 */

using BITCollege_FV.Data;
using BITCollegeWindows.CollegeRegistrationService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace BITCollegeWindows
{
    public partial class Grading : Form
    {
        BITCollege_FVContext db = new BITCollege_FVContext();
        CollegeRegistrationClient registrationClient = new CollegeRegistrationClient();

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
        public Grading(ConstructorData constructor)
        {
            InitializeComponent();

            constructorData = constructor;

            studentNumberMaskedLabel.Text = constructorData.StudentData.StudentNumber.ToString(); 
            fullNameLabel1.Text = constructorData.StudentData.FullName;
            descriptionLabel1.Text = constructorData.StudentData.AcademicProgram.Description;

            courseNumberMaskedLabel.Text = constructorData.RegistrationData.Course.CourseNumber;
            titleLabel1.Text = constructorData.RegistrationData.Course.Title;
            courseTypeLabel1.Text = constructorData.RegistrationData.Course.CourseType;
            // if Grade is not null, use its value formatted as a percentage with two decimal places; otherwise, use 0 as the default value.
            gradeTextBox.Text = $"{(constructorData.RegistrationData.Grade ?? 0):P2}";
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
        /// given:  Always open in this form in the top right corner of the frame.
        /// further code required:
        /// </summary>
        private void Grading_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            courseNumberMaskedLabel.Mask = Utility.BusinessRules.CourseFormat(constructorData.RegistrationData.Course.CourseType);

            bool gradeEntered = constructorData.RegistrationData.Grade.HasValue;

            if(!gradeEntered)
            {
                gradeTextBox.Enabled = true;
                lnkUpdate.Enabled = true;
            }
            else
            {
                lblExisting.Visible = true;
            }
        }

        /// <summary>
        /// Handles the logic for updating a student grade
        /// </summary>
        private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string unformattedGrade = Utility.Numeric.ClearFormatting(gradeTextBox.Text, "%");
            if (!Utility.Numeric.IsNumeric(unformattedGrade, System.Globalization.NumberStyles.Any))
            {
                MessageBox.Show("Invalid grade. Please enter a valid numeric value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                double numericGrade = Convert.ToDouble(unformattedGrade);
                if (numericGrade >= 0 && numericGrade <= 1)
                {
                    gradeTextBox.Enabled = false;
                    gradeTextBox.Text = (numericGrade).ToString("P2");

                    double? updatedGPA = registrationClient
                        .UpdateGrade(numericGrade, constructorData.RegistrationData.RegistrationId, "");
                }
                else
                {
                    MessageBox.Show("Invalid range. Please enter a value between 0 and 1.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
