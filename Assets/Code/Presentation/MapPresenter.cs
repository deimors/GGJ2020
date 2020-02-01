﻿using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class MapPresenter : MonoBehaviour
{
    private readonly int BOARD_SIZE = 20;

    [SerializeField] private GameObject ground;

    [SerializeField] private GameObject pathLeftToBottom;
    [SerializeField] private GameObject pathLeftToRight;
    [SerializeField] private GameObject pathLeftToTop;
    [SerializeField] private GameObject pathTopToRight;
    [SerializeField] private GameObject pathTopToBottom;
    [SerializeField] private GameObject pathRightToBottom;

    [SerializeField] private GameObject brokenBasicTower;
    [SerializeField] private GameObject brokenFireTower;
    [SerializeField] private GameObject brokenIceTower;
    [SerializeField] private GameObject brokenPlasmaTower;

    [SerializeField] private GameObject brokenWall;
    [SerializeField] private GameObject wall;

    [SerializeField] private GameObject castle;

    [Inject]
    public MapAggregate MapAggregate { get; set; }

    [Inject]
    public WallsAggregate WallsAggregate { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        MapAggregate.Events.OfType<MapEvent, MapEvent.Initialized>().Subscribe(HandleMapInitializedEvent);
        WallsAggregate.Events.OfType<WallsEvent, WallsEvent.WallRepaired>().Subscribe(HandleWallRepairedEvent);

        Observable.NextFrame().Subscribe(_ => CreateLevel());
    }

    void Update()
    {
        //Handle clicks
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var xCoordinate = (int)Math.Round(clickedPosition.x);
            var yCoordinate = (int)Math.Round(clickedPosition.y);

            MapAggregate.ClickMapCell(xCoordinate, yCoordinate);
        }
    }

    private void CreateLevel()
    {
        MapAggregate.Initialize(BOARD_SIZE, BOARD_SIZE);
    }

    private void HandleMapInitializedEvent(MapEvent.Initialized initializedEvent)
    {
        var map = GetComponent<MapPresenter>().transform;

        for (var x = 0; x < initializedEvent.MapCells.GetLength(0); x++)
        {
            for (var y = 0; y < initializedEvent.MapCells.GetLength(1); y++)
            {
                switch (initializedEvent.MapCells[x, y])
                {
                    case GroundCell cell:
                        Instantiate(ground, new Vector3(x, y, 0), Quaternion.identity, map);
                        break;
                    case TowerCell cell:
                        Instantiate(brokenBasicTower, new Vector3(x, y, 0), Quaternion.identity, map);
                        break;
                    case PathCell cell:
                        var pathTile = ChoosePathSprite(initializedEvent, x, y);
                        Instantiate(pathTile, new Vector3(x, y, 0), Quaternion.identity, map);
                        break;
                    case WallCell cell:
                        Instantiate(brokenWall, new Vector3(x, y, 0), Quaternion.identity, map);
                        break;
                    case StartCell cell:
                        Instantiate(pathTopToRight, new Vector3(x, y, 0), Quaternion.identity, map);
                        break;
                    case GoalCell cell:
                        Instantiate(castle, new Vector3(x, y, 0), Quaternion.identity, map);
                        break;
                    default:
                        throw new NotImplementedException("Unknown cell type");
                }
            }
        }
    }

    private GameObject ChoosePathSprite(MapEvent.Initialized initializedEvent, int x, int y)
    {
        //Start of path
        if (x == 1 && y == 19)
        {
            return pathLeftToBottom;
        }
        //End of path
        else if (x == 1 && y == 0)
        {
            return pathLeftToRight;
        }
        //Other path tiles
        else
        {
            //Path to the left
            if (x != 0 && initializedEvent.MapCells[x - 1, y] is PathCell)
            {
                //Path above
                if (y != 19 && initializedEvent.MapCells[x, y + 1] is PathCell)
                {
                    return pathLeftToTop;
                }

                //Path to the right
                else if (x != 19 && initializedEvent.MapCells[x + 1, y] is PathCell)
                {
                    return pathLeftToRight;
                }

                //Must be path below
                else
                {
                    return pathLeftToBottom;
                }

            }
            //There exists a path to the top
            else if (y != 19 && initializedEvent.MapCells[x, y + 1] is PathCell)
            {
                //Path to the right
                if (x != 19 && initializedEvent.MapCells[x + 1, y] is PathCell)
                {
                    return pathTopToRight;
                }

                //Must be path below
                else
                {
                    return pathTopToBottom;
                }
            }
            //Must be path to the right and below
            else
            {
                return pathRightToBottom;
            }
        }
    }

    private void HandleWallRepairedEvent(WallsEvent.WallRepaired repairedEvent)
    {
        Debug.Log(repairedEvent);
    }
}
