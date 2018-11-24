using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
    public class CurrentFloor : MonoBehaviour
    {
        public Text text;

        public void UpdateText(string t)
        {
            if (text == null)
                text = GetComponent<Text>();
            text.text = t;
        }

        static public CurrentFloor singleton;
        void Awake()
        {
            singleton = this;
        }
      
    }
}
