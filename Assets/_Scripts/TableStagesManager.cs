using System;
using System.Collections.Generic;
using _Scripts.Data;
using _Scripts.Enums;
using _Scripts.Helpers;
using UnityEngine;

namespace _Scripts
{
	public class TableStagesManager : MonoBehaviour
	{
		[SerializeField] private PokerManager _pokerManager;
		[SerializeField] private GameObject[] _tableCardPos;
		[SerializeField] private CardComponent _cardPrefab;

		private readonly List<CardData> _tableCards = new();
		private readonly List<CardComponent> _tableCardsComponent = new();
		public TableStage CurrentStage { get; private set; } = TableStage.PreFlop;
		
		private void Start()
		{
			for (var i = 0; i < 5; i++)
			{
				var templateCards = Instantiate(_cardPrefab, _tableCardPos[i].transform);

				templateCards.SetDefaultCardImage();
				templateCards.gameObject.SetActive(false);
				
				_tableCardsComponent.Add(templateCards);
			}
		}

		public void NextTableStage()
		{
			CurrentStage = (int)CurrentStage + 1 >= Enum.GetValues(typeof(TableStage)).Length ? 0 : CurrentStage + 1;
			
			Debug.Log($"New Stage: {CurrentStage}");
			
			switch (CurrentStage)
			{
				case TableStage.PreFlop:
					return;
				
				case TableStage.Flop:
					var flopCards = _pokerManager.DealFlop();
					_tableCards.AddRange(flopCards);
					
					break;
				case TableStage.Turn:
					var turnCard = _pokerManager.DealTurn();
					_tableCards.Add(turnCard);
					
					break;
				case TableStage.River:
					var riverCard = _pokerManager.DealRiver();
					_tableCards.Add(riverCard);
					
					break;
			}
			
			DisplayTableCards();
		}
		
		private void DisplayTableCards()
		{
			var k = 0;
			
			foreach (var card in _tableCards)
			{
				var cardSprite = CardSpritesHelper.GetCardSprite(card);
				
				_tableCardsComponent[k].SetCardImage(cardSprite);
				_tableCardsComponent[k].gameObject.SetActive(true);
				k++;
			}
		}
		
		public void ClearTableCards()
		{
			_tableCards.Clear();
			
			foreach (var cardComponent in _tableCardsComponent)
			{
				cardComponent.SetDefaultCardImage();
				cardComponent.gameObject.SetActive(false);
			}
			
			CurrentStage = TableStage.PreFlop;
		}
	}
}