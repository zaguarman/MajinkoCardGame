using UnityEngine;
using EnumCollection;
using PeggleAttacks.AttackManager;
using PeggleWars.Enemies;
using PeggleWars.Orbs;
using PeggleWars.Characters.Interfaces;
using PeggleWars.Utilities;

namespace PeggleWars.Attacks
{
    internal abstract class Attack : MonoBehaviour, IHaveBark
    {
        #region Fields and Properties

        protected PlayerAttackManager _playerAttackManager;
        protected Collider2D _collision;

        [SerializeField] protected AttackOrigin _attackOrigin;
        [SerializeField] protected int _damage;

        protected readonly string NO_TARGET_PARAM = "No enemy!";
        protected readonly string ATTACK_ANIMATION = "Attack";

        internal int Damage
        {
            get { return _damage; }
            private set { _damage = value; }
        }

        public abstract string Bark { get; }

        #endregion

        #region Functions

        protected virtual void Start()
        {
            _playerAttackManager = PlayerAttackManager.Instance;
            if (_attackOrigin == AttackOrigin.Player)
            {
                _damage = Mathf.RoundToInt(_damage * _playerAttackManager.DamageModifierTurn);
                Player.Instance.GetComponentInChildren<Animator>().SetTrigger(ATTACK_ANIMATION);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            _collision = collision;

            if ((_attackOrigin == AttackOrigin.Player && collision.gameObject.GetComponent<Enemy>() != null)
                || _attackOrigin == AttackOrigin.Enemy && collision.gameObject.GetComponent<Player>() != null)
            {
                IDamagable target = collision.GetComponent<IDamagable>();
                target?.TakeDamage(_damage);
                OnHitPolish();
                AdditionalEffectsOnImpact();
                if (_attackOrigin == AttackOrigin.Player) { OrbEvents.Instance.OrbEffectEnd?.Invoke(); }
                DestroyGameObject();
            }
        }

        internal abstract void ShootAttack(Vector3 instantiatePosition);

        protected abstract void AdditionalEffectsOnImpact();

        protected abstract void OnHitPolish();

        protected virtual void DestroyGameObject()
        {
            Destroy(gameObject);
        }

        #endregion

        internal enum AttackOrigin
        {
            Player,
            Enemy,
        }
    }
}
