﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] availableDotTypes;
    public GameObject[,] allDotsOnBoard;
    private BackgroundTile[,] allBackgroundTiles;

    void Start()
    {
        allBackgroundTiles = new BackgroundTile[width, height];
        allDotsOnBoard = new GameObject[width, height];
        Setup();
    }

    private void Setup()
    {
        doForEveryDot((i, j) =>
        {
            var cellName = $"( {i}, {j} )";

            var tempPosition = new Vector2(i, j);
            var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
            backgroundTile.name = "bg";

            int dotToUse = Random.Range(0, availableDotTypes.Length);

            int maxIterations = 0;
            while (MatchesAt(i, j, availableDotTypes[dotToUse]) && maxIterations < 100)
            {
                dotToUse = Random.Range(0, availableDotTypes.Length);
                maxIterations++;
                Debug.Log(maxIterations);
            }

            GameObject dot = Instantiate(availableDotTypes[dotToUse], tempPosition, Quaternion.identity);
            dot.transform.parent = transform;
            dot.name = "dot" + cellName;
            backgroundTile.transform.parent = dot.transform;

            allDotsOnBoard[i, j] = dot;
        });
    }


    private bool MatchesAt(int column, int row, GameObject gameObject)
    {
        if (column > 1 && row > 1)
        {
            if (allDotsOnBoard[column - 1, row].tag == gameObject.tag &&
                allDotsOnBoard[column - 2, row].tag == gameObject.tag)
            {
                return true;
            }
            if (allDotsOnBoard[column, row - 1].tag == gameObject.tag &&
                allDotsOnBoard[column, row - 2].tag == gameObject.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDotsOnBoard[column, row - 1].tag == gameObject.tag &&
                    allDotsOnBoard[column, row - 2].tag == gameObject.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDotsOnBoard[column - 1, row].tag == gameObject.tag &&
                    allDotsOnBoard[column - 2, row].tag == gameObject.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDotsOnBoard[column, row] != null && allDotsOnBoard[column, row].GetComponent<Dot>().isMatched)
        {
            Destroy(allDotsOnBoard[column, row]);
            allDotsOnBoard[column, row] = null;
        }
    }

    public void DestroyAllMatches()
    {
        doForEveryDot((i, j) =>
        {
            DestroyMatchesAt(i, j);
        });
        StartCoroutine(DecreaseRowCoroutine());
    }

    private IEnumerator DecreaseRowCoroutine()
    {
        int nullCount = 0;
        doForEveryDot((i, j) => {
            if (allDotsOnBoard[i, j] == null)
            {
                nullCount++;
            }
            else if (nullCount > 0)
            {
                allDotsOnBoard[i, j].GetComponent<Dot>().row -= nullCount;
                allDotsOnBoard[i, j] = null;
            }
        }, afterEachColumnAction: () => {
            nullCount = 0;
        });
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCoroutine());
    }

    private void doForEveryDot(Action<int, int> mainAction, Action afterEachColumnAction = null)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                mainAction.Invoke(i, j);
            }
            afterEachColumnAction?.Invoke();
        }
    }

    private void RefillBoard()
    {
        doForEveryDot((i, j) => {
            if (allDotsOnBoard[i, j] == null)
            {
                var tempPosition = new Vector2(i, j);
                var dotToUse = Random.Range(0, availableDotTypes.Length);
                var gameObject = Instantiate(availableDotTypes[dotToUse], tempPosition, Quaternion.identity);
                allDotsOnBoard[i, j] = gameObject;
            }
        });
    }

    private bool AreAnyMatchesOnBoard()
    {
        var areAnyMatchesOnBoard = false;
        doForEveryDot((i, j) => {
            if (allDotsOnBoard[i, j] != null && allDotsOnBoard[i, j].GetComponent<Dot>().isMatched)
            {
                areAnyMatchesOnBoard = true;
            }
        });
        return areAnyMatchesOnBoard;
    }

    private IEnumerator FillBoardCoroutine()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);
        while(AreAnyMatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyAllMatches();
        }
    }
}
