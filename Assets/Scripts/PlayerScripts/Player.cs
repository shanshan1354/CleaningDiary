using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 玩家主脚本
/// </summary>
public class Player : MonoBehaviour
{
    //资源
    public GameObject blood;//血迹
    public GameObject bloodParticle;//减血特效
    public AudioClip bonus;//奖励音效
    public GameObject explosion;
    public GameObject deadBloodGO;

    //数据
    public bool isMoving;//玩家是否移动
    public float moveAngle;//移动角度
    public int speed;//移动速度
    public int maxHP;//血量上限
    public float HP;//当前血量
    public int regenHpSpeed;//HP回复速度
    public int delayRegen;//脱离战斗一定时间后（进行血量回复）
    public float delayTimer;//回复血量计时器
    public int kills;//通过需要击杀的敌人数量
    public int curKills;//当前击杀的敌人数量
    public bool bossIsDead;
    private bool decreaseHP;//被感染后持续掉血

    //引用
    private Rigidbody2D playerRig;
    private AudioSource audioSource;
    public List<Enemy> enemyList;
    public PlayerShooting playerShooting;

    void OnEnable()
    {
        GameManager.Instance.player = this;
        HP=maxHP = 100;
        speed = 15;
        regenHpSpeed = delayRegen = 1;
        audioSource = GetComponent<AudioSource>();
        playerRig = GetComponent<Rigidbody2D>();
        enemyList = new List<Enemy>();
        playerShooting = GameObject.Find("ShootPoint").GetComponent<PlayerShooting>();
        if (GameManager.Instance.selectLevel>3)
        {
            bossIsDead = false;
        }
        else
        {
            bossIsDead = true;
        }
        if (GameManager.Instance.selectLevel==6&&!GameManager.Instance.anthonyIsDead)
        {
            decreaseHP = true;
        }
    }
    void Update()
    {
        PlayerMove();
        if (decreaseHP)
        {
            HP -= Time.deltaTime;
            LimitHP();
            Die();
        }
        else
        {
            RegenHP();
        }
    }
    //玩家移动
    private void PlayerMove()
    {
        isMoving = false;

#if UNITY_STANDALONE_WIN
        if (Input.GetKey(KeyCode.W))
        {
            isMoving = true;
            playerRig.AddForce(new Vector2(0, speed / (1 + 0.1f * playerShooting.weight)) * 70 * Time.deltaTime);
            moveAngle = 0;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            isMoving = true;
            playerRig.AddForce(new Vector2(0, -speed / (1 + 0.1f * playerShooting.weight)) * 70 * Time.deltaTime);
            moveAngle = 180;
        }
        if (Input.GetKey(KeyCode.A))
        {
            isMoving = true;
            playerRig.AddForce(new Vector2(-speed / (1 + 0.1f * playerShooting.weight), 0) * 70 * Time.deltaTime);
            moveAngle = 90;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            isMoving = true;
            playerRig.AddForce(new Vector2(speed / (1 + 0.1f * playerShooting.weight), 0) * 70 * Time.deltaTime);
            moveAngle = -90;
        }
#elif UNITY_ANDROID
        //摇杆控制移动
        playerRig.AddForce(new Vector2(GameManager.Instance.inputValue.x,GameManager.Instance.inputValue.y)
            * speed / (1 + 0.1f * playerShooting.weight)*70*Time.deltaTime);
        
        if (GameManager.Instance.inputValue!=Vector2.zero)
        {
            isMoving = true;
            moveAngle = GameManager.Instance.inputAngle;
        }
        else
        {
            isMoving = false;
            moveAngle = 0;
        }
#endif
    }

    //玩家脱离战斗后进行血量回复
    private void RegenHP()
    {
        if (delayTimer<=0&&HP<maxHP)
        {
            HP += regenHpSpeed * Time.deltaTime;
            LimitHP();
        }
        else if(delayTimer>0)
        {
            delayTimer -= Time.deltaTime;
        }
    }
    //道具
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string colTagStr = collision.tag;
        switch (colTagStr)
        {
            case "EnemyBullet":
                Instantiate(bloodParticle, transform.position, Quaternion.Euler
                    (0, 0, collision.transform.rotation.eulerAngles.z + 180));
                Instantiate(blood, transform.position, Quaternion.Euler
                    (0, 0, collision.transform.rotation.eulerAngles.z + 180));
                Destroy(collision.gameObject);
                HP -= 10;
                LimitHP();
                    Die();
                break;
            case "EnemyMine":
                Instantiate(explosion, transform.position, Quaternion.identity);
                HP -= 60;
                LimitHP();
                Destroy(collision.gameObject);
                Die();
                break;
            case "BulletItem":
                if (playerShooting.curBullets<playerShooting.totalBullets)
                {
                    DestryItem(collision);
                    playerShooting.curBullets +=(int) (playerShooting.totalBullets * 0.25f);
                    playerShooting.LimitBullet();
                }
                break;
            case "HealthItem":
                if (HP<maxHP)
                {
                    DestryItem(collision);
                    HP += 20;
                    LimitHP();
                }
                break;
            case "MineItem":
                if (playerShooting.curMine<3)
                {
                    DestryItem(collision);
                    playerShooting.curMine += 1;
                }
                GameManager.Instance.gameUIManager.UpdateMineUI((float)playerShooting.curMine / 3);
                break;
            case "MoneyItem":
                DestryItem(collision);
                GameManager.Instance.money += 5;
                break;
            case "TurretItem":
                if (!playerShooting.hasTurret)
                {
                    DestryItem(collision);
                    playerShooting.hasTurret = true;
                }
                GameManager.Instance.gameUIManager.UpdateTurretUI(playerShooting.hasTurret);
                break;
            case "Key":
                audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                collision.GetComponent<Key>().OpenDoor();
                Destroy(collision.gameObject);
                break;
            default:
                break;
        }

    }

    private void DestryItem(Collider2D collision)
    {
        Destroy(collision.gameObject);
        audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
    }
    //补给箱
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Box")
        {
            if (collision.gameObject.name.Contains("Bullet"))
            {
                if (playerShooting.curBullets<playerShooting.totalBullets)
                {
                    playerShooting.curBullets += (int)(playerShooting.totalBullets * 0.25f);
                    playerShooting.LimitBullet();
                    audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                }
            }
            else if (collision.gameObject.name.Contains("Health"))
            {
                if (HP<maxHP)
                {
                    HP += 20;
                    LimitHP();
                    audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                }
            }
            else if (collision.gameObject.name.Contains("Turret"))
            {
                if (playerShooting.curMine<3)
                {
                    audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                    playerShooting.curMine = playerShooting.mines;
                    GameManager.Instance.gameUIManager.UpdateMineUI(1);
                }
                if (!playerShooting.hasTurret)
                {
                    playerShooting.hasTurret = true;
                    GameManager.Instance.gameUIManager.UpdateTurretUI(playerShooting.hasTurret);
                    audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                }

            }
        }
        
    }

    //玩家死亡
    public void Die()
    {
        if (HP<=0)
        {
            Instantiate(deadBloodGO, transform.position, transform.rotation);
            GameManager.Instance.LoadMainScene();
            Destroy(gameObject);
        }
    }

    //显示玩家血量
    public void LimitHP()
    {
        if (HP>=maxHP)
        {
            HP = maxHP;
        }
        else if (HP<0)
        {
            HP = 0;
        }
        GameManager.Instance.gameUIManager.UpdateHPSlider(HP / maxHP);
    }
    
}
