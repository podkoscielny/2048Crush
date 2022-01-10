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

    public Mesh SpecialTileMesh => specialTileMesh;
    public Vector3 SpecialTileScale => specialTileScale;
    public Vector3 MeshOffset => meshOffset;
    public Vector3 MeshRotation => meshRotation;
    public Material SpecialTileMaterial => specialTileMaterial;
}
