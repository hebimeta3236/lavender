using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacter : MonoBehaviour
{
    //���N���b�N����C���^���N�g�ł��鎞��
    [SerializeField] float iTime = 0.2f;
    [SerializeField] UIcontroller uictr;
    [SerializeField] ItemStack itemStack;
    [SerializeField] DT_Item dtitem;
    [SerializeField] DemoPlayer demoPlayer;
    [SerializeField] bool touchingOnother = false;
    

    // Start is called before the first frame update
    [SerializeField]
    Gimicks gimicks = null;
    RaycastHit hit;
    void Start()
    {
        //uictr = GetComponent<UIcontroller>();
        //itemStack = GetComponent<ItemStack>();
        demoPlayer = FindAnyObjectByType<DemoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 0.8f);
        if (Physics.Raycast(transform.position, transform.forward * 0.8f, out hit))
        {
            //�Ƃ肠�����G��Ă���
            if (hit.collider.CompareTag("Gimick"))
            {
                if (gimicks == null)
                {
                    Gimicks tmpgm = hit.collider.GetComponent<Gimicks>();
                    gimicks = tmpgm;
                    gimicks.EmitColor();
                }
            }
        }
        //�M�~�b�N���痣�ꂽ��I�t
        if (gimicks != null && hit.collider.gameObject != gimicks.gameObject)
        {
            gimicks.TurnOffColor();
            gimicks = null;
        }
        //�C���^���N�g�{�^���I��
        if (gimicks!=null && Input.GetMouseButtonDown(0))
        {
            //StartCoroutine(InteractTrigger());
            string textid;
            DTGimick dT;
            //ui �N��
            if (gimicks != null)
            {
                dT = gimicks.InteractGimick();
                if (dT == null)
                {
                    return;
                }
                textid = dT.textID;
                Debug.Log(textid);
                uictr.ActiveUI(textid);

                //�A�C�e���ǉ�
                Debug.Log("itemID->" + dT.itemID);
                //GameObject i = dtitem.FindItem(dT.itemID);
                //itemStack.AddItem(i);
                itemStack.EnableItem(dT.itemID);
                itemStack.DisableItem(dT.downFrag);
                //gimicks.TurnOffColor();
                //gimicks = null;

                //�C�x���g����
                Debug.Log(dT.demoID);
                demoPlayer.DemoPlay(dT.demoID);
            }
        }
    }
}
