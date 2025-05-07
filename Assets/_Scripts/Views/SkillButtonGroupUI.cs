using System.Collections.Generic;
using UnityEngine;
using AiGameProject.Models;

namespace AiGameProject.Views  // ✅ 반드시 이 네임스페이스로 감싸야 함
{
    public class SkillButtonGroupUI : MonoBehaviour
    {
        [SerializeField] private GameObject skillButtonPrefab;
        [SerializeField] private Transform buttonRoot;

        private List<SkillButtonUI> currentButtons = new List<SkillButtonUI>();

        public void Setup(List<Hero> heroes, System.Action<Skill> onClick)
        {
            foreach (var btn in currentButtons)
            {
                if (btn != null) Destroy(btn.gameObject);
            }
            currentButtons.Clear();

            foreach (var hero in heroes)
            {
                var obj = Instantiate(skillButtonPrefab, buttonRoot);
                var ui = obj.GetComponent<SkillButtonUI>();

                if (ui != null)
                {
                    var skill = hero.Skill;
                    ui.SetSkill(skill.Name, null, 0f, 0f); // icon은 아직 null
                    ui.SetOnClick(() => onClick(skill));
                    currentButtons.Add(ui);
                }
            }
        }
    }
}
