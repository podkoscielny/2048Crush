using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Tags = TagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class TileBehaviour : MonoBehaviour
    {
        [SerializeField] Score score;
        [SerializeField] ObjectPool objectPool;
        [SerializeField] TilePoints tilePoints;
        [SerializeField] TileTypePicker tileTypePicker;

        public bool IsSpecial { get; private set; } = false;
        public BehaviourDelegate Behaviour { get; private set; }
        public Behaviours BehaviourEnum { get; private set; } = Behaviours.Default;

        private void OnEnable() => tileTypePicker.OnTileTypePicked += CacheTileBehaviour;

        private void OnDisable() => tileTypePicker.OnTileTypePicked -= CacheTileBehaviour;

        private void DefaultBehaviour(Sequence tileMoveSequence, SelectedTile tileToBeDestroyed)
        {
            tilePoints.MultiplyPoints(2);
            score.AddPoints(tilePoints.PointsWorth);
        }

        private void MatchAnyTile(Sequence tileMoveSequence, SelectedTile tileToBeDestroyed)
        {
            Debug.Log("Blank");
        }

        private void MultiplyTilePoints(Sequence tileMoveSequence, SelectedTile tileToBeDestroyed)
        {

        }

        private void BombNearbyTiles(Sequence tileMoveSequence, SelectedTile tileToBeDestroyed)
        {

        }

        private void CacheTileBehaviour(TileType tileType)
        {
            if (!tileType.IsSpecial)
            {
                Behaviour = DefaultBehaviour;
                return;
            }

            switch (tileType.TileBehaviour)
            {
                case Behaviours.Default:
                    Behaviour = DefaultBehaviour;
                    break;

                case Behaviours.MultiplyAnyTile:
                    Behaviour = MultiplyTilePoints;
                    break;

                case Behaviours.MatchAnyTile:
                    Behaviour = MatchAnyTile;
                    break;

                case Behaviours.BombNearbyTiles:
                    Behaviour = BombNearbyTiles;
                    break;

                default:
                    break;
            }
        }
    }

    public enum Behaviours
    {
        Default,
        MultiplyAnyTile,
        MatchAnyTile,
        BombNearbyTiles
    }

    public delegate void BehaviourDelegate(Sequence tileMoveSequence, SelectedTile tileToBeDestroyed);
}
