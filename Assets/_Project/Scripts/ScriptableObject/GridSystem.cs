using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridSystem", menuName = "ScriptableObjects/GridSystem")]
public class GridSystem : ScriptableObject
{
    [SerializeField] int rows;
    [SerializeField] int columns;

    public 

    void OnEnable()
    {
            
    }

    void OnValidate()
    {
        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);
    }
}