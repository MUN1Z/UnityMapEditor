using UnityEngine;
using System.Collections;
using Managers;

namespace Map.Walls
{
    public class WallCreatorUI_Button : MonoBehaviour
    {
        public WallType type;
        public void PressButton()
        {
            WallCreator.singleton.curType = type;
        }

    }
}
