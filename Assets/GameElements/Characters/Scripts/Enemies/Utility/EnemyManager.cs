using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Characters.Enemies
{
    [RequireComponent(typeof(EnemySpawnManager))]
    [RequireComponent(typeof(EnemyTurnMovement))]
    public class EnemyManager : MonoBehaviour
    {
        #region Fields and Properties

        public static EnemyManager Instance { get; private set; }
        public List<Enemy> EnemiesInScene { get; set; } = new();
        public Vector3[,] EnemyPositions { get; private set; }

        //Fields to calculate EnemyPositions regardless of screen size
        private readonly int _amountOfXScreenDivisions = 10;
        private readonly int _amountOfCharacterPositionsOnXAxis = 6;
        private readonly int _amountOfEnemyRows = 2;

        #endregion

        #region Functions

        private void Awake()
        {       
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            //needs to be in Awake for timing issues
            EnemyPositions = new Vector3[_amountOfEnemyRows, _amountOfCharacterPositionsOnXAxis];
            SetEnemyPositions();
        }

        private void SetEnemyPositions()
        {
            Camera camera = Camera.main;
            int cellHeight = Screen.height / 10;
            float yUpperRow = Screen.height - cellHeight / 2;
            float cellWidth = (float)Screen.width / _amountOfXScreenDivisions;
            float xPositionOffset = 3;

            for (int x = 0; x < _amountOfCharacterPositionsOnXAxis; x++)
            {
                for (int y = 0; y < _amountOfEnemyRows; y++)
                {
                    if (y == 0)
                    {
                        Vector2 possiblePositionOnX = new((x + xPositionOffset) * cellWidth, 0);
                        Vector2 possibleWorldPositionOnX = camera.ScreenToWorldPoint(possiblePositionOnX);
                        Vector2 possibleWorldPosition = new(possibleWorldPositionOnX.x, 7.74f);
                        EnemyPositions[y, x] = new Vector3(possibleWorldPosition.x, possibleWorldPosition.y, -1);
                    }
                    else
                    {
                        //was supposed to be flying units - which I don't think I'll be doing
                        //deprecated, if you want to use this set a static Y position instead of calculating it dynamically
                        Vector2 possibleCharacterPositionOnScreen = new((x + xPositionOffset) * cellWidth, yUpperRow);
                        Vector2 possibleCharacterPositionAsWorldPoint = camera.ScreenToWorldPoint(possibleCharacterPositionOnScreen);
                        EnemyPositions[y, x] = new Vector3(possibleCharacterPositionAsWorldPoint.x, possibleCharacterPositionAsWorldPoint.y, -1);
                    }
                }
            }
        }

        #endregion
    }

    public enum EnemyAttackType
    {
        Melee,
        Ranged,
    }
}
