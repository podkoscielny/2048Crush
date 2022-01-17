using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "Highscore", menuName = "ScriptableObjects/Highscore")]
    public class Highscore : ScriptableObject
    {
        public int Value => _value;

        private int _value = 0;

        public void SetValue(int value) => _value = value;
    }
}