using Assets.Scripts.Player;
using Managers;
using Map;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float offsetY = 2;

    Vector3 oldPos;
    
    void Start()
    {
        oldPos = transform.position;

        GridBase.instance.LoadMap2(GetPlayerPosition());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.position = new Vector3(oldPos.x, oldPos.y, oldPos.z + 2);
            GridBase.instance.CreateGridUp(GetPlayerPosition());
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.position = new Vector3(oldPos.x, oldPos.y, oldPos.z - 2);
            GridBase.instance.CreateGridDown(GetPlayerPosition());
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position = new Vector3(oldPos.x - 2, oldPos.y, oldPos.z);
            GridBase.instance.CreateGridLeft(GetPlayerPosition());
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position = new Vector3(oldPos.x + 2, oldPos.y, oldPos.z);
            GridBase.instance.CreateGridRight(GetPlayerPosition());
        }

        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Debug.Log("Plus");
            var oldPos = transform.position;
            transform.position = new Vector3(oldPos.x, oldPos.y + 2, oldPos.z);
            UpdateAllFloors();
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Debug.Log("Minus");
            var oldPos = transform.position;
            transform.position = new Vector3(oldPos.x, oldPos.y - 2, oldPos.z);
            UpdateAllFloors();
        }

        if (oldPos != transform.position)
            UpdateFloors();

        oldPos = transform.position;

        Debug.Log(GetPlayerPositionString());
    }

    private void UpdateFloors()
    {
        try
        {
            var node = GridBase.instance.NodeFromWorldPositionUp(transform.position, 0, 1);
            var node2 = GridBase.instance.NodeFromWorldPositionUp(transform.position, 1, 1);
            var node3 = GridBase.instance.NodeFromWorldPositionUp(transform.position, 0, 1, 1);

            var rendName = node.nr.groundRenderer.materials[0].name;
            var rendName2 = node2.nr.groundRenderer.materials[0].name;
            var rendName3 = node3.nr.groundRenderer.materials[0].name;

            var NoHasGround = rendName.Contains("transparent");
            var NoHasGround2 = rendName2.Contains("transparent");
            var NoHasGround3 = rendName3.Contains("transparent");

            if (NoHasGround || NoHasGround2 || NoHasGround3)
            {
                Debug.Log("NoHasGround");

                foreach (var floor in LevelManager.singleton.level_floors)
                {
                    floor.wallHolder.SetActive(true);
                    floor.nodeHolder.SetActive(true);
                    floor.objHolder.SetActive(true);
                }
            }
            else
            {
                foreach (var floor in LevelManager.singleton.level_floors)
                {
                    if (floor.level > GetPlayerFloor())
                    {
                        LevelManager.singleton.hasHiddenFloors = true;

                        floor.wallHolder.SetActive(false);
                        floor.nodeHolder.SetActive(false);
                        floor.objHolder.SetActive(false);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Exception: {ex.Message}");
        }
    }

    private void UpdateAllFloors()
    {
        foreach (var floor in LevelManager.singleton.level_floors)
        {
            floor.wallHolder.SetActive(true);
            floor.nodeHolder.SetActive(true);
            floor.objHolder.SetActive(true);
        }
    }

    public int GetPlayerFloor()
    {
        float worldY = transform.position.y;
        worldY /= offsetY;
        return Mathf.FloorToInt(worldY);
    }

    public string GetPlayerPositionString()
    {
        float worldX = transform.position.x;
        float worldY = transform.position.y;
        float worldZ = transform.position.z;

        //worldX /= offsetY;
        worldY /= offsetY;
        //worldZ /= offsetY;

        return $"X: {FloatToIntRound(worldX)} - Y: {FloatToIntRound(worldY)} - Z: {FloatToIntRound(worldZ)}";
    }

    public Position GetPlayerPosition()
    {
        float worldX = transform.position.x;
        float worldY = transform.position.y;
        float worldZ = transform.position.z;

        //worldX /= offsetY;
        worldY /= offsetY;
        //worldZ /= offsetY;

        return new Position
        {
            X = FloatToIntRound(worldX),
            Y = FloatToIntRound(worldY),
            Z = FloatToIntRound(worldZ)
        };
    }

    public int FloatToIntRound(float value)
    {
        var result = Mathf.FloorToInt(value);
        return result < 0 ? 0 : result;
    }
}
