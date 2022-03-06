using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class GameRestart : MonoBehaviour
    {
        public static event Action OnGameRestart;

        [SerializeField] GridSystem gridSystem;
        [SerializeField] ObjectPool objectPool;

        public void RestartGame()
        {
            foreach (GameObject tile in gridSystem.TilesAtGridCells)
            {
                objectPool.AddToPool(tile);
            }

            gridSystem.ResetCellArrays();
            OnGameRestart?.Invoke();
        }
    }
}
