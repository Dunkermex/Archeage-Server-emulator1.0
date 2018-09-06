using System;
using System.Collections.Generic;

namespace ArcheAge.ArcheAge.Structuring
{
    /// <summary>
    /// Structure For Character.
    /// </summary>
    public class Character
    {

        public Character()
        {
        }

        public Character(long id, long accountId, byte worldId, string charName, byte charRace, byte charGender,
            string gUid, long v, int[] type, float[] weight, float scale, float rotate, float moveX, float moveY,
            int lip, int leftPupil, int rightPupil, int eyebrow, int decor, string modifiers, byte[] a, byte level)
        {
            Id = id;
            AccountId = accountId;
            WorldId = worldId;
            if (charName != null) CharName = charName;
            CharRace = charRace;
            CharGender = charGender;
            if (gUid != null) Guid = gUid;
            V = v;
            if (type != null) Type = type;
            if (weight != null) Weight = weight;
            Scale = scale;
            Rotate = rotate;
            MoveX = moveX;
            MoveY = moveY;
            Lip = lip;
            LeftPupil = leftPupil;
            RightPupil = rightPupil;
            Eyebrow = eyebrow;
            Decor = decor;
            Modifiers = modifiers;
            if (a != null) A = a;
            Level = level;
        }

        internal long Id { get; set; }
        public long AccountId { get; set; }
        public byte WorldId { get; set; }
        public string CharName { get; set; }
        public byte CharRace { get; set; }
        public byte CharGender { get; set; }
        public string Guid { get; set; } = "DC0D0CFCD3E01847AD2A5D55EA471CDF"; //для теста
        public long V { get; set; }
        public int[] Type { get; set; } = new int[18];
        public float[] Weight { get; set; } = new float[18];
        public float Scale { get; set; }
        public float Rotate { get; set; }
        public float MoveX { get; set; }
        public float MoveY { get; set; }
        public int Lip { get; set; }
        public int LeftPupil { get; set; }
        public int RightPupil { get; set; }
        public int Eyebrow { get; set; }
        public int Decor { get; set; }
        public string Modifiers { get; set; }
        public byte[] A { get; set; } = new byte[3];
        public byte Level { get; set; }
    }
}
