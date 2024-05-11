﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Static_Resources;

namespace WindowsFormsApp1.AllForms.Passenger
{
    public partial class passengerCheckProfile : Form
    {
        string conStr = UserFunctions.connectionString;
        private string emailFromPassenger = "umair@rmsdb.com";
        public passengerCheckProfile()
        {
            InitializeComponent();
        }
        public passengerCheckProfile(string _email)
        {
            InitializeComponent();
            emailFromPassenger = _email;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (passengerPassword.PasswordChar == '*')
            {
                passengerPassword.PasswordChar = (char)0;
                pictureBox1.Image = Properties.Resources.icons8_eye_25;
            }
            else
            {
                passengerPassword.PasswordChar = '*';
                pictureBox1.Image = Properties.Resources.icons8_eye_25__1_;
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                Dashboard.getDashboard().Show();
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            string p_name = passengerName.Text.Trim();
            string p_email_id = passengerEmail.Text.Trim();
            string p_password = passengerPassword.Text.Trim();
            string p_cnic = passengerCNIC.Text;
            string p_ph = passengerPh.Text.Trim();

            string sql = "UPDATE Passenger SET p_NAME = :name, p_EMAIL_ID = :Email, p_PASSWORD = :Password, p_cnic = :cnic, p_phone_number = :phone WHERE p_EMAIL_ID = :OldEmail";

            using (OracleConnection connection = new OracleConnection(conStr))
            using (OracleCommand cmd = new OracleCommand(sql, connection))
            {
                cmd.Parameters.Add(":name", OracleDbType.Varchar2).Value = p_name;
                cmd.Parameters.Add(":Email", OracleDbType.Varchar2).Value = p_email_id;
                cmd.Parameters.Add(":Password", OracleDbType.Varchar2).Value = p_password;
                cmd.Parameters.Add(":cnic", OracleDbType.Varchar2).Value = p_cnic;
                cmd.Parameters.Add(":phone", OracleDbType.Varchar2).Value = p_ph;
                cmd.Parameters.Add(":OldEmail", OracleDbType.Varchar2).Value = emailFromPassenger;

                try
                {
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Passenger information updated successfully.");
                        // Update the emailFromPassenger if the email is changed
                        emailFromPassenger = p_email_id;
                    }
                    else
                    {
                        MessageBox.Show("No Passenger found with the given email.");
                    }
                }
                catch (OracleException ex)
                {
                    MessageBox.Show("An error occurred while updating Passenger information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void passengerCheckProfile_Load(object sender, EventArgs e)
        {
            string sql = "SELECT P_NAME, P_PASSWORD, P_CNIC , P_PHONE_NUMBER FROM PASSENGER WHERE P_EMAIL_ID = :Email";

            using (OracleConnection connection = new OracleConnection(conStr))
            using (OracleCommand cmd = new OracleCommand(sql, connection))
            {
                cmd.Parameters.Add(":Email", OracleDbType.Varchar2).Value = emailFromPassenger.ToString().Trim();

                try
                {
                    connection.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string pName = reader["P_NAME"].ToString();
                            string pPassword = reader["P_PASSWORD"].ToString();
                            string pCNIC = reader["P_CNIC"].ToString();
                            string pPH = reader["P_PHONE_NUMBER"].ToString();

                            passengerName.Text = pName;
                            passengerPassword.Text = pPassword;
                            passengerEmail.Text = emailFromPassenger;
                            passengerCNIC.Text = pCNIC;
                            passengerPh.Text = pPH;
                        }
                        else
                        {
                            MessageBox.Show("Passenger information not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (OracleException ex)
                {
                    MessageBox.Show("An error occurred while retrieving Passenger information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
