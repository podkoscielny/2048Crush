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
    [SerializeField] ObjectPool objectPool;
    [SerializeField] GridSystem gridSystem;
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

    private void MultiplyAnyTile(SelectedTile selectedTile, GameObject specialTile)
    {
        Debug.Log("Multiply");
    }

    private void MatchAnyTile(SelectedTile selectedTile, GameObject specialTile)
    {
        Debug.Log("Blank");

        GameObject spawnedTile = objectPool.GetFromPool(TagSystem.Tags.Tile);
        Vector2Int specialTileCell = gridSystem.GetTileGridCell(specialTile);
        Vector3 specialTilePosition = gridSystem.GridCells[specialTileCell.x, specialTileCell.y];

        spawnedTile.transform.SetPositionAndRotation(specialTilePosition, new Quaternion(0, 0, 0, 0));
        spawnedTile.GetComponent<TilePoints>().SetPoints(selectedTile.TilePoints.PointsWorth);
        gridSystem.AssignTileToCell(spawnedTile, specialTileCell);

        specialTile.SetActive(false);
        selectedTile.TileObject.SetActive(false);

        spawnedTile.transform.DOPunchScale(new Vector3(0.4f, 0.4f, 0.4f), 0.3f, 1);
    }

    private void BombNearbyTiles(SelectedTile selectedTile, GameObject specialTile)
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

public delegate void SpecialTileBehaviour(SelectedTile selectedTile, GameObject specialTile);

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