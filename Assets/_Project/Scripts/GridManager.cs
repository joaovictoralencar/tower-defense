using System.Collections;
using System.Collections.Generic;
using Singletons;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject notWalkablePrefab;
    public GameObject walkablePrefab;
    public GameObject startTilePrefab;
    public GameObject endTilePrefab;
    public int width;
    public int height;
    public float tileWidth;
    public float tileHeight;
    public Transform originTransform;
    private Vector3 startPosition, endPosition;
    List<Vector3> path = new List<Vector3>();

    private void Start()
    {
        GenerateStartAndEnd();
        Vector3 startPositionWithOffset = new Vector3(startPosition.x + (tileWidth / 2), startPosition.y,
            startPosition.z + (tileHeight / 2));
        Vector3 endPositionWithOffset = new Vector3(endPosition.x + (tileWidth / 2), endPosition.y,
            endPosition.z + (tileHeight / 2));
        GameManager.Instance.OnGenerateGrid.Invoke(startPositionWithOffset, endPositionWithOffset);
        GeneratePath();
    }

    private void GenerateStartAndEnd()
    {
        int startX = Random.Range(0, width);
        int endX = Random.Range(0, width);

        startPosition = new Vector3(
            originTransform.position.x + startX * tileWidth,
            originTransform.position.y,
            originTransform.position.z
        );
        endPosition = new Vector3(
            originTransform.position.x + endX * tileWidth,
            originTransform.position.y,
            originTransform.position.z + (height - 1) * tileHeight
        );

        if (startTilePrefab)
            Instantiate(startTilePrefab, startPosition, Quaternion.identity, originTransform);
        if (endTilePrefab)
            Instantiate(endTilePrefab, endPosition, Quaternion.identity, originTransform);
    }

    [SerializeField] private int _minTurns = 1;
    [SerializeField] private int _maxTurns = 4;

    private bool _hasTurned;

    private void GeneratePath()
    {
        int remainingTurns = Random.Range(_minTurns, _maxTurns + 1);
        path = new List<Vector3>();
        Vector3 currentPos = startPosition;
        path.Add(currentPos);

        int lastXDiff = 0;
        int lastZDiff = 0;

        int sameDirectionCount = 0;
        while (currentPos != endPosition)
        {
            _hasTurned = (lastZDiff != 0 && lastXDiff == 0) || (lastZDiff == 0 && lastXDiff != 0);

            if (_hasTurned) remainingTurns--;

            int xDiff = (int)(endPosition.x - currentPos.x);
            int zDiff = (int)(endPosition.z - currentPos.z);

            if (xDiff != 0 && zDiff != 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    xDiff = 0; //Up
                }
                else
                {
                    zDiff = 0; //Sides
                }
            }

            if (remainingTurns > 0)
            {
                //chance to turn
                //was going up
                if (lastZDiff != 0)
                {
                    sameDirectionCount++;
                    if (sameDirectionCount > 1)
                    {
                        //turn
                        xDiff = Random.Range(-1, 1); //sides
                        sameDirectionCount = 0;
                        remainingTurns--;
                    }
                }
                else if (lastXDiff != 0) //was going sides
                {
                    sameDirectionCount++;
                    if (sameDirectionCount > 1)
                    {
                        //turn
                        zDiff = 1; //or down
                        sameDirectionCount = 0;
                        remainingTurns--;
                    }
                }
            }


            if (xDiff > 0)
            {
                currentPos += new Vector3(tileWidth, 0f, 0f); //move right
            }
            else if (xDiff < 0)
            {
                currentPos -= new Vector3(tileWidth, 0f, 0f); //move left
            }
            else if (zDiff > 0)
            {
                currentPos += new Vector3(0f, 0f, tileHeight); //move up
            }
            else if (zDiff < 0)
            {
                currentPos -= new Vector3(0f, 0f, tileHeight); //move down
            }

            lastXDiff = xDiff;
            lastZDiff = zDiff;
            path.Add(currentPos);
        }

        for (int x = 0;
             x < width;
             x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(
                    originTransform.position.x + x * tileWidth,
                    originTransform.position.y,
                    originTransform.position.z + z * tileHeight
                );

                if (path.Contains(pos))
                {
                    if (pos == startPosition || pos == endPosition) continue;

                    if (walkablePrefab)
                        Instantiate(walkablePrefab, pos, Quaternion.identity, originTransform);
                }
                else
                {
                    Instantiate(notWalkablePrefab, pos, Quaternion.identity, originTransform);
                }
            }
        }
    }
}