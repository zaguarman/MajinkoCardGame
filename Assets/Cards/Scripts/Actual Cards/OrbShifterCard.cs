using UnityEngine;
using EnumCollection;
using PeggleWars.Orbs;

namespace Cards
{
    internal class OrbShifterCard : Card
    {
        internal int OrbAmount { get; private set; }
        internal OrbType OrbType { get; private set; }

        protected override void CardEffect()
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(GetComponent<RectTransform>().transform.position);            
            OrbManager.Instance.SwitchOrbs(OrbType, worldPosition, OrbAmount);
        }

        protected override void SetCardFields()
        {
            base.SetCardFields();
            ScriptableOrbshifterCard scriptableOrbshifterCard = (ScriptableOrbshifterCard)_scriptableCard;
            OrbType = scriptableOrbshifterCard.OrbType;
            OrbAmount = scriptableOrbshifterCard.OrbAmount;
        }
    }
}
