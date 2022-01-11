using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialTileSO", menuName = "ScriptableObjects/SpecialTileSO")]
public class SpecialTileSO : ScriptableObject
{
    [SerializeField] Mesh specialTileMesh;
    [SerializeField] Vector3 specialTileScale;
    [SerializeField] Vector3 meshOffset;
    [SerializeField] Vector3 meshRotation;
    [SerializeField] Material specialTileMaterial;
    [SerializeField] SpecialBehaviours specialBehaviours;

    public List<SpecialTileBehaviour> SpecialTileBehaviours { get; private set; }
    public Mesh SpecialTileMesh => specialTileMesh;
    public Vector3 SpecialTileScale => specialTileScale;
    public Vector3 MeshOffset => meshOffset;
    public Vector3 MeshRotation => meshRotation;
    public Material SpecialTileMaterial => specialTileMaterial;

    private Behaviour[] behaviours;


    private void OnValidate()
    {
        InitializeBehaviours();

        SpecialTileBehaviours = new List<SpecialTileBehaviour>();

        foreach (Behaviour behaviour in behaviours)
        {
            if (specialBehaviours.HasFlag(behaviour.BehaviourEnum)) SpecialTileBehaviours.Add(behaviour.SpecialBehaviour);
        }
    }

    private void MultiplyAnyTile()
    {
        Debug.Log("Multiply");
    }

    private void MatchAnyTile()
    {
        Debug.Log("Blank");
    }

    private void BombNearbyTiles()
    {
        Debug.Log("Bomb");
    }

    private void InitializeBehaviours()
    {
        behaviours = new Behaviour[3];

        behaviours[0] = new Behaviour(SpecialBehaviours.BombNearbyTiles, BombNearbyTiles);
        behaviours[1] = new Behaviour(SpecialBehaviours.MatchAnyTile, MatchAnyTile);
        behaviours[2] = new Behaviour(SpecialBehaviours.MultiplyAnyTile, MultiplyAnyTile);
    }
}

public delegate void SpecialTileBehaviour();

[Flags]
public enum SpecialBehaviours
{
    MultiplyAnyTile = 1 << 0,
    MatchAnyTile = 1 << 1,
    BombNearbyTiles = 1 << 2
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
