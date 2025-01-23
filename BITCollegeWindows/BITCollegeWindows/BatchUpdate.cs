/*
 * Name: Faye Vaquilar
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-12-09
 * Updated: 2023-12-14
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
    public partial class BatchUpdate : Form
    {
        private Batch batch = new Batch();
        private BITCollege_FVContext db = new BITCollege_FVContext();

        /// <summary>
        /// Initializes an instance of the BatchUpdate class.
        /// </summary>
        public BatchUpdate()
        {
            InitializeComponent();
            academicProgramComboBox.Enabled = false;
        }

        /// <summary>
        /// Handles the Batch processing
        /// </summary>
        private void lnkProcess_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (radSelect.Checked)
            {
                batch.ProcessTransmission(academicProgramComboBox.SelectedValue.ToString());
            }
            else if (radAll.Checked)
            {
                foreach (AcademicProgram program in academicProgramComboBox.Items)
                {
                    string programAcronym = program.ProgramAcronym;
                    batch.ProcessTransmission(programAcronym.ToString());
                }
            }

            string result = batch.WriteLogData();
            rtxtLog.Text += result;
        }

        /// <summary>
        /// Loads BatchUpdate form with data from the AcademicPrograms table in the database.
        /// </summary>
        private void BatchUpdate_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            IQueryable<AcademicProgram> academicPrograms = db.AcademicPrograms;
            List<AcademicProgram> programList = academicPrograms.ToList();

            academicProgramBindingSource.DataSource = programList;

        }

        /// <summary>
        /// Handles the radio button checked selection.
        /// </summary>
        private void radSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (radSelect.Checked)
            {
                academicProgramComboBox.Enabled = true;
            }
            else
            {
                academicProgramComboBox.Enabled = false;
            }
        }
    }
}
