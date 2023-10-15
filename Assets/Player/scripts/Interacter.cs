using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacter : MonoBehaviour
{
    //���N���b�N����C���^���N�g�ł��鎞��
    [SerializeField] float iTime = 0.2f;
    private bool interactTrigger = false;
    // Start is called before the first frame update

    IEnumerator InteractTrigger()
    {
        interactTrigger = true;
        yield return new WaitForSeconds(iTime);
        interactTrigger = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(InteractTrigger());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Gimick" && interactTrigger)
        {
            interactTrigger = false;
            Gimicks gimicks = other.GetComponent<Gimicks>();
            if (gimicks != null)
            {
                gimicks.InteractGimick();
            }
        }
    }
}
