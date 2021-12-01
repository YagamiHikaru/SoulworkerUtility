using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CinemaParser
{
    public class MyCinemaTalk
    {
        public string Maze_EpisodeNumber;
        public string Quest_ID;
        public string Entity_Type;
        public string SituationStringStep01_ID;
        public string ObjectFaceType;
        public string ObjectFmodEventName01;
    }

    public interface IIdReader
    {
        void Read(StreamReader sReader);
    }

    public class Sentences : IIdReader
    {
        public string PC_A;
        public string PC_B;
        public string PC_C;
        public string PC_D;
        public string PC_E;
        public string PC_F;
        public string PC_G;
        public string PC_H;
        public string PC_I;
        public string Mob;
        public string NPC;

        public void Read(StreamReader sReader)
        {
            PC_A = sReader.ReadLine();
            PC_B = sReader.ReadLine();
            PC_C = sReader.ReadLine();
            PC_D = sReader.ReadLine();
            PC_E = sReader.ReadLine();
            PC_F = sReader.ReadLine();
            PC_G = sReader.ReadLine();
            PC_H = sReader.ReadLine();
            PC_I = sReader.ReadLine();
            Mob = sReader.ReadLine();
            NPC = sReader.ReadLine();
        }
    }

    public class Quest : IIdReader
    {
        public string QuestName;
        public string Speech;
        public string Quest1;
        public string Quest2;
        public string Quest3;

        public void Read(StreamReader sReader)
        {
            QuestName = sReader.ReadLine();
            Speech = sReader.ReadLine();
            Quest1 = sReader.ReadLine();
            Quest2 = sReader.ReadLine();
            Quest3 = sReader.ReadLine();
        }
    }

    public class Contents
    {
        [XmlAttribute]
        public string ContentsCount;
        [XmlAttribute]
        public string Operation;
        [XmlAttribute]
        public string Situation;
        [XmlAttribute]
        public string InfoDisplay;
        [XmlElement]
        public List<VOperationTalk> VOperationTalk { get; set; }
    }

    public class VOperationTalk
    {
        public string UITotalTime;
        public string ObjectFmodEventName01;
        public string Entity_Type;
        public string MouthResumeTime;
        public string ObjectTable_ID;
        public string MouthStopTime;
        public string CinemaTalk_ID;
        public string CinemaTalk_GroupID;
        public string ObjectFaceType;
        public string Quest_ID;
        public string MouthDelayTime;
    }

    public class Cinematalk_Test
    {
        [XmlElement]
        public List<VCinemaTalk> VCinemaTalk { get; set; }
    }
    
    public class VCinemaTalk
    {
        [XmlAttribute]
        public string CinemaTalk_GroupID;
        public string CinemaTalk_Type;
        public string CinemaTalk_ID;
        public string Maze_EpisodeNumber;
        public string Quest_ID;
        public string Condition_ID;
        public string Entity_Type;
        public string ObjectTable_ID;
        public string SituationPosition;
        public string SituationStringStep01_ID;
        public string NextDelayTime;
        public string GroupStep;
        public string ObjectFaceType;
        public string ObjectFaceType_JP;
        public string ObjectFmodEventName01;
    }

}
