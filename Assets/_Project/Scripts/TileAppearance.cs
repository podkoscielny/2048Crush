using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileAppearance : MonoBehaviour
{
    [SerializeField] TextMeshPro tileText;
    [SerializeField] TextMeshPro backgroundText;
    [SerializeField] BoxCollider tileCollider;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] TileSwipe tileSwipe;
    [SerializeField] Gradient tileBackgroundGradient;
    [SerializeField] Gradient tileTextGradient;

    private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);

    private const float CELL_SIZE_FACTOR = 0.8f;
    private const int MAXED_COLOR_AT_TWO_TO_THE_POWER = 13;

    private void OnEnable()
    {
        tileSwipe.OnPointsUpdated += UpdateTile;
        InitializeTileType();
    }

    private void OnDisable() => tileSwipe.OnPointsUpdated -= UpdateTile;

    private void Awake() => InitializeTileSize();

    private void UpdateTile()
    {
        UpdateTileText();
        SetBackgroundColor();
    }

    private void InitializeTileType()
    {
        transform.rotation = _initialRotation;

        UpdateTileText();
        SetBackgroundColor();
    }

    private void UpdateTileText() => tileText.text = tileSwipe.PointsWorth.ToString();

    private void SetBackgroundColor()
    {
        int pointsAsTwoToThePower = PointsAsTwoToThePower();

        float colorGradientPercentage = Mathf.Min((float)pointsAsTwoToThePower / (float)MAXED_COLOR_AT_TWO_TO_THE_POWER, 1);
        backgroundText.color = tileBackgroundGradient.Evaluate(colorGradientPercentage);
        tileText.color = tileTextGradient.Evaluate(colorGradientPercentage);
    }

    private int PointsAsTwoToThePower()
    {
        int pointsAsTwoToThePower = tileSwipe.PointsWorth;
        int powers = 0;

        do
        {
            pointsAsTwoToThePower /= 2;
            powers++;

        } while (pointsAsTwoToThePower != 1);

        return powers;
    }

    private void InitializeTileSize()
    {
        float tileScaleX = gridSystem.CellWidth * CELL_SIZE_FACTOR;
        float tileScaleY = gridSystem.CellHeight * CELL_SIZE_FACTOR;
        float boxColliderFactor = 1 / CELL_SIZE_FACTOR;

        transform.localScale = new Vector3(tileScaleX, tileScaleY, transform.localScale.z);
        tileCollider.size = new Vector3(boxColliderFactor, boxColliderFactor, tileCollider.size.z);
    }
}