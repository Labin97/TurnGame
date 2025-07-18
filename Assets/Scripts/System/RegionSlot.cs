using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegionSlot : MonoBehaviour
{
    public TMP_Text regionLabel;
    public GameObject lockIcon;

    public void Init(int id, System.Action<int> callback, bool isUnlocked)
    {
        regionLabel.text = $"Region {id}";           // 지역 레이블 설정
        lockIcon.SetActive(!isUnlocked);               // 잠금 아이콘 토글

        var btn = GetComponent<Button>();               // 버튼 컴포넌트 참조
        btn.interactable = isUnlocked;                 // 언락된 경우에만 클릭 가능
        btn.onClick.RemoveAllListeners();              // 기존 리스너 초기화
        btn.onClick.AddListener(() => callback(id));   // 클릭 시 콜백 호출
    }

    // ------------------------------
    // 추가된 오버로드 Init: 애니메이션 플래그 처리
    // ------------------------------
    public void Init(int id, System.Action<int> callback, bool isUnlocked, bool isNextRegion)
    {
        // 1) 기존 기능 재사용
        Init(id, callback, isUnlocked);

        // 2) 새로 언락된 Region만 블링크 애니메이션
        if (isNextRegion)
        {
            // 테스트용 블링크: lockIcon 깜빡임
            StartCoroutine(BlinkLockIcon());
        }
    }

    // ------------------------------
    // 테스트용 블링크 코루틴
    // ------------------------------
    private System.Collections.IEnumerator BlinkLockIcon()
    {
        for (int i = 0; i < 3; i++)
        {
            lockIcon.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            lockIcon.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
        // 최종적으로 잠금 상태 해제 표시
        lockIcon.SetActive(false);
    }
}

