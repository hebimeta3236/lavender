using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.Playables;

public class UIcontroller : MonoBehaviour
{
    [SerializeField] DT_Text dttext;
    [SerializeField] TextAsset playesence;
    [SerializeField] string pathsence = "Assets/Player/scripts/DataTable/PlayerSence.json";
    [SerializeField] Vector2 maxSence;
    [SerializeField] RectTransform[] UIs;
    [SerializeField] RectTransform[] choices;
    [SerializeField] TMP_Text[] uitexts;
    [SerializeField] TMP_Text dialogPageView;
    //�e�L�X�g�_�C�A���O�̃y�[�W
    [SerializeField] int pageNum = 0;
    [SerializeField] List<string> dialog;
    [SerializeField] Cinemachine.CinemachineVirtualCamera vc;
    Cinemachine.CinemachinePOV pov;
    [SerializeField] Slider xslider, yslder;

    [SerializeField] float voiceSpeed = 0.2f,finishWait = 2.5f;
    [System.Serializable]
    private class Sence
    {
        public float Xsence;
        public float Ysence;
    }

    [SerializeField] Sence sence;
    Vector2 nowvec;

    Coroutine E;

    //�M�~�b�N�̃C�x���g�Ăяo��
    //�ėp�̑I�����Ŏg��
    void ChosesAction(DTText dT)
    {
        //chose �� nextText������->�e�L�X�g�\���C�x���g
        
        if (dT.nextText != "")
        {
            ActiveUI(dT.nextText);
        }
        //�A�C�e������C�x���g�͂�������
    }

    //�e�L�X�g�_�C�A���O�̃y�[�W����
    public void PageSend(int p)
    {
        Debug.Log("next page->"+(pageNum + p).ToString());
        if(pageNum+p<0 || pageNum + p >= dialog.Count)
        {
            return;
        }
        pageNum += p;
        uitexts[1].text = dialog[pageNum];
        dialogPageView.text = (pageNum + 1).ToString() + "/" + dialog.Count.ToString();
    }

    
    public void CloseUIs()
    {
        Debug.Log("close ui");

        //interactColider.enabled = true;
        pageNum = 0;
        SetVirtualCamera(true);
        Cursor.lockState = CursorLockMode.Locked;
        foreach (RectTransform r in UIs)
        {
            r.gameObject.SetActive(false);
        }
    }
    public void ActiveUI(string textid)
    {
        //�C���^���N�g�R���C�_�[�𖳌���
        //interactColider.enabled = false;
        int i = -1;
        DTText dT = dttext.Find(textid);
        //�J�[�\�����b�N���O��
        Cursor.lockState = CursorLockMode.None;
        //�J�����Œ�
        SetVirtualCamera(false);
        if (textid == "esc")
        {
            i = 3;
        }
        else if (dT != null)
        {
            i = dT.ui;
        }

        Debug.Log(i);
        int j = 0;
        foreach (RectTransform r in UIs)
        {
            r.gameObject.SetActive(false);
            if (i == j)
            {
                
                r.gameObject.SetActive(true);
            }
            j += 1;
        }
        
        if(i == 0)
        {
            //�ėp
            uitexts[i].text = dT.text;
            Cursor.lockState = CursorLockMode.None;
            List<string> choiseID = new List<string>
            {
                dT.choise0,
                dT.choise1,
                dT.choise2,
                dT.choise3
            };

            for (int c=0; c < choiseID.Count; c++)
            {
                if (choiseID[c] == "")
                {
                    choices[c].gameObject.SetActive(false);
                }
                else
                {
                    choices[c].gameObject.SetActive(true);
                    DTText choiseText = dttext.Find(choiseID[c]);
                    choices[c].GetComponentInChildren<TMP_Text>().text = choiseText.text;
                    //�C�x���g��o�^
                    choices[c].GetComponent<Button>().onClick.AddListener(() => ChosesAction(choiseText));
                }
            }
        }
        else if (i == 1)
        {
            //�e�L�X�g�_�C�A���O
            //�{���ɑ��
            int pagenum;
            dialog = new List<string>();
            (pagenum, dialog) = dttext.Pages(textid);
            uitexts[i].text = dialog[0];
            dialogPageView.text = (pageNum + 1).ToString() + "/" + dialog.Count.ToString();
            
        }
        else if (i == 2)
        {
            //voice
            if (E != null)
            {
                StopCoroutine(E);
                E = null;
            }
            int pagenum;
            dialog = new List<string>();
            (pagenum, dialog) = dttext.Pages(textid);
            Cursor.lockState = CursorLockMode.Locked;
            SetVirtualCamera(true);
            E = StartCoroutine(VoiceText());
        }
        else
        {
            //esc
            xslider.value = sence.Xsence;
            yslder.value = sence.Ysence;
        }
    }
    public void XSence()
    {
        nowvec.x = xslider.value * maxSence.x;
    }
    public void YSence()
    {
        nowvec.y = yslder.value * maxSence.y;
    }
    public void WriteSence()
    {
        pov.m_HorizontalAxis.m_MaxSpeed = nowvec.x;
        pov.m_VerticalAxis.m_MaxSpeed = nowvec.y;
        sence.Xsence = xslider.value;
        sence.Ysence = yslder.value;
        string json = JsonUtility.ToJson(sence);
        StreamWriter wr = new StreamWriter(pathsence);
        wr.WriteLine(json);
        wr.Close();
    }

    void SetVirtualCamera(bool b)
    {
        if (b) {
            pov.m_HorizontalAxis.m_MaxSpeed = nowvec.x;
            pov.m_VerticalAxis.m_MaxSpeed = nowvec.x;
        }
        else
        {
            pov.m_HorizontalAxis.m_MaxSpeed = 0f;
            pov.m_VerticalAxis.m_MaxSpeed = 0f;
        }
    }

    IEnumerator VoiceText()
    {
        WaitForSeconds delay;
        foreach (string d in dialog)
        {
            uitexts[2].text = d;
            int textLength = uitexts[2].text.Length;
            delay = new WaitForSeconds(voiceSpeed);
            Debug.Log(textLength);
            for (int i = 0; i <= textLength; i++)
            {
                uitexts[2].maxVisibleCharacters = i;
                yield return delay;
            }
            delay = new WaitForSeconds(finishWait);
            yield return delay;
        }
        delay = null;
        uitexts[2].text = "";
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pov = vc.GetCinemachineComponent<Cinemachine.CinemachinePOV>();
        sence = JsonUtility.FromJson<Sence>(playesence.text);
        nowvec.x = sence.Xsence * maxSence.x;
        nowvec.y = sence.Ysence * maxSence.y;

    }
    // Update is called once per frame
    void Update()
    {
        //Esc
        if (Input.GetKeyDown(KeyCode.Escape)){
            ActiveUI("esc");
            sence = JsonUtility.FromJson<Sence>(playesence.text);

        }
    }
}
