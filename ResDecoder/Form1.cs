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

namespace ResDecoder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessResFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ProcessResFile()
        {
            string iPath = textBox1.Text;
            string oPath = textBox2.Text;
            Envirment.NumberOfCharacter = Convert.ToInt32(numericUpDown1.Value);

            // for tb_Achievement_Script.res
            ProcessFile(iPath, oPath, "tb_Achievement_Script_TWN", new TbAchievementScript());

            // for tb_Akashic_Parts.res
            //ProcessFile(path, "tb_Akashic_Parts", new TbAkashicParts());

            // for tb_Appearance.res
            //ProcessFile(path, "tb_Appearance", new TbAppearance());

            // for tb_Booster.res
            ProcessFile(iPath, oPath, "tb_Booster", new TbBooster());

            // for tb_Booster_Script.res
            //ProcessFile(iPath, oPath, "tb_Booster_Script", new TbBoosterScript());

            // for tb_Buff_Script.res
            ProcessFile(iPath, oPath, "tb_Buff_Script_TWN", new TbBuffScript());

            // for tb_Character_Info.res
            //ProcessFile(path, "tb_Character_Parts", new TbCharacterParts());

            // for tb_ChattingFilter.res
            ProcessFile(iPath, oPath, "tb_ChattingFilter", new TbChattingFilter());

            // for Tb_Chattingcommand.res
            ProcessFile(iPath, oPath, "Tb_Chattingcommand", new TbChattingCommand());

            // for tb_cinema_string.res
            ProcessFile(iPath, oPath, "tb_Cinema_String_TWN", new TbCinemaString());

            // for Tb_Class_Form.res
            ProcessFile(iPath, oPath, "Tb_Class_Form", new TbClassForm());

            // for Tb_Class_Speech.res
            ProcessFile(iPath, oPath, "Tb_Class_Speech", new TbClassSpeech());

            // for tb_Cutscene_String.res
            ProcessFile(iPath, oPath, "tb_Cutscene_String_TWN", new TbCutsceneString());

            // for tb_item_script.res
            ProcessFile(iPath, oPath, "tb_item_script_TWN", new TbItemScript());

            // for tb_LeagueSkill_Script.res
            ProcessFile(iPath, oPath, "tb_LeagueSkill_Script_TWN", new TbLeagueSkillScript());

            // for tb_Loading_Img.res
            //ProcessFile(path, "tb_Loading_Img", new TbLoadingImg());

            // for tb_MazeReward_GoldDirect.res
            //ProcessFile(path, "tb_MazeReward_GoldDirect", new TbMazeRewardGoldDirect());

            // for tb_Monster_script.res
            ProcessFile(iPath, oPath, "tb_Monster_script_TWN", new TbMonsterScript());

            // for Tb_Namefilter.res
            ProcessFile(iPath, oPath, "Tb_Namefilter", new TbNameFilter());

            // for tb_NPC_Script.res
            ProcessFile(iPath, oPath, "tb_NPC_Script_TWN", new TbNPCScript());

            // for tb_Quest_Script.res
            ProcessFile(iPath, oPath, "tb_Quest_Script_TWN", new TbQuestScript());

            // for tb_Shop_String.res
            ProcessFile(iPath, oPath, "tb_Shop_String_TWN", new TbShopString());

            // for tb_Skill.res
            ProcessFile(iPath, oPath, "tb_Skill", new TbSkill());

            // for tb_Skill_Script.res
            ProcessFile(iPath, oPath, "tb_Skill_Script_TWN", new TbSkillScript());

            // for tb_soul_metry.res
            //ProcessFile(path, "tb_soul_metry", new TbSoulMetry());

            // for tb_soul_metry_string.res
            ProcessFile(iPath, oPath, "tb_soul_metry_string_TWN", new TbSoulMetryString());

            // for tb_Speech.res
            ProcessFile(iPath, oPath, "tb_Speech", new TbSpeech());

            // for tb_Speech_String.res
            ProcessFile(iPath, oPath, "tb_Speech_String_TWN", new TbSpeechString());

            // for tb_Speech_tag.res
            ProcessFile(iPath, oPath, "tb_Speech_tag", new TbSpeechTag());

            // for tb_SystemMail.res
            ProcessFile(iPath, oPath, "tb_SystemMail_TWN", new TbSystemMail());

            // for Tb_Talk.res
            ProcessFile(iPath, oPath, "Tb_Talk", new TbTalk());

            // for Tb_Talk_List.res
            ProcessFile(iPath, oPath, "Tb_Talk_List", new TbTalkList());

            // for Tb_Talk_String.res
            ProcessFile(iPath, oPath, "Tb_Talk_String", new TbTalkString());

            // for Tb_Title_Info.res
            ProcessFile(iPath, oPath, "Tb_Title_Info", new TbTitleInfo());

            // for tb_Title_String.res
            ProcessFile(iPath, oPath, "tb_Title_String_TWN", new TbTitleString());

            // for tb_Tooltip_String.res
            ProcessFile(iPath, oPath, "tb_Tooltip_String_TWN", new TbTooltipString());

            // for tb_ui_string.res
            ProcessFile(iPath, oPath, "tb_ui_string_TWN", new TbUIString());
            
            MessageBox.Show("finish");
        }

        private void ProcessFile<T>(string iPath, string oPath, string name, T type) where T : IResReader, new()
        {
            string file = name;
            string inputFilename = Path.Combine(iPath, file + ".res");
            string outputFilename = Path.Combine(oPath, file + ".txt");
            if (!File.Exists(inputFilename))
                throw new Exception(inputFilename + "不存在");

            List<T> tbs;
            Read(inputFilename, out tbs);
            Write(outputFilename, tbs);
        }

        private void Read<T>(string filename, out List<T>lists) where T : IResReader, new()
        {
            lists = new List<T>();
            using (FileStream fs = File.Open(filename, FileMode.Open))
            {
                EntryCount entryCount = new EntryCount();
                entryCount.Read(fs);

                for (int i = 0; i < entryCount.count; i++)
                {
                    T tb = new T();
                    tb.Read(fs);
                    lists.Add(tb);
                }
            }
        }

        private void Write<T>(string filename, List<T> lists) where T : IResReader, new()
        {
            MyStreamWriter writer = new MyStreamWriter(filename);
            StringBuilder sb = new StringBuilder();
            foreach (var list in lists)
            {
                list.Write(sb);
            }

            writer.AppendLine(sb.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string tmp = ChoiceFolder();
            if (tmp != "")
                textBox1.Text = tmp;
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
            string tmp = ChoiceFolder();
            if (tmp != "")
                textBox2.Text = tmp;
        }
    }
}
