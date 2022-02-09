using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public static class TileTypeConverter
    {
        public static TileType SerializableToNormal(CachedTileType cachedTileType, List<TileType> tileTypes)
        {
            foreach (TileType tileType in tileTypes)
            {
                if (tileType.isSpecial == cachedTileType.IsSpecial && tileType.pointsWorth == cachedTileType.PointsWorth && tileType.tileBehaviour == cachedTileType.TileBehaviour)
                    return tileType;
            }

            return null;
        }

        public static CachedTileType[,] NormalArrayToSerializable(TileType[,] tileTypes)
        {
            int rows = tileTypes.GetLength(0);
            int columns = tileTypes.GetLength(1);

            CachedTileType[,] cachedTileTypes = new CachedTileType[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    TileType tileType = tileTypes[i, j];

                    CachedTileType cachedTileType = new CachedTileType(tileType.pointsWorth, tileType.isSpecial, tileType.tileBehaviour);
                    cachedTileTypes[i, j] = cachedTileType;
                }
            }

            return cachedTileTypes;
        }
    }
}