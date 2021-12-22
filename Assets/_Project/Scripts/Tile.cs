using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public static event Action<SelectedTile, Transform> OnTilesMatch;

    [SerializeField] TextMeshPro tileText;
    [SerializeField] TextMeshPro backgroundText;
    [SerializeField] BoxCollider tileCollider;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] Gradient tileBackgroundGradient;

    private static SelectedTile _selectedTile;
    private static SelectedTile _tileToBeSwipedInto;
    private static bool _canTilesBeClicked = true;

    private int _pointsWorth = 2;
    private bool _isGoingToBeUpdated = false;
    private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);
    private SelectedTile _emptyTileSelection = new SelectedTile();

    private const float CELL_SIZE_FACTOR = 0.8f;
    private const int MAXED_COLOR_AT_TWO_TO_THE_POWER = 13;

    private void OnEnable()
    {
        Board.OnTileSequenceEnded += UpdateMergedTile;
        InitializeTileType();
    }

    private void OnDisable()
    {
        Board.OnTileSequenceEnded -= UpdateMergedTile;
        ResetProperties();
    }

    private void Awake() => InitializeTileSize();

    private void OnMouseDown()
    {
        if (!_canTilesBeClicked) return;

        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        _selectedTile = new SelectedTile(gameObject, _pointsWorth, tileCell);
    }

    private void OnMouseUp()
    {
        if (!_isGoingToBeUpdated && !_canTilesBeClicked)
            _canTilesBeClicked = true;
    }

    private void OnMouseOver()
    {
        if (_selectedTile.TileObject != null && _selectedTile.TileObject != gameObject && _tileToBeSwipedInto.TileObject == null)
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            _tileToBeSwipedInto = new SelectedTile(gameObject, _pointsWorth, tileCell);

            bool areTilesClose = gridSystem.AreTilesClose(_selectedTile.TileCell, _tileToBeSwipedInto.TileCell);
            bool areTilesWorthSame = _selectedTile.PointsWorth == _tileToBeSwipedInto.PointsWorth;

            if (areTilesClose && areTilesWorthSame)
            {
                _canTilesBeClicked = false;
                _isGoingToBeUpdated = true;
                OnTilesMatch?.Invoke(_selectedTile, transform);
            }
            else
                SetNotMatchedTilesProperties();
        }
    }

    private void SetNotMatchedTilesProperties()
    {
        Transform selectedTileTransform = _selectedTile.TileObject.transform;
        Transform swipedTileTransform = _tileToBeSwipedInto.TileObject.transform;

        selectedTileTransform.transform.DODynamicLookAt(swipedTileTransform.position, 0.1f)
            .OnComplete(() => selectedTileTransform.transform.DORotateQuaternion(_initialRotation, 0.1f).SetEase(Ease.InBack));

        _selectedTile = _emptyTileSelection;
        _tileToBeSwipedInto = _emptyTileSelection;
    }

    private void UpdateMergedTile()
    {
        if (!_isGoingToBeUpdated) return;

        _selectedTile = _emptyTileSelection;
        _tileToBeSwipedInto = _emptyTileSelection;

        _canTilesBeClicked = true;
        _isGoingToBeUpdated = false;
        _pointsWorth *= 2;

        UpdateTileText();
        SetBackgroundColor();
    }
    private void UpdateTileText() => tileText.text = _pointsWorth.ToString();

    private void InitializeTileType()
    {
        TileType randomTile = GetRandomTileType();

        _pointsWorth = randomTile.PointsWorth;
        tileText.text = randomTile.PointsToString;
        transform.rotation = _initialRotation;

        SetBackgroundColor();
    }

    private void SetBackgroundColor()
    {
        int pointsAsTwoToThePower = PointsAsTwoToThePower();

        float colorGradientPercentage = Mathf.Min((float)pointsAsTwoToThePower / (float)MAXED_COLOR_AT_TWO_TO_THE_POWER, 1);
        backgroundText.color = tileBackgroundGradient.Evaluate(colorGradientPercentage);
    }

    private int PointsAsTwoToThePower()
    {
        int pointsAsTwoToThePower = _pointsWorth;
        int powers = 0;

        do
        {
            pointsAsTwoToThePower /= 2;
            powers++;

        } while (pointsAsTwoToThePower != 1);

        return powers;
    }

    private TileType GetRandomTileType()
    {
        TileProbabilityPair[] tileTypes = gridSystem.GridSize.TileTypes;
        float probabilitySum = gridSystem.GridSize.ProbabilitySum;
        float randomProbability = Random.Range(0, probabilitySum);
        float subtractFromSum = 0;

        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (randomProbability - subtractFromSum <= tileTypes[i].probability) return tileTypes[i].tileType;

            subtractFromSum -= tileTypes[i].probability;
        }

        return tileTypes[tileTypes.Length - 1].tileType;
    }

    private void InitializeTileSize()
    {
        float tileScaleX = gridSystem.CellWidth * CELL_SIZE_FACTOR;
        float tileScaleY = gridSystem.CellHeight * CELL_SIZE_FACTOR;
        float boxColliderFactor = 1 / CELL_SIZE_FACTOR;

        transform.localScale = new Vector3(tileScaleX, tileScaleY, transform.localScale.z);
        tileCollider.size = new Vector3(boxColliderFactor, boxColliderFactor, tileCollider.size.z);
    }

    private void ResetProperties() => transform.rotation = _initialRotation;
}
