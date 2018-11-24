using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Map;

namespace Managers
{
    public class SessionMaster : MonoBehaviour
    {
        //public variables
        public bool isGame;
        public bool isEditor;
        public bool isMultiplayer;
        public string currentLevelName = "level";

        //internal variables
        SessionType currentSession;

        //References
        MenuManager menuManager;

        public List<string> availableLevels = new List<string>();

        void Start()
        {
            menuManager = MenuManager.GetInstance();
            LoadMain();
        }

        public void LoadMain()
        {
            StartCoroutine("InitGame");
        }

        IEnumerator InitGame()
        {
            menuManager.LoadingStatus(true);//this opens the loading screen (not scene!)
            yield return LoadEmpty();//the loading of a new scene happens here

            /*This is the initial loading of the game, add any behaviors you want in here
            for example:
            - loading an asset bundle
            - create icons for loaded assets
            - loading available levels, not the actual level load, just what level files we have on our disk
            */

            LevelSerializer.singleton.LoadAllFileLevels();

            menuManager.LoadingStatus(false);//the loading stops here
            InitSession(SessionType.menu);
            //after the initialization phase of the game, you are ready to start the session
        }

        public void InitSession(SessionType st)
        {
            menuManager.CloseAll();

            switch (st)
            {
                case SessionType.menu:
                    menuManager.mainMenu.SetActive(true);
                    break;
                case SessionType.solo:
                    menuManager.backButton.SetActive(true);
                    StartSolo();
                    break;
                case SessionType.leveleditor:
                    menuManager.backButton.SetActive(true);
                    StartLevelEditor();
                    break;
                case SessionType.quit:
                    Application.Quit();
                    break;
                default:
                    break;
            }
        }

        void StartSolo()
        {
            menuManager.InitLevelSelectGrid();
            menuManager.levelSelectGrid.SetActive(true);
        }

        public void StartSoloActual(string targetLevel)
        {
            menuManager.CloseAll();
            menuManager.backButton.SetActive(true);
            currentLevelName = targetLevel;
            isEditor = false;
            isMultiplayer = false;
            RequestSceneLoad(SceneType.game);
        }

        void StartLevelEditor()
        {          
            isEditor = true;
            isMultiplayer = false;           
            RequestSceneLoad(SceneType.editor);
        }

        #region Load Scene
        enum SceneType
        {
            empty,game,editor
        }

        void RequestSceneLoad(SceneType t)
        {
            StartCoroutine(SceneLoad(t));
        }

        IEnumerator SceneLoad(SceneType t)
        {
            switch (t)
            {
                case SceneType.empty:
                    StartCoroutine(LoadEmpty());
                    break;
                case SceneType.game:
                    StartCoroutine(LoadGame());
                    break;
                case SceneType.editor:
                    StartCoroutine(LoadEditor());
                    break;
            }
            yield return null;
        }

        IEnumerator LoadEmpty()
        {
            yield return SceneManager.LoadSceneAsync("empty", LoadSceneMode.Single);
        }

        IEnumerator LoadGame()
        {
            yield return SceneManager.LoadSceneAsync("game", LoadSceneMode.Single);
        }

        IEnumerator LoadEditor()
        {
            yield return SceneManager.LoadSceneAsync("editor", LoadSceneMode.Single);
        }


        #endregion

        public enum SessionType
        {
            menu,
            solo,
            leveleditor,
            quit
        }

        #region Singleton
        public static SessionMaster instance;
        public static SessionMaster GetInstance()
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

