using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "IntValue", menuName = "ScriptableObjects/IntValue")]
    public class IntValue : ScriptableObject
    {
        public event Action OnValueChanged;

        [SerializeField] int baseValue = 0;
        [SerializeField] int value;

        public int BaseValue => baseValue;
        public int Value
        {
            get => value; 

            set
            {
                this.value = value;
                OnValueChanged?.Invoke();
            }
        }
    }
}
