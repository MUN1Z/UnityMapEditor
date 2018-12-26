using UnityEngine;
using System.Collections;
using Map;
using UI;
using UnityEngine.EventSystems;
using System.Linq;
using Assets.Scripts.Player;
using System.Collections.Generic;

namespace Managers
{
    public class LevelEditor : MonoBehaviour
    {
        SessionMaster sm;
        ResourcesManager rm;
        GridBase grid;
        LE_UI ui;
        WallCreator wallCreator;
        LevelManager lvlManager;
        public bool canPaint;
        Vector3 mousePosition;
        public string activeId;
        public PaintType curType;
        public bool mouseOverUIElement;
        GameObject ghostObject;
        LevelObjectsAsset prevObject;
        float targetY;

        private static LevelEditor _instance;

        public static LevelEditor GetInstance() => _instance;

        public void Init()
        {
            sm = SessionMaster.GetInstance();
            grid = GridBase.GetInstance();
            rm = ResourcesManager.singleton;
            lvlManager = LevelManager.singleton;
            wallCreator = WallCreator.singleton;
            wallCreator.Init();

            if (sm.isEditor)
            {
                ui = LE_UI.singleton;
                ui.Init();
            }

            activeId = "grass";
            PaintAllInFloor(0);
        }

        private void Start()
        {
            _instance = this;
        }

        void Update()
        {
            mouseOverUIElement = EventSystem.current.IsPointerOverGameObject();
            if (!canPaint)
                return;

            Node curNode = FindMousePosition();

            if(curNode != null)
            {
                 Paint(curNode);
            }
        }

        public void PaintAllInFloor(int y)
        {
            for (int x = 0; x < grid.maxX; x++)
            {
                for (int z = 0; z < grid.maxZ; z++)
                {
                    Node n = grid.GetNode(x, y, z);
                    PaintTexture(n, activeId);
                }
            }
        }

        public void PaintNodeFromPosition(int x, int y, int z)
        {
            Node n = grid.GetNode(x, y, z);
            PaintTexture(n, activeId);
        }

        public void UpdateType(string id, PaintType targetType)
        {
            activeId = id;
            curType = targetType;
        }

        void Paint(Node n)
        {
            bool isMouseButtonDown = Input.GetMouseButton(0) && !mouseOverUIElement;
            wallCreator.IndicatorsStatus(false);

            if(curType != PaintType.levelObject)
            {
                if (ghostObject != null)
                    Destroy(ghostObject);
            }

            switch (curType)
            {
                case PaintType.groundTexture:
                    wallCreator.wallIndicators[0].SetActive(true);
                    wallCreator.wallIndicators[0].transform.position = n.nr.transform.position;
                    if(isMouseButtonDown)
                        PaintTexture(n, activeId);
                    break;
                case PaintType.levelObject:
                        LevelObjectPlace(n, activeId);
                    break;
                case PaintType.delete_object:
                    wallCreator.wallIndicators[0].SetActive(true);
                    wallCreator.wallIndicators[0].transform.position = n.nr.transform.position;
                    if (isMouseButtonDown)
                    {
                        DeleteObjects(n);
                    }
                    break;
                case PaintType.wallobject:
                    wallCreator.IndicatorsStatus(true);
                    wallCreator.WallCreation(n, mousePosition);
                    break;
                default:
                    break;
            }
        }

        public void PaintTexture(Node n, string textureID)
        {
            if(n != null)
            {
                if (rm == null)
                    rm = ResourcesManager.singleton;

                if (textureID == "air")
                    n.nodeType = NodeType.air;
                else
                    n.nodeType = NodeType.ground;

                TextureAsset txt = rm.GetTexture(textureID);
                if (txt == null)
                {
                    Debug.Log("No texture with id " + textureID + " found");
                    return;
                }

                n.textureId = txt.id;
                n.nr.UpdateGroundTexture(txt.material);
            }
        }

