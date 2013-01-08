using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;

namespace pf8DataEntry
{
    public partial class Form1 : Form
    {

        List<KeyValuePair<string, int>> stationTimingDiff = new List<KeyValuePair<string, int>>();
        

        public Form1()
        {
            InitializeComponent();
            KeyValuePair<string, int> rootvalue = new KeyValuePair<string, int>("Churchgate", 0);

            stationTimingDiff.Add(rootvalue);

            KeyValuePair<string,int> values= new KeyValuePair<string,int>("MarineLines", 3);

            stationTimingDiff.Add(values);
            
            values= new KeyValuePair<string,int>("CharniRoad", 2);
            stationTimingDiff.Add(values);
            values= new KeyValuePair<string,int>("GrantRoad", 3);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("MumbaiCentral", 2);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Mahalaxmi", 3);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("LowerParel", 3);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("ElphinstoneRoad", 3);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Dadar", 2);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Matunga", 2);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Mahim", 3);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Bandra", 4);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Khar", 3);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Santacruz", 2);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("VileParle", 3);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Andheri", 5);
            stationTimingDiff.Add(values);
            
            values=new KeyValuePair<string,int>("Jogeshwari", 3);
            stationTimingDiff.Add(values);
          
            values=new KeyValuePair<string,int>("Goregaon", 5);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Malad", 4);
            stationTimingDiff.Add(values);
            values=new KeyValuePair<string,int>("Kandivali", 3);
            stationTimingDiff.Add(values);
            values = new KeyValuePair<string, int>("Borivali", 0);
            stationTimingDiff.Add(values);
            

        }

        private string timeTrainReachStationFromPrevious(string time, int timeToAdd)
        {
            string output = string.Empty;
            int[] splitTimeInHoursAndMinutes = Array.ConvertAll(time.Split(':'), t => int.Parse(t));
            splitTimeInHoursAndMinutes[1]=splitTimeInHoursAndMinutes[1] + timeToAdd;
            if ( splitTimeInHoursAndMinutes[1] >= 60)
            {

                splitTimeInHoursAndMinutes[1] = splitTimeInHoursAndMinutes[1]  - 60;
                splitTimeInHoursAndMinutes[0] += 1;


            }

            output = splitTimeInHoursAndMinutes[0].ToString() + ":" + splitTimeInHoursAndMinutes[1];
            return output;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string station = comboBox1.SelectedItem.ToString();
            string fromTime = textBox1.Text;
            string toTime = textBox2.Text;
            string platformNo = textBox3.Text;
            //MessageBox.Show(station + fromTime + toTime + platformNo);
            int index=0;
            index=stationTimingDiff.FindIndex(item =>item.Key==station)+1;
            var conn = new SqlCeConnection("Data Source=c:\\documents and settings\\amey\\my documents\\visual studio 2010\\Projects\\pf8DataEntry\\pf8DataEntry\\Database1.sdf");
            conn.Open();

            var startStationQuery = "Insert into Trains (time,platform,station) Values(@fromTime, @platformNo,@station )";
            var startStationCmd = new SqlCeCommand(startStationQuery, conn);
            startStationCmd.Parameters.Add("@fromTime", fromTime);
            startStationCmd.Parameters.Add("@platformNo", platformNo);
            startStationCmd.Parameters.Add("@station", station);
            startStationCmd.ExecuteNonQuery();
 
            for (int i = index; i < stationTimingDiff.Count; i++)
            {

                 fromTime = timeTrainReachStationFromPrevious(fromTime, stationTimingDiff[i].Value);
                 var stationQuery = "Insert into Trains (time,platform,station) Values(@fromTime ,@platformNo,@station)";
                 var stationCmd = new SqlCeCommand(stationQuery, conn);
                 stationCmd.Parameters.Add("@fromTime", fromTime);
                 stationCmd.Parameters.Add("@platformNo", platformNo);
                 stationCmd.Parameters.Add("@station", stationTimingDiff[i].Key);
                 stationCmd.ExecuteNonQuery();


            }
            var lastStationQuery = "Insert into Trains (time,platform,station) Values(@toTime ,@platformNo,@station )";
            var lastStationCmd = new SqlCeCommand(lastStationQuery, conn);
            lastStationCmd.Parameters.Add("@toTime", toTime);
            lastStationCmd.Parameters.Add("@platformNo", platformNo);
            lastStationCmd.Parameters.Add("@station", "Borivali");
            lastStationCmd.ExecuteNonQuery();
            
            conn.Close();
            MessageBox.Show("Done");
            comboBox1.SelectedIndex = -1;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";

           //var borivaliCmd="Insert into Borivali('time',platform') values 
            
        
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Dictionary<string, List<KeyValuePair<string, string>>>> singleStationTrainTimings = new List<Dictionary<string, List<KeyValuePair<string, string>>>>();
            var conn = new SqlCeConnection("Data Source=c:\\documents and settings\\amey\\my documents\\visual studio 2010\\Projects\\pf8DataEntry\\pf8DataEntry\\Database1.sdf");
            conn.Open();
            StreamWriter sw = File.CreateText(AppDomain.CurrentDomain.BaseDirectory+"\\trains.txt");

            foreach (KeyValuePair<string, int> station in stationTimingDiff)
            {
                var getStationDataQuery = new SqlCeCommand("Select * from Trains where station='" + station.Key + "'", conn);
                var stationResultSet = getStationDataQuery.ExecuteResultSet(ResultSetOptions.Scrollable);
                List<KeyValuePair<string, string>> timingsWithPlatformSet = new List<KeyValuePair<string, string>>();
                sw.Write(station.Key);
                sw.Write("={");

                foreach (SqlCeUpdatableRecord record in stationResultSet)
                {
                    KeyValuePair<string, string> timingsWithPlatform = new KeyValuePair<string, string>(record[0].ToString(), record[1].ToString());
                    timingsWithPlatformSet.Add(timingsWithPlatform);
                    sw.Write("'" + record[0].ToString() + "'");
                    sw.Write(":");
                    sw.Write("'" + record[1].ToString() + "'");
                    sw.Write(",");
                }
                sw.Write("}");
                sw.WriteLine("");
                Dictionary<string, List<KeyValuePair<string, string>>> item = new Dictionary<string, List<KeyValuePair<string, string>>>();
                item.Add(station.Key, timingsWithPlatformSet);
                singleStationTrainTimings.Add(item);
            
            
            }
            conn.Close();
            sw.Close();


        }
    }
}
