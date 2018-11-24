using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Map;

namespace UI
{
    public class LE_MenuButton : MonoBehaviour
    {
        public LE_MenuButtonType type;
        public InputField inputField;

        public void PressButton()
        {
            switch (type)
            {
                case LE_MenuButtonType.newMap:
                    GridBase.GetInstance().ClearMap();
                    GridBase.GetInstance().InitLevel();
                    
                    break;
                case LE_MenuButtonType.saveMap:
                    if (inputField == null)
                        inputField = transform.parent.GetComponentInChildren<InputField>();
                    LevelSerializer.singleton.SaveLevel(inputField.text);
                    break;
                default:
                    break;
            }
        }
       
    }

    public enum LE_MenuButtonType
    {
        newMap,saveMap
    }
}
