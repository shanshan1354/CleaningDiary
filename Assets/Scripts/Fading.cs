using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 血迹渐渐消失
/// </summary>
public class Fading : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float speed;//血迹消失速度,可以外面进行调节
    public float waitTime;
    private bool startFading;
    public bool isEnemyBlood;
    public AudioSource audioSource;
    public AudioClip enemyDieClip;
    public AudioClip playerDieClip;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource!=null)
        {
            if (isEnemyBlood)
            {
                audioSource.PlayOneShot(enemyDieClip,GameManager.Instance.volume);
            }
            else
            {
                audioSource.PlayOneShot(playerDieClip, GameManager.Instance.volume);
            }
        }
        Invoke("FadeBlood", waitTime);
    }

    void Update()
    {
        if (startFading)
        {
            //血迹消失褪色
            Color color = spriteRenderer.color;
            color.a -= speed * Time.deltaTime;
            Mathf.Clamp(color.a, 0, 1);
            spriteRenderer.color = color;
            if (color.a<=0)
            {
                Destroy(gameObject);
            }
        }
    }
    private void FadeBlood()
    {
        startFading = true;
    }
}
