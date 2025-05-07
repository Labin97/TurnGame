using System.Collections.Generic;

namespace AiGameProject.Models
{
    public class Enemy
    {
        public string Name { get; private set; }
        public SoulFrame SoulFrame { get; private set; }
        public List<Skill> Skills { get; private set; }
        public bool IsBoss { get; private set; }

        public Enemy(string name, SoulFrame soulFrame, List<Skill> skills, bool isBoss = false)
        {
            Name = name;
            SoulFrame = soulFrame;
            Skills = skills ?? new List<Skill>();
            IsBoss = isBoss;
        }

        public Skill GetNextSkill(int turnIndex)
        {
            if (Skills == null || Skills.Count == 0) return null;
            return Skills[turnIndex % Skills.Count];
        }
    }
}
