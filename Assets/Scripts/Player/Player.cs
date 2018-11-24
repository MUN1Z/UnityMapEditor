using Managers;
using Map;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float offsetY = 2;

    Vector3 oldPos;
    
    void Start()
    {
        oldPos = transform.position;
    }

    private void Update()
    {
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
    }

    private void UpdateFloors()
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
}
