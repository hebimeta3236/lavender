using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    //UI�ɕ\������A�C�e����
    public string itemname = "";
    public string useTextID = "";
    public Sprite itemIcon;
    public virtual string UseItem()
    {
        Debug.Log("Default Item Use " + gameObject.name);
        return useTextID;
    }
}
