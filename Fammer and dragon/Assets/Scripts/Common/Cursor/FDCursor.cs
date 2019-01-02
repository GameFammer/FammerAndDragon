using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FDCursor : MonoBehaviour
{
    public Transform followRoot;
    public Image followImg;
    public Text followSizeText;
    public GameObject FollowObj
    {
        get
        {
            return followObj;
        }
        set
        {
            followObj = value;
            if (value != null)
            {
                followImg.sprite = followObj.GetComponent<SpriteRenderer>().sprite;
                followImg.gameObject.SetActive(true);
                followSizeText.text = followObj.GetComponent<Item>().stackSize.ToString();
            }
            else
            {
                followImg.sprite = null;
                followImg.gameObject.SetActive(false);
                followSizeText.text = string.Empty;
            }
        }
    }
    private GameObject followObj;
    // Use this for initialization
    void Start()
    {
        if(followObj == null)
        {
            followImg.sprite = null;
            followImg.gameObject.SetActive(false);
            followSizeText.text = string.Empty;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<RectTransform>().position = Input.mousePosition;
    }
}
