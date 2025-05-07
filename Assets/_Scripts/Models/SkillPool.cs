using AiGameProject.Models;
using System.Collections.Generic;

public class SkillPool
{
    private List<Skill> allSkills = new List<Skill>();

    public void AddSkill(Skill skill) => allSkills.Add(skill);

    public Skill GetRandomSkill()
    {
        if (allSkills.Count == 0) return null;
        return allSkills[UnityEngine.Random.Range(0, allSkills.Count)];
    }
}
