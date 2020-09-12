using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextWriteEffect : MonoBehaviour
{
    public LoadGame loadGame;
    public float charPerSecond;//打字时间间隔
    private string words;//整个需要打出的日记内容
    private bool startWrite;//是否开始撕打字特效
    private float timer;//计时器
    private Text text;
    private int currentPos;
    private AudioSource audioSource;
    void Start()
    {
        
        timer = 0;
        startWrite = false;
        charPerSecond = 0.1f;
        text = GetComponent<Text>();
        int storiesIndex = GameManager.Instance.selectLevel - 1;
        if (storiesIndex==5)
        {
            if (GameManager.Instance.anthonyIsDead)
            {
                storiesIndex = 5;
            }
            else
            {
                storiesIndex = 7;
            }
            if (GameManager.Instance.showEnd)
            {
                storiesIndex++;
            }
        }
        words = GameManager.Instance.stories[storiesIndex];
        text.text = "";
        audioSource = GetComponent<AudioSource>();
        StartEffect();
    }

    void Update()
    {
        StartWriting();
    }

    public void StartEffect()
    {
        startWrite = true;
    }
    //具体的书写方法
    private void StartWriting()
    {
        if (startWrite)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            timer += Time.deltaTime;
            if (timer>charPerSecond)
            {
                timer = 0;
                currentPos++;
                text.text = words.Substring(0, currentPos);
                if (currentPos>=words.Length)
                {
                    FinisshWriting();
                }
            }
        }
    }

    private void FinisshWriting()
    {
        startWrite = false;
        audioSource.Stop();
        timer = 0;
        currentPos = 0;
        text.text = words;
        Invoke("DelayLoadNextScene", 2);
    }

    private void DelayLoadNextScene()
    {
        GameManager.Instance.showEnd = false;
        loadGame.LoadNextScene();
    }
}
