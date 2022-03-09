using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "MultiplyBehaviour", menuName = "ScriptableObjects/TileBehaviours/MultiplyBehaviour")]
    public class MultiplyBehaviour : BaseBehaviour
    {
        [SerializeField] int multiplier;

        public override void Invoke(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            if (secondSelectedTile.TileBehaviour.IsSpecial)
            {
                Transform firstSelectedTransform = firstSelectedTile.TileObject.transform;
                firstSelectedTransform.rotation = initialRotation;

                gridSystem.AssignTileToCell(firstSelectedTile.TileObject, secondSelectedTile.TileCell);
                MoveTileToPool(firstSelectedTile.TileCell, secondSelectedTile.TileObject);
                MultiplyTilePoints(firstSelectedTile.TilePoints, multiplier);
                UpdateScore(firstSelectedTile.TilePoints.PointsWorth);
                DoPunchScale(firstSelectedTransform);
            }
            else
            {
                MoveTileToPool(firstSelectedTile.TileCell, firstSelectedTile.TileObject);
                MultiplyTilePoints(secondSelectedTile.TilePoints, multiplier);
                UpdateScore(secondSelectedTile.TilePoints.PointsWorth);
                DoPunchScale(secondSelectedTile.TileObject.transform);
            }

            Vector3 spawnPosition = secondSelectedTile.TileObject.transform.position;
            PlaySoundEffect(soundEffect);
            SpawnParticleEffect(spawnPosition, Tags.MatchEffect);
        }
    }
}
