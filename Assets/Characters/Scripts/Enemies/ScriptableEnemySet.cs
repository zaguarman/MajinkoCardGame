using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumCollection;

namespace Enemies
{
    [CreateAssetMenu]
    internal class ScriptableEnemySet : ScriptableObject
    {
        [SerializeField] private EnemyType[] _enemyArray;
        internal EnemyType[] EnemyArray { get => _enemyArray; }
    }
}