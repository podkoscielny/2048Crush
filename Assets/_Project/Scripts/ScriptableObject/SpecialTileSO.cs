using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "SpecialTileSO", menuName = "ScriptableObjects/SpecialTileSO")]
public class SpecialTileSO : ScriptableObject
{
    [SerializeField] Mesh specialTileMesh;
    [SerializeField] Vector3 specialTileScale;
    [SerializeField] Vector3 meshOffset;
    [SerializeField] Vector3 meshRotation;
    [SerializeField] Material specialTileMaterial;
    [SerializeField] SpecialBehaviours behaviourEnum;

    public SpecialTileBehaviour SpecialTileBehaviour { get; private set; }
    public Mesh SpecialTileMesh => specialTileMesh;
    public Vector3 SpecialTileScale => specialTileScale;
    public Vector3 MeshOffset => meshOffset;
    public Vector3 MeshRotation => meshRotation;
    public Material SpecialTileMaterial => specialTileMaterial;

    private Behaviour[] _behaviours;
    private Sequence _tileMoveSequence;


    private void OnValidate()
    {
        InitializeBehaviours();

        foreach (Behaviour behaviour in _behaviours)
        {
            if (behaviourEnum == behaviour.BehaviourEnum)
            {
                SpecialTileBehaviour = behaviour.SpecialBehaviour;
                break;
            }
        }   
    }

    private void MultiplyAnyTile(SelectedTile selectedTile, Transform specialTileTransform)
    {
        Debug.Log("Multiply");
    }

    private void MatchAnyTile(SelectedTile selectedTile, Transform specialTileTransform)
    {
        Debug.Log("Blank");
        //Do that in special tile
        //_tileMoveSequence = DOTween.Sequence().SetAutoKill(false);
        //_tileMoveSequence.Append(selectedTile.TileObject.transform.DODynamicLookAt(specialTileTransform.position, 0.1f));
        //_tileMoveSequence.Append(selectedTile.TileObject.transform.DOMove(specialTileTransform.position, 0.15f).SetEase(Ease.InBack));

    }

    private void BombNearbyTiles(SelectedTile selectedTile, Transform specialTileTransform)
    {
        Debug.Log("Bomb");
    }

    private void InitializeBehaviours()
    {
        _behaviours = new Behaviour[3];

        _behaviours[0] = new Behaviour(SpecialBehaviours.BombNearbyTiles, BombNearbyTiles);
        _behaviours[1] = new Behaviour(SpecialBehaviours.MatchAnyTile, MatchAnyTile);
        _behaviours[2] = new Behaviour(SpecialBehaviours.MultiplyAnyTile, MultiplyAnyTile);
    }
}

public delegate void SpecialTileBehaviour(SelectedTile selectedTile, Transform specialTileTransform);

public enum SpecialBehaviours
{
    MultiplyAnyTile,
    MatchAnyTile,
    BombNearbyTiles
}

public struct Behaviour
{
    public SpecialBehaviours BehaviourEnum { get; private set; }
    public SpecialTileBehaviour SpecialBehaviour { get; private set; }

    public Behaviour(SpecialBehaviours behaviourEnum, SpecialTileBehaviour specialBehaviour)
    {
        BehaviourEnum = behaviourEnum;
        SpecialBehaviour = specialBehaviour;
    }
}
