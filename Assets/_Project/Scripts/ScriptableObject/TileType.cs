using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType")]
public class TileType : ScriptableObject
{
    [SerializeField] int pointsWorth;
    [SerializeField] Material tileMaterial;

    public int PointsWorth => pointsWorth;
    public string PointsToString { get; private set; }
    public Material TileMaterial => tileMaterial;

    void OnValidate()
    {
        PointsToString = pointsWorth.ToString();    
    }
}