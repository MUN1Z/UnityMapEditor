using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Managers;

namespace UI
{
    public class LevelSelectButton : MonoBehaviour
    {
        public Text txt;
        string lvlName;

        public void InitButton(string n)
        {
            if (txt == null)
                txt = GetComponentInChildren<Text>();

            txt.text = n;
            lvlName = n;
        }

        public void PressButton()
        {
            SessionMaster.GetInstance().StartSoloActual(lvlName);
        }
    }
}
