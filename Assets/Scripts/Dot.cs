﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float radianAngle = 0;

    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        column = targetX;
        targetY = (int)transform.position.y;
        row = targetY;
    }

    void Update()
    {
        targetX = column;
        targetY = row;
        // X
        tempPosition = new Vector2(targetX, transform.position.y);
        if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            // move towards the target
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);
        }
        else
        {
            // directly set the position
            transform.position = tempPosition;
            board.allDotsOnBoard[column, row] = gameObject;
        }
        // Y
        tempPosition = new Vector2(transform.position.x, targetY);
        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            // move towards the target
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);
        }
        else
        {
            // directly set the position
            transform.position = tempPosition;
            board.allDotsOnBoard[column, row] = gameObject;
        }
    }

    private void OnMouseDown() {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp() {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        radianAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        CheckAngle();
    }

    private void CheckAngle()
    {
        var swipeAngle = SwipeHelpers.getSwipeAngleFromRadian(radianAngle);
        Debug.Log($"Swipe {swipeAngle}");
        SwapGemIfPossible(swipeAngle);
    }

    private void SwapGemIfPossible(SwipeAngle swipeAngle)
    {
        switch (swipeAngle)
        {
            case SwipeAngle.Right:
                if (column < board.width) {
                    otherDot = board.allDotsOnBoard[column + 1, row];
                    otherDot.GetComponent<Dot>().column--;
                    column++;
                }
                break;
            case SwipeAngle.Up:
                if (row < board.height) {
                    otherDot = board.allDotsOnBoard[column, row + 1];
                    otherDot.GetComponent<Dot>().row--;
                    row++;
                }
                break;
            case SwipeAngle.Left:
                if (column > 0) {
                    otherDot = board.allDotsOnBoard[column - 1, row];
                    otherDot.GetComponent<Dot>().column++;
                    column--;
                }
                break;
            case SwipeAngle.Down:
                if (row > 0) {
                    otherDot = board.allDotsOnBoard[column, row - 1];
                    otherDot.GetComponent<Dot>().row++;
                    row--;
                }
                break;
        }
    }
}