using UnityEngine;
using UnityEngine.UI;
using AiGameProject.Models;
using AiGameProject.ViewModels;

namespace AiGameProject.Views
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Text label;

        private Skill skill;
        private BattleManager battleManager;

        public void Initialize(Skill skill, BattleManager manager)
        {
            this.skill = skill;
            this.battleManager = manager;

            if (label != null)
                label.text = skill.Name;

            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            battleManager.RegisterSkill(skill);
        }
    }
}