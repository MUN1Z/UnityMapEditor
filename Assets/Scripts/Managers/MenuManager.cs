using UnityEngine;
using System.Collections;
using UI;

namespace Managers
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject loading;
        public GameObject mainMenu;
        public GameObject levelSelectGrid;
        public GameObject backButton;

        public void ButtonPressed(MM_Button t)
        {
            switch (t)
            {
                case MM_Button.solo:
                    SessionMaster.GetInstance().InitSession(SessionMaster.SessionType.solo);
                    break;
                case MM_Button.editor:
                    SessionMaster.GetInstance().InitSession(SessionMaster.SessionType.leveleditor);
                    break;
                case MM_Button.multi:
                    break;
                case MM_Button.load:
                    break;
                case MM_Button.exit:
                    break;
                default:
                    break;
            }
        }

        public void CloseAll()
        {
            backButton.SetActive(false);
            mainMenu.SetActive(false);
            loading.SetActive(false);
            levelSelectGrid.SetActive(false);
        }

        public void InitLevelSelectGrid()
        {
            SessionMaster sm = SessionMaster.GetInstance();
            GameObject prefab = Resources.Load("level_select_ui") as GameObject;
            for (int i = 0; i < sm.availableLevels.Count; i++)
            {
                GameObject go = Instantiate(prefab) as GameObject;
                go.transform.SetParent(levelSelectGrid.transform);
                LevelSelectButton b = go.GetComponent<LevelSelectButton>();
                b.InitButton(sm.availableLevels[i]);
            }
        }

        public void LoadingStatus(bool status)
        {
            if (status == true)
            {
                CloseAll();
            }
            loading.SetActive(status);      
        }

        void Start()
        {
            CloseAll();
        }

        #region Singleton
        public static MenuManager instance;
        public static MenuManager GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}
