using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Managers;

namespace UI
{
    public class LE_UI : MonoBehaviour
    {
        ResourcesManager rm;
        public GameObject subCategoryGrid;
        public GameObject wallCreatorSubCat;
        List<GameObject> prevButtons = new List<GameObject>();

        GameObject le_Ui_button;

        public void Init()
        {
            le_Ui_button = Resources.Load("le_ui_button") as GameObject;
            CreateButtons(PaintType.groundTexture);
            wallCreatorSubCat.SetActive(false);
        }

        public void CreateButtons(PaintType t)
        {
            rm = ResourcesManager.singleton;

            foreach (GameObject g in prevButtons)
            {
                Destroy(g);
            }
            prevButtons.Clear();

            subCategoryGrid.SetActive(true);

            switch (t)
            {
                case PaintType.groundTexture:
                    for (int i = 0; i < rm.groundTextures.Count; i++)
                    {
                        TextureAsset txt = rm.groundTextures[i];
                        GameObject go = Instantiate(le_Ui_button) as GameObject;
                        go.transform.localScale = new Vector3(0, 0, 0);
                        go.transform.SetParent(subCategoryGrid.transform);
                        LE_UI_button txtBt = go.GetComponent<LE_UI_button>();
                        txtBt.InitButton(txt.id, t, txt.icon);
                        txtBt.type = PaintType.groundTexture;
                        prevButtons.Add(go);
                    }
                    break;
                case PaintType.levelObject:
                    for (int i = 0; i < rm.levelObjects.Count; i++)
                    {
                        LevelObjectsAsset lvlobj = rm.levelObjects[i];
                        GameObject go = Instantiate(le_Ui_button) as GameObject;
                        go.transform.SetParent(subCategoryGrid.transform);
                        go.transform.localScale = new Vector3(0, 0, 0);
                        LE_UI_button objBt = go.GetComponent<LE_UI_button>();
                        objBt.InitButton(lvlobj.id, t, lvlobj.icon);
                        objBt.type = PaintType.levelObject;
                        prevButtons.Add(go);
                    }
                    break;
                default:
                    break;
            }

        }

        public void ChangeCategory(CategoryType t)
        {
            wallCreatorSubCat.SetActive(false);
            switch (t)
            {
                case CategoryType.ground:
                    CreateButtons(PaintType.groundTexture);
                    break;
                case CategoryType.levelobject:
                    CreateButtons(PaintType.levelObject);
                    break;
                case CategoryType.wallCreator:
                    subCategoryGrid.SetActive(false);
                    wallCreatorSubCat.SetActive(true);
                    LevelEditor.singleton.curType = PaintType.wallobject;
                    break;
                case CategoryType.delete:
                    subCategoryGrid.SetActive(false);
                    LevelEditor.singleton.curType = PaintType.delete_object;
                    break;
                default:
                    break;
            } 
        }
        

        static public LE_UI singleton;
        void Awake()
        {
            singleton = this;
        }
    }

    public enum CategoryType
    {
        ground,levelobject,wallCreator,delete
    }
}
