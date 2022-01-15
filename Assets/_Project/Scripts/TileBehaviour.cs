using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Tags = TagSystem.Tags;

public class TileBehaviour : MonoBehaviour
{
    [SerializeField] Score score;
    [SerializeField] ObjectPool objectPool;
    [SerializeField] TilePoints tilePoints;

    public BehaviourDelegate Behaviour { get; private set; }
    public Behaviours BehaviourEnum { get; private set; } = Behaviours.Default;

    private void Start()
    {
        Behaviour = DefalutBehaviour;
    }

    private void DefalutBehaviour(Sequence tileMoveSequence, SelectedTile tileToBeDestroyed)
    {
        UpdateScore(2);
    }

    private void UpdateScore(int pointsMultiplier)
    {
        tilePoints.MultiplyPoints(pointsMultiplier);
        score.AddPoints(tilePoints.PointsWorth);
    }
}

public delegate void BehaviourDelegate(Sequence tileMoveSequence, SelectedTile tileToBeDestroyed);
