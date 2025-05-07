using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AiGameProject.Models;

namespace AiGameProject.Views
{
    public class SkillQueueUI : MonoBehaviour
    {
        [SerializeField] private Transform skillListContainer;
        [SerializeField] private GameObject skillSlotPrefab;

        private readonly List<GameObject> spawnedSlots = new List<GameObject>();

        public void Refresh(List<Skill> skillQueue)
        {
            Clear();

            foreach (Skill skill in skillQueue)
            {
                GameObject slot = Instantiate(skillSlotPrefab, skillListContainer);
                Text text = slot.GetComponentInChildren<Text>();

                if (text != null)
                    text.text = skill.Name;

                spawnedSlots.Add(slot);
            }
        }

        public void Clear()
        {
            foreach (var slot in spawnedSlots)
            {
                Destroy(slot);
            }
            spawnedSlots.Clear();
        }
    }
}
