using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    public enum SceneType
    {
        Title = 0,
        Lobby,
        GateReady,
        Gate,
        GateSelection,
        StageSelection,
        Battle,
        // ...
    }

    /**
     * 특정 Scene 으로 전환하는 함수.
     * Button 의 OnClick 에 바인딩하는 경우, SceneType Enum 값 변경으로 Scene 을 전환할 수 있게 한다.
     */
    [VisibleEnum(typeof(SceneType))]
    public void ChangeScene(int _sceneType)
    {
        SceneType sceneType = (SceneType)_sceneType;

        switch (sceneType)
        {
            case SceneType.Title:
            {
                SceneManager.LoadScene(Const.TitleSceneName);
            }
            break;
            case SceneType.Lobby:
            {
                SceneManager.LoadScene(Const.LobbySceneName);
            }
            break;
            case SceneType.GateReady:
            {
                SceneManager.LoadScene(Const.GateReadySceneName);
            }
            break;
            case SceneType.Gate:
            {
                SceneManager.LoadScene(Const.GateSceneName);
            }
                break;
            case SceneType.GateSelection:
                {
                    SceneManager.LoadScene(Const.GateSelectionSceneName);
                }
                break;
            case SceneType.StageSelection:
                {
                    SceneManager.LoadScene(Const.StageSelectionSceneName);
                }
                break;
            case SceneType.Battle:
            {
                SceneManager.LoadScene(Const.BattleSceneName);
            }
            break;
            default:
            break;
        }
    }
}
