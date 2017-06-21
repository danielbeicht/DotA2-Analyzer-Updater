using System;
using System.Collections.Generic;
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

namespace DotaAnalyzerUpdater
{
    public partial class Form1 : Form
    {
        private readonly string USER = "???";
        private readonly string PASS = "???";

        public static string[] heroNames = { "abaddon", "alchemist", "ancient-apparition", "anti-mage", "arc-warden", "axe",
                           "bane", "batrider", "beastmaster", "bloodseeker", "bounty-hunter",
                           "brewmaster", "bristleback", "broodmother", "centaur-warrunner", "chaos-knight",
                           "chen", "clinkz", "clockwerk", "crystal-maiden", "dark-seer",
                           "dazzle", "death-prophet", "disruptor", "doom", "dragon-knight",
                           "drow-ranger", "earthshaker", "earth-spirit", "elder-titan", "ember-spirit",
                           "enchantress", "enigma", "faceless-void", "gyrocopter", "huskar",
                           "invoker", "io", "jakiro", "juggernaut", "keeper-of-the-light",
                           "kunkka", "legion-commander", "leshrac", "lich", "lifestealer",
                           "lina", "lion", "lone-druid", "luna", "lycan",
                           "magnus", "medusa", "meepo", "mirana", "monkey-king", "morphling",
                           "naga-siren", "natures-prophet", "necrophos", "night-stalker", "nyx-assassin",
                           "ogre-magi", "omniknight", "oracle", "outworld-devourer", "phantom-assassin",
                           "phantom-lancer", "phoenix", "puck", "pudge", "pugna",
                           "queen-of-pain", "razor", "riki", "rubick", "sand-king",
                           "shadow-demon", "shadow-fiend", "shadow-shaman", "silencer", "skywrath-mage",
                           "slardar", "slark", "sniper", "spectre", "spirit-breaker",
                           "storm-spirit", "sven", "techies", "templar-assassin", "terrorblade",
                           "tidehunter", "timbersaw", "tinker", "tiny", "treant-protector",
                           "troll-warlord", "tusk", "underlord", "undying", "ursa", "vengeful-spirit",
                           "venomancer", "viper", "visage", "warlock", "weaver",
                           "windranger", "winter-wyvern", "witch-doctor", "wraith-king", "zeus"
                           };
        public static string[] heroNamesF = { "Abaddon", "Alchemist", "Ancient Apparition", "Anti Mage", "Arc Warden", "Axe",
                           "Bane", "Batrider", "Beastmaster", "Bloodseeker", "Bounty Hunter",
                           "Brewmaster", "Bristleback", "Broodmother", "Centaur Warrunner", "Chaos Knight",
                           "Chen", "Clinkz", "Clockwerk", "Crystal Maiden", "Dark Seer",
                           "Dazzle", "Death Prophet", "Disruptor", "Doom", "Dragon Knight",
                           "Drow Ranger", "Earthshaker", "Earth Spirit", "Elder Titan", "Ember Spirit",
                           "Enchantress", "Enigma", "Faceless Void", "Gyrocopter", "Huskar",
                           "Invoker", "Io", "Jakiro", "Juggernaut", "Keeper of the Light",
                           "Kunkka", "Legion Commander", "Leshrac", "Lich", "Lifestealer",
                           "Lina", "Lion", "Lone Druid", "Luna", "Lycan",
                           "Magnus", "Medusa", "Meepo", "Mirana", "Monkey King", "Morphling",
                           "Naga Siren", "Nature's Prophet", "Necrophos", "Night Stalker", "Nyx Assassin",
                           "Ogre Magi", "Omniknight", "Oracle", "Outworld Devourer", "Phantom Assassin",
                           "Phantom Lancer", "Phoenix", "Puck", "Pudge", "Pugna",
                           "Queen of Pain", "Razor", "Riki", "Rubick", "Sand King",
                           "Shadow Demon", "Shadow Fiend", "Shadow Shaman", "Silencer", "Skywrath Mage",
                           "Slardar", "Slark", "Sniper", "Spectre", "Spirit Breaker",
                           "Storm Spirit", "Sven", "Techies", "Templar Assassin", "Terrorblade",
                           "Tidehunter", "Timbersaw", "Tinker", "Tiny", "Treant Protector",
                           "Troll Warlord", "Tusk", "Underlord", "Undying", "Ursa", "Vengeful Spirit",
                           "Venomancer", "Viper", "Visage", "Warlock", "Weaver",
                           "Windranger", "Winter Wyvern", "Witch Doctor", "Wraith King", "Zeus"
                           };
        // Initialize global variables
        public static int heroCount = heroNames.Length;
        public Hero[] heroList = new Hero[heroCount];

