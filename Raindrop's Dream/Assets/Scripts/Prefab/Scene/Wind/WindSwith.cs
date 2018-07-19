using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSwith : MonoBehaviour
{

    public Sprite switchOnSprite;
    public Sprite switchOffSprite;
    public GameObject wall;
    public GameObject windRight;
    public GameObject windUp;
    public bool isOn = true;

    private SpriteRenderer renderer;
    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        SwitchStatus();

    }
    void SwitchStatus()
    {
        if (isOn)
        {
            renderer.sprite = switchOnSprite;
            wall.SetActive(true);
            windUp.SetActive(false);
            windRight.SetActive(true);
        }
        else
        {
            renderer.sprite = switchOffSprite;
            wall.SetActive(false);
            windRight.SetActive(false);
            windUp.SetActive(true);
        }
    }
    void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.tag == "Player")
        {
            if (isOn)
            {
                isOn = false;
                SwitchStatus();
            }
            else
            {
                isOn = true;
                SwitchStatus();
            }
        }
    }
}
