using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "DefaultBehaviour", menuName = "ScriptableObjects/TileBehaviours/DefaultBehaviour")]
    public class DefaultBehaviour : BaseBehaviour
    {
        public override void Invoke(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            Vector3 spawnPosition = secondSelectedTile.TileObject.transform.position;
            spawnPosition.z -= 0.1f;

            TilePoints tilePoints = secondSelectedTile.TilePoints;

            MoveTileToPool(firstSelectedTile.TileCell, firstSelectedTile.TileObject);
            MultiplyTilePoints(tilePoints, 2);
            UpdateScore(tilePoints.PointsWorth);
            SpawnParticleEffect(spawnPosition, Tags.MatchEffect);
            PlaySoundEffect(soundEffect);
            DoPunchScale(secondSelectedTile.TileObject.transform);
        }
    }
}
