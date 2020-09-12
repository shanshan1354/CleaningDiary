using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 玩家AI管理者，负责搜索敌人以及瞄准敌人
/// </summary>
public class PlayerAIM : MonoBehaviour
{
    public Transform cameraTrans;//相机位置
    public GameObject mark;//用来标记敌人的标记
    public float minDistance;//敌人离玩家的最近距离（优先攻击）
    public float maxDistance;//最远距离，玩家视野最远范围
    private Player player;
    private Enemy nearestEnemy;//最近的敌人
    private RaycastHit2D hit;
    private LayerMask layerValue;//检测层
    private float distance;

    void Start()
    {
        cameraTrans = Camera.main.transform;
        player = GameManager.Instance.player;
        mark.SetActive(false);
        minDistance = maxDistance = 10;
        layerValue = ~(1 << 9)&~(1<<2);
    }

    void Update()
    {
        for (int i = 0; i < player.enemyList.Count; i++)
        {
            if (player.enemyList[i]==null)
            {
                return;
            }
            hit = Physics2D.Raycast(transform.position,
                player.enemyList[i].transform.position - transform.position,4, layerValue);
            if (hit.collider!=null)
            {
                if (!hit.collider.gameObject.CompareTag("Wall"))
                {
                    distance = Vector3.Distance(transform.position, player.enemyList[i].transform.position);
                    if (distance<maxDistance&&distance<minDistance)//在玩家视野里
                    {
                        minDistance = distance;
                        nearestEnemy = player.enemyList[i];
                    }
                }
            }
        }
        if (nearestEnemy!=null)
        {
            mark.SetActive(true);
            
                Vector3 moveDirection = nearestEnemy.transform.position - transform.position;
                if (moveDirection != Vector3.zero)
                {
                    float angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, -Vector3.forward);
                }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, player.moveAngle);
            mark.SetActive(false);
        }
        
    }


    private void LateUpdate()
    {
        cameraTrans.transform.position = new Vector3
            (transform.position.x, transform.position.y, -7.8f);

        if (nearestEnemy!= null)
        {
            mark.transform.position = nearestEnemy.transform.position;
            
            if (hit.collider!=null)
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    minDistance = maxDistance = 10;
                    nearestEnemy = null;
                }
            }
        }
        else
        {
            minDistance = maxDistance = 10;
            nearestEnemy = null;
        }
    }
}
