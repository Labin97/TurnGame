using System.Collections.Generic;
using AiGameProject.Models;

namespace AiGameProject.Utils
{
    public static class MadnessManager
    {
        public static void IncreaseMadnessForAll(List<Hero> heroes, int amount)
        {
            foreach (var hero in heroes)
            {
                hero.Madness.Add(amount);
            }
        }

        public static void ReduceMadnessForAll(List<Hero> heroes, int amount)
        {
            foreach (var hero in heroes)
            {
                hero.Madness.Reduce(amount);
            }
        }

        public static void ResetAll(List<Hero> heroes)
        {
            foreach (var hero in heroes)
            {
                hero.Madness.Reset();
            }
        }
    }
}
