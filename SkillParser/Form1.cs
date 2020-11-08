using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkillParser
{
    public partial class Form1 : Form
    {
        string mInfoFile = "tb_Skill_Script.txt";
        string mParamFile = "tb_Skill.txt";
        List<string> mCharactorList = new List<string>() { "哈露", "歐文", "莉莉", "金", "史黛菈", "伊莉絲", "琪", "Ephnel" };

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                Process();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void Process()
        {
            string inputPath = textBox1.Text;
            string path1 = Path.Combine(inputPath, mParamFile);
            string path2 = Path.Combine(inputPath, mInfoFile);

            if (!File.Exists(path1)) throw new Exception($"{path1} doesn't exist.");
            if (!File.Exists(path2)) throw new Exception($"{path2} doesn't exist.");

            List<TbSkill> skill;
            List<TbSkillScript> skillScript;

            GetIdTable(path1, out skill);
            GetIdTable(path2, out skillScript);

            for (int i = 0; i < mCharactorList.Count; i++)
                ProcessByCharactor(skill, skillScript, i);

            MessageBox.Show("finish");
        }

        private void ProcessByCharactor(List<TbSkill> skills, List<TbSkillScript> skillScripts, int i)
        {
            string outputPath = textBox2.Text;
            string nPath = Path.Combine(outputPath, mCharactorList[i] + ".txt");
            using (FileStream fs = new FileStream(nPath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs)) {
                var queue = skills.Where(o => o.str[5] == (i + 1).ToString()).OrderBy(o => Convert.ToInt32(o.id));
                foreach (var skill in queue) {
                    var skillScript = skillScripts.Find(o => o.id == skill.str[0]);
                    if (skillScript == null) continue;
                    if (string.IsNullOrEmpty(skillScript.str[0])) continue;
                    if (skillScript.str[0] == "0") continue;

#region For debug
#if false
                    if (list.str[30] != "2" && list.str[30] != "3" && list.str[30] != "4" && list.str[31] != "0")
                    {
                        Debug.WriteLine($"技能名稱：{skillScript.str[0]}");
                        Debug.WriteLine($"技能ID：{skillScript.id}");
                        Debug.WriteLine($"");
                    }
#endif
#endregion

                    string costStr = (skill.str[30] == "2") ? "靈魂值消耗量" :
                        (skill.str[30] == "3") ? "耐力消耗量" :
                        (skill.str[30] == "4") ? "SV消耗量" : "未知";
                    sw.WriteLine($"技能名稱：{skillScript.str[0]}");
                    sw.WriteLine($"習得技能等級：{skill.str[6]}");
                    sw.WriteLine($"習得技能所需SP：{skill.str[8]}");
                    if (skillScript.str[1] != "-")
                        sw.WriteLine($"技能資訊：{skillScript.str[1]}");
                    if (skillScript.str[1] != "0")
                        sw.WriteLine($"下個技能資訊：{skillScript.str[2]}");
                    sw.WriteLine($"圖示：{skillScript.str[3]}");
                    if (skillScript.str[5] != "0")
                        sw.WriteLine($"影片：{skillScript.str[5]}");

                    //if (skill.str[21] != "0")
                    //    sw.WriteLine($"傷害量：{skill.str[21]}");

                    //if (skill.str[23] != "0")
                    //    sw.WriteLine($"霸體破壞量：{skill.str[23]}");

                    if (skill.str[30] != "0" && skill.str[31] != "0")
                        sw.WriteLine($"{costStr}：{skill.str[31]}");

                    //if (list.str[54] != "0")
                    //    sw.WriteLine($"冷卻時間：{Convert.ToSingle(list.str[54]) / 1000.0f}");

                    sw.WriteLine($"");
                }
            }
        }

        private void GetIdTable<T>(string path, out List<T> sentencesList) where T : MyReader, new()
        {
            sentencesList = new List<T>();
            using (StreamReader sReader = new StreamReader(path, Encoding.UTF8)) {
                string line;
                while (true) {
                    line = sReader.ReadLine();
                    if (line == null) {
                        break;
                    }

                    if (line != "\n" && line.IndexOf("ID") != -1) {
                        T sentences = new T();
                        string id = line.Split(new string[] { "ID=" }, StringSplitOptions.None)[1];
                        sentences.id = id;
                        sentences.Read(sReader);
                        sentencesList.Add(sentences);
                    }
                }
            }
        }
    }
}
