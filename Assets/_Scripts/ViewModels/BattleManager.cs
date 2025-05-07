using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiGameProject.Models;
using AiGameProject.Views;
using AiGameProject.Utils;

namespace AiGameProject.ViewModels
{
    public class BattleManager : MonoBehaviour
    {
        [Header("전투 유닛")]
        public List<Hero> Heroes { get; private set; }
        public Enemy CurrentEnemy { get; private set; }

        [Header("턴 및 자원")]
        private int turnIndex = 0;
        private float playerTimeLeft = 30f;
        private bool isPlayerTurn = true;
        private Queue<Skill> playerSkillQueue = new Queue<Skill>();

        [Header("UI 참조")]
        [SerializeField] private TimeGaugeUI timeGauge;
        [SerializeField] private SkillQueueUI skillQueueUI;
        [SerializeField] private PopupController popupController;
        [SerializeField] private SkillCastPopupUI skillCastPopupUI;
        [SerializeField] private List<HeroStatusUI> heroStatusUIs;
        [SerializeField] private SkillButtonGroupUI skillButtonGroupUI;

        //  추가된 스킬 버튼 UI 연결
        [SerializeField] private Transform skillButtonGroupTransform;
        [SerializeField] private GameObject skillButtonPrefab;

        void Update()
        {
            if (!isPlayerTurn) return;

            playerTimeLeft -= Time.deltaTime;
            timeGauge.UpdateTime(playerTimeLeft);

            if (playerTimeLeft <= 0f)
            {
                EndPlayerTurn();
            }
        }
        void Start()
        {
            var dummyHeroes = new List<Hero>();
            for (int i = 0; i < 3; i++)
            {
                var skill = new Skill(
                    $"스킬{i + 1}",
                    $"설명{i + 1}",
                    5 + i * 2,
                    SoulFrameType.ENFP,
                    () => Debug.Log($"스킬{i + 1} 발동"),
                    () => Debug.Log($"스킬{i + 1} 광기 발동")
                );

                var hero = new Hero(
                    $"영웅{i + 1}",
                    new SoulFrame(SoulFrameType.ENFP), // ✅ 수정됨
                    skill
                );

                dummyHeroes.Add(hero);
            }

            var enemySkills = new List<Skill>
    {
        new Skill("적스킬1", "적 스킬 설명", 5, SoulFrameType.INTJ, () => Debug.Log("적 스킬 발동"), () => {})
    };

            var dummyEnemy = new Enemy(
                "더미 적",
                new SoulFrame(SoulFrameType.INTJ),
                enemySkills,
                false
            );

            StartBattle(dummyHeroes, dummyEnemy);
        }




        public void StartBattle(List<Hero> heroes, Enemy enemy)
        {
            Heroes = heroes;
            CurrentEnemy = enemy;

            turnIndex = 0;
            isPlayerTurn = true;
            playerSkillQueue.Clear();

            MadnessManager.ResetAll(Heroes);

            for (int i = 0; i < Heroes.Count && i < heroStatusUIs.Count; i++)
            {
                heroStatusUIs[i].SetHero(Heroes[i]);
            }

            skillCastPopupUI.Hide(); // 시작 시 숨김

            skillButtonGroupUI.Setup(Heroes, RegisterSkill);

            StartPlayerTurn();
        }

        public void StartPlayerTurn()
        {
            isPlayerTurn = true;
            playerTimeLeft = 30f;

            timeGauge.SetMaxTime(playerTimeLeft);
            playerSkillQueue.Clear();
            skillQueueUI.Refresh(new List<Skill>());

            // 🔥 기존 스킬 버튼 제거
            foreach (Transform child in skillButtonGroupTransform)
                Destroy(child.gameObject);

            // 🔥 새로운 스킬 버튼 생성
            foreach (var hero in Heroes)
            {
                var skill = hero.Skill;
                var go = Instantiate(skillButtonPrefab, skillButtonGroupTransform);
                var ui = go.GetComponent<SkillButtonUI>();
                ui.SetSkill(skill.Name, null, 0f, 0f); // 아이콘은 아직 null
                ui.SetOnClick(() => RegisterSkill(skill));
            }

            Debug.Log("플레이어 턴 시작");
        }

        public void RegisterSkill(Skill skill)
        {
            if (!isPlayerTurn || playerTimeLeft < skill.TimeCost) return;

            playerSkillQueue.Enqueue(skill);
            playerTimeLeft -= skill.TimeCost;

            skillQueueUI.Refresh(new List<Skill>(playerSkillQueue));
        }

        public void EndPlayerTurn()
        {
            isPlayerTurn = false;
            ExecutePlayerSkills();
        }

        private void ExecutePlayerSkills()
        {
            Debug.Log("플레이어 스킬 실행 시작");
            StartCoroutine(ExecuteSkillSequence());
        }

        private IEnumerator ExecuteSkillSequence()
        {
            while (playerSkillQueue.Count > 0)
            {
                Skill skill = playerSkillQueue.Dequeue();

                // 팝업 연출
                skillCastPopupUI.Show(null, skill.Name, skill.SoulFrameType.ToString());
                yield return new WaitForSeconds(1.0f);

                skill.Execute(false);

                skillCastPopupUI.Hide();
                yield return new WaitForSeconds(0.3f);
            }

            skillQueueUI.Refresh(new List<Skill>());
            StartEnemyTurn();
        }

        public void ExecuteSkills() => ExecutePlayerSkills();

        public void OnTurnEndButtonClicked()
        {
            if (isPlayerTurn)
            {
                Debug.Log("수동으로 턴 종료");
                EndPlayerTurn();
            }
        }

        private void StartEnemyTurn()
        {
            isPlayerTurn = false;

            Skill enemySkill = CurrentEnemy.GetNextSkill(turnIndex);
            Debug.Log($"적 스킬 사용: {enemySkill.Name}");
            enemySkill.Execute(false);

            turnIndex++;

            popupController.ShowPopup(() =>
            {
                Debug.Log("개입 선택됨 → 플레이어 턴 시작");
                StartPlayerTurn();
            });

            Invoke(nameof(StartEnemyTurnIfNoIntervene), 1.1f);
        }

        private void StartEnemyTurnIfNoIntervene()
        {
            if (isPlayerTurn) return;

            MadnessManager.IncreaseMadnessForAll(Heroes, 10);
            foreach (var ui in heroStatusUIs) ui.UpdateUI();

            Debug.Log("개입 안함 → 모든 영웅 광기 +10");

            StartEnemyTurn();
        }
    }
}
