﻿using System.Linq;
using UniRx;

public class InitializeTowersWhenMapInitialized
{
    public InitializeTowersWhenMapInitialized(MapAggregate map, TowersAggregate towers)
    {
        map.Events
            .OfType<MapEvent, MapEvent.Initialized>()
            .Subscribe(initialized => InitializeTowers(initialized, towers));
    }

    private void InitializeTowers(MapEvent.Initialized initialized, TowersAggregate towers)
    {
        var mapCoords = Enumerable.Range(0, initialized.MapCells.GetLength(0)).SelectMany(x => Enumerable.Range(0, initialized.MapCells.GetLength(1)).Select(y => new MapCoordinate(x, y)));

        var initialTowers = mapCoords
            .Where(coord => initialized.MapCells[coord.X, coord.Y] is TowerCell)
            .Select(coord => new InitialTower(TowerIdentifier.Create(), coord))
            .ToArray();

        towers.Initialize(initialTowers);
    }
}