﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Alarm_Application.Properties;

namespace Alarm_Application
{
    public partial class frmAlarmList : Form
    {
        private bool first;
        DataTable tbl;
        DataView dtView;
        BindingSource bs;
        public frmAlarmList()
        {
            InitializeComponent();
            tbl = new DataTable();
            bs = new BindingSource();
            tmr.Start();
            first = true;
            lblName.Text = Settings.Default.username;
            dataGridView1.DataSource = bs;
            
        }
        
        private DataTable getAlarm()
        {
            DataTable dt = new DataTable();
            try
            {
                string cmd = "Select * FROM dbo.ALARM";
                string connectionString = Settings.Default.ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(cmd, con))
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(dt);
                    }
                    con.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("An error occurd when trying to connect to database", "Error");
            }
            return dt;
        }
        private DataView viewAlarms(DataTable dt)
        {
            DataView dv = new DataView(dt);
            dv.RowFilter = "Acknowledged = 0";
            return dv; 
        }
        private void acknowledgeAlarm(int alarmID)
        {

            string cmd = "UPDATE dbo.ALARM SET AcknowledgeTime = @time, AcknowledgePerson = @person,Acknowledged = @aBit WHERE AlarmID = @Aid";
            SqlCommand command = new SqlCommand();
            command.CommandText = cmd;
            command.Parameters.AddWithValue("@time", DateTime.Now);
            command.Parameters.AddWithValue("@person",Settings.Default.username);
            command.Parameters.AddWithValue("@Aid", alarmID);
            command.Parameters.AddWithValue("@aBit", 1);
            string connectionString = Settings.Default.ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                command.Connection = con;
                    command.ExecuteNonQuery();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            tbl = getAlarm();
            dtView = viewAlarms(tbl);
            if (first == true)
            {
                setupDGV();   
            }

            int rowIndex = dataGridView1.FirstDisplayedScrollingRowIndex;
            int collumnIndex = dataGridView1.FirstDisplayedScrollingColumnIndex;
            bs.DataSource = dtView;
            if(dataGridView1.RowCount >= 1)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = rowIndex;
                dataGridView1.FirstDisplayedScrollingColumnIndex = collumnIndex;
            }

            

           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowNumb = dataGridView1.CurrentCell.RowIndex;
            int alarmId = Convert.ToInt16(dataGridView1.Rows[rowNumb].Cells[1].Value);
            acknowledgeAlarm(alarmId);
        }
        private void setupDGV()
        {
            dataGridView1.AutoGenerateColumns = false;
            
            dataGridView1.Columns.Add(AlarmID);
            DataGridViewCheckBoxColumn check = new DataGridViewCheckBoxColumn();
            dataGridView1.Columns.Add(check);
            dataGridView1.Columns[0].HeaderText = "Acknowledge Alarm";
            dataGridView1.Columns[0].Width = 80;
            first = false;
            dataGridView1.AllowUserToAddRows = false;
        }
 
    }
    
}
