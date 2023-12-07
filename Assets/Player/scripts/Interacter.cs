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
    [SerializeField] Gimicks gimicks = null;
    private Lavender action;


    // Start is called before the first frame update
    RaycastHit hit;
    void Start()
    {
        //�R�[���o�b�N�ݒ�
        action = new Lavender();
        action.Enable();
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
        if (gimicks!=null && action.Player.Fire.triggered)
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

                itemStack.EnableItem(dT.itemID);
                itemStack.DisableItem(dT.downFrag);

                //�C�x���g����
                Debug.Log(dT.demoID);
                demoPlayer.DemoPlay(dT.demoID);
            }
        }
    }
}
