using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Managers;

namespace UI
{
    public class LE_UI_button : MonoBehaviour
    {
        public string id;
        public PaintType type;
        public Image img;

        public void InitButton(string objId, PaintType t, Sprite icon)
        {
            if (img == null)
                img = GetComponentInChildren<Image>();

            id = objId;
            t = type;
            img.sprite = icon;
        }

        public void PressButton()
        {
            Debug.Log(id);
            LevelEditor.singleton.UpdateType(id, type);
        }
        
    }
}
