using Managers;
using Map;
using System;
using UI;
using UnityEditor;
using UnityEngine;

public class MapEditor : EditorWindow
{
    public int tab = 0;
    public float level = 0;
    public float levelPrev = 0;
    public string mapName = string.Empty;

    [MenuItem("Window/MapEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapEditor));
    }
    
    void OnGUI()
    {
        level = EditorGUILayout.Slider("Level", (int)level, 0, 7);
        
        if (levelPrev != level)
        {
            try
            {
                LevelManager.singleton.ChangeFloor((int)level);
                Debug.Log((int)level);

                levelPrev = level;
            }
            catch (Exception ex)
            {

            }
        }

        tab  = GUILayout.Toolbar(tab, new[] {"Ground", "Nature", "Wall Creator", "Map"});
        
        switch (tab)
        {
            #region Ground
            case 0:
                try
                {
                    LE_UI.singleton.ChangeCategory(CategoryType.ground);
                    
                    if (GUILayout.Button("Grass"))
                    {
                        LevelEditor.singleton.UpdateType("grass", PaintType.groundTexture);
                    }
                    if(GUILayout.Button("Stone"))
                    {
                        LevelEditor.singleton.UpdateType("stone", PaintType.groundTexture);
                    }
                    if (GUILayout.Button("Air"))
                    {
                        LevelEditor.singleton.UpdateType("air", PaintType.groundTexture);
                    }
                }
                catch
                {

                }
                break;
            #endregion

            #region Nature
            case 1:

                try
                {
                    LE_UI.singleton.ChangeCategory(CategoryType.levelobject);

                    if (GUILayout.Button("Barrel"))
                    {
                        LevelEditor.singleton.UpdateType("barrel", PaintType.levelObject);
                    }
                    if (GUILayout.Button("Bucket"))
                    {
                        LevelEditor.singleton.UpdateType("bucket", PaintType.levelObject);
                    }
                    if (GUILayout.Button("Plank"))
                    {
                        LevelEditor.singleton.UpdateType("plank", PaintType.levelObject);
                    }
                    if (GUILayout.Button("Rock"))
                    {
                        LevelEditor.singleton.UpdateType("rock", PaintType.levelObject);
                    }
                    if (GUILayout.Button("Tree1"))
                    {
                        LevelEditor.singleton.UpdateType("tree1", PaintType.levelObject);
                    }
                    if (GUILayout.Button("Tree3"))
                    {
                        LevelEditor.singleton.UpdateType("tree3", PaintType.levelObject);
                    }
                    if (GUILayout.Button("Fence"))
                    {
                        LevelEditor.singleton.UpdateType("fence", PaintType.levelObject);
                    }
                    if (GUILayout.Button("Pillar"))
                    {
                        LevelEditor.singleton.UpdateType("pillar", PaintType.levelObject);
                    }
                    if (GUILayout.Button("Post"))
                    {
                        LevelEditor.singleton.UpdateType("post", PaintType.levelObject);
                    }
                    if (GUILayout.Button("Remove"))
                    {
                        LE_UI.singleton.ChangeCategory(CategoryType.delete);
                    }
                }
                catch
                {

                }
                break;
            #endregion
                
            #region Wall
            case 2:
                try
                {
                    LE_UI.singleton.ChangeCategory(CategoryType.wallCreator);

                    if (GUILayout.Button("Wall"))
                    {
                        try
                        {
                            WallCreator.singleton.curType = WallType.wall;
                            Debug.Log("Wall");
                        }
                        catch
                        {

                        }
                    }
                    if (GUILayout.Button("Door"))
                    {
                        try
                        {
                            WallCreator.singleton.curType = WallType.door;
                            Debug.Log("Door");
                        }
                        catch
                        {

                        }
                    }
                    if (GUILayout.Button("Window"))
                    {
                        try
                        {
                            WallCreator.singleton.curType = WallType.window;
                            Debug.Log("Window");
                        }
                        catch
                        {

                        }
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        try
                        {
                            WallCreator.singleton.curType = WallType.delete;
                        Debug.Log("Delete");
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
                
                break;
            #endregion
                
            #region Map
            case 3:
                try
                {
                    if (GUILayout.Button("New Map"))
                    {
                        try
                        {
                            GridBase.GetInstance().ClearMap();
                            GridBase.GetInstance().InitLevel();
                        }
                        catch
                        {

                        }
                    }

                    if (GUILayout.Button("Save Map"))
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(mapName))
                                LevelSerializer.singleton.SaveLevel(mapName);
                        }
                        catch
                        {

                        }
                    }

                    if (GUILayout.Button("Load Map"))
                    {
                        try
                        {
                            GridBase.GetInstance().ClearMap();
                            GridBase.GetInstance().LoadMap(mapName);
                            LevelManager.singleton.ChangeFloor((int)level);
                        }
                        catch
                        {

                        }
                    }

                    mapName = EditorGUILayout.TextField ("Map Name: ", mapName);
                }
                catch
                {

                }
                
                break;
                
            #endregion
        }
    }
}