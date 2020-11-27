using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillParser
{
    public interface IMyReader
    {
        void Read(StreamReader sr);
    }

    public abstract class MyReader : IMyReader
    {
        public string id;

        public abstract void Read(StreamReader sr);
    }

    public class TbSkill : MyReader
    {
        public string[] str = new string[82];

        public override void Read(StreamReader sr)
        {
            for (int j = 0; j < 82; j++)
                str[j] = sr.ReadLine();
        }
    }

    public class TbSkillScript : MyReader
    {
        public string[] str = new string[6];

        public override void Read(StreamReader sr)
        {
            for (int j = 0; j < 6; j++)
                str[j] = sr.ReadLine();
        }
    }
}
