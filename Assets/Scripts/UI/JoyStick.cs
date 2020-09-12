using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyStick : ScrollRect
{
    private float radius;

    private Transform imgArrowTrans;

    protected override void Start()
    {
        base.Start();
        radius = content.sizeDelta.x * 0.5f;
        imgArrowTrans = transform.Find("Img_Arrow");
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        Vector2 contentPos = content.anchoredPosition;
        //判断摇杆的位置是否大于半径
        if (contentPos.magnitude>radius)
        {
            //将摇杆设置到边界处
            contentPos = contentPos.normalized*radius;
            SetContentAnchoredPosition(contentPos);
        }
        Vector2 inputVector = content.anchoredPosition.normalized;
        GameManager.Instance.inputValue = inputVector;
        float inputAngle = -Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg;
        GameManager.Instance.inputAngle = inputAngle;
        imgArrowTrans.rotation = Quaternion.Euler(0, 0, inputAngle);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        SetContentAnchoredPosition(Vector2.zero);
        GameManager.Instance.inputValue = Vector2.zero;
        imgArrowTrans.rotation = Quaternion.identity;
    }
}
