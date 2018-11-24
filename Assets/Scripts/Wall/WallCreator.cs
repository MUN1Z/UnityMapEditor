using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Map;
using Map.Walls;

namespace Managers
{
    public class WallCreator : MonoBehaviour
    {
        GridBase gb;
        LevelEditor lvlEditor;

        public WallType curType;
        bool draging;
        GameObject wallPrefab;
        GameObject doorPrefab;
        GameObject windowPrefab;
        public GameObject wallIndicatorPrefab;
        [HideInInspector]
        public GameObject[] wallIndicators = new GameObject[4];
        Direction startDir;
        Node startNode;

        public void Init()
        {
            gb = GridBase.GetInstance();
            lvlEditor = LevelEditor.singleton;
            wallIndicatorPrefab = Resources.Load("wallIndicator") as GameObject;
            wallPrefab = Resources.Load("wallPrefab") as GameObject;
            doorPrefab = Resources.Load("doorPrefab") as GameObject;
            windowPrefab = Resources.Load("windowPrefab") as GameObject;

            for (int i = 0; i < wallIndicators.Length; i++)
            {
                if (wallIndicators[i] == null)
                {
                    GameObject go = Instantiate(wallIndicatorPrefab) as GameObject;
                    wallIndicators[i] = go;
                }
            }

            IndicatorsStatus(false);
        }

        public void IndicatorsStatus(bool status)
        {
            foreach (GameObject go in wallIndicators)
            {
                go.SetActive(status);
            }
        }

        public void WallCreation(Node n, Vector3 mousePosition)
        {
            switch (curType)
            {
                case WallType.wall:
                    NormalWall(n, mousePosition,false);
                    break;
                case WallType.door:
                case WallType.window:
                    PlaceExtra(n, mousePosition);
                    break;
                case WallType.delete:
                    NormalWall(n, mousePosition, true);
                    break;
            }
        }

        void NormalWall(Node n, Vector3 mousePosition, bool delete)
        {
            Vector3 curPosition = n.nr.transform.position + ReturnVectorPosition(startDir);

            if (!draging)
            {
                startDir = ClosestDirection(n, mousePosition);
                IndicatorsStatus(false);
                wallIndicators[0].SetActive(true);
                wallIndicators[0].transform.position = curPosition;
                startNode = n;
            }
            else
            {
                wallIndicators[1].transform.position = curPosition;

                int diffX = n.x - startNode.x;
                int diffZ = n.z - startNode.z;

                Node adjNodeX = gb.GetNode(n.x - diffX, n.y, n.z);
                wallIndicators[2].
                    transform.position =
                    adjNodeX.nr.transform.position + ReturnVectorPosition(startDir);

                Node adjNodeZ = gb.GetNode(n.x, n.y, n.z - diffZ);
                wallIndicators[3].
                   transform.position =
                   adjNodeZ.nr.transform.position + ReturnVectorPosition(startDir);

                if (Input.GetMouseButtonUp(0) && !lvlEditor.mouseOverUIElement)
                {
                    if(!delete)
                        FindNodesToCreate(startNode, n, startDir);
                    else
                        FindNodesToDelete(startNode, n, startDir);
                }
            }

            if (Input.GetMouseButtonDown(0) && !lvlEditor.mouseOverUIElement)
                draging = true;
            if (Input.GetMouseButtonUp(0) && !lvlEditor.mouseOverUIElement)
                draging = false;
        }

        void PlaceExtra(Node n, Vector3 mousePosition)
        {
            startDir = ClosestDirection(n, mousePosition);
            Vector3 curPosition = n.nr.transform.position + ReturnVectorPosition(startDir);
            IndicatorsStatus(false);

            if (n.hasWallOnDir(startDir))
            {
                wallIndicators[0].SetActive(true);
                wallIndicators[0].transform.position = curPosition;
            }

            if(Input.GetMouseButtonDown(0) && !lvlEditor.mouseOverUIElement)
            {
                CreateExtra(n, curType, startDir);
            }
        }

        public void DeleteNode(Node n, Direction dir, bool deleteNeighbor)
        {
            DirectionInfo info = n.GetDirectionInfo(dir);
            if (info.wall == null)
                return;

            Destroy(info.wall.objReference);
            info.hasWall = false;
            info.wall = null;

            if (deleteNeighbor)
            {
                Direction oppositeDir = dir;
                Node neighbor = GetNodeFromDir(n, dir, ref oppositeDir);
                DeleteNode(neighbor, oppositeDir, false);
            }
        }

