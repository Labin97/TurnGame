using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class StageSlot : MonoBehaviour
{
    public TMP_Text stageLabel;
    public GameObject lockIcon;
    private Button button;

    // 초기화 메서드
    public void Init(int regionId, int stageId, System.Action<int, int> callback, bool isCleared)
    {
        // 유효하지 않은 스테이지 ID 처리 (1 미만 또는 존재하지 않는 키)
        if (stageId < 1 || !FileSystem.Instance.IsValidStage(regionId, stageId))
        {
            Debug.LogWarning($"StageSlot.Init: Invalid stageId ({regionId}-{stageId}) on {name}, disabling slot.");
            gameObject.SetActive(false);
            return;
        }

        // 라벨과 아이콘이 연결되어 있는지 확인
        if (stageLabel == null || lockIcon == null)
        {
            Debug.LogError($"StageSlot.Init: Missing reference on {name}.");
            return;
        }

        // 텍스트와 잠금 아이콘 설정
        stageLabel.text = $"{regionId}-{stageId}";
        lockIcon.SetActive(!isCleared);

        // 버튼 설정
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"StageSlot.Init: Button component missing on {name}.");
            return;
        }
        button.interactable = isCleared;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback(regionId, stageId));
    }

    // 이후 다음 스테이지 강조용 애니메이션 함수 자리
    public void AnimateNext()
    {
        // TODO: 애니메이터 또는 코루틴 기반 강조 효과 구현
    }
}
