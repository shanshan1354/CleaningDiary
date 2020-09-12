using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 操作说明
/// </summary>
public class Instruction : MonoBehaviour
{
    private TextMesh textMesh;
    private float viewDistance;//极限距离
    private float distance;
    private Transform playerTrans;
    void Start()
    {
        viewDistance = 2f;
        playerTrans = GameManager.Instance.player.transform;
        textMesh = GetComponent<TextMesh>();
    }

    void Update()
    {
        if (playerTrans==null)
        {
            return;
        }
        distance = Vector3.Distance(transform.position, playerTrans.position);
        if (distance<viewDistance)
        {
            textMesh.characterSize = 0.05f * (1-distance / viewDistance);
        }
        else
        {
            textMesh.characterSize = 0;
        }
    }
}
