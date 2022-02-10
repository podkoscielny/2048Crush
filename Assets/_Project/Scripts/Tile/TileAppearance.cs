using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Crush2048
{
    public class TileAppearance : MonoBehaviour
    {
        [Header("3D UI")]
        [SerializeField] TextMeshPro tileText;
        [SerializeField] TextMeshPro backgroundText;
        [SerializeField] SpriteRenderer specialImageRenderer;

        [Header("Components attached to Tile")]
        [SerializeField] BoxCollider tileCollider;
        [SerializeField] Renderer tileRenderer;
        [SerializeField] TilePoints tilePoints;
        [SerializeField] TileTypePicker tileTypePicker;

        [Header("Systems")]
        [SerializeField] GridSystem gridSystem;
        [SerializeField] TileGradients tileGradients;

        private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);

        private const float CELL_SIZE_FACTOR = 0.8f;
        private const int MAXED_COLOR_AT_TWO_TO_THE_POWER = 13;

        private void OnEnable()
        {
            tilePoints.OnPointsUpdated += UpdateTile;
            tileTypePicker.OnTileTypePicked += SetTileUI;
            tileTypePicker.OnGetCachedTileType += SetTileUI;

            InitializeTileType();
        }

        private void OnDisable()
        {
            tilePoints.OnPointsUpdated -= UpdateTile;
            tileTypePicker.OnTileTypePicked -= SetTileUI;
            tileTypePicker.OnGetCachedTileType += SetTileUI;
        }

        private void Awake() => InitializeTileSize();

        private void UpdateTile()
        {
            UpdateTileText();
            SetTileColor();
        }

        private void InitializeTileType()
        {
            transform.rotation = _initialRotation;

            UpdateTileText();
            SetTileColor();
        }

        private void SetTileUI(TileType tileType)
        {
            specialImageRenderer.sprite = tileType.image;

            if (tileType.isSpecial)
            {
                tileText.gameObject.SetActive(false);
            }
            else
            {
                tileText.gameObject.SetActive(true);
            }
        }

        private void UpdateTileText() => tileText.text = tilePoints.PointsWorth.ToString();

        private void SetTileColor()
        {
            int pointsAsTwoToThePower = PointsAsTwoToThePower();
            float colorGradientPercentage = Mathf.Min((float)pointsAsTwoToThePower / (float)MAXED_COLOR_AT_TWO_TO_THE_POWER, 1);
            Color backgroundColor = tileGradients.GetBackgroundColor(colorGradientPercentage);

            backgroundText.color = backgroundColor;
            tileRenderer.material.color = backgroundColor;
            tileText.color = tileGradients.GetTextColor(colorGradientPercentage);
        }

        private int PointsAsTwoToThePower()
        {
            int pointsAsTwoToThePower = tilePoints.PointsWorth;
            int powers = 0;

            if (pointsAsTwoToThePower <= 0) return 0;

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
}
