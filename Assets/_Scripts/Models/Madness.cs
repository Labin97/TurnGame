using System;

namespace AiGameProject.Models
{
    public enum MadnessState
    {
        Normal,
        Disturbed,
        Afflicted
    }

    public class Madness
    {
        public int Gauge { get; private set; } = 0;

        public MadnessState State
        {
            get
            {
                if (Gauge >= 100) return MadnessState.Afflicted;
                if (Gauge >= 60) return MadnessState.Disturbed;
                return MadnessState.Normal;
            }
        }

        public void Add(int value)
        {
            Gauge = Math.Min(100, Gauge + value);
        }

        public void Reduce(int value)
        {
            Gauge = Math.Max(0, Gauge - value);
        }

        public void Reset()
        {
            Gauge = 0;
        }

        public string GetEffect()
        {
            return State switch
            {
                MadnessState.Normal => "정상",
                MadnessState.Disturbed => "불안정: 집중력 저하",
                MadnessState.Afflicted => "광기: 무작위 행동, 스킬 실패 등",
                _ => "알 수 없음"
            };
        }
    }
}