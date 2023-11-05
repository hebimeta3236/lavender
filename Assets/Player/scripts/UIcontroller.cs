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
    [SerializeField] DT_Item dtitem;
    [SerializeField] ItemStack itemStack;
    [SerializeField] TextAsset playesence;
    [SerializeField] string pathsence = "Assets/Player/scripts/DataTable/PlayerSence.json";
    [SerializeField] Vector2 maxSence;
    [SerializeField] RectTransform[] UIs;
    [SerializeField] RectTransform[] choices;
    [SerializeField] TMP_Text[] uitexts;
    [SerializeField] TMP_Text dialogPageView;
    //テキストダイアログのページ
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

    //ギミックのイベント呼び出し
    //汎用の選択肢で使う
    void ChosesAction(DTText dT)
    {
        //chose の nextTextがある->テキスト表示イベント
        Debug.Log("Choise Act ->" + dT.id);
        if (dT.nextText != "")
        {
            //テキスト
            ActiveUI(dT.nextText);
        }
        //アイテム入手イベントはここから
        GameObject[] itemchosen = new GameObject[4];
        DTDemo d = null;
        if (dT.choise0 != "")
        {
            itemchosen[0] = dtitem.FindItem(dT.choise0);
        }
        if(dT.choise1 != "")
        {
            itemchosen[1] = dtitem.FindItem(dT.choise1);
        }
        if (dT.choise2 != "")
        {
            itemchosen[2] = dtitem.FindItem(dT.choise2);
        }
        if (dT.choise3 != "")
        {
            itemchosen[3] = dtitem.FindItem(dT.choise3);
        }
        foreach(GameObject i in itemchosen)
        {
            if (i != null)
            {
                itemStack.AddItem(i);
            }
        }
        
    }

    //テキストダイアログのページ送り
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
        //インタラクトコライダーを無効か
        //interactColider.enabled = false;
        int i = -1;
        DTText dT = dttext.Find(textid);
        //カーソルロックを外す
        Cursor.lockState = CursorLockMode.None;
        //カメラ固定
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

        if (i == 0)
        {
            //汎用
            uitexts[i].text = dT.text;
            Cursor.lockState = CursorLockMode.None;
            List<string> choiseID = new List<string>
            {
                dT.choise0,
                dT.choise1,
                dT.choise2,
                dT.choise3
            };

            for (int c = 0; c < choiseID.Count; c++)
            {
                if (choiseID[c] == "")
                {
                    choices[c].gameObject.SetActive(false);
                }
                else
                {
                    
                    DTText choiseText = dttext.Find(choiseID[c]);
                    Debug.Log(choiseID[c]);
                    if (choiseText != null)
                    {
                        choices[c].gameObject.SetActive(true);
                        choices[c].GetComponentInChildren<TMP_Text>().text = choiseText.text;
                        //イベントを登録
                        Button button = choices[c].GetComponent<Button>();
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => ChosesAction(choiseText));
                    }
                }
            }
        }
        else if (i == 1)
        {
            //テキストダイアログ
            //本文に代入
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
        else if (i == -1)
        {
            //esc
            xslider.value = sence.Xsence;
            yslder.value = sence.Ysence;
        }
        else
        {
            return;
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
