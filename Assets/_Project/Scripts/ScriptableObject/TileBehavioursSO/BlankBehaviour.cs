using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "BlankBehaviour", menuName = "ScriptableObjects/TileBehaviours/BlankBehaviour")]
    public class BlankBehaviour : BaseBehaviour
    {
        public override void Invoke(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            if (secondSelectedTile.TileBehaviour.IsSpecial)
            {
                Transform firstSelectedTransform = firstSelectedTile.TileObject.transform;
                firstSelectedTransform.rotation = initialRotation;

                gridSystem.AssignTileToCell(firstSelectedTile.TileObject, secondSelectedTile.TileCell);
                MoveTileToPool(firstSelectedTile.TileCell, secondSelectedTile.TileObject);

                DoPunchScale(firstSelectedTransform);
            }
            else
            {
                MoveTileToPool(firstSelectedTile.TileCell, firstSelectedTile.TileObject);
                DoPunchScale(secondSelectedTile.TileObject.transform);
            }
            PlaySoundEffect(soundEffect);
        }
    }
}