        Direction ClosestDirection(Node n, Vector3 mp)
        {
            int r = 0;
            Vector3[] positions = new Vector3[4];
            positions[0] = n.nr.transform.position + ReturnVectorPosition(Direction.n);
            positions[1] = n.nr.transform.position + ReturnVectorPosition(Direction.s);
            positions[2] = n.nr.transform.position + ReturnVectorPosition(Direction.w);
            positions[3] = n.nr.transform.position + ReturnVectorPosition(Direction.e);

            float minDist = 50;

            for (int i = 0; i < positions.Length; i++)
            {
                float tempDist = Vector3.Distance(positions[i], mp);
                if(tempDist<minDist)
                {
                    minDist = tempDist;
                    r = i;
                }
            }

            return (Direction) r;
        }

        Vector3 ReturnVectorPosition(Direction dir)
        {
            float offset = gb.offsetXZ / 2;

            switch (dir)
            {
                case Direction.n:
                    return Vector3.forward * offset;
                case Direction.s:
                    return -Vector3.forward * offset;
                case Direction.w:
                    return -Vector3.right * offset;
                case Direction.e:
                    return Vector3.right * offset;
                default:
                    return Vector3.zero;
            }
        }

        void FindNodesToCreate(Node from, Node to, Direction fromDir)
        {
            int diffX = to.x - from.x;
            int diffZ = to.z - from.z;

            // if (diffX == 0 && diffZ == 0)
            //   return;

            if(diffX != 0)
            { 
                bool posX = (diffX < 0);
                for (int i = 0; i < Mathf.Abs(diffX) + 1; i++)
                {
                    int offset = i;
                    offset = (posX) ? offset : -offset;
                    Node n = gb.GetNode(from.x - offset, from.y, from.z);
                    //the direction changes based on the Z
                    CreateWall(n, (diffZ > 0) ? Direction.s : Direction.n);
                }
            }
            else
            {
                if (fromDir == Direction.n || fromDir == Direction.s)
                {
                    CreateWall(from, fromDir);
                }

            }


            if (diffZ != 0)
            {
                bool posZ = (diffZ < 0);
                for (int i = 0; i < Mathf.Abs(diffZ) + 1; i++)
                {
                    int offset = i;
                    offset = (posZ) ? offset : -offset;
                    Node n = gb.GetNode(from.x, from.y, from.z - offset);
                    //the direction changes based on the X
                    CreateWall(n, (diffX > 0) ? Direction.w : Direction.e);
                }
            }
            else
            {
                if (fromDir == Direction.e || fromDir == Direction.w)
                {
                    CreateWall(from, fromDir);
                }
            } 
            

            if (diffX != 0 && diffZ != 0)
            { //Diagonal
                Node adjNode = gb.GetNode(from.x + diffX, from.y, from.z + diffZ);

                bool positiveX = (diffX < 0);
                for (int i = 0; i < Mathf.Abs(diffX)+1; i++)
                {
                    int offset = i;
                    offset = (!positiveX) ? offset : -offset;
                    Node n = gb.GetNode(adjNode.x - offset, adjNode.y, adjNode.z);
                    //the direction changes based on the Z
                    CreateWall(n, (diffZ < 0) ? Direction.s : Direction.n);
                }

                bool positiveZ = (diffZ < 0);
                for (int i = 0; i < Mathf.Abs(diffZ)+1; i++)
                {
                    int offset = i;
                    offset = (!positiveZ) ? offset : -offset;
                    Node n = gb.GetNode(adjNode.x, adjNode.y, adjNode.z - offset);
                    //the direction changes based on the X
                    CreateWall(n, (diffX < 0) ? Direction.w : Direction.e);
                }
            }
        }

        void FindNodesToDelete(Node from, Node to, Direction fromDir)
        {
            int diffX = to.x - from.x;
            int diffZ = to.z - from.z;

            if (diffX == 0 && diffZ == 0)
                return;

            if (diffX != 0)
            {
                bool positive = (diffX < 0);
                for (int i = 0; i < Mathf.Abs(diffX) + 1; i++)
                {
                    int offset = i;
                    offset = (positive) ? offset : -offset;
                    Node n = gb.GetNode(from.x - offset, from.y, from.z);
                    //the direction changes based on the Z
                    DeleteNode(n, (diffZ > 0) ? Direction.s : Direction.n,true);
                }
            }
            else
            {
                if (fromDir == Direction.n || fromDir == Direction.s)
                {
                    DeleteNode(from, fromDir, true);
                }
            }

            if (diffZ != 0)
            {
                bool positive = (diffZ < 0);
                for (int i = 0; i < Mathf.Abs(diffZ) + 1; i++)
                {
                    int offset = i;
                    offset = (positive) ? offset : -offset;
                    Node n = gb.GetNode(from.x, from.y, from.z - offset);
                    //the direction changes based on the X
                    DeleteNode(n, (diffX > 0) ? Direction.w : Direction.e, true);
                }
            }
            else
            {
                if (fromDir == Direction.n || fromDir == Direction.s)
                {
                    DeleteNode(from, fromDir, true);
                }
            }

            if (diffX != 0 && diffZ != 0)
            { //Diagonal
                Node adjNode = gb.GetNode(from.x + diffX, from.y, from.z + diffZ);

                bool positiveX = (diffX < 0);
                for (int i = 0; i < Mathf.Abs(diffX) + 1; i++)
                {
                    int offset = i;
                    offset = (!positiveX) ? offset : -offset;
                    Node n = gb.GetNode(adjNode.x - offset, adjNode.y, adjNode.z);
                    //the direction changes based on the Z
                    DeleteNode(n, (diffZ < 0) ? Direction.s : Direction.n, true);
                }

                bool positiveZ = (diffZ < 0);
                for (int i = 0; i < Mathf.Abs(diffZ) + 1; i++)
                {
                    int offset = i;
                    offset = (!positiveZ) ? offset : -offset;
                    Node n = gb.GetNode(adjNode.x, adjNode.y, adjNode.z - offset);
                    //the direction changes based on the X
                    DeleteNode(n, (diffX < 0) ? Direction.w : Direction.e, true);
                }
            }
        }

