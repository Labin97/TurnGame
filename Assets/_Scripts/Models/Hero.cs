using AiGameProject.Models;

namespace AiGameProject.Models
{
    public class Hero
    {
        public string Name { get; }
        public SoulFrame SoulFrame { get; }
        public Skill Skill { get; }
        public Madness Madness { get; }

        public Hero(string name, SoulFrame soulFrame, Skill skill)
        {
            Name = name;
            SoulFrame = soulFrame;
            Skill = skill;
            Madness = new Madness();
        }

        public bool IsAfflicted()
        {
            return Madness.State == MadnessState.Afflicted;
        }

        // 굳이 스킬 변형 필요 없이, 원본 반환
        public Skill GetUsableSkill()
        {
            return Skill;
        }
    }
}
