using UnityEngine;
using System.Collections;

namespace UI
{
    public class FloorButtons : MonoBehaviour
    {
        public bool up;

        public void PressButton()
        {
            Managers.LevelManager.singleton.ChangeFloor(up);
        }
       
    }
    
}