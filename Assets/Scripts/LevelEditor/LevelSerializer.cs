using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Map;
using Managers;
using System;
using Newtonsoft.Json;

public class LevelSerializer : MonoBehaviour
{
    GridBase gb;

    public string saveName = "level";
    public bool save;

    public void Tick()
    {
        if (save)
        {
            SaveLevel(saveName);
            save = false;
        }
    }

    public void SaveLevel(string saveName)
    {
        gb = GridBase.GetInstance();

        SaveFile saveFile = new SaveFile();
        saveFile.mapX = gb.maxX;
        saveFile.mapY = gb.maxY;
        saveFile.mapZ = gb.maxZ;
        saveFile.saveableNodes = NodeToSaveable();

        string saveLocation = SaveLocation(saveName, true);
        
        File.WriteAllText(saveLocation, JsonConvert.SerializeObject(saveFile));

        //IFormatter formatter = new BinaryFormatter();
        //Stream stream = new FileStream(saveLocation, FileMode.Create, FileAccess.Write, FileShare.None);
        //formatter.Serialize(stream, saveFile);
        //stream.Close();

        Debug.Log("Level saved at " + saveLocation);
    }

    List<SaveableNode> NodeToSaveable()
    {
        List<SaveableNode> r = new List<SaveableNode>();

        for (int x = 0; x < gb.maxX; x++)
        {
            for (int y = 0; y < gb.maxY; y++)
            {
                for (int z = 0; z < gb.maxZ; z++)
                {
                    Node n = gb.GetNode(x, y, z);
                    SaveableNode sn = n.GetSaveable();
                    r.Add(sn);
                }
            }
        }

        return r;
    }

    static string SaveLocation(string LevelName, bool save = false)
    {
        string saveLocation = Application.streamingAssetsPath + "/Levels/";

        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }

        return saveLocation + LevelName + ".json";
    }

    public SaveFile ReturnSaveFile(string saveName)
    {
        SaveFile retVal = null;
        string saveFile = SaveLocation(saveName);
        if(!File.Exists(saveFile))
        {
            Debug.Log(saveName + "can't find level!");
        }
        else
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(saveFile, FileMode.Open);
            SaveFile save = (SaveFile)formatter.Deserialize(stream);
            retVal = save;
            stream.Close();    
        }

        return retVal;
    }

    public void LoadAllFileLevels()
    {
        SessionMaster.GetInstance().availableLevels.Clear();

        DirectoryInfo dirInfo = new DirectoryInfo(Application.streamingAssetsPath + "/Levels");
        FileInfo[] fileInfo = dirInfo.GetFiles();

        foreach (FileInfo f in fileInfo)
        {
            string[] noMeta = f.Name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            if (noMeta.Length == 1)
            {
                SessionMaster.GetInstance().availableLevels.Add(f.Name);
            }             
        }
    }

    static public LevelSerializer singleton;
    void Awake()
    {
        singleton = this;
    }

}

[System.Serializable]
public class SaveFile
{
    public int mapX;
    public int mapY;
    public int mapZ;
    public List<SaveableNode> saveableNodes;
}

[System.Serializable]
public class SaveableNode
{
    public int x;
    public int y;
    public int z;
    public string textureId;
    public int nodeType;
    public SaveAbleDirection[] directionInfo;
    public List<SaveableObject> savedObjects;
}

[System.Serializable]
public class SaveAbleDirection
{
    public int directionId;
    public bool hasWall;
    public string textureId;
    public int wallType;
}

[System.Serializable]
public class SaveableObject
{
    public string objid;
    public float targetY;
}

