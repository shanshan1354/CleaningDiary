using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public GameObject locked;
    public GameObject unLocked;

    public void OpenDoor()
    {
        locked.SetActive(false);
        unLocked.SetActive(true);
    }
}
