using UnityEngine;
using System.Collections;
using Managers;

namespace UI
{
    public class MainMenuButton : MonoBehaviour
    {
        public MM_Button type;
        public void PressButton()
        {
            MenuManager.GetInstance().ButtonPressed(type);
        }
    }

    public enum MM_Button
    {
        solo,editor,multi,load,exit
    }
}
