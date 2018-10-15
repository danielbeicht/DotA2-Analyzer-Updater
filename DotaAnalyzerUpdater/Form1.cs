using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Timers;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;
using System.Globalization;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace DotaAnalyzerUpdater
{
    public partial class Form1 : Form
    {
        // Initialize global variables
        private readonly string USER = "dota";
        private readonly string PASS = "DotA123";
        
        public static ArrayList heroNamesDotabuff = new ArrayList();
        public static ArrayList heroNamesFull = new ArrayList();
        public static int heroCount;
        public Hero[] heroList;

        private System.Timers.Timer timer;
        int remainingMinutes = 600;
        ThreadStart threadStart;
        Thread myUpdateThread;

        public delegate void delUpdateUIProgressBar(int value);
        public delegate void delUpdateUILabelInformation(string text);
        public delegate void delUpdateUILabelUpdateStatus(string text);


        public Form1()
        {
            InitializeComponent();
            Init();
        }


        private void Init()
        {
            DownloadAPIData();
            heroCount = heroNamesDotabuff.Count;
            heroList = new Hero[heroCount];
            progressBar1.Maximum = heroCount;

            //Create all heroes with their properties
            for (int i = 0; i < heroCount; i++)
            {
                Hero hero = new Hero(((KeyValuePair<int,string>)heroNamesDotabuff[i]).Value, ((KeyValuePair<int, string>)heroNamesFull[i]).Value, ((KeyValuePair<int, string>)heroNamesFull[i]).Key);
                heroList[i] = hero;
            }

            // Configure timer
            timer = new System.Timers.Timer(60000);
            timer.Elapsed += new ElapsedEventHandler(PerformUpdate);
            timer.Enabled = true;
        }

        
        public void UpdateUIProgressBar(int value) // delegate Update GUI-Methods
        {
            progressBar1.Value = value;
        }
        public void UpdateUILabelInformation(string text)
        {
            labelInformation.Text = text;
        }
        public void UpdateUILabelUpdateStatus(string text)
        {
            labelUpdateStatus.Text = text;
        }


        private void UpdateDatabase()
        {
            delUpdateUILabelInformation DelUpdateUILabelInformation = new delUpdateUILabelInformation(UpdateUILabelInformation);

            //Create connection to database
            SqlConnection connection;
            SqlCommand com;
            connection = new SqlConnection("data source = 192.168.3.202; database = DotAAnalyzerDB; user id = " + USER + "; password = " + PASS);
            connection.Open();

            //Delete Matchup table
            string deleteMatchupRequest = "delete from Matchup";
            com = new SqlCommand(deleteMatchupRequest, connection);
            com.ExecuteNonQuery();

            //Update Names and IDs of heroes
            string heroExistsRequest;
            int heroExistsCount;
            string addHeroRequest = "insert into Held (HeldFullName, HeldID) values (@HeldFullName, @HeldID)"; //name index und valveindex gelöscht
            int[] heroId = new int[heroCount];
            for (int i = 0; i < heroCount; i++)
            {
                this.labelInformation.BeginInvoke(DelUpdateUILabelInformation, "Insert hero-data into database: " + ((KeyValuePair<int, string>)heroNamesDotabuff[i]).Value);
                heroExistsRequest = "select COUNT(*) from Held where HeldFullName = '" + heroList[i].fullName + "'";
                com = new SqlCommand(heroExistsRequest, connection);
                heroExistsCount = Convert.ToInt32(com.ExecuteScalar().ToString());

                if (heroExistsCount < 1)
                {
                    com = new SqlCommand(addHeroRequest, connection);
                    com.Parameters.AddWithValue("@HeldFullName", heroList[i].fullName);
                    com.Parameters.AddWithValue("@HeldID", heroList[i].index);
                    com.ExecuteNonQuery();
                }
            }

            //Add Matchup entries
            string addMatchupRequest = "insert into Matchup (HeldID1, HeldID2, Advantage, NormalizedAdvantage, Winrate) values (@HeldID1, @HeldID2, @Advantage, @NormalizedAdvantage, @Winrate)";
            for (int i = 0; i < heroCount; i++)
            {
                for (int j = 0; j < heroCount; j++)
                {
                    this.labelInformation.BeginInvoke(DelUpdateUILabelInformation, "Add Matchup entry: " + ((KeyValuePair<int, string>)heroNamesDotabuff[i]).Value + " vs. " + ((KeyValuePair<int, string>)heroNamesDotabuff[j]).Value);
                    com = new SqlCommand(addMatchupRequest, connection);
                    com.Parameters.AddWithValue("@HeldID1", heroList[i].index);
                    com.Parameters.AddWithValue("@HeldID2", heroList[j].index);
                    com.Parameters.AddWithValue("@Advantage", heroList[i].advantage[j]);
                    com.Parameters.AddWithValue("@NormalizedAdvantage", heroList[i].advantage_zeroAsAvg[j]);
                    com.Parameters.AddWithValue("@Winrate", heroList[i].winrate[j]);
                    com.ExecuteNonQuery();  
                }
            }
            this.labelInformation.BeginInvoke(DelUpdateUILabelInformation, "Success");
            connection.Close();
        }


        private void DownloadAPIData()
        {
            //Download Names and Valve-Indices of heroes to automatically update the database in case of the release of new heroes.
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string jsonSourceCode = webClient.DownloadString(@"https://api.opendota.com/api/heroes");
            JArray jsonObjects = JArray.Parse(jsonSourceCode);
            foreach (JObject jsonObject in jsonObjects)
            {
                int heroId = (int)jsonObject["id"];
                string heroNameFull = ((string)jsonObject["localized_name"]).Replace("'", string.Empty);
                heroNamesFull.Add(new KeyValuePair<int, string> (heroId, heroNameFull));
                string heroNameDotabuff = heroNameFull.ToLower().Replace(" ", "-").Replace("'", string.Empty);
                heroNamesDotabuff.Add(new KeyValuePair<int, string>(heroId, heroNameDotabuff));
            }
        }


        private bool DownloadData()
        {
            //Download data from Dotabuff
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            delUpdateUIProgressBar DelUpdateUIProgressBar = new delUpdateUIProgressBar(UpdateUIProgressBar);
            delUpdateUILabelInformation DelUpdateUILabelInformation = new delUpdateUILabelInformation(UpdateUILabelInformation);

            WebClient webClient = new WebClient();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            this.progressBar1.BeginInvoke(DelUpdateUIProgressBar, 0);

            for (int i = 0; i < heroCount; i++)
            {
                this.labelInformation.BeginInvoke(DelUpdateUILabelInformation, "Fetching hero-data: " + ((KeyValuePair<int, string>)heroNamesDotabuff[i]).Value);

                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                string pageSourceCode;
                try
                {
                    pageSourceCode = webClient.DownloadString(@"https://www.dotabuff.com/heroes/" + ((KeyValuePair<int, string>)heroNamesDotabuff[i]).Value + "/matchups");
                }
                catch (WebException we)
                {
                    HttpWebResponse webResp = (HttpWebResponse)we.Response;

                    if (webResp.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.Write("Page of hero " + ((KeyValuePair<int, string>)heroNamesDotabuff[i]).Value + " not found.");
                        return false;
                    }
                    else
                    {
                        Console.Write("An unknown error occured. Update stopped.");
                        return false;
                    }
                }

                int indextbody = pageSourceCode.IndexOf("tbody");
                indextbody = pageSourceCode.IndexOf("tbody", indextbody + 6);
                indextbody = pageSourceCode.IndexOf("tbody", indextbody + 6);
                pageSourceCode = pageSourceCode.Substring(indextbody);

                for (int j = 0; j < heroCount; j++)
                {
                    if (i == j) { continue; }

                    string curHeroPosString = ";" + ((KeyValuePair<int, string>)heroNamesDotabuff[j]).Value;
                    int curHeroPos = pageSourceCode.IndexOf(curHeroPosString);
                    int dataValuePos = pageSourceCode.IndexOf("data-value=\"", curHeroPos) + 12;

                    int advantagePos = pageSourceCode.IndexOf("data-value=\"", dataValuePos) + 12;
                    int advantageEndPos = pageSourceCode.IndexOf("\">", advantagePos);
                    string advantageString = pageSourceCode.Substring(advantagePos, advantageEndPos - advantagePos);
                    float advantage = - float.Parse(advantageString, NumberStyles.Any, ci); //-1 oder nicht?
                    heroList[i].advantage[j] = advantage;
                    heroList[i].advantage_zeroAsAvg[j] = advantage;

                    int winratePos = pageSourceCode.IndexOf("data-value=\"", advantageEndPos) + 12;
                    int winrateEndPos = pageSourceCode.IndexOf("\">", winratePos);
                    string deinVater = pageSourceCode.Substring(winratePos, winrateEndPos - winratePos);
                    float winrate = float.Parse(deinVater, NumberStyles.Any, ci);
                    heroList[i].winrate[j] = winrate;
                }

                heroList[i].CalculateExtendedAdvantage();

                this.progressBar1.BeginInvoke(DelUpdateUIProgressBar, i);
            }
            this.progressBar1.BeginInvoke(DelUpdateUIProgressBar, 0);
            return true;
        }


        private void CreateUpdateThread()
        {
            threadStart = new ThreadStart(StartUpdate);
            myUpdateThread = new Thread(threadStart);
            myUpdateThread.Name = "Second Thread";
            myUpdateThread.Start();
        }


        private void StartUpdate()
        {
            delUpdateUILabelUpdateStatus DelUpdateUILabelUpdateStatus = new delUpdateUILabelUpdateStatus(UpdateUILabelUpdateStatus);
            this.labelUpdateStatus.BeginInvoke(DelUpdateUILabelUpdateStatus, "Update In Progress");
            if (DownloadData())
            {
                UpdateDatabase();
            }
            remainingMinutes = 600;
            this.labelUpdateStatus.BeginInvoke(DelUpdateUILabelUpdateStatus, "Next Auto Update in 600 minutes.");
        }


        private void PerformUpdate(object source, ElapsedEventArgs e)
        {
            remainingMinutes--;
            if (labelUpdateStatus.InvokeRequired)
            {
                labelUpdateStatus.Invoke((MethodInvoker)delegate { labelUpdateStatus.Text = "Next Auto Update in " + remainingMinutes + " minutes."; });
            }
            else
            {
                labelUpdateStatus.Text = labelUpdateStatus.Text = "Next Auto Update in " + remainingMinutes + " minutes.";
            }
            if (remainingMinutes == 0)
            {
                remainingMinutes = 600;
                CreateUpdateThread();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            CreateUpdateThread();
        }
    }
}