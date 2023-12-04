using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemStack : MonoBehaviour
{
    [System.Serializable]
    struct ItemSet
    {
        public bool enable;
        public DTItem item;
        public Items itemComponent;
        public Image img;
    }
    [SerializeField] float itemUIdistance = 5f;
    [SerializeField] Transform itemIconAnker;
    [SerializeField] GameObject itemIconPrefab;
    [SerializeField] UIcontroller uic;
    [SerializeField] DT_Frag DTfrag;
    [SerializeField] DT_Item DTitem;
    [SerializeField] List<ItemSet> itemList = new();
    [SerializeField] Sprite dummy;
    [SerializeField] TMP_Text itemname;
    [SerializeField] Color selectColor, Offcolor;
    
    [SerializeField] int nowitem = 0;
    int EnableNum()
    {
        int i = 0;
        foreach(ItemSet set in itemList)
        {
            if(set.enable == true)
            {
                i++;
            }
        }
        return i;
    }
    void LineupItems()
    {
        int i = 0;
        foreach(ItemSet set in itemList)
        {
            //set�@��z�u
            set.img.transform.localPosition = new Vector3(itemUIdistance * i, 0f, 0f);
            set.img.color = Offcolor;
            //�A�C�e������肵��
            if (set.enable)
            {
                set.img.sprite = set.itemComponent.itemIcon;
                DTfrag.SetVal(set.itemComponent.myfrag, false);
            }
            else
            {
                //���Ă��Ȃ��Ȃ�_�~�[�摜
                set.img.sprite = dummy;
                itemname.text = "???";
            }
            i++;
        }
        //�I�𒆃A�C�e��
        itemList[nowitem].img.color = selectColor;
        if (itemList[nowitem].enable)
        {
            itemname.text = itemList[nowitem].itemComponent.itemname;
            DTfrag.SetVal(itemList[nowitem].itemComponent.myfrag, true);

        }
    }
    public void DisableItem(string id)
    {
        //���i�K id��ItemList id�ɍ��v���邩�H �����烊�X�g�̃t���O��܂�
        if (id == "")
        {
            return;
        }
        for(int i = 0; i < itemList.Count; i++)
        {
            ItemSet set = itemList[i];
            if (set.item.id == id)
            {
                set.enable = false;
                DTfrag.SetVal(set.itemComponent.myfrag, false);
                itemList[i] = set;
                break;
            }
        }
        RotateItem(1);
        LineupItems();
    }
    public void EnableItem(string id)
    {
        if (id == "")
        {
            return;
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            ItemSet set = itemList[i];
            if (set.item.id == id)
            {
                set.enable = true;
                itemList[i] = set;
                Debug.Log(i);

                RotateItem(i - nowitem);
            }
        }
        LineupItems();
    }

    void RotateItem(int r)
    {
        //r...�i�߂鐔
        int itemN = itemList.Count;
        
        if (nowitem == 0 && r < 0)
        {
            //nowitem �őO�� �Ō��ɍs�������Ƃ��̓��X�g�̍Ō���ɃW�����v
            nowitem = itemN - 1;
        }
        else if (nowitem == itemN - 1 && r > 0)
        {
            //nowitem �Ō�� �őO�ɍs�������Ƃ��͏��߂ɃW�����v
            nowitem = 0;
        }
        else
        {
            nowitem += r;
        }
    }
    // Start is called before the first frame update
    
    void Start()
    {
        int x = 0;
        foreach(DTItem dti in DTitem.ItemsList)
        {
            //�I�u�W�F��ǉ�
            GameObject i = Instantiate(dti.item);
            DTItem tItem = new();
            tItem.id = dti.id;
            tItem.item = i;
            //items.Add(tItem);
            //�A�C�R����ǉ�
            Items itemComp = i.GetComponent<Items>();
            GameObject iconobj = Instantiate(itemIconPrefab, itemIconAnker);
            Image iconImg = iconobj.GetComponent<Image>();
            Sprite sprite = itemComp.itemIcon;
            iconImg.sprite = sprite;
            //itemIcons.Add(iconImg);

            ItemSet set = new();
            set.enable = false;
            set.item = tItem;
            set.img = iconImg;
            set.itemComponent = itemComp;
            itemList.Add(set);
            x++;
        }
        Debug.Log(EnableNum());
        LineupItems();
    }
    
    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.X))
        {
            DisableItem(itemList[nowitem].item.id);
            //LineupItems();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            EnableItem("KL_CassetteTape_003_Obtained");
        }
        */
        if (EnableNum() > 0)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateItem(-1);
                LineupItems();

            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateItem(1);
                LineupItems();
            }
            else if (itemList[nowitem].enable && Input.GetKeyDown(KeyCode.F))
            {
                Items itemComponent = itemList[nowitem].itemComponent;//items[nowitem].item.GetComponent<Items>();
                string id = itemComponent.UseItem();
                Debug.Log("Use item " + id);
                //�A�C�e���e�L�X�g�\��

                uic.ActiveUI(id);
            }
        }
    }
}
