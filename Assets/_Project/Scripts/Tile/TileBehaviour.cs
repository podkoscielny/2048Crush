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
        [SerializeField] GridSystem gridSystem;
        [SerializeField] TilePoints tilePoints;
        [SerializeField] TileTypePicker tileTypePicker;

        public bool IsSpecial { get; private set; } = false;
        public BehaviourDelegate Behaviour { get; private set; }
        public Behaviours BehaviourEnum { get; private set; } = Behaviours.Default;

        private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);
        private Vector3 _enlargedTileScale = new Vector3(0.4f, 0.4f, 0.4f);

        private void OnEnable() => tileTypePicker.OnTileTypePicked += CacheTileBehaviour;

        private void OnDisable() => tileTypePicker.OnTileTypePicked -= CacheTileBehaviour;

        private void DefaultBehaviour(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            tilePoints.MultiplyPoints(2);
            score.AddPoints(tilePoints.PointsWorth);

            gridSystem.DeAssignTileFromCell(firstSelectedTile.TileCell);
            objectPool.AddToPool(Tags.Tile, firstSelectedTile.TileObject);

            MakePunchScale(secondSelectedTile.TileObject.transform);
        }

        private void MatchAnyTile(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            if (secondSelectedTile.TileBehaviour.IsSpecial)
            {
                Transform firstSelectedTransform = firstSelectedTile.TileObject.transform;

                firstSelectedTransform.rotation = _initialRotation;

                gridSystem.AssignTileToCell(firstSelectedTile.TileObject, secondSelectedTile.TileCell);
                gridSystem.DeAssignTileFromCell(firstSelectedTile.TileCell);
                objectPool.AddToPool(Tags.Tile, gameObject);

                MakePunchScale(firstSelectedTransform);
            }
            else
            {
                gridSystem.DeAssignTileFromCell(firstSelectedTile.TileCell);
                objectPool.AddToPool(Tags.Tile, firstSelectedTile.TileObject);

                MakePunchScale(secondSelectedTile.TileObject.transform);
            }
        }

        private void MultiplyTilePoints(SelectedTile tileToBeDestroyed, SelectedTile secondSelectedTile)
        {

        }

        private void BombNearbyTiles(SelectedTile tileToBeDestroyed, SelectedTile secondSelectedTile)
        {

        }

        private void MakePunchScale(Transform tileTransform) => tileTransform.DOPunchScale(_enlargedTileScale, 0.3f, 1);

        private void CacheTileBehaviour(TileType tileType)
        {
            IsSpecial = tileType.IsSpecial;

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

    public delegate void BehaviourDelegate(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile);
}
