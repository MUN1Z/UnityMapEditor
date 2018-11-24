using UnityEngine;
using System.Collections;
using Managers;

namespace UI
{
    public class LE_UI_Category_button : MonoBehaviour
    {
        public CategoryType type;
       
        public void PressButton()
        {
            LE_UI.singleton.ChangeCategory(type);
        }
    }
}
