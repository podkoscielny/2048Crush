using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType")]
public class TileType : ScriptableObject
{
    [SerializeField] int pointsWorth;

    public int PointsWorth => pointsWorth;
    public string PointsToString { get; private set; }

    void OnValidate()
    {
        PointsToString = pointsWorth.ToString();    
    }
}