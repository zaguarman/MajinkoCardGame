using Utility;
using System.Collections;
using UnityEngine;
using Attacks;
using Spheres;
using Characters.Enemies;

namespace Orbs
{
    internal class FireBombOrb : Orb, IAmSphere
    {
        [SerializeField] private Attack _fireBomb;
        [SerializeField] private GameObject _bombRadiusObject;

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<IAmSphere>(out _))
            {
                GetComponent<Collider2D>().enabled = false;
                AdditionalEffectsOnCollision();
                PlayOrbOnHitSound();
                OnCollisionVisualPolish();
                SpawnMana();
                ReplaceHitOrb();
                StartCoroutine(DestroyOrbWithDelay());
            }
        }

        protected override void AdditionalEffectsOnCollision()
        {
            OrbActionManager.Instance.AddOrbToActionList(this);
            _bombRadiusObject.SetActive(true);
        }

        public override void SetDisplayDescription()
        {
            IDisplayOnScroll displayOnScroll = GetComponent<IDisplayOnScroll>();
            displayOnScroll.DisplayDescription = "<size=120%><b>Fire Bomb Sphere</b><size=20%>\n\n<size=100%>Will explode on impact, activating nearby spheres aswell. " +
                "Enables the playes to cast <b>Fire Bomb</b>, hitting all enemies for <b>20 damage</b> and applying <b>5 Burning</b>.";
        }

        internal override IEnumerator OrbEffect()
        {
            _fireBomb.ShootAttack(EnemyManager.Instance.EnemyPositions[0, 0]);
            yield return new WaitForSeconds(0.2f);
        }

        protected override IEnumerator DestroyOrbWithDelay()
        {
            yield return new WaitForSeconds(1);
            Destroy(gameObject);
        }
    }
}
