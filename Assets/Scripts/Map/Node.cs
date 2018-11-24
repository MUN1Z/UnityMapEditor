using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Map
{
    public class Node
    {
        public int x;
        public int y;
        public int z;
        public string textureId;
        public NodeReferences nr;
        public DirectionInfo[] directions;
        public NodeType nodeType;

        public void Init()
        {
            directions = new DirectionInfo[4];
            for (int i = 0; i < 4; i++)
            {
                DirectionInfo d = new DirectionInfo();
                directions[i] = d;
            }
            directions[0].id = Direction.n;
            directions[1].id = Direction.s;
            directions[2].id = Direction.w;
            directions[3].id = Direction.e;
        }

        public void AddWall(Direction dir, GameObject objRef, bool updateNeighbor)
        {
            DirectionInfo di = GetDirectionInfo(dir);
            di.hasWall = true;
            if (di.wall == null)
                di.wall = new Walls.Wall();

            di.wall.direction = dir;
            di.wall.objReference = objRef;
            di.wall.wallType = Managers.WallType.wall;

            if (!updateNeighbor)
                return;      
            Node n = this;
            Direction targetDir = Direction.n;

            switch (dir)
            {
                case Direction.n:
                    n = GridBase.GetInstance().GetNode(x, y, z + 1);
                    targetDir = Direction.s;
                    break;
                case Direction.s:
                    n = GridBase.GetInstance().GetNode(x, y, z - 1);
                    targetDir = Direction.n;
                    break;
                case Direction.w:
                    n = GridBase.GetInstance().GetNode(x-1, y, z);
                    targetDir = Direction.e;
                    break;
                case Direction.e:
                    n = GridBase.GetInstance().GetNode(x+1, y, z);
                    targetDir = Direction.w;
                    break;
            }

            n.AddWall(targetDir,objRef,false);
        }

        public bool isWallDuplicate(Direction dir)
        {
            bool r = false;
            for (int i = 0; i < directions.Length; i++)
            {
                if(directions[i].hasWall && directions[i].id == dir)
                {
                    r = true;
                    break;
                }
            }

            return r;
        }

        public bool hasWallOnDir(Direction dir)
        {
            DirectionInfo i = GetDirectionInfo(dir);
            return i.hasWall;
        }

        public DirectionInfo GetDirectionInfo(Direction dir)
        {
            DirectionInfo r = null;
            for (int i = 0; i < directions.Length; i++)
            {
                if(directions[i].id == dir)
                {
                    r = directions[i];
                }
            }
            return r;
        }

        public SaveableNode GetSaveable()
        {
            SaveableNode s = new SaveableNode();
            s.x = x;
            s.y = y;
            s.z = z;
            s.textureId = textureId;
            s.nodeType = (int)nodeType;
            s.directionInfo = new SaveAbleDirection[4];
            s.savedObjects = new List<SaveableObject>();

            for (int i = 0; i < directions.Length; i++)
            {
                SaveAbleDirection sd = new SaveAbleDirection();

                sd.directionId = (int)directions[i].id;
                sd.hasWall = directions[i].hasWall;

                if (sd.hasWall)
                {
                    sd.textureId = directions[i].wall.textureId;
                    sd.wallType = (int)directions[i].wall.wallType;
                }

                s.directionInfo[i] = sd;
            }

            for (int i = 0; i < nr.objectsOnNode.Count; i++)
            {
                SaveableObject so = new SaveableObject();
                so.objid = nr.objectsOnNode[i].id;
                so.targetY = nr.objectsOnNode[i].targetY;
                s.savedObjects.Add(so);
            }

            return s;
        }  
    }

    [System.Serializable]
    public class DirectionInfo
    {
        public Direction id;
        public bool hasWall;
        public Walls.Wall wall;
    }

    public enum Direction
    {
        n,s,w,e
    }

    public enum NodeType
    {
        ground,air
    }
}
