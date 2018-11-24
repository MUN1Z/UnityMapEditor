using UnityEngine;
using System.Collections;

namespace Map.Walls
{
    public class Wall
    {
        public Direction direction;
        public Managers.WallType wallType;
        public string textureId;
        public GameObject objReference;
    }
}
