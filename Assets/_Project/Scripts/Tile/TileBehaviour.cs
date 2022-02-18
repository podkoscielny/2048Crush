using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class TileBehaviour : MonoBehaviour
    {
        [SerializeField] Score score;
        [SerializeField] ObjectPool objectPool;
        [SerializeField] GridSystem gridSystem;
        [SerializeField] TilePoints tilePoints;
        [SerializeField] TileTypePicker tileTypePicker;

        public bool IsSpecial { get; private set; } = false;
        public BehaviourDelegate Behaviour { get; private set; }

        private void OnEnable()
        {
            tileTypePicker.OnTileTypePicked += CacheTileBehaviour;
            tileTypePicker.OnGetCachedTileType += CacheTileBehaviour;
        }

        private void OnDisable()
        {
            tileTypePicker.OnTileTypePicked -= CacheTileBehaviour;
            tileTypePicker.OnGetCachedTileType += CacheTileBehaviour;
        }

        private void CacheTileBehaviour(TileType tileType) => Behaviour = tileType.behaviour.Invoke;
    }

    public delegate void BehaviourDelegate(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile);
}
