using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Managers
{
    public class ResourcesManager : MonoBehaviour
    {
        public List<TextureAsset> groundTextures = new List<TextureAsset>();
        public List<LevelObjectsAsset> levelObjects = new List<LevelObjectsAsset>();

        public TextureAsset GetTexture(string id)
        {
            TextureAsset r = null;

            for (int i = 0; i < groundTextures.Count; i++)
            {
                if(groundTextures[i].id == id)
                {
                    r = groundTextures[i];
                    break;
                }
            }
            return r;
        }

        public LevelObjectsAsset GetLvlObject(string id)
        {
            LevelObjectsAsset r = null;
            for (int i = 0; i < levelObjects.Count; i++)
            {
                if (levelObjects[i].id == id)
                {
                    r = levelObjects[i];
                    break;
                }
            }

            return r;
        }

        static public ResourcesManager singleton;

        void Awake()
        {
            singleton = this;
        }
    }

    [System.Serializable]
    public class LevelObjectsAsset
    {
        public string id;
        public GameObject gameModel;
        public Sprite icon;
    }
     
    [System.Serializable]
    public class TextureAsset
    {
        public string id;
        public Material material;
        public Sprite icon;
    }
}
