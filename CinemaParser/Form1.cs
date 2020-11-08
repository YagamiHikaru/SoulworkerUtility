using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CinemaParser
{
    public partial class Form1 : Form
    {
        private const string mCinemaIdFileName = "tb_cinema_string.txt";
        private const string mQuestIdFileName = "tb_quest_script.txt";
        private static string mSettingFileName = Path.Combine(Application.StartupPath, "Setting.ini");

        IniFile ini = new IniFile(mSettingFileName);

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string tmp = ChoiceFolder();
            if (tmp != "")
                textBox1.Text = tmp;
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            string tmp = ChoiceFolder();
            if (tmp != "")
                textBox3.Text = tmp;
        }

        private string ChoiceFolder()
        {
            string path = "";
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path = fbd.SelectedPath;
                }
            }

            return path;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessSentence();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ProcessSentence()
        {
            if (!File.Exists(mSettingFileName))
                throw new Exception(mSettingFileName + "不存在");

            if (textBox1.Text == "")
                throw new Exception("必須指定ID文件路徑");
            if (textBox3.Text == "")
                throw new Exception("必須指定XML文件路徑");

            var cinemaIdFilePath = Path.Combine(textBox1.Text, mCinemaIdFileName);
            var questIdFilePath = Path.Combine(textBox1.Text, mQuestIdFileName);
            var cinemaXmlFilePath = textBox3.Text;

            if(!File.Exists(questIdFilePath))
                throw new Exception(questIdFilePath + "不存在");
            Dictionary<string, Quest> questList;
            GetIdTable(questIdFilePath, out questList);
            
            if (!File.Exists(cinemaIdFilePath))
                throw new Exception(cinemaIdFilePath + "不存在");
            Dictionary<string, Sentences> sentencesList;
            GetIdTable(cinemaIdFilePath, out sentencesList);

            var pcXmlFiles = GetPCXmlFiles(cinemaXmlFilePath);
            if (pcXmlFiles != null)
            {
                foreach (var pcXmlFile in pcXmlFiles)
                {
                    Dictionary<int, string> contents = new Dictionary<int, string>();
                    string PC = pcXmlFile.Key;
                    string name = ReadIni("Character", pcXmlFile.Key);
                    string filename = $"副本內劇情{name}.txt";
                    foreach (var xmlFilename in pcXmlFile.Value)
                    {
                        //Debug.WriteLine($"{xmlFilename}");
                        //if(xmlFilename == @"")
                        //   Debug.WriteLine($"{xmlFilename}");
                        StringBuilder writer = new StringBuilder("");
                        var levelname = GetLevelName(xmlFilename);
                        var cinemaPC = GetData<Cinematalk_Test>(xmlFilename);
                        var datas = Arrange(cinemaPC);
                        writer.AppendLine("副本：" + ReadIni("Translate", levelname));
                        WriteCinema(writer, PC, sentencesList, questList, datas);
                        var order = Convert.ToInt32(ReadIni("Order", levelname));
                        var content = writer.ToString();
                        contents.Add(order, content);
                    }

                    MyStreamWriter writer2 = new MyStreamWriter(Path.Combine(Application.StartupPath, filename));
                    var rets = from content in contents.ToList()
                              orderby content.Key
                              select content.Value;
                    foreach(string ret in rets)
                        writer2.AppendLine(ret);
                }

                MessageBox.Show("finish");
            }
        }

        private string GetLevelName(string path)
        {
            var tmp = path.Split(new string[] { "/" }, StringSplitOptions.None);
            string filename = tmp[tmp.Length - 1];
            string levelname = filename.Split(new string[] { "_" }, StringSplitOptions.None)[1].Split(new string[] { "_" }, StringSplitOptions.None)[0];
            return levelname;
        }
        
        private Dictionary<string, string[]> GetPCXmlFiles(string path)
        {
            var allFiles = Directory.GetFiles(path);
            if (!CheckXmlExist(allFiles))
                throw new Exception("沒有XML檔案");

            Dictionary<string, string[]> pcFiles = new Dictionary<string, string[]>();
            pcFiles.Add("PC_A", allFiles.Where(o => (o.IndexOf("PC_A") != -1)).ToArray());
            pcFiles.Add("PC_B", allFiles.Where(o => (o.IndexOf("PC_B") != -1)).ToArray());
            pcFiles.Add("PC_C", allFiles.Where(o => (o.IndexOf("PC_C") != -1)).ToArray());
            pcFiles.Add("PC_D", allFiles.Where(o => (o.IndexOf("PC_D") != -1)).ToArray());
            pcFiles.Add("PC_E", allFiles.Where(o => (o.IndexOf("PC_E") != -1)).ToArray());
            pcFiles.Add("PC_F", allFiles.Where(o => (o.IndexOf("PC_F") != -1)).ToArray());
            pcFiles.Add("PC_G", allFiles.Where(o => (o.IndexOf("PC_G") != -1)).ToArray());
            return pcFiles;
        }

        private bool CheckXmlExist(string [] files)
        {
            return files.Where(o => (o.IndexOf("xml") != -1)).ToList().Count > 0;
        }

        private void WriteCinema(StringBuilder writer, string PC, Dictionary<string, Sentences> sentencesList, Dictionary<string, Quest> questsList, Dictionary<string, List<MyCinemaTalk>> datas)
        {
            string oEpisodeNumber = "";
            string oQuestID = "";
            foreach (var data in datas)
            {
                // 進入不同的 Group 以防不知道現在是EP幾 所以清掉oQuestID 讓他在打印一次
                //outputFile.WriteLine("GroupID " + data.Key);
                oEpisodeNumber = "";
                foreach (var line in data.Value)
                {
                    var character = ReadIni("Translate", line.ObjectFmodEventName01);
                    string ttt = ini.Read(line.ObjectFmodEventName01, "Translate");
                    var face = ReadIni("Translate", line.ObjectFaceType);
                    var sentence = (line.Entity_Type == "NPC") ? sentencesList[line.SituationStringStep01_ID].NPC :
                        (line.Entity_Type == "Mob") ? sentencesList[line.SituationStringStep01_ID].Mob :
                        (PC == "PC_A") ? sentencesList[line.SituationStringStep01_ID].PC_A :
                        (PC == "PC_B") ? sentencesList[line.SituationStringStep01_ID].PC_B :
                        (PC == "PC_C") ? sentencesList[line.SituationStringStep01_ID].PC_C :
                        (PC == "PC_D") ? sentencesList[line.SituationStringStep01_ID].PC_D :
                        (PC == "PC_E") ? sentencesList[line.SituationStringStep01_ID].PC_E :
                        (PC == "PC_F") ? sentencesList[line.SituationStringStep01_ID].PC_F :
                        sentencesList[line.SituationStringStep01_ID].PC_G;

                    string str = $"{character}({face}):{sentence}";
                    var nEpisodeNumber = line.Maze_EpisodeNumber;
                    var nQuestID = line.Quest_ID;

                    if (oQuestID != nQuestID && nQuestID != "0")
                    {
                        string tmp = $"任務：{questsList[nQuestID].QuestName} (Quest ID {nQuestID})";
                        writer.AppendLine(tmp);
                    }

                    if (oEpisodeNumber != nEpisodeNumber)
                        writer.AppendLine("EP " + nEpisodeNumber);

                    writer.AppendLine(str);
                    oEpisodeNumber = nEpisodeNumber;
                    oQuestID = nQuestID;
                }
                writer.AppendLine("\n");
            }
        }

        private void GetIdTable<T>(string path, out Dictionary<string, T> sentencesList) where T : IIdReader, new()
        {
            sentencesList = new Dictionary<string, T>();
            using (StreamReader sReader = new StreamReader(path, Encoding.UTF8))
            {
                string line;
                while (true)
                {
                    line = sReader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    if (line != "\n" && line.IndexOf("ID") != -1)
                    {
                        T sentences = new T();
                        string id = line.Split(new string[] { "ID=" }, StringSplitOptions.None)[1];
                        sentences.Read(sReader);
                        sentencesList.Add(id, sentences);
                    }
                }
            }
        }
        
        private T GetData<T>(string path) where T : class
        {
            var serilizer = new XmlSerializer(typeof(T));
            T result;
            using (var fileTemp = File.OpenRead(path))
            {
                result = serilizer.Deserialize(fileTemp) as T;
            }
            return result;
        }

        private Dictionary<string, List<MyCinemaTalk>> Arrange(Cinematalk_Test datas)
        {
            Dictionary<string, List<MyCinemaTalk>> ndatas = new Dictionary<string, List<MyCinemaTalk>>();
            var groupIDs = GetGroupIDs(datas);

            foreach (var groupID in groupIDs)
            {
                var temp = datas.VCinemaTalk.Where(o => o.CinemaTalk_GroupID == groupID).ToList();
                var temp2 = temp.OrderBy(o => Convert.ToInt32(o.GroupStep)).ToList();

                List<MyCinemaTalk> temp3 = new List<MyCinemaTalk>();
                foreach (var tmp in temp2)
                {
                    string ttt = tmp.ObjectFmodEventName01;
                    var aaa = tmp.ObjectFmodEventName01.Split(new string[] { "/" }, StringSplitOptions.None);
                    string name = aaa[aaa.Length - 1].Split(new string[] { "_" }, StringSplitOptions.None)[0];
                    temp3.Add(new MyCinemaTalk
                    {
                        Maze_EpisodeNumber = tmp.Maze_EpisodeNumber,
                        Quest_ID = tmp.Quest_ID,
                        Entity_Type = tmp.Entity_Type,
                        SituationStringStep01_ID = tmp.SituationStringStep01_ID,
                        ObjectFaceType = tmp.ObjectFaceType,
                        ObjectFmodEventName01 = name
                    });
                }

                ndatas.Add(groupID, temp3);
            }

            return ndatas;
        }

        private List<string> GetGroupIDs(Cinematalk_Test datas)
        {
            Dictionary<string, int> lists = new Dictionary<string, int>();
            foreach (var data in datas.VCinemaTalk)
            {
                if (!lists.ContainsKey(data.CinemaTalk_GroupID))
                {
                    lists.Add(data.CinemaTalk_GroupID, 0);
                }
            }

            List<string> result = new List<string>();
            foreach (var list in lists)
            {
                result.Add(list.Key);
            }
            return result;
        }

        private string ReadIni(string section, string key)
        {
            string result = ini.Read(key, section);
            return (result == "") ? key : result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }        
    }
}
