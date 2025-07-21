using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataManager : Singleton<SkillDataManager>
{
    public SkillData GetSkillData(string Id)
    {
        SkillData skillData = new SkillData();
        return skillData;
    }

}
