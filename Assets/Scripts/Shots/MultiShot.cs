using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PeggleWars.Orbs;

namespace PeggleWars.Shots
{
    public class MultiShot : Shot
    {
        #region Fields

        private bool _cannotSpawnMoreShots;

        public bool CannotSpawnMoreShots
        {
            get { return _cannotSpawnMoreShots; }
            set { _cannotSpawnMoreShots = value; }
        }

        protected int _amountOfExtraBalls = 2;

        public int ExtraBalls
        {
            get { return _amountOfExtraBalls; }
            set { _amountOfExtraBalls = value; }
        }

        private int _ballsInScene = 3;

        public int BallsInScene { get => _ballsInScene; set { _ballsInScene = value; } }
        
        private bool _hasHitPortal;

        #endregion

        #region Functions

        protected override void Start()
        {
            base.Start();
            _shotManager.BallDestructionEvent?.AddListener(OnBallDestruction);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _shotManager.BallDestructionEvent.RemoveListener(OnBallDestruction);
        }

        protected void OnBallDestruction()
        {
            _ballsInScene--;

            if (_hasHitPortal)
            {
                if (_ballsInScene <= 0)
                {
                    StartCoroutine(GameManager.Instance.SwitchState(EnumCollection.GameState.PlayerActions));
                }
                Destroy(gameObject);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name.Contains(PORTAL_PARAM) && !_hasHitPortal)
            {
                _hasHitPortal = true;
                _shotManager.BallDestructionEvent?.Invoke();
            }
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);

            if (collision.gameObject.TryGetComponent(out Orb orb) && !_cannotSpawnMoreShots)
            {
                SplitBall(_amountOfExtraBalls, collision.transform);
            }     
        }

        private void SplitBall(int amountOfExtraOrbs, Transform peggleTransform)
        {
            float peggleX = peggleTransform.position.x + 1;
            float calculateAngleY = peggleTransform.position.y + 1;
            _cannotSpawnMoreShots = true;

            for (int i = 1; i < amountOfExtraOrbs + 1; i++) 
            {
                Vector2 shotDestination = new(peggleX, calculateAngleY * i);
                Vector2 pegglePosition = peggleTransform.position;
                Vector2 direction = shotDestination - pegglePosition;
                direction = direction.normalized;         

                MultiShot tempShot = Instantiate(this, new Vector2(peggleTransform.position.x, peggleTransform.position.y), Quaternion.identity);
                tempShot.CannotSpawnMoreShots = true;
                tempShot.BallsInScene = _ballsInScene;
                tempShot.SetShotAsShotAlready();
                StartCoroutine(DisableAndResetCollider(tempShot));

                Rigidbody2D rigidbody = tempShot.GetComponent<Rigidbody2D>();
                rigidbody.gravityScale = _gravity;
                rigidbody.velocity = direction * _shotSpeed;               
            }
        }

        private IEnumerator DisableAndResetCollider(MultiShot tempShot)
        {
            Collider2D collider = tempShot.GetComponent<Collider2D>();
            collider.enabled = false;

            yield return new WaitForSeconds(0.1f);
            collider.enabled = true;
        }

        public override void ShotStackEffect()
        {
            _amountOfExtraBalls++;
            _ballsInScene++;
        }

        protected override void OnShootAdditions()
        {
            //ToDo Sound and other polish
        }
        #endregion

    }
}