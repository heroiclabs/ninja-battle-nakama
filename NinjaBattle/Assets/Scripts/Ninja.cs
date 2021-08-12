﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    private const float TickRate = 5f;

    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Vector2Int startingCoordinates = new Vector2Int();
    [SerializeField] private Direction startingDirection = Direction.East;
    [SerializeField] private KeyCode keyUp = KeyCode.None;
    [SerializeField] private KeyCode keyLeft = KeyCode.None;
    [SerializeField] private KeyCode keyRight = KeyCode.None;
    [SerializeField] private KeyCode keyDown = KeyCode.None;

    private Direction currentDirection = Direction.East;
    private Queue<Direction> nextDirections = new Queue<Direction>();
    private Vector2Int desiredCoordinates = new Vector2Int();
    private Vector2Int currentCoordinates = new Vector2Int();
    private bool isJumping = false;

    private void Start()
    {
        currentCoordinates = desiredCoordinates = startingCoordinates;
        desiredCoordinates += startingDirection.ToVector2();
        currentDirection = startingDirection;
        StartCoroutine(Step());
    }

    private IEnumerator Step()
    {
        var tickDuration = 1 / TickRate;
        while (true)
        {
            yield return new WaitForSeconds(tickDuration);
            if (!isJumping)
            {
                while (nextDirections.Count > 0)
                {
                    var nextDirection = nextDirections.Dequeue();
                    if (nextDirection == currentDirection)
                        continue;

                    if (nextDirection == currentDirection.Opposite())
                        continue;

                    currentDirection = nextDirection;
                    break;
                }
            }

            Vector2Int newCoordinates = currentCoordinates + currentDirection.ToVector2();
            if (Map.instance.IsWallTile(newCoordinates))
            {
                Map.instance.SetTileAsDangerous(currentCoordinates);
                KillNinja();
            }
            else if (!isJumping && Map.instance.IsDangerousTile(newCoordinates))
            {
                JumpStart();
                Map.instance.SetTileAsDangerous(currentCoordinates);
            }
            else if (isJumping)
            {
                JumpEnd();
                if (Map.instance.IsDangerousTile(newCoordinates))
                    KillNinja();
            }
            else
            {
                Map.instance.SetTileAsDangerous(currentCoordinates);
                if (Map.instance.IsDangerousTile(newCoordinates))
                    KillNinja();
            }

            currentCoordinates = newCoordinates;
            spriteRenderer.transform.position = ((Vector3Int)currentCoordinates);
        }
    }

    private void JumpStart()
    {
        isJumping = true;
        spriteRenderer.transform.localScale = Vector3.one * 0.55f;
    }

    private void JumpEnd()
    {
        isJumping = false;
        spriteRenderer.transform.localScale = Vector3.one * 0.4f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyUp))
            nextDirections.Enqueue(Direction.North);
        else if (Input.GetKeyDown(keyLeft))
            nextDirections.Enqueue(Direction.West);
        else if (Input.GetKeyDown(keyRight))
            nextDirections.Enqueue(Direction.East);
        else if (Input.GetKeyDown(keyDown))
            nextDirections.Enqueue(Direction.South);
    }

    private void KillNinja()
    {
        Destroy(gameObject);
    }
}