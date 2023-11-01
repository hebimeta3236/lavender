using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacter : MonoBehaviour
{
    //���N���b�N����C���^���N�g�ł��鎞��
    [SerializeField] float iTime = 0.2f;
    [SerializeField] UIcontroller uictr;
    public bool interactTrigger = false;
    

    // Start is called before the first frame update
    [SerializeField]
    Gimicks gimicks = null;
    IEnumerator InteractTrigger()
    {
        interactTrigger = true;
        yield return new WaitForSeconds(iTime);
        interactTrigger = false;
    }
    void Start()
    {
        //uictr = GetComponent<UIcontroller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(InteractTrigger());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Gimick")
        {
            Gimicks tmpgm = other.GetComponent<Gimicks>();
            if (gimicks == null)
            {
                gimicks = tmpgm;
                gimicks.EmitColor();
            }else if(gimicks != null)
            {
                gimicks.TurnOffColor();
                gimicks = tmpgm;
                gimicks.EmitColor();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Gimick" && interactTrigger)
        {
            interactTrigger = false;
            string textid;
            //ui �N��
            if (gimicks != null)
            {
                textid = gimicks.InteractGimick();
                Debug.Log(textid);
                uictr.ActiveUI(textid);
                gimicks.TurnOffColor();
                gimicks = null;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (gimicks != null && other.gameObject == gimicks.gameObject)
        {
            gimicks.TurnOffColor();
            gimicks = null;
        }
    }
}
