using AiGameProject.Models;

namespace AiGameProject.ViewModels
{
    public class EnemyAI
    {
        private Enemy enemy;

        public EnemyAI(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public Skill GetNextSkill(int turnIndex)
        {
            return enemy.GetNextSkill(turnIndex); // 단순 순환 기반. 확장 가능
        }
    }
}
