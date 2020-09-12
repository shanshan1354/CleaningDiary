using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 加载游戏
/// </summary>
public class LoadGame : MonoBehaviour
{
    public GameObject diaryPanelGO;
    public Text text;
    private AsyncOperation ao;
    void Start()
    {
        diaryPanelGO.SetActive(false);
        if (GameManager.Instance.showEnd)
        {
            ao = SceneManager.LoadSceneAsync(0);
        }
        else
        {
            ao = SceneManager.LoadSceneAsync(GameManager.Instance.selectLevel);
        }
        ao.allowSceneActivation = false;
    }

    void Update()
    {
        if (ao.progress>=0.9f)
        {
            text.text = "按下任意键继续...";
            if (Input.anyKey)
            {
                if (GameManager.Instance.isFirstEnterLevels[GameManager.Instance.selectLevel-1])
                {
                    diaryPanelGO.SetActive(true);
                    gameObject.SetActive(false);
                    GameManager.Instance.isFirstEnterLevels[GameManager.Instance.selectLevel - 1] = false;
                    GameManager.Instance.SetBoolArray("FirstEnter", GameManager.Instance.isFirstEnterLevels);
                }
                else
                {
                    LoadNextScene();
                }
                
            }
        }
    }

    public void LoadNextScene()
    {
        ao.allowSceneActivation = true;
    }
}
