using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiGameProject.Models;

namespace AiGameProject.ViewModels
{
    public class TurnManager : MonoBehaviour
    {
        public enum TurnPhase
        {
            PlayerTurn,
            EnemyTurn,
            TurnIntervention,
            TurnEnd
        }

        public TurnPhase CurrentPhase { get; private set; }

        [SerializeField] private BattleManager battleManager;

        [SerializeField] private List<Hero> heroes;   // 플레이어 모델
        [SerializeField] private Enemy enemy;         // 단일 적 모델

        private Queue<Hero> playerTurnQueue = new();
        private float playerTimeRemaining = 10f;
        private float maxPlayerTime = 10f;
        private int enemyTurnIndex = 0;

        public delegate void OnPhaseChanged(TurnPhase newPhase);
        public event OnPhaseChanged PhaseChanged;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            battleManager.StartBattle(heroes, enemy);

            foreach (var hero in heroes)
                playerTurnQueue.Enqueue(hero);

            CurrentPhase = TurnPhase.PlayerTurn;
            PhaseChanged?.Invoke(CurrentPhase);

            StartPlayerTurn();
        }

        private void Update()
        {
            if (CurrentPhase == TurnPhase.PlayerTurn)
            {
                playerTimeRemaining -= Time.deltaTime;
                if (playerTimeRemaining <= 0f)
                {
                    EndPlayerTurn();
                }
            }
        }

        public void StartPlayerTurn()
        {
            playerTimeRemaining = maxPlayerTime;
            CurrentPhase = TurnPhase.PlayerTurn;
            PhaseChanged?.Invoke(CurrentPhase);

            battleManager.StartPlayerTurn();
        }

        public void EndPlayerTurn()
        {
            CurrentPhase = TurnPhase.EnemyTurn;
            PhaseChanged?.Invoke(CurrentPhase);

            battleManager.ExecuteSkills(); // 큐에 등록된 스킬 실행
            StartCoroutine(ExecuteEnemyTurn());
        }

        private IEnumerator ExecuteEnemyTurn()
        {
            yield return new WaitForSeconds(1f);

            var skill = enemy.GetNextSkill(enemyTurnIndex);
            if (skill != null)
            {
                Debug.Log($" 적 [{enemy.Name}]이 스킬 [{skill.Name}] 사용! (소울프레임: {enemy.SoulFrame.Type})");
                // TODO: 여기에 타겟 선택 및 데미지 적용 구현 예정
            }
            else
            {
                Debug.Log($" 적 [{enemy.Name}]은 사용할 스킬이 없습니다.");
            }

            enemyTurnIndex++;

            yield return new WaitForSeconds(1f);

            StartPlayerTurn();
        }

        public void TriggerIntervention()
        {
            CurrentPhase = TurnPhase.TurnIntervention;
            PhaseChanged?.Invoke(CurrentPhase);

            Debug.Log(" 턴 개입 발생");
            // 추후: 개입 UI 또는 선택지 팝업 등
        }
    }
}
