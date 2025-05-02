using UnityEngine; // Unity에서 게임 오브젝트, 컴포넌트, Transform 같은 핵심 기능들을 쓰기 위해 필요한 코드
using UnityEngine.SceneManagement; // 다른 씬으로 이동하는 기능(Scene 전환)을 사용하려면 꼭 필요한 코드

public class SceneLoader : MonoBehaviour // 'SceneLoader'라는 이름의 스크립트 클래스(설계도) 선언
{ // MonoBehaviour는 유니티에서 '이건 게임 오브젝트에 붙일 수 있는 스크립트야' 라는 뜻

    // public은 누구나 밖에서 호출할 수 있는 함수, 버튼 클릭 같은 이벤트가 이 함수를 실행할 수 있도록 만듦
    // LoadDungeon은 함수 이름. 마음대로 바꿀 수 있지만 지금은 유지
    public void LoadDungeon()
    {
        // TODO: 나중에 서버에 "던전 입장 시작" 기록 요청을 보낼 예정
        Debug.Log("서버 호출 예정 - 던전 입장 기록 남기기");

        // DungeonScene이라는 이름의 씬으로 전환 (SceneManager를 통해)
        SceneManager.LoadScene("DungeonScene");
    }
}

