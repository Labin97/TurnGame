using System;

namespace AiGameProject.Models
{
    public class Skill
    {
        public string Name { get; }
        public string Description { get; }
        public int TimeCost { get; } // 초 단위 시간 자원 소모
        public SoulFrameType SoulFrameType { get; }

        public Action PerformEffect { get; private set; }
        public Action PerformMadnessEffect { get; private set; }

        public Skill(string name, string description, int timeCost, SoulFrameType soulFrameType,
                     Action performEffect, Action performMadnessEffect)
        {
            Name = name;
            Description = description;
            TimeCost = timeCost;
            SoulFrameType = soulFrameType;
            PerformEffect = performEffect;
            PerformMadnessEffect = performMadnessEffect;
        }

        public void Execute(bool isAfflicted)
        {
            if (isAfflicted)
                PerformMadnessEffect?.Invoke();
            else
                PerformEffect?.Invoke();
        }

        public string GetDisplayDescription(bool isAfflicted)
        {
            return isAfflicted
                ? $" [광기] {Name} - {Description} (효과 왜곡됨)"
                : $"{Name} - {Description}";
        }
    }
}
