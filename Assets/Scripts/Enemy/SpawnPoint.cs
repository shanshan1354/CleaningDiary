using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private float delayTimer;
    public float spawnTime;
    public int limit;//生成敌人数量
    public GameObject enemyGO;
    private bool startSpawn;//是否开始生成敌人

    void Start()
    {
        delayTimer = spawnTime;
    }

    void Update()
    {
        if (startSpawn)
        {
            if (delayTimer>0)
            {
                delayTimer -= Time.deltaTime;
            }
            else
            {
                if (limit>0)
                {
                    Instantiate(enemyGO, transform.position+
                        new Vector3(Random.Range(-0.1f,0.1f), Random.Range(-0.1f, 0.1f), 0), transform.rotation);
                    delayTimer = spawnTime;
                    limit -= 1;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!startSpawn)
            {
                startSpawn = true;
            }
        }
    }
}
