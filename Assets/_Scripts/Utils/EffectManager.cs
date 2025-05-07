using UnityEngine;

namespace AiGameProject.Utils
{
    public static class EffectManager
    {
        public static void PlayEffect(string effectName, Vector3 position)
        {
            Debug.Log($"[이펙트] {effectName} at {position}");
            // 실제 파티클 등 연동은 추후
        }
    }
}