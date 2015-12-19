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

namespace DotaAnalyzerUpdater
{
    public partial class Form1 : Form
    {
        public static string[] heroNames = { "abaddon", "alchemist", "ancient-apparition", "anti-mage", "axe",
                           "bane", "batrider", "beastmaster", "bloodseeker", "bounty-hunter",
                           "brewmaster", "bristleback", "broodmother", "centaur-warrunner", "chaos-knight",
                           "chen", "clinkz", "clockwerk", "crystal-maiden", "dark-seer",
                           "dazzle", "death-prophet", "disruptor", "doom", "dragon-knight",
                           "drow-ranger", "earthshaker", "earth-spirit", "elder-titan", "ember-spirit",
                           "enchantress", "enigma", "faceless-void", "gyrocopter", "huskar",
                           "invoker", "io", "jakiro", "juggernaut", "keeper-of-the-light",
                           "kunkka", "legion-commander", "leshrac", "lich", "lifestealer",
                           "lina", "lion", "lone-druid", "luna", "lycan",
                           "magnus", "medusa", "meepo", "mirana", "morphling",
                           "naga-siren", "natures-prophet", "necrophos", "night-stalker", "nyx-assassin",
                           "ogre-magi", "omniknight", "oracle", "outworld-devourer", "phantom-assassin",
                           "phantom-lancer", "phoenix", "puck", "pudge", "pugna",
                           "queen-of-pain", "razor", "riki", "rubick", "sand-king",
                           "shadow-demon", "shadow-fiend", "shadow-shaman", "silencer", "skywrath-mage",
                           "slardar", "slark", "sniper", "spectre", "spirit-breaker",
                           "storm-spirit", "sven", "techies", "templar-assassin", "terrorblade",
                           "tidehunter", "timbersaw", "tinker", "tiny", "treant-protector",
                           "troll-warlord", "tusk", "undying", "ursa", "vengeful-spirit",
                           "venomancer", "viper", "visage", "warlock", "weaver",
                           "windranger", "winter-wyvern", "witch-doctor", "wraith-king", "zeus"
                           };
        public static string[] heroNamesF = { "Abaddon", "Alchemist", "Ancient Apparition", "Anti Mage", "Axe",
                           "Bane", "Batrider", "Beastmaster", "Bloodseeker", "Bounty Hunter",
                           "Brewmaster", "Bristleback", "Broodmother", "Centaur Warrunner", "Chaos Knight",
                           "Chen", "Clinkz", "Clockwerk", "Crystal Maiden", "Dark Seer",
                           "Dazzle", "Death Prophet", "Disruptor", "Doom", "Dragon Knight",
                           "Drow Ranger", "Earthshaker", "Earth Spirit", "Elder Titan", "Ember Spirit",
                           "Enchantress", "Enigma", "Faceless Void", "Gyrocopter", "Huskar",
                           "Invoker", "Io", "Jakiro", "Juggernaut", "Keeper of the Light",
                           "Kunkka", "Legion Commander", "Leshrac", "Lich", "Lifestealer",
                           "Lina", "Lion", "Lone Druid", "Luna", "Lycan",
                           "Magnus", "Medusa", "Meepo", "Mirana", "Morphling",
                           "Naga Siren", "Nature's Prophet", "Necrophos", "Night Stalker", "Nyx Assassin",
                           "Ogre Magi", "Omniknight", "Oracle", "Outworld Devourer", "Phantom Assassin",
                           "Phantom Lancer", "Phoenix", "Puck", "Pudge", "Pugna",
                           "Queen of Pain", "Razor", "Riki", "Rubick", "Sand King",
                           "Shadow Demon", "Shadow Fiend", "Shadow Shaman", "Silencer", "Skywrath Mage",
                           "Slardar", "Slark", "Sniper", "Spectre", "Spirit Breaker",
                           "Storm Spirit", "Sven", "Techies", "Templar Assassin", "Terrorblade",
                           "Tidehunter", "Timbersaw", "Tinker", "Tiny", "Treant Protector",
                           "Troll Warlord", "Tusk", "Undying", "Ursa", "Vengeful Spirit",
                           "Venomancer", "Viper", "Visage", "Warlock", "Weaver",
                           "Windranger", "Winter Wyvern", "Witch Doctor", "Wraith King", "Zeus"
                           };
        public static int heroCount = heroNames.Length;

        public Hero[] heroList = new Hero[heroCount];

        SqlConnection connection;

        private System.Timers.Timer timer;



        public Form1()
        {
            InitializeComponent();

            //Create all heroes with their properties
            for (int i = 0; i < heroCount; i++)
            {
                Hero hero = new Hero(heroNames[i], heroNamesF[i], i);
                heroList[i] = hero;
            }

            timer = new System.Timers.Timer(36000000);
            timer.Elapsed += new ElapsedEventHandler(PerformUpdate);
            timer.Enabled = true;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            
        }



