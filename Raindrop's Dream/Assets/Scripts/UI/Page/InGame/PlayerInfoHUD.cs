using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RDUI
{
    public class PlayerInfoHUD : BasePage
    {
        [SerializeField]
        private Image healthMask;
        // Use this for initialization
        void Start()
        {
            messageDelegate += OnHealthChange;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void OnHealthChange(object _health)
        {
          
        }
    }
}

