using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Map.Walls;

namespace Map
{
    public class NodeReferences : MonoBehaviour
    {
        public Renderer groundRenderer;
        public List<Wall> walls = new List<Wall>();
        public List<LevelObjectsActual> objectsOnNode = new List<LevelObjectsActual>();

        public bool IsObjectDuplicate(string id)
        {
            bool r = false;
            for (int i = 0; i < objectsOnNode.Count; i++)
            {
                if(objectsOnNode[i].id == id)
                {
                    r = true;
                    break;
                }
            }
            return r;
        }

        public void Init()
        {
            groundRenderer = GetComponentInChildren<Renderer>();
        }

        public void UpdateGroundTexture(Material gTexture)
        {
            groundRenderer.material = gTexture;
        }
    }

    public class LevelObjectsActual
    {
        public string id;
        public GameObject objReference;
        public float targetY;
    }
}
