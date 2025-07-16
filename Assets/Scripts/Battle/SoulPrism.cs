using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulPrism : MonoBehaviour
{
    private SoulPrismStats stats;
    private PersonalityType personalityType;
    private List<Skill> skills = new List<Skill>();

    public PersonalityType SPPersonalityType => personalityType;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<SoulPrismStats>();
        if (stats == null)
        {
            Debug.LogError("SoulPrism: SoulPrismStats is null");
            return;
        }
        stats.Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }


}
