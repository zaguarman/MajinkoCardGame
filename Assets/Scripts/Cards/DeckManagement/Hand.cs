using System.Collections.Generic;
using UnityEngine;
using PeggleWars.TurnManagement;
using PeggleWars.Audio;
using System.Collections;
using EnumCollection;
using PeggleWars.Utilities;
using System.Linq;

namespace PeggleWars.Cards
{
    internal class Hand : MonoBehaviour
    {
        #region Fields and Properties

        internal static Hand Instance { get; private set; }

        [SerializeField] private List<Card> _handCards = new();
        internal List<Card> HandCards { get => _handCards; set => _handCards = value; }

        internal int DrawAmount { get; set; } = 5;

        private List<Card> _instantiatedCards = new();
        internal List<Card> InstantiatedCards { get => _instantiatedCards; set => _instantiatedCards = value; }

        private Deck _deck;

        private Transform _parentTransform;

        private Canvas _cardCanvas; //The Card Canvas will be a child of the Hand and contain the UI of instantiated Cards
        #endregion

        #region Functions

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _deck = Deck.Instance;
            _cardCanvas = GetComponentInChildren<Canvas>();
            _parentTransform = _cardCanvas.transform;

            TurnManager.Instance.EndCardTurn?.AddListener(OnCardTurnEnd);
            TurnManager.Instance.StartCardTurn?.AddListener(OnCardTurnStart);
            WinLoseConditionManager.Instance.LevelVictory?.AddListener(OnLevelVictory);
            CardEvents.Instance.CardDestructionEvent?.AddListener(OnCardDestructionWrap);
        }

        internal void OnLevelVictory()
        {
            TurnManager.Instance.StartCardTurn?.RemoveListener(OnCardTurnStart);
            TurnManager.Instance.EndCardTurn?.RemoveListener(OnCardTurnEnd);
        }

        private void OnDisable()
        {
            WinLoseConditionManager.Instance.LevelVictory?.RemoveListener(OnLevelVictory);
        }

        internal void OnCardTurnStart()
        {
            DrawHand(DrawAmount);
        }

        internal void OnCardTurnEnd()
        {
            foreach (Card card in _handCards)
            {
                _deck.DiscardPile.Add(card);
            }

            _handCards.Clear();
            DisplayHand();

            DrawAmount = 5;
        }

        internal void DrawHand(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Card card = _deck.DrawCard();
                if (card != null)
                {
                    HandCards.Add(card);
                }
            }

