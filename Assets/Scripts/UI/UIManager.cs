using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 菜单场景UI管理
/// </summary>
public class UIManager : MonoBehaviour
{
    //共用变量
    private int mainChoice;//玩家选择的界面索引
    public GameObject[] panels;//关卡、武器、设置界面
    public Image[] imgSelects;//选择界面的图标
    public Text txtMoney;
    private GameManager gameManager;

    //关卡界面
    public Image[] imgLevels;
    private Button[] btnLevels;
    private Text[] txtLevels;
    //武器界面
    public Image[] imgSelectedWeapons;
    private Button[] btnSelectedWeapons;
    private Text[] txtSelectedWeapons;
    public int[] weaponPrice;
    //设置界面
    public Image imgJoy;
    public Image imgVol;
    public Sprite volSprite;//音量显示图标
    public Sprite muteSprite;//静音显示图标
    
    //UI音频
    public AudioClip[] clipSound;
    private AudioSource audioSource;

    public GameObject gameManagerGO;

    private void Awake()
    {
        if (GameManager.Instance==null)
        {
            Instantiate(gameManagerGO);
        }
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        weaponPrice = new int[] { 0, 100, 200, 350, 400, 500 };
        audioSource = GetComponent<AudioSource>();
        btnLevels = new Button[imgLevels.Length];
        txtLevels = new Text[imgLevels.Length];
        for (int i = 0; i < imgLevels.Length; i++)
        {
            btnLevels[i] = imgLevels[i].GetComponent<Button>();
            txtLevels[i] = imgLevels[i].transform.GetChild(0).GetComponent<Text>();
        }

        btnSelectedWeapons = new Button[imgSelectedWeapons.Length];
        txtSelectedWeapons = new Text[imgSelectedWeapons.Length];
        for (int i = 0; i < imgSelectedWeapons.Length; i++)
        {
            btnSelectedWeapons[i] = imgSelectedWeapons[i].GetComponent<Button>();
            txtSelectedWeapons[i] = imgSelectedWeapons[i].transform.GetChild(0).GetComponent<Text>();
        }
        mainChoice = 0;
        UpdateMoneyText();
        InitUI();
    }

    #region 初始化UI

    //初始化UI
    void InitUI()
    {
        InitCommon();
        InitLevel();
        InitWeapon();
        InitSetting();
    }

    //共用界面
    void InitCommon()
    {
        //隐藏panels
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
            imgSelects[i].color = new Color(0, 1, 0, 0.3f);
        }
        imgSelects[mainChoice].color = new Color(0, 1, 0, 1);
        panels[mainChoice].SetActive(true);
    }
    //初始化关卡界面
    void InitLevel()
    {
        for (int i = 0; i < imgLevels.Length; i++)
        {
            if (gameManager.unLockedLevel>=i+1)
            {
                imgLevels[0].color = new Color(0, 1, 0, 0.3f);
                txtLevels[i].color = new Color(0, 1, 0, 0.3f);
                btnLevels[i].interactable = true;
            }
            else
            {
                imgLevels[i].color = new Color(1, 0, 0, 0.3f);
                txtLevels[i].color = new Color(1, 0, 0, 0.3f);
                btnLevels[i].interactable = false;
            }
        }
        imgLevels[gameManager.selectLevel - 1].color = new Color(0, 1, 0, 1);
        txtLevels[gameManager.selectLevel - 1].color = new Color(0, 1, 0, 1);
    }
    //初始化武器界面
    void InitWeapon()
    {
        for (int i = 0; i < imgSelectedWeapons.Length; i++)
        {
            if (gameManager.money>=weaponPrice[i])
            {
                imgSelectedWeapons[i].color = new Color(0, 1, 0, 0.3f);
                txtSelectedWeapons[i].color = new Color(0, 1, 0, 0.3f);
                btnSelectedWeapons[i].interactable = true;
            }
            else
            {
                imgSelectedWeapons[i].color = new Color(1, 0, 0, 0.3f);
                txtSelectedWeapons[i].color = new Color(1, 0, 0, 0.3f);
                btnSelectedWeapons[i].interactable = false;
            }
        }
        imgSelectedWeapons[gameManager.gunLevel-1].color = new Color(0, 1, 0, 1);
        txtSelectedWeapons[gameManager.gunLevel - 1].color = new Color(0, 1, 0, 1);
    }
    //初始化设置界面
    void InitSetting()
    {
        imgJoy.rectTransform.localScale = new Vector2(gameManager.joystickSize * 2, gameManager.joystickSize * 2);
        if (gameManager.volume==0)
        {
            imgVol.sprite = muteSprite;
            imgVol.color = new Color(1, 0, 0, 0.3f);
        }
        else
        {
            imgVol.sprite = volSprite;
            imgVol.color = new Color(0, 1, 0, gameManager.volume);
        }
    }

    #endregion

    #region 共用界面
    //更新金钱的方法
    void UpdateMoneyText()
    {
        txtMoney.text = gameManager.money.ToString();
    }

    //播放按钮音效
    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(clipSound[0], gameManager.volume);
    }
    //播放界面切换音效
    public void PlayPanelSound()
    {
        audioSource.PlayOneShot(clipSound[1], gameManager.volume);
    }

    //显示相关界面
    public void ShowPanel(int Choice)
    {
        mainChoice = Choice;
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
            imgSelects[i].color = new Color(0, 1, 0, 0.3f);
        }
        panels[mainChoice].SetActive(true);
        imgSelects[mainChoice].color = new Color(0, 1, 0, 1);

    }

    //开始游戏
    public void StartGame()
    {
        SceneManager.LoadScene(7);
    }
    #endregion

    #region 关卡界面
    //选关卡
    public void SelectLevel(int selectLevel)
    {
        gameManager.selectLevel = selectLevel;
        InitLevel();
    }
    #endregion
    #region 武器界面
    //选枪
    public void SelectGun(int gunLevel)
    {
        gameManager.gunLevel = gunLevel;
        InitWeapon();
    }
    #endregion
    #region 设置界面
    //设置摇杆大小
    public void SetJoyStickSize()
    {
        gameManager.joystickSize += 0.05f;
        if (gameManager.joystickSize>0.5f)
        {
            gameManager.joystickSize = 0.3f;
        }
        imgJoy.rectTransform.localScale = new Vector2(gameManager.joystickSize * 2, gameManager.joystickSize * 2);
        PlayerPrefs.SetFloat("Joystick", gameManager.joystickSize);
    }
    //设置音量
    public void SetVolume()
    {
        gameManager.volume += 0.2f;
        if (gameManager.volume>1)
        {
            gameManager.volume = 0;
            imgVol.color = new Color(1, 0, 0, 0.3f);
            imgVol.sprite = muteSprite;
        }
        else
        {
            imgVol.color = new Color(0, 1, 0, gameManager.volume);
            imgVol.sprite = volSprite;
        }
        PlayerPrefs.SetFloat("Sound", gameManager.volume);
    }
    //清空存档
    public void ClearSave()
    {
        PlayerPrefs.DeleteAll();
        gameManager.unLockedLevel = 1;
        gameManager.gunLevel = 1;
        gameManager.selectLevel = 1;
        gameManager.money = 0;

        SceneManager.LoadScene(0);
    }
    //退出游戏
    public void ExitGame()
    {
        PlayerPrefs.SetFloat("Sound", gameManager.volume);
        PlayerPrefs.SetFloat("Joystick", gameManager.joystickSize);
        PlayerPrefs.SetInt("Money", gameManager.money);
        PlayerPrefs.SetInt("Levels", gameManager.unLockedLevel);
        Application.Quit();
    }
    #endregion
}
