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
            ProcessFile<TbAchievementScript>(iPath, oPath, "tb_Achievement_Script_TWN");

            // for tb_Akashic_Parts.res
            //ProcessFile<TbAkashicParts>(iPath, oPath, "tb_Akashic_Parts");

            // for tb_Appearance.res
            //ProcessFile<TbAppearance>(iPath, oPath, "tb_Appearance");

            // for tb_Booster.res
            ProcessFile<TbBooster>(iPath, oPath, "tb_Booster");

            // for tb_Booster_Script.res
            //ProcessFile<TbBoosterScript>(iPath, oPath, "tb_Booster_Script");

            // for tb_Buff_Script.res
            ProcessFile<TbBuffScript>(iPath, oPath, "tb_Buff_Script_TWN");

            // for tb_Character_Info.res
            //ProcessFile<TbCharacterParts>(iPath, oPath, "tb_Character_Parts");

            // for tb_ChattingFilter.res
            ProcessFile<TbChattingFilter>(iPath, oPath, "tb_ChattingFilter");

            // for Tb_Chattingcommand.res
            ProcessFile<TbChattingCommand>(iPath, oPath, "Tb_Chattingcommand");

            // for tb_cinema_string.res
            ProcessFile<TbCinemaString>(iPath, oPath, "tb_Cinema_String_TWN");

            // for Tb_Class_Form.res
            ProcessFile<TbClassForm>(iPath, oPath, "Tb_Class_Form");

            // for Tb_Class_Speech.res
            ProcessFile<TbClassSpeech>(iPath, oPath, "Tb_Class_Speech");

            // for tb_Cutscene_String.res
            ProcessFile<TbCutsceneString>(iPath, oPath, "tb_Cutscene_String_TWN");

            // for tb_item_script.res
            ProcessFile<TbItemScript>(iPath, oPath, "tb_item_script_TWN");

            // for tb_LeagueSkill_Script.res
            ProcessFile<TbLeagueSkillScript>(iPath, oPath, "tb_LeagueSkill_Script_TWN");

            // for tb_Loading_Img.res
            //ProcessFile<TbLoadingImg>(iPath, oPath, "tb_Loading_Img");

            // for tb_MazeReward_GoldDirect.res
            //ProcessFile<TbMazeRewardGoldDirect>(iPath, oPath, "tb_MazeReward_GoldDirect");

            // for tb_Monster_script.res
            ProcessFile<TbMonsterScript>(iPath, oPath, "tb_Monster_script_TWN");

            // for Tb_Namefilter.res
            ProcessFile<TbNameFilter>(iPath, oPath, "Tb_Namefilter");

            // for tb_NPC_Script.res
            ProcessFile<TbNPCScript>(iPath, oPath, "tb_NPC_Script_TWN");

            // for tb_Quest_Script.res
            ProcessFile<TbQuestScript>(iPath, oPath, "tb_Quest_Script_TWN");

            // for tb_Shop_String.res
            ProcessFile<TbShopString>(iPath, oPath, "tb_Shop_String_TWN");

            // for tb_Skill.res
            ProcessFile<TbSkill>(iPath, oPath, "tb_Skill");

            // for tb_Skill_Script.res
            ProcessFile<TbSkillScript>(iPath, oPath, "tb_Skill_Script_TWN");

            // for tb_soul_metry.res
            //ProcessFile<TbSoulMetry>(iPath, oPath, "tb_soul_metry");

            // for tb_soul_metry_string.res
            ProcessFile<TbSoulMetryString>(iPath, oPath, "tb_soul_metry_string_TWN");

            // for tb_Speech.res
            ProcessFile<TbSpeech>(iPath, oPath, "tb_Speech");

            // for tb_Speech_String.res
            ProcessFile<TbSpeechString>(iPath, oPath, "tb_Speech_String_TWN");

            // for tb_Speech_tag.res
            ProcessFile<TbSpeechTag>(iPath, oPath, "tb_Speech_tag");

            // for tb_SystemMail.res
            ProcessFile<TbSystemMail>(iPath, oPath, "tb_SystemMail_TWN");

            // for Tb_Talk.res
            ProcessFile<TbTalk>(iPath, oPath, "Tb_Talk");

            // for Tb_Talk_List.res
            ProcessFile<TbTalkList>(iPath, oPath, "Tb_Talk_List");

            // for Tb_Talk_String.res
            ProcessFile<TbTalkString>(iPath, oPath, "Tb_Talk_String");

            // for Tb_Title_Info.res
            ProcessFile<TbTitleInfo>(iPath, oPath, "Tb_Title_Info");

            // for tb_Title_String.res
            ProcessFile<TbTitleString>(iPath, oPath, "tb_Title_String_TWN");

            // for tb_Tooltip_String.res
            ProcessFile<TbTooltipString>(iPath, oPath, "tb_Tooltip_String_TWN");

            // for tb_ui_string.res
            ProcessFile<TbUIString>(iPath, oPath, "tb_ui_string_TWN");

            MessageBox.Show("finish");
        }

        private void ProcessFile<T>(string iPath, string oPath, string name) where T : IResReader, new()
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