        SqlConnection connection;

        private System.Timers.Timer timer;

        int remainingMinutes = 600;


        public delegate void delUpdateUIProgressBar(int value);
        public delegate void delUpdateUILabelInformation(string text);
        public delegate void delUpdateUILabelUpdateStatus(string text);

        ThreadStart threadStart;
        Thread myUpdateThread;



        public Form1()
        {
            InitializeComponent();
            Init();
        }


        private void Init()
        {
            progressBar1.Maximum = heroCount;

            //Create all heroes with their properties
            for (int i = 0; i < heroCount; i++)
            {
                Hero hero = new Hero(heroNames[i], heroNamesF[i], i);
                heroList[i] = hero;
            }

            // Configure timer
            timer = new System.Timers.Timer(60000);
            timer.Elapsed += new ElapsedEventHandler(PerformUpdate);
            timer.Enabled = true;

        }

        // delegate Update GUI-Methods
        public void UpdateUIProgressBar(int value)
        {
            progressBar1.Value = value;
        }



        public void UpdateUILabelInformation(String text)
        {
            labelInformation.Text = text;
        }



        public void UpdateUILabelUpdateStatus(String text)
        {
            labelUpdateStatus.Text = text;
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



        private void UpdateDatabase()
        {
            delUpdateUILabelInformation DelUpdateUILabelInformation = new delUpdateUILabelInformation(UpdateUILabelInformation);
            //Create connection to database
            connection = new SqlConnection("data source = 192.168.3.202; database = DotAAnalyzerDB; user id = " + USER + "; password = " + PASS);
            connection.Open();

            
            //string deleteAktualisierungRequest = "delete from Aktualisierung";
            SqlCommand com;
            //com = new SqlCommand(deleteAktualisierungRequest, connection);
            //com.ExecuteNonQuery();

            //string addAktualisierungRequest = "insert into Aktualisierung (lastUpdate) values (@lastUpdate)";
            //com = new SqlCommand(addAktualisierungRequest, connection);
            //com.Parameters.AddWithValue("@lastUpdate", DateTime.Now.ToString());
            //com.ExecuteNonQuery();

            //Delete Matchup table
            string deleteMatchupRequest = "delete from Matchup";
            com = new SqlCommand(deleteMatchupRequest, connection);
            com.ExecuteNonQuery();


            //Load Ids of heroes
            string heroExistsRequest;
            int heroExistsCount;
            string heroIdRequest;
            string addHeroRequest = "insert into Held (HeldName, HeldFullName, HeldIndex) values (@HeldName, @HeldFullName, @HeldIndex)";
            int[] heroId = new int[heroCount];
            bool newHero = false;
            for (int i = 0; i < heroCount; i++)
            {
                this.labelInformation.BeginInvoke(DelUpdateUILabelInformation, "Insert hero-data into database: " + heroNames[i]);
                heroExistsRequest = "select COUNT(*) from Held where HeldName = '" + heroList[i].name + "'";
                com = new SqlCommand(heroExistsRequest, connection);
                heroExistsCount = Convert.ToInt32(com.ExecuteScalar().ToString());

                if (heroExistsCount >= 1)
                {
                    heroIdRequest = "select HeldID from Held where HeldName='" + heroList[i].name + "'";
                    com = new SqlCommand(heroIdRequest, connection);
                    heroId[i] = Convert.ToInt32(com.ExecuteScalar().ToString());
                }
                else
                {
                    newHero = true;
                    com = new SqlCommand(addHeroRequest, connection);
                    com.Parameters.AddWithValue("@HeldName", heroList[i].name);
                    com.Parameters.AddWithValue("@HeldFullName", heroList[i].fullName);
                    com.Parameters.AddWithValue("@HeldIndex", heroList[i].index);
                    com.ExecuteNonQuery();

                    heroIdRequest = "select HeldID from Held where HeldName='" + heroList[i].name + "'";
                    com = new SqlCommand(heroIdRequest, connection);
                    heroId[i] = Convert.ToInt32(com.ExecuteScalar().ToString());
                }
            }


            //Update Indices of heroes
            if (newHero)
            {
                string updateIndexRequest;
                for (int i = 0; i < heroCount; i++)
                {
                    this.labelInformation.BeginInvoke(DelUpdateUILabelInformation, "Update hero-data-indices in database: " + heroNames[i]);
                    updateIndexRequest = "update Held set HeldIndex=@HeldIndex where HeldName=@HeldName";
                    com = new SqlCommand(updateIndexRequest, connection);
                    com.Parameters.AddWithValue("@HeldIndex", i);
                    com.Parameters.AddWithValue("@HeldName", heroNames[i]);
                    com.ExecuteNonQuery();
                }
            }


            //Add Matchup entries
            string addMatchupRequest = "insert into Matchup (HeldID1, HeldID2, Advantage, NormalizedAdvantage, Winrate) values (@HeldID1, @HeldID2, @Advantage, @NormalizedAdvantage, @Winrate)";
            for (int i = 0; i < heroCount; i++)
            {
                for (int j = 0; j < heroCount; j++)
                {
                    this.labelInformation.BeginInvoke(DelUpdateUILabelInformation, "Add Matchup entry: " + heroNames[i] + " vs. " + heroNames[j]);
                    com = new SqlCommand(addMatchupRequest, connection);
                    com.Parameters.AddWithValue("@HeldID1", heroId[i]);
                    com.Parameters.AddWithValue("@HeldID2", heroId[j]);
                    com.Parameters.AddWithValue("@Advantage", heroList[i].advantage[j]);
                    com.Parameters.AddWithValue("@NormalizedAdvantage", heroList[i].advantage_zeroAsAvg[j]);
                    com.Parameters.AddWithValue("@Winrate", heroList[i].winrate[j]);
                    com.ExecuteNonQuery();  
                }
            }
            this.labelInformation.BeginInvoke(DelUpdateUILabelInformation, "Success");
            connection.Close();
        }



        //Download data from Dotabuff
        private bool DownloadData()
        {
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            delUpdateUIProgressBar DelUpdateUIProgressBar = new delUpdateUIProgressBar(UpdateUIProgressBar);
            delUpdateUILabelInformation DelUpdateUILabelInformation = new delUpdateUILabelInformation(UpdateUILabelInformation);

            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            
            this.progressBar1.BeginInvoke(DelUpdateUIProgressBar, 0);

            for (int i = 0; i < heroCount; i++)
            {
                this.labelInformation.BeginInvoke(DelUpdateUILabelInformation, "Fetching hero-data: " + heroNames[i]);
                try
                {
                    string pageSourceCode = webClient.DownloadString(@"https://www.dotabuff.com/heroes/" + heroNames[i] + "/matchups");


                    
                    int indextbody = pageSourceCode.IndexOf("tbody");
                    indextbody = pageSourceCode.IndexOf("tbody", indextbody+6);
                    indextbody = pageSourceCode.IndexOf("tbody", indextbody + 6);
                    int indextbody2 = pageSourceCode.IndexOf("tbody", indextbody + 6);
                    pageSourceCode = pageSourceCode.Substring(indextbody);
                    //pageSourceCode = pageSourceCode.Substring(indextbody, indextbody2 - indextbody);

                    for (int j = 0; j < heroCount; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        int curHeroPos = pageSourceCode.IndexOf(heroNames[j]);

                        int dataValuePos = pageSourceCode.IndexOf("data-value=\"", curHeroPos) + 12;

                        int advantagePos = pageSourceCode.IndexOf("data-value=\"", dataValuePos) + 12;
                        int advantageEndPos = pageSourceCode.IndexOf("\">", advantagePos);
                        float advantage = float.Parse(pageSourceCode.Substring(advantagePos, advantageEndPos - advantagePos), NumberStyles.Any, ci); //-1 oder nicht?
                        heroList[i].advantage[j] = advantage;
                        heroList[i].advantage_zeroAsAvg[j] = advantage;

                        int winratePos = pageSourceCode.IndexOf("data-value=\"", advantageEndPos) + 12;
                        int winrateEndPos = pageSourceCode.IndexOf("\">", winratePos);
                        string deinVater = pageSourceCode.Substring(winratePos, winrateEndPos - winratePos);
                        float winrate = float.Parse(deinVater, NumberStyles.Any, ci);
                        heroList[i].winrate[j] = winrate;
                    }
                    
                    webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                }
                catch (WebException we)
                {
                    HttpWebResponse webResp = (HttpWebResponse)we.Response;

                    if (webResp.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.Write("Page of hero " + heroNames[i] + " not found.");
                        return false;
                    }
                    else
                    {
                        Console.Write("An unknown error occured. Update stopped.");
                        return false;
                    }
                }
                heroList[i].CalculateExtendedAdvantage();

                this.progressBar1.BeginInvoke(DelUpdateUIProgressBar, i);
                
                
            }
            return true;
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
