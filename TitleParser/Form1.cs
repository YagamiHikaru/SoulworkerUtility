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

namespace TitleParser
{
    public partial class Form1 : Form
    {
        private string mInputPath
        {
            get { return textBox1.Text; }
            set
            {
                textBox1.Text = value;
                mTitleInfoFileName = Path.Combine(mInputPath, "Tb_Title_Info.txt");
                mTitleStringFileName = Path.Combine(mInputPath, "tb_Title_String_TWN.txt");
            }
        }

        private string mOutputPath
        {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }

        //private static string mSettingFileName = Path.Combine(Application.StartupPath, "Setting.ini");
        private static string mSettingFileName = Path.Combine(Application.StartupPath, "Setting.json");
        private string mTitleInfoFileName;
        private string mTitleStringFileName;

        //IniFile ini = new IniFile(mSettingFileName);
        JsonFile ini = new JsonFile(mSettingFileName);
        Dictionary<string, string> mCache = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process();
        }

        private void Process()
        {
            try
            {
                List<TbTitle> title;
                List<TbTitleInfo> titleInfo;

                mInputPath = textBox1.Text;
                mOutputPath = textBox2.Text;

                if(String.IsNullOrEmpty(mInputPath))
                    throw new System.InvalidOperationException($"輸入目錄不可為空");

                if (String.IsNullOrEmpty(mOutputPath))
                    throw new System.InvalidOperationException($"輸出目錄不可為空");

                mTitleInfoFileName = Path.Combine(mInputPath, "Tb_Title_Info.txt");
                mTitleStringFileName = Path.Combine(mInputPath, "tb_Title_String_TWN.txt");

                if (!File.Exists(mTitleInfoFileName))
                    throw new System.InvalidOperationException($"{mTitleInfoFileName} 檔案不存在");

                if (!File.Exists(mTitleStringFileName))
                    throw new System.InvalidOperationException($"{mTitleStringFileName} 檔案不存在");

                GetIdTable(mTitleStringFileName, out title);
                GetIdTable(mTitleInfoFileName, out titleInfo);

                List<TbTitleInfo> frontTitle = titleInfo.Where(o => (o.id.ToCharArray()[0] == '1')).ToList();
                List<TbTitleInfo> backTitle = titleInfo.Where(o => (o.id.ToCharArray()[0] == '2')).ToList();

                WritePackage format = (radioButton1.Checked) ? WritePackageFactory.GetInstance(WritePackageFactory.Type.TEXT) :
                                      (radioButton2.Checked) ? WritePackageFactory.GetInstance(WritePackageFactory.Type.CSV) :
                                      WritePackageFactory.GetInstance(WritePackageFactory.Type.HTML);

                WriteData(format, title, frontTitle, backTitle);

                MessageBox.Show("finish");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void WriteData(WritePackage format, List<TbTitle> title, List<TbTitleInfo> frontTitle, List<TbTitleInfo> backTitle)
        {
            ProcessByIndividual(format.method, title, frontTitle, $"前方稱號.{format.ext}");
            ProcessByIndividual(format.method, title, backTitle, $"後方稱號.{format.ext}");
        }

        private void ProcessByIndividual(WriteFormat method, List<TbTitle> titles, List<TbTitleInfo> titleInfos, string filename)
        {
            var aaa = Encoding.Default;
            string nPath =  Path.Combine(mOutputPath, filename);
            using (FileStream fs = new FileStream(nPath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                method.WriteHead(sw, filename);

                var queue = from titleInfo in titleInfos
                             orderby Convert.ToInt32(titleInfo.str[3]), titleInfo.str[2]
                             select titleInfo;
                foreach (var titleInfo in queue)
                {
                    method.WriteBegin(sw);

                    var title = titles.Find(o => o.id == titleInfo.str[0]);
                    method.WriteNumber(sw, titleInfo.str[3]);
                    string name = "";
                    if (titleInfo.str[2] != "0")
                    {
                        name = (titleInfo.str[2] == "1") ? "哈露" :
                            (titleInfo.str[2] == "2") ? "歐文" :
                            (titleInfo.str[2] == "3") ? "莉莉" :
                            (titleInfo.str[2] == "4") ? "金" :
                            (titleInfo.str[2] == "5") ? "史黛菈" :
                            (titleInfo.str[2] == "6") ? "伊莉絲" : 
                            (titleInfo.str[2] == "7") ? "琪" :
                            (titleInfo.str[2] == "8") ? "艾芙妮爾" : "李娜飛";
                    }
                    method.WriteCharacter(sw, name);

                    method.WriteTitle(sw, title.str[0]);
                    method.WriteDescription(sw, title.str[1]);
                    method.WriteObtain(sw, title.str[2]);

                    if (titleInfo.str[12] == "0" && titleInfo.str[13] == "0" && titleInfo.str[14] == "0" && titleInfo.str[15] == "0" && titleInfo.str[16] == "0")
                    {
                        method.WriteNoEffect(sw);
                    }
                    else
                    {
                        List<string> effects = new List<string>();
                        if (titleInfo.str[12] != "0")
                            effects.Add($"{GetValue(titleInfo.str[12])}：{titleInfo.str[17]}");
                        if (titleInfo.str[13] != "0")
                            effects.Add($"{GetValue(titleInfo.str[13])}：{titleInfo.str[18]}");
                        if (titleInfo.str[14] != "0")
                            effects.Add($"{GetValue(titleInfo.str[14])}：{titleInfo.str[19]}");
                        if (titleInfo.str[15] != "0")
                            effects.Add($"{GetValue(titleInfo.str[15])}：{titleInfo.str[20]}");
                        if (titleInfo.str[16] != "0")
                            effects.Add($"{GetValue(titleInfo.str[16])}：{titleInfo.str[21]}");
                        method.WriteEffect(sw, effects.ToArray());
                    }
                    
                    method.WriteEnd(sw);
                }

                method.WriteTail(sw);
            }
        }

        private void GetIdTable<T>(string path, out List<T> sentencesList) where T : MyReader, new()
        {
            sentencesList = new List<T>();
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
                        sentences.id = id;
                        sentences.Read(sReader);
                        sentencesList.Add(sentences);
                    }
                }
            }
        }

        private string GetValue(string key)
        {
            string result;
            if (!mCache.ContainsKey(key))
            {
                result = ReadIni("Tag", key);
                mCache.Add(key, result);
                return result;
            }

            return mCache[key];
        }

        private string ReadIni(string section, string key)
        {
            if (key == "149")
                Debug.Write("0");
            string result = ini.Read(key, section);
            return (String.IsNullOrEmpty(result)) ? key : result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ret = ChoiceFolder();
            if (!String.IsNullOrEmpty(ret))
            {
                mInputPath = ret;
            }
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

        private void button3_Click(object sender, EventArgs e)
        {
            var ret = ChoiceFolder();
            if (!String.IsNullOrEmpty(ret))
            {
                mOutputPath = ret;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // default
            radioButton1.Checked = true;
        }
    }
}
