using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TilePoints : MonoBehaviour
{
    public event Action OnPointsUpdated;

    [SerializeField] GridSystem gridSystem;

    public int PointsWorth
    {
        get => _pointsWorth;

        private set
        {
            _pointsWorth = value;
            OnPointsUpdated?.Invoke();
        }
    }

    private int _pointsWorth = 2;

    private void OnEnable()
    {
        Board.OnAssignPointsWorthToCells += AssignPointsWorthToCell;
        Board.OnTilesReverse += ReversePoints;
        InitializePoints();
    }

    private void OnDisable()
    {
        Board.OnAssignPointsWorthToCells -= AssignPointsWorthToCell;
        Board.OnTilesReverse -= ReversePoints;
    }

    public void MultiplyPoints(int multiplier) => PointsWorth *= multiplier;

    public void SetPoints(int pointsAmount) => PointsWorth = pointsAmount;

    private void InitializePoints() => PointsWorth = GetRandomPointsWorth();

    private void ReversePoints()
    {
        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        PointsWorth = gridSystem.CachedPointsWorthAtCells[tileCell.x, tileCell.y];
    }

    private int GetRandomPointsWorth()
    {
        TileProbabilityPair[] tileTypes = gridSystem.GridSize.TileTypes;
        float probabilitySum = gridSystem.GridSize.ProbabilitySum;
        float randomProbability = Random.Range(0, probabilitySum);
        float subtractFromSum = 0;

        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (randomProbability - subtractFromSum <= tileTypes[i].probability) return tileTypes[i].pointsWorth;

            subtractFromSum -= tileTypes[i].probability;
        }

        return tileTypes[tileTypes.Length - 1].pointsWorth;
    }

    private void AssignPointsWorthToCell()
    {
        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        gridSystem.AssignPointsWorthToCell(PointsWorth, tileCell);
    }

#if UNITY_EDITOR
    public void ChangeTilePointsWorth(int pointsToSet) => PointsWorth = pointsToSet;

    public void RandomizeTilePoints()
    {
        int randomPoints;

        do
        {
            randomPoints = Random.Range(2, 512);

        } while (!IsPowerOfTwo(randomPoints));

        PointsWorth = randomPoints;
    }

    private bool IsPowerOfTwo(int x) => (x != 0) && ((x & (x - 1)) == 0);
#endif
}
