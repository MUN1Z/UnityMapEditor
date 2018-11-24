using UnityEngine;
using System.Collections;
using Managers;

namespace UI
{
    public class BackButton : MonoBehaviour
    {
        public void PressButton()
        {
            SessionMaster.GetInstance().LoadMain();
        }
    }
}
