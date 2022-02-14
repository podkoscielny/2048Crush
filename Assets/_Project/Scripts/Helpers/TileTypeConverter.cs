using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public static class TileTypeConverter
    {
        public static TileType SerializableToNormal(CachedTileType cachedTileType, TileType[] tileTypes)
        {
            foreach (TileType tileType in tileTypes)
            {
                if (tileType.name == cachedTileType.Name) return tileType;
            }

            return null;
        }

        public static CachedTileType[,] NormalArrayToSerializable(TileType[,] tileTypeVariants)
        {
            int rows = tileTypeVariants.GetLength(0);
            int columns = tileTypeVariants.GetLength(1);

            CachedTileType[,] cachedTileTypes = new CachedTileType[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    TileType tileType = tileTypeVariants[i, j];

                    CachedTileType cachedTileType = new CachedTileType(tileType.name);
                    cachedTileTypes[i, j] = cachedTileType;
                }
            }

            return cachedTileTypes;
        }

        public static TileType[,] SerializableArrayToNormal(CachedTileType[,] cachedTileTypes, TileType[] tileTypeVariants)
        {
            int rows = cachedTileTypes.GetLength(0);
            int columns = cachedTileTypes.GetLength(1);

            TileType[,] tileTypes = new TileType[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    CachedTileType cachedTileType = cachedTileTypes[i, j];

                    TileType tileType = SerializableToNormal(cachedTileType, tileTypeVariants);
                    tileTypes[i, j] = tileType;
                }
            }

            return tileTypes;
        }
    }
}