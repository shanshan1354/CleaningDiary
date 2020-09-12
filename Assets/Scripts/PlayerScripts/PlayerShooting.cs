using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 玩家的射击控制（切枪，装弹等等）
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    //资源
    public GameObject[] bullets;//0.Pistol/AK/MG 1.Snipe 2.ShotGun 3.CrossBow
    public GameObject[] turrets;//炮台
    public GameObject mineGO;//地雷
    public AudioClip[] shootClips;//射击音效
    public AudioClip reloadClip;//装弹音效
   

    //引用
    public GameObject[] weapons;//武器
    public AudioSource audioSource;
    public GameObject lineLeft;
    public GameObject lineRight;
    private Player player;
    private WeaponProperties weaponProperties;
    public ParticleSystem flash;
    

    //属性
    public float maxInaccuracy;//最大不精确度
    public float minInaccuracy;//最小不精确度
    public float recoilForce;//后坐力（攻击中影响射击精度）
    public float destabilization;//不稳定性（在所有情况下都会影响精度）
    public float aimingDeSpeed;//瞄准不精确度的减少速度
    public float attackCD;//攻击CD
    public int magazine;//弹夹中子弹数量
    public int totalBullets;//全部弹药
    public float reload;//装弹时间
    public float weight;//枪重
    public int repulsion;//子弹击退力
    private int gunLevel;
    private int turretLevel;

    //当前属性
    private float curInaccuracy;//当前不精确度
    private float curDestabilization;//当前不稳定性
    private float curAttackCD;//当前攻击CD
    public int curMagazine;//当前弹夹里的子弹数量
    public int curBullets;//当前全部的子弹数量(包含弹夹里的子弹)
    public float curReload;//当前装弹Cd
    public bool hasTurret;//当前是否携带炮台
    public int curMine;//当前持有的地雷数量
    public int mines;//当前可持有的最大地雷数量


    void Start()
    {
        player = GameManager.Instance.player;
        gunLevel = GameManager.Instance.gunLevel;
        weapons[gunLevel - 1].SetActive(true);
        hasTurret = true;
        curMine = mines = 3;
        weaponProperties = GameManager.Instance.weaponPropertiesList[GameManager.Instance.gunLevel-1];
        maxInaccuracy = (float)weaponProperties.maxInaccuracy;
        recoilForce = (float)weaponProperties.recoilForce;
        destabilization = (float)weaponProperties.destabilization;
        aimingDeSpeed = (float)weaponProperties.aimingDeSpeed;
        minInaccuracy = (float)weaponProperties.minInaccuracy;
        attackCD = (float)weaponProperties.attackCD;
        magazine = weaponProperties.magazine;
        totalBullets = weaponProperties.totalBullets;
        reload = (float)weaponProperties.reload;
        weight = (float)weaponProperties.weight;
        repulsion = weaponProperties.repulsion;
        audioSource = GetComponent<AudioSource>();
        //当前属性赋值
        curBullets = totalBullets;
        curMagazine = magazine;
        curReload = curAttackCD = 0;
        turretLevel = GameManager.Instance.gunLevel / 2;
        if (turretLevel == 0)
        {
            turretLevel = 1;
        }

        player.playerShooting = this;
    }

    void Update()
    {
        CalculateDestabilization();
        CalculateInaccuracy();
        CalculateCD();
#if UNITY_STANDALONE_WIN
        MonitorInput();
#endif
    }
    //计算CD时间
    private void CalculateCD()
    {
        //攻击
        if (curAttackCD > 0)
        {
            curAttackCD -= Time.deltaTime;
        }
        GameManager.Instance.gameUIManager.UpdateAttackCDUI((attackCD - curAttackCD) / attackCD);
        //装弹
        if (curReload > 0)
        {
            curReload -= Time.deltaTime;
            GameManager.Instance.gameUIManager.UpdateReloadAndMagazineBulletUI((reload - curReload) / reload);
        }
        else
        {
            if (curMagazine <= 0)
            {
                curMagazine = magazine;
            }
        }
        
    }
    //计算算不稳定性
    private void CalculateDestabilization()
    {
        //移动后，不稳定性逐渐增加
        if (player.isMoving)
        {
            if (curDestabilization < destabilization)
            {
                curDestabilization += destabilization * Time.deltaTime * 1.5f;
            }
            else
            {
                curDestabilization = destabilization;
            }
        }
        else
        {
            curDestabilization = 0;
        }
    }
    //计算不精确度
    private void CalculateInaccuracy()
    {
        //当前不精确度>=最小不精确度+当前不稳定性
        if (curInaccuracy >= minInaccuracy + curDestabilization)
        {
            //表示当前不精确度很大，玩家会随着时间的推移慢慢的集中精神
            curInaccuracy -= Time.deltaTime * aimingDeSpeed;
        }
        else
        {
            curInaccuracy = minInaccuracy + curDestabilization;
        }
        lineLeft.transform.localRotation = Quaternion.AngleAxis(curInaccuracy, Vector3.forward);
        lineRight.transform.localRotation = Quaternion.AngleAxis(-curInaccuracy, Vector3.forward);
    }
    //射击
    public void Shoot()
    {
        if (curBullets > 0 && curMagazine > 0 && curAttackCD <= 0)
        {
            audioSource.PlayOneShot(shootClips[gunLevel - 1], GameManager.Instance.volume);
            switch (gunLevel)
            {
                case 1:
                case 2:
                case 3:
                    CreatBullet(0);
                    break;
                case 4:
                    CreatBullet(1);
                    break;
                case 5:
                    CreatBullet(2);
                    break;
                default:
                    CreatBullet(3);
                    break;
            }
            if (gunLevel != 1|| gunLevel != 6)
            {
                flash.Play();
            }
            curMagazine -= 1;
            GameManager.Instance.gameUIManager.UpdateReloadAndMagazineBulletUI((float)curMagazine / magazine);
            curBullets -= 1;
            LimitBullet();
            curAttackCD = attackCD;
            curInaccuracy += recoilForce;
            if (curInaccuracy>maxInaccuracy)
            {
                curInaccuracy = maxInaccuracy;
            }
            if (curMagazine<=0&&curReload<=0)
            {
                Reload();
            }
        }
    }
    //装弹方法
    public void Reload()
    {
        if (curMagazine<magazine)
        {
            GameManager.Instance.gameUIManager.UpdateReloadAndMagazineBulletUI((reload - curReload) / reload);
            audioSource.PlayOneShot(reloadClip, GameManager.Instance.volume);
            curReload = reload;
            curMagazine = 0;
        }
    }
    //放置地雷
    public void SetMine()
    {
        if (curMine > 0)
        {
            curMine -= 1;
            GameManager.Instance.gameUIManager.UpdateMineUI((float)curMine / 3);
            Instantiate(mineGO, transform.position, Quaternion.identity);
        }
    }
    //放置炮台
    public void SetTurret()
    {
        if (hasTurret)
        {
            hasTurret = false;
            GameManager.Instance.gameUIManager.UpdateTurretUI(hasTurret);
            Instantiate(turrets[turretLevel - 1], transform.position, player.transform.rotation);
        }
    }
    //产生子弹
    private void CreatBullet(int bulletIndex)
    {
        if (gunLevel == 5)
        {
            //散弹枪
            Instantiate(bullets[bulletIndex], transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z
                        - curInaccuracy));
            Instantiate(bullets[bulletIndex], transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z
                        - curInaccuracy * 0.5f));
            Instantiate(bullets[bulletIndex], transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));

            Instantiate(bullets[bulletIndex], transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z
                        + curInaccuracy * 0.5f));
            Instantiate(bullets[bulletIndex], transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z
                        + curInaccuracy));
        }
        else
        {
            Instantiate(bullets[bulletIndex], transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z
                        + UnityEngine.Random.Range(-curInaccuracy, curInaccuracy)));
        }
    }
    //监听玩家输入
    private void MonitorInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetTurret();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetMine();
        }
    }

    //限制以及更新子弹条
    public void LimitBullet()
    {
        if (curBullets >= totalBullets)
        {
            curBullets = totalBullets;
        }
        else if (curBullets < 0)
        {
            curBullets = 0;
        }
        GameManager.Instance.gameUIManager.UpdateBulletSlider((float)curBullets / totalBullets);
    }
}
/// <summary>
/// 武器属性结构体
/// </summary>
public struct WeaponProperties
{
    //属性
    public double maxInaccuracy;//最大不精确度
    public double recoilForce;//后坐力（攻击中影响射击精度）
    public double destabilization;//不稳定性（在所有情况下都会影响精度）
    public double aimingDeSpeed;//瞄准不精确度的减少速度
    public double minInaccuracy;//最小不精确度
    public double attackCD;//攻击CD
    public int magazine;//弹夹中子弹数量
    public int totalBullets;//全部弹药
    public double reload;//装弹时间
    public double weight;//枪重
    public int repulsion;//子弹击退力
}
