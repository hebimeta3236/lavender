using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public Sprite itemIcon;
    public virtual void UseItem()
    {
        Debug.Log("Default Item Use " + gameObject.name);
    }
}
