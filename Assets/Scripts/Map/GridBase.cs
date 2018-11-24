using UnityEngine;
using System.Collections.Generic;
using Managers;

namespace Map
{
    public class GridBase : MonoBehaviour
    {
        public int maxX = 64;
        public int maxY = 1;
        public int maxZ= 64;
        public float offsetXZ = 2;
        public float offsetY = 2;

        Node[ , , ] grid;

        GameObject visParent;
        GameObject tilePrefab;

        SessionMaster sm;
        LevelManager lvl_m;
        LevelEditor lvlEditor;

        void Start()
        {
            InitLevel();
        }

        public void InitLevel()
        {
            sm = SessionMaster.GetInstance();
            lvl_m = LevelManager.singleton;
            lvlEditor = LevelEditor.singleton;
           

            if (sm.isEditor)
            {
                CreateFloorsAndCollision();
                CreateGrid();
                lvlEditor.Init();

            }
            else
            {
                if(sm.isMultiplayer)
                {

                }
                else
                {
                    LoadMap();
                }
            }

            WrapUp();
        }

        void WrapUp()
        {
            if(sm.isEditor)
            {
                lvlEditor.canPaint = true;
            }
        }

        #region Level Functions
        public void CreateGrid()//Grid Creation
        {
            grid = new Node[maxX, maxY, maxZ];

            //takes care of the offset values
            if (offsetXZ == 0)
                offsetXZ = 1;
            if (offsetY == 0)
                offsetY = 1;

            //Create an object to parent everything underneath
            visParent = new GameObject();
            visParent.name = "vis parent";
            tilePrefab = Resources.Load("tilePrefab") as GameObject;
            if (tilePrefab == null)
            {
                Debug.Log("No tile prefab found!");
                return;
            }

            //Actual creation
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        Node n = CreateNode(x, y, z);
                        grid[x, y, z] = n;
                    }
                }
            }
        }

        Node CreateNode(int x,int y, int z)
        {
            Node n = new Node();
            n.Init();

            //find the target position for the new node
            Vector3 targetPosition = Vector3.zero;
            targetPosition.x = x * offsetXZ;
            targetPosition.z = z * offsetXZ;
            targetPosition.y = y * offsetY;

            //instantiate the prefab
            GameObject go = Instantiate(tilePrefab, targetPosition, Quaternion.identity) as GameObject;
            //go.AddComponent<OcclusionObject>();
            go.transform.parent = lvl_m.level_floors[y].nodeHolder.transform;
            go.transform.name = x.ToString() + " " + y.ToString() + " " + z.ToString();
            //go.AddComponent<BoxCollider>();
            go.AddComponent<NodeReferences>();//add reference
            
            //update the new node values with the references
            n.x = x;
            n.y = y;
            n.z = z;
            n.nr = go.GetComponent<NodeReferences>();
            n.nr.Init();
            
            //Replace it in the grid
            return n;
        }

        public void LoadMap(string mapName = "")
        {
            SaveFile sf = LevelSerializer.singleton.ReturnSaveFile(sm.currentLevelName);

            if(!string.IsNullOrEmpty(mapName))
                sf = LevelSerializer.singleton.ReturnSaveFile(mapName);

            maxX = sf.mapX;
            maxY = sf.mapY;
            maxZ = sf.mapZ;

            CreateFloorsAndCollision();
            CreateGrid();
            lvlEditor.Init();

            for (int i = 0; i < sf.saveableNodes.Count; i++)
            {
                SaveableNode sn = sf.saveableNodes[i];
                Node n = grid[sn.x, sn.y, sn.z];
                n.nodeType = (NodeType)sn.nodeType;
                LevelEditor.singleton.PaintTexture(n,sn.textureId);

                if (!sm.isEditor)
                    if (n.nodeType == NodeType.air)
                        n.nr.gameObject.SetActive(false);

                for (int d = 0; d < n.directions.Length; d++)
                {
                    n.directions[d].id = (Direction) sn.directionInfo[d].directionId;

                    if (!sn.directionInfo[d].hasWall)
                        continue;

                    WallType t = (WallType) sn.directionInfo[d].wallType;

                    switch (t)
                    {
                        case WallType.wall:
                            WallCreator.singleton.CreateWall(n, n.directions[d].id);
                            break;
                        case WallType.door:
                        case WallType.window:
                            WallCreator.singleton.CreateWall(n, n.directions[d].id);
                            WallCreator.singleton.CreateExtra(n, t, n.directions[d].id);
                            break;
                    }  
                }

                for (int o = 0; o < sn.savedObjects.Count; o++)
                {
                    LevelObjectsAsset objAsset = ResourcesManager.singleton.GetLvlObject(sn.savedObjects[o].objid);
                    if (objAsset == null)
                        continue;
                    lvlEditor.PaintLvlObj(n, objAsset, sn.savedObjects[o].targetY);
                }
            }
        }

        void CreateFloorsAndCollision()
        {
            for (int i = 0; i < maxY; i++)
            {
                lvl_m.AddFloor();
                CreateMouseCollision(i);
            }
        }

        void CreateMouseCollision(int y)
        {
            float posY = y * offsetY;
            GameObject go = new GameObject();
            go.AddComponent<BoxCollider>();
            go.GetComponent<BoxCollider>().size = new Vector3(maxX * offsetXZ + (offsetXZ * 2),
                0.1f, maxZ * offsetXZ + (offsetXZ * 2));

            go.transform.position = new Vector3((maxX * offsetXZ) / 2 - (offsetXZ/2), posY,
                (maxZ * offsetXZ) / 2- (offsetXZ / 2));

            lvl_m.level_floors[y].floorCollision = go;
            go.name = "mouse collision for floor " + y.ToString();
            go.transform.parent = this.transform;
            go.SetActive(false);
            
            //if (y > 0)
            //    go.AddComponent<BoxCollider>();

            go.tag = "Hide";
        }

        public void AddFloor()
        {
            Node[,,] oldGrid = grid;
            maxY++;
            lvl_m.AddFloor();
            CreateMouseCollision(maxY - 1);

            grid = new Node[maxX, maxY, maxZ];

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        if (y < maxY-1)
                        {
                            grid[x, y, z] = oldGrid[x, y, z];
                        }
                        else
                        {
                            Node n = CreateNode(x, maxY-1, z);
                            n.nodeType = NodeType.air;
                            grid[x, y , z] = n;
                        }
                    }
                }
            }

            string curId = LevelEditor.singleton.activeId;

            LevelEditor.singleton.activeId = "air";
            LevelEditor.singleton.PaintAllInFloor(maxY - 1);
            LevelEditor.singleton.activeId = curId;
        }

        public void ClearMap()
        {
            maxY = 1;
            Destroy(visParent);
            lvl_m.ClearAll();
            grid = null;
        }
        #endregion

        public Node GetNode(int x, int y, int z, bool nullable = false)
        {
            Node n = null;

            int tX = x;
            int tY = y;
            int tZ = z;

            if(!nullable)
            {
                if (tX > maxX - 1)
                    tX = maxX - 1;
                if (tX < 0)
                    tX = 0;
                if (tY > maxY - 1)
                    tY = maxY - 1;
                if (tY < 0)
                    tY = 0;
                if (tZ > maxZ - 1)
                    tZ = maxZ - 1;
                if (tZ < 0)
                    tZ = 0;
            }
            else
            {
                if (tX > maxX - 1 || tX < 0 || tY > maxY - 1
                    || tY < 0 || tZ > maxZ - 1 || tZ < 0 )
                    return null;
            }

            n = grid[tX, tY, tZ];

            return n;
        }

        public Node NodeFromWorldPosition(Vector3 worldPosition, bool nullable = false)
        {
            float worldX = worldPosition.x;
            float worldY = worldPosition.y;
            float worldZ = worldPosition.z;

            worldX /= offsetXZ;
            worldY /= offsetY;
            worldZ /= offsetXZ;

            int x = Mathf.FloorToInt(worldX);
            int y = Mathf.FloorToInt(worldY);
            int z = Mathf.FloorToInt(worldZ);

            return GetNode(x, y, z, nullable);
        }

        public Node NodeFromWorldPositionUp(Vector3 worldPosition, int plusX = 0, int plusY = 0, int plusZ = 0)
        {
            float worldX = worldPosition.x;
            float worldY = worldPosition.y;
            float worldZ = worldPosition.z;

            worldX /= offsetXZ;
            worldY /= offsetY;
            worldZ /= offsetXZ;

            int x = Mathf.FloorToInt(worldX);
            int y = Mathf.FloorToInt(worldY);
            int z = Mathf.FloorToInt(worldZ);

            return GetNode((x + plusX), (y + plusY), (z + plusZ), false);
        }

        public List<Node> GetAllNodesOnFloor(int y)
        {
            List<Node> r = new List<Node>();

            for (int x = 0; x < maxX; x++)
            {
                for (int z = 0; z < maxZ; z++)
                {
                    r.Add(grid[x, y, z]);
                }
            }

            return r;
        }

        #region Singleton
        public static GridBase instance;
        public static GridBase GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
        }
        #endregion
    }
}
