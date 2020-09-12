using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 炮台
/// </summary>
public class Turret : MonoBehaviour
{
    public GameObject mark;
    private float minDistance;
    private float maxDistance;
    private Enemy nearestEnemy;
    private RaycastHit2D hit;
    private LayerMask layerMask;
    private AudioSource audioSource;
    public float attackCD;
    private float curAttackCD;
    public int bullets;
    public GameObject bulletGO;
    public Transform[] shootPoints;
    private int turretLevel;
    public float inaccuracy;
    public AudioClip shootClip;
    private Player player;
    private float distance;
    public bool isEnemyTurret;
    void Start()
    {
        player = GameManager.Instance.player;
        minDistance = maxDistance = 7;
        audioSource = GetComponent<AudioSource>();
        mark.SetActive(false);
        if (isEnemyTurret)
        {
            layerMask = ~(1 << 10) & ~(1 << 2);
        }
        else
        {
            layerMask = ~(1 << 9) & ~(1 << 2);
        }
        turretLevel = GameManager.Instance.gunLevel / 2;
        if (turretLevel==0)
        {
            turretLevel = 1;
        }
    }

    void Update()
    {
        if (curAttackCD > 0)
        {
            curAttackCD -= Time.deltaTime;
        }
        if (isEnemyTurret)//敌人炮台
        {
            if (player==null)
            {
                return;
            }
            hit = Physics2D.Raycast(transform.position,
                   player.transform.position - transform.position, 10, layerMask);
            if (hit.collider != null)
            {
                distance = Vector3.Distance(player.transform.position, transform.position);
                if (!hit.collider.gameObject.CompareTag("Wall"))
                {
                    Vector3 moveDirection = player.transform.position - transform.position;
                    if (moveDirection != Vector3.zero)//确定炮台的方向
                    {
                        float angle;
                        angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.AngleAxis(angle, -Vector3.forward);
                    }
                    if (curAttackCD <= 0 && bullets > 0)
                    {
                        Instantiate(bulletGO, shootPoints[0].position,
                            Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + Random.Range(-inaccuracy, inaccuracy)));
                        
                        audioSource.PlayOneShot(shootClip, GameManager.Instance.volume * 0.6f);
                        bullets -= 1;
                        curAttackCD = attackCD;
                    }
                }
            }
        }
        else//玩家炮台
        {
            for (int i = 0; i < player.enemyList.Count; i++)
            {
                hit = Physics2D.Raycast(transform.position,
                    player.enemyList[i].transform.position - transform.position, 3, layerMask);
                if (hit.collider != null)
                {
                    if (!hit.collider.gameObject.CompareTag("Wall"))
                    {
                        distance = Vector3.Distance(transform.position, player.enemyList[i].transform.position);

                        if (distance < maxDistance && distance < minDistance)//在炮台视野中
                        {
                            minDistance = distance;
                            nearestEnemy = player.enemyList[i];
                        }
                    }
                }
            }
            if (nearestEnemy != null)//标记敌人
            {
                mark.SetActive(true);
                Vector3 moveDirection = nearestEnemy.transform.position - transform.position;

                if (moveDirection != Vector3.zero)//确定炮台的方向
                {
                    float angle;
                    angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, -Vector3.forward);
                }
                if (curAttackCD <= 0 && bullets > 0)
                {
                    Instantiate(bulletGO, shootPoints[0].position,
                        Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + Random.Range(-inaccuracy, inaccuracy)));
                    if (turretLevel >= 3)
                    {
                        Instantiate(bulletGO, shootPoints[1].position,
                        Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + Random.Range(-inaccuracy, inaccuracy)));
                    }
                    audioSource.PlayOneShot(shootClip, GameManager.Instance.volume * 0.6f);
                    bullets -= 1;
                    curAttackCD = attackCD;
                }
                
            }
            else
            {
                mark.SetActive(false);
            }
            if (hit.collider==null)
            {
                mark.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        if (!isEnemyTurret)
        {
            if (nearestEnemy!=null)
            {
                mark.transform.position = nearestEnemy.transform.position;
                if (hit.collider!=null)
                {
                    if (hit.collider.CompareTag("Wall"))
                    {
                        minDistance = maxDistance = 7;
                        nearestEnemy = null;
                    }
                }
            }
            else
            {
                minDistance = maxDistance = 7;
                nearestEnemy = null;
            }
        }
    }
}