        public void CreateWall(Node n, Direction dir)
        {
            if (n.isWallDuplicate(dir))
            {
                return;
            }

            Direction oppositeDir = dir;
            Node neighbor = GetNodeFromDir(n, dir,ref oppositeDir);
           
            if (neighbor.isWallDuplicate(oppositeDir))
                return;

            GameObject go = Instantiate(wallPrefab) as GameObject;
            go.transform.position = n.nr.transform.position + ReturnVectorPosition(dir);
            WallDirection wallDir = GetWallDir(dir);
            RotateByDir(go, wallDir);
            n.AddWall(dir, go, true);
            go.transform.parent = LevelManager.singleton.level_floors[n.y].wallHolder.transform;
        }

        public void CreateExtra(Node n, WallType t , Direction dir)
        {
            GameObject prefab = null;

            if(doorPrefab == null)
                doorPrefab = Resources.Load("doorPrefab") as GameObject;
            if(windowPrefab == null)
                windowPrefab = Resources.Load("windowPrefab") as GameObject;

            switch (t)
            {
                case WallType.door:
                    prefab = doorPrefab;
                    break;
                case WallType.window:
                    prefab = windowPrefab;
                    break;
            }

            DirectionInfo info = n.GetDirectionInfo(dir);
            if (info.wall == null)
            {
                return;
            }

            GameObject go = Instantiate(prefab) as GameObject;
            go.transform.parent = LevelManager.singleton.level_floors[n.y].wallHolder.transform;
            Vector3 targetPositon = n.nr.transform.position + ReturnVectorPosition(dir);
            go.transform.position = targetPositon;
            RotateByDir(go, GetWallDir(dir));

            if (info.wall.objReference != null)
                Destroy(info.wall.objReference);

            info.wall.objReference = go;
            info.wall.wallType = t;
        }

        Node GetNodeFromDir(Node curN,Direction dir, ref Direction oppositeDir)
        {
            Node n = null;

            switch (dir)
            {
                case Direction.n:
                    n = GridBase.GetInstance().GetNode(curN.x, curN.y, curN.z + 1);
                    oppositeDir = Direction.s;
                    break;
                case Direction.s:
                    n = GridBase.GetInstance().GetNode(curN.x, curN.y, curN.z - 1);
                    oppositeDir = Direction.n;
                    break;
                case Direction.w:
                    n = GridBase.GetInstance().GetNode(curN.x - 1, curN.y, curN.z);
                    oppositeDir = Direction.e;
                    break;
                case Direction.e:
                    n = GridBase.GetInstance().GetNode(curN.x + 1, curN.y, curN.z);
                    oppositeDir = Direction.w;
                    break;
            }

            return n;
        }

        WallDirection GetWallDir(Direction d)
        {
            switch (d)
            {
                case Direction.n:
                case Direction.s:
                    return WallDirection.horizontal;
                case Direction.w:
                case Direction.e:
                default:
                    return WallDirection.vertical;
            }
        }

        void RotateByDir(GameObject go,WallDirection wd)
        {
            Vector3 euler = Vector3.zero;

            switch (wd)
            {
                case WallDirection.horizontal:
                    euler.y = 90;
                    break;
                case WallDirection.vertical:
                    euler.y = 0;
                    break;
                default:
                    break;
            }

            go.transform.localEulerAngles = euler;
        }

        static public WallCreator singleton;
        void Awake()
        {
            singleton = this;
        }

    }

    public enum WallType
    {
        wall,door,window,delete
    }

    public enum WallDirection
    {
        horizontal,vertical
    }
}