            AudioManager.Instance.PlaySoundEffectOnce(SFX._0012_DrawHand);
            DisplayHand();
        }

        //basically makes a new set of displayed instantiated cards for each card in the _handCards list
        internal void DisplayHand()
        {
            foreach (Card card in _instantiatedCards)
            {
                Destroy(card.gameObject);
            }

            _instantiatedCards.Clear();

            foreach (Card card in _handCards)
            {
                Card cardObject = Instantiate(card, _parentTransform);
                cardObject.gameObject.SetActive(false);
                _instantiatedCards.Add(cardObject);
            }
            AlignCards(true);
        }

        internal void AlignCards(bool isStartTurnDealing = false)
        {
            if(_instantiatedCards.Count > 0)
            {
                float canvasHeight = _cardCanvas.GetComponent<RectTransform>().rect.height;
                float cardHeight = _instantiatedCards[0].GetComponent<RectTransform>().rect.height;
                float xOffset = 100;
                float yOffset = 10;
                float yPosition = ((-canvasHeight / 2) + (cardHeight / 3));

                //if the number of cards is even
                if (_instantiatedCards.Count % 2 == 0)
                {
                    int offsetCounterHelper = 0;
                    float offsetCounter = 1.5f;
                    List<Vector2> newCardPositions = new();

                    for (int i = 0; i < _instantiatedCards.Count; i++)
                    {
                        if (offsetCounterHelper >= 2)
                        {
                            offsetCounter++;
                            offsetCounterHelper = 0;
                        }

                        if (i == 0)
                        {
                            float angleOffsetCounter = offsetCounter - 0.5f;
                            float angleAdjustedYOffset = angleOffsetCounter * (yOffset + ((angleOffsetCounter - 1) * (yOffset / 2)));
                            Vector2 cardPosition = new((xOffset / 2), yPosition - angleAdjustedYOffset);
                            newCardPositions.Add(cardPosition);
                        }
                        else if (i == 1)
                        {
                            float angleOffsetCounter = offsetCounter - 0.5f;
                            float angleAdjustedYOffset = angleOffsetCounter * (yOffset + ((angleOffsetCounter - 1) * (yOffset / 2)));
                            Vector2 cardPosition = new((-xOffset / 2), yPosition - angleAdjustedYOffset);
                            newCardPositions.Add(cardPosition);
                        }
                        else if (i % 2 == 0)
                        {
                            float angleOffsetCounter = offsetCounter + 0.5f;
                            float angleAdjustedYOffset = angleOffsetCounter * (yOffset + ((angleOffsetCounter - 1) * (yOffset / 2)));
                            Vector2 cardPosition = new((offsetCounter * xOffset), yPosition - angleAdjustedYOffset);
                            newCardPositions.Add(cardPosition);
                            offsetCounterHelper += 1;
                        }
                        else
                        {
                            float angleOffsetCounter = offsetCounter + 0.5f;
                            float angleAdjustedYOffset = angleOffsetCounter * (yOffset + ((angleOffsetCounter - 1) * (yOffset / 2)));
                            Vector2 cardPosition = new((-offsetCounter * xOffset), yPosition - angleAdjustedYOffset);
                            newCardPositions.Add(cardPosition);
                            offsetCounterHelper += 1;
                        }
                    }
                    List<Vector2> sortedCardPositions = newCardPositions.OrderBy(vector => vector.x).ToList<Vector2>();
                    StartCoroutine(FadeInCards(sortedCardPositions, isStartTurnDealing));
                }
                else //if the number of cards is uneven
                {
                    int offsetCounterHelper = 0;
                    float offsetCounter = 1;
                    List<Vector2> newCardPositions = new();

                    for (int i = 0; i < _instantiatedCards.Count; i++)
                    {
                        if (offsetCounterHelper >= 2)
                        {
                            offsetCounter++;
                            offsetCounterHelper = 0;
                        }
                        float angleAdjustedYOffset = offsetCounter * (yOffset + ((offsetCounter - 1) * (yOffset / 2)));

                        if (i == 0)
                        {
                            Vector2 cardPosition = new(0, yPosition);
                            newCardPositions.Add(cardPosition);
                        }
                        else if (i % 2 == 0)
                        {
                            Vector2 cardPosition = new((offsetCounter * xOffset), yPosition - angleAdjustedYOffset);
                            newCardPositions.Add(cardPosition);
                            offsetCounterHelper += 1;
                        }
                        else
                        {
                            Vector2 cardPosition = new((-offsetCounter * xOffset), yPosition - angleAdjustedYOffset);
                            newCardPositions.Add(cardPosition);
                            offsetCounterHelper += 1;
                        }
                    }
                    List<Vector2> sortedCardPositions = newCardPositions.OrderBy(vector => vector.x).ToList<Vector2>();
                    StartCoroutine(FadeInCards(sortedCardPositions, isStartTurnDealing));
                }
                SetCardAngles();
            }
        }

        private IEnumerator FadeInCards(List<Vector2> newCards, bool hasVisualDelay)
        {           
            for (int i = 0; i < _instantiatedCards.Count; i++)
            {               
                _instantiatedCards[i].gameObject.SetActive(true);
                RectTransform rectTransform = _instantiatedCards[i].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = newCards[i];
                if (hasVisualDelay)
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
            yield return null;
        }

        private void OnCardDestructionWrap()
        {
            StartCoroutine(OnCardDestruction());
        }

        private IEnumerator OnCardDestruction()
        {
            yield return new WaitForSeconds(0.1f);
            AlignCards();
        }

        private void SetCardAngles()
        {
            List<int> cardAngles = new();
           

            //uneven amount cards
            if (_instantiatedCards.Count % 2 != 0)
            {
                int angleStepSize = 5;
                int minimumAngle = (_instantiatedCards.Count / 2) * angleStepSize;
                for (int i = 0; i < _instantiatedCards.Count; i++)
                {
                    cardAngles.Add(minimumAngle - (i * angleStepSize));
                }
            }
            else
            {
                int angleStepSize = 5;
                int minimumAngle = (_instantiatedCards.Count / 2) * angleStepSize;
                //no center card -> no 0 angle, we need to skip at this index
                int doubleStepIndex = _instantiatedCards.Count / 2;
                for (int i = 0; i < _instantiatedCards.Count; i++)
                {
                    if (i >= doubleStepIndex)
                    {
                        cardAngles.Add(minimumAngle - ((i + 1) * angleStepSize));
                    }
                    else
                    {
                        cardAngles.Add(minimumAngle - (i * angleStepSize));
                    }
                }
            }

            for (int i = 0; i < _instantiatedCards.Count; i++)
            {
                Card card = _instantiatedCards[i];
                int zAngleOffset = cardAngles[i];
                Vector3 newAngle = new(0, 0, zAngleOffset);
                card.GetComponent<RectTransform>().eulerAngles = newAngle;
            }
        }






        #endregion
    }
}
