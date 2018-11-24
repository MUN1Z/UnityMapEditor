using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Map.Walls;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public int currentFloor;
        public bool hasHiddenFloors;
        public List<Floor> level_floors = new List<Floor>();

        public Floor GetActiveFloor()//top
        {
            return level_floors[level_floors.Count - 1];
        }

        public void AddFloor()
        {
            Floor f = new Floor();
            level_floors.Add(f);
            currentFloor = level_floors.Count - 1;
            f.nodeHolder = new GameObject();
            f.nodeHolder.name = "node holder for " + currentFloor;
            f.wallHolder = new GameObject();
            f.wallHolder.name = "wall holder for " + currentFloor;
            f.objHolder = new GameObject();
            f.objHolder.name = "obj holder for " + currentFloor;

            f.level = currentFloor;

            if(currentFloor > 0)
            {
                f.nodeHolder.tag = "Hide";
                f.wallHolder.tag = "Hide";
                f.objHolder.tag = "Hide";
            }
        }

        public void ClearAll()
        {
            foreach (Floor f in level_floors)
            {
                Destroy(f.floorCollision);
                Destroy(f.wallHolder);
                Destroy(f.objHolder);
                Destroy(f.nodeHolder);
            }

            level_floors.Clear();           
        }

        public void ChangeFloor(bool up)
        {
            int y = (up) ? 1 : -1;
            //the current floor changes if you add a new floor, so store this one
            int prevFloor = currentFloor;
            int targetY = currentFloor + y;
            if(targetY < 0)        
                return;
   
            if(targetY > level_floors.Count-1)
            {
                //Add floor?
                if (SessionMaster.GetInstance().isEditor)
                {
                    Map.GridBase.GetInstance().AddFloor();
                }
                else
                {
                    return;
                }
            }

        
            if (!up)
            {
                if(SessionMaster.GetInstance().isEditor)
                    level_floors[prevFloor].floorCollision.SetActive(false);

                level_floors[prevFloor].wallHolder.SetActive(false);
                level_floors[prevFloor].nodeHolder.SetActive(false);
                level_floors[prevFloor].objHolder.SetActive(false);
            }

            if (SessionMaster.GetInstance().isEditor)
                level_floors[targetY].floorCollision.SetActive(true);

            level_floors[targetY].wallHolder.SetActive(true);
            level_floors[targetY].nodeHolder.SetActive(true);
            level_floors[targetY].objHolder.SetActive(true);
            
            currentFloor = targetY;
            UI.CurrentFloor.singleton.UpdateText(currentFloor.ToString());
        }

        public void ChangeFloor(int level)
        {
            if(level > currentFloor)
                while(level > currentFloor)
                    ChangeFloor(true);

            else if(level < currentFloor)
                while(level < currentFloor)
                    ChangeFloor(false);
        }

        static public LevelManager singleton;
        void Awake()
        {
            singleton = this;
        }

    }

    public class Floor
    {
        public GameObject floorCollision;
        public GameObject wallHolder;
        public GameObject nodeHolder;
        public GameObject objHolder;
        public int level;
    }
}