        void LevelObjectPlace(Node n, string id)
        {
            LevelObjectsAsset lvlAsset = rm.GetLvlObject(id);

            if (lvlAsset == null)
            {
                Debug.Log("No object with id " + lvlAsset + " found");
                return;
            }

            if(lvlAsset != prevObject)
            {
                if (ghostObject != null)
                    Destroy(ghostObject);

                ghostObject = Instantiate(lvlAsset.gameModel) as GameObject;
            }

            if(ghostObject == null)
                ghostObject = Instantiate(lvlAsset.gameModel) as GameObject;

            prevObject = lvlAsset;
            //ghostObject.transform.position = n.nr.transform.position;

            ghostObject.transform.position = new Vector3(n.nr.transform.position.x + 1.255f, n.nr.transform.position.y, n.nr.transform.position.z + 1.255f);

            Quaternion targetRotation = Quaternion.Euler(0, targetY, 0);
            ghostObject.transform.rotation = targetRotation;

            if(Input.GetMouseButtonDown(0) && !mouseOverUIElement)
            {
                PaintLvlObj(n, lvlAsset, targetY);
            }

            if(Input.GetMouseButtonDown(1) && !mouseOverUIElement)
            {
                targetY += 45;
                if(targetY > 360)
                {
                    targetY = 0;
                }
            }

        }

        public void PaintLvlObj(Node n, LevelObjectsAsset obj, float y, ref List<MappedGameObject> list)
        {
            if(n.nr.IsObjectDuplicate(obj.id))
            {
                //Object is placed on the same node more than once,
                return;
            }
            
            GameObject go = Instantiate(obj.gameModel) as GameObject;

            go.transform.parent = lvlManager.level_floors[n.y].objHolder.transform;
            //go.transform.localPosition = n.nr.transform.position;
            go.transform.localPosition = new Vector3(n.nr.transform.position.x + 1.255f, n.nr.transform.position.y, n.nr.transform.position.z + 1.255f);

            Quaternion targetRotation = Quaternion.Euler(0, y, 0);
            go.transform.rotation = targetRotation;
            Map.LevelObjectsActual lvlobj = new LevelObjectsActual();
            lvlobj.id = obj.id;
            lvlobj.objReference = go;
            lvlobj.targetY = targetY;
            n.nr.objectsOnNode.Add(lvlobj);

            if(list != null)
                list.Add(new MappedGameObject { X = n.x, Y = n.y, Z = n.z, GameObject = go });
        }

        public void PaintLvlObj(Node n, LevelObjectsAsset obj, float y)
        {
            if (n.nr.IsObjectDuplicate(obj.id))
            {
                //Object is placed on the same node more than once,
                return;
            }

            GameObject go = Instantiate(obj.gameModel) as GameObject;

            go.transform.parent = lvlManager.level_floors[n.y].objHolder.transform;
            //go.transform.localPosition = n.nr.transform.position;
            go.transform.localPosition = new Vector3(n.nr.transform.position.x + 1.255f, n.nr.transform.position.y, n.nr.transform.position.z + 1.255f);

            Quaternion targetRotation = Quaternion.Euler(0, y, 0);
            go.transform.rotation = targetRotation;
            Map.LevelObjectsActual lvlobj = new LevelObjectsActual();
            lvlobj.id = obj.id;
            lvlobj.objReference = go;
            lvlobj.targetY = targetY;
            n.nr.objectsOnNode.Add(lvlobj);
        }

        public void DeleteObjects(Node n)
        {
            foreach (LevelObjectsActual obj in n.nr.objectsOnNode)
            {
                if (obj.objReference != null)
                    Destroy(obj.objReference);
            }
            
            n.nr.objectsOnNode.Clear();
        }

        Node FindMousePosition()
        {
            Node n = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray,out hit, Mathf.Infinity))
            {
                mousePosition = hit.point;
            }

            Node curNode = grid.NodeFromWorldPosition(mousePosition);
            n = curNode;
            return n;
        }

        static public LevelEditor singleton;
        void Awake()
        {
            singleton = this;
        }
       
    }

    public enum PaintType
    {
        groundTexture, levelObject, wallobject, delete_object, delete_wall
    }
}