        private void UpdateDatabase()
        {
            //Create connection to database
            connection = new SqlConnection("data source = 192.168.2.202; database = test; user id = testuser2; password = 13DotAMZHostAnalyzer37");
            connection.Open();
            
            //Delete Matchup table
            string deleteMatchupRequest = "delete from Matchup";
            //string deleteHeroRequest = "delete from Held";
            SqlCommand com;
            com = new SqlCommand(deleteMatchupRequest, connection);
            com.ExecuteNonQuery();
            //com = new SqlCommand(deleteHeroRequest, connection);
            //com.ExecuteNonQuery();

            /*
            string addHeroRequest = "insert into Held (HeldName, HeldFullName, HeldIndex) values (@HeldName, @HeldFullName, @HeldIndex)";
            for (int i = 0; i < heroCount; i++)
            {
                com = new SqlCommand(addHeroRequest, connection);
                com.Parameters.AddWithValue("@HeldName", heroList[i].name);
                com.Parameters.AddWithValue("@HeldFullName", heroList[i].fullName);
                com.Parameters.AddWithValue("@HeldIndex", heroList[i].index);
                com.ExecuteNonQuery();
            }
            */

            //Load Ids of heroes
            string heroExistsRequest;
            int heroExistsCount;
            string heroIdRequest;
            string addHeroRequest = "insert into Held (HeldName, HeldFullName, HeldIndex) values (@HeldName, @HeldFullName, @HeldIndex)";
            int[] heroId = new int[heroCount];
            for (int i = 0; i < heroCount; i++)
            {
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

            //Add Matchup entries
            string addMatchupRequest = "insert into Matchup (HeldID1, HeldID2, Advantage, NormalizedAdvantage, Winrate) values (@HeldID1, @HeldID2, @Advantage, @NormalizedAdvantage, @Winrate)";
            for (int i = 0; i < heroCount; i++)
            {
                for (int j = 0; j < heroCount; j++)
                {
                    com = new SqlCommand(addMatchupRequest, connection);
                    com.Parameters.AddWithValue("@HeldID1", heroId[i]);
                    com.Parameters.AddWithValue("@HeldID2", heroId[j]);
                    com.Parameters.AddWithValue("@Advantage", heroList[i].advantage[j]);
                    com.Parameters.AddWithValue("@NormalizedAdvantage", heroList[i].advantage_zeroAsAvg[j]);
                    com.Parameters.AddWithValue("@Winrate", heroList[i].winrate[j]);
                    com.ExecuteNonQuery();  
                }
            }

            connection.Close();
        }



        //Download data from Dotabuff
        private bool DownloadData()
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke((MethodInvoker) delegate { progressBar1.Value = 0; });
            }
            else
            {
                progressBar1.Value = 0;
            }

            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            for (int i = 0; i < heroCount; i++)
            {
                try
                {
                    string pageSourceCode = webClient.DownloadString(@"http://www.dotabuff.com/heroes/" + heroNames[i] + "/matchups");

                    pageSourceCode = pageSourceCode.Substring(pageSourceCode.IndexOf("tbody") + 6);
                    pageSourceCode = pageSourceCode.Substring(0, pageSourceCode.IndexOf("tbody") - 2);

                    for (int j = 0; j < heroCount; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        int curHeroPos = pageSourceCode.IndexOf(heroNames[j]);

                        int advantagePos = pageSourceCode.IndexOf("<td>", curHeroPos) + 4;
                        int advantageEndPos = pageSourceCode.IndexOf("<", advantagePos);
                        float advantage = float.Parse(pageSourceCode.Substring(advantagePos, advantageEndPos - advantagePos - 1)) * 0.01f;
                        heroList[i].advantage[j] = advantage;
                        heroList[i].advantage_zeroAsAvg[j] = advantage;

                        int winratePos = pageSourceCode.IndexOf("<td>", advantageEndPos) + 9;
                        int winrateEndPos = pageSourceCode.IndexOf("<", winratePos);
                        float winrate = float.Parse(pageSourceCode.Substring(winratePos, winrateEndPos - winratePos - 1)) * 0.01f;
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

                if (progressBar1.InvokeRequired)
                {
                    progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value++; });
                }
                else
                {
                    progressBar1.Value++;
                }
            }

            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = 0; });
            }
            else
            {
                progressBar1.Value = 0;
            }

            return true;
        }



        private void PerformUpdate(object source, ElapsedEventArgs e)
        {
            if (DownloadData())
            {
                UpdateDatabase();
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            PerformUpdate(null, null);
        }
    }
}




/*



 SqlCommand com = new SqlCommand(checkuser, connection);                                                        // Erstelle command
            int temp = Convert.ToInt32(com.ExecuteScalar().ToString());                                             // Führe command aus und schreibe Wert in temp var


            string checkuser = "select count(*) from Account where AccountName='" + "MZG.Necrsdoss" + "'";                 // Erstelle String zur Abfrage
*/