using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Game场景中的UI管理
/// </summary>
public class GameUIManager : MonoBehaviour
{
    public Slider hpSlider;//血条
    public Slider bulletSlider;//子弹条
    public CanvasGroup canvasGroup;
    public GameObject joyStick;//摇杆
    public Image imgWeapon;//武器显示
    public Sprite[] weaponSprite;//显示不同的武器图片（根据玩家当前选择的武器）

    private GameManager gameManager;

    public Image imgAttackCD;//显示攻击cd
    public Image imgReloadCD;//换弹cd
    public Image imgLeftMine;//地雷个数显示
    public Image imgTurret;//炮台图片
    public RenderTexture renderTexture;
    public RawImage rawImage;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.gameUIManager = this;
#if UNITY_STANDALONE_WIN
        canvasGroup.interactable = false;
        imgWeapon.gameObject.SetActive(true);
        imgWeapon.sprite = weaponSprite[gameManager.gunLevel - 1];
        joyStick.SetActive(false);
#elif UNITY_ANDROID
        canvasGroup.interactable = true;
        imgWeapon.transform.parent.gameObject.SetActive(false);
        joyStick.SetActive(true);
        gameManager.inputValue = Vector2.zero;
        gameManager.inputAngle = 0;
#endif

    }

    #region UI更新方法
    //更新血条
    public void UpdateHPSlider(float value)
    {
        hpSlider.value = value;
    }

    //更新子弹条
    public void UpdateBulletSlider(float value)
    {
        bulletSlider.value = value;
    }
    //炮台是否使用UI显示
    public void UpdateTurretUI(bool hasTurret)
    {
        if (hasTurret)
        {
            imgTurret.fillAmount = 1;
        }
        else
        {
            imgTurret.fillAmount = 0;
        }
    }

    //剩余地雷个数的UI显示
    public void UpdateMineUI(float value)
    {
        imgLeftMine.fillAmount = value;
    }

    //攻击cd
    public void UpdateAttackCDUI(float value)
    {
        imgAttackCD.fillAmount = value;
    }
    //换弹时显示装弹完成剩余时间，装弹完毕后显示弹夹中子弹剩余数量
    public void UpdateReloadAndMagazineBulletUI(float value)
    {
        imgReloadCD.fillAmount = value;
    }
    #endregion
    #region 按钮方法
    public void Reload()
    {
        gameManager.player.playerShooting.Reload();
    }

    public void SetMine()
    {
        gameManager.player.playerShooting.SetMine();
    }

    public void SetTurret()
    {
        gameManager.player.playerShooting.SetTurret();
    }

    public void Attack()
    {
        gameManager.player.playerShooting.Shoot();
    }
    #endregion
}
