using System;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _gameStateText;
        [SerializeField] private CanvasGroup _previewCanvasGroup;
        [SerializeField] private TMP_Text _previewNameText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _spriteImage;
        [SerializeField] private Image _consumptionImage;
        [SerializeField] private Image _badHabitImage;
        [SerializeField] private Image _secondBadHabitImage;

        [SerializeField] private TMP_Text _hostelConditions;
        [SerializeField] private TMP_Text _barConditionText;
        [SerializeField] private Image _barConditionImage;

        [SerializeField] private TMP_Text _leaveEffect;
        
        private void Start()
        {
            _previewCanvasGroup.alpha = 0;
            
            SetPreview(false);
        }

        public void SetGameStateText(string text)
        {
            _gameStateText.text = text;
        }

        public void SetPreview(bool doShow, CardData data, Color color)
        {
            _backgroundImage.color = color;
            
            SetPreview(doShow, data);
        }
        
        public void SetPreview(bool doShow, CardData data = null)
        {
            _previewCanvasGroup.DOKill();
            _previewCanvasGroup.DOFade(doShow ? 1 : 0, 0.2f);

            if (doShow == false || data == null) return;

            _spriteImage.sprite = data.Sprite;
            
            _previewNameText.text = data.Name;
            _badHabitImage.sprite = GameManager.Instance.SpritesData.GetBadHabitSprite(data.BadHabit);
            if (data.BadHabitAmount > 1)
            {
                _secondBadHabitImage.gameObject.SetActive(true);
                _secondBadHabitImage.sprite = GameManager.Instance.SpritesData.GetBadHabitSprite(data.BadHabit);
            }
            else
            {
                _secondBadHabitImage.gameObject.SetActive(false);
            }
            _consumptionImage.sprite = GameManager.Instance.SpritesData.GetConsumptionSprite(data.Consumption);

            if (data.BarCondition != BadHabitType.None)
            {
                _barConditionText.gameObject.SetActive(true);
                _barConditionImage.gameObject.SetActive(true);
                _barConditionImage.sprite = GameManager.Instance.SpritesData.GetBadHabitSprite(data.BarCondition);
            }
            else
            {
                _barConditionText.gameObject.SetActive(false);
                _barConditionImage.gameObject.SetActive(false);
            }
            
            string hostelConditions = string.Empty;
            foreach (var condition in data.HostelConditions)
            {
                if (condition.Type == ConditionType.DrinkComparedToFoodInHostel || condition.Type == ConditionType.DrinkComparedToFoodOnBar)
                {
                    string place = condition.Type == ConditionType.DrinkComparedToFoodInHostel ? "hostel" : "bar";
                    if (condition.Operator == Operator.Less)
                    {
                        hostelConditions += $"Less drink than food in {place}\n";
                    }
                    else if (condition.Operator == Operator.Greater)
                    {
                        hostelConditions += $"More drink than food in {place}\n";
                    }
                    else if (condition.Operator == Operator.Equal)
                    {
                        hostelConditions += $"Equal drink and food in {place}\n";
                    }
                    continue;
                }
                hostelConditions += $"{GetStringForCondition(condition.Type)} {GetStringForOperator(condition.Operator)} {condition.Value}\n";
            }
            _hostelConditions.text = hostelConditions;
            
            _leaveEffect.text = GetLeaveEffectString(data);
        }

        private static string GetStringForCondition(ConditionType condition)
        {
            return condition switch
            {
                ConditionType.DrinkOnBar => "Drinks in bar",
                ConditionType.FoodOnBar => "Foods in bar",
                ConditionType.DrinkInHostel => "Drinks in hostel",
                ConditionType.FoodInHostel => "Foods in hostel",
                ConditionType.GoblinInHostel => "Goblins in hostel",
                ConditionType.DrinkUpOrDown => "Drinks in neighbor rooms",
                ConditionType.DrinkUpAndDown => "Drinks in both neighbor rooms",
                ConditionType.FoodUpOrDown => "Foods in neighbor rooms",
                ConditionType.FoodUpAndDown => "Foods in both neighbor rooms",
                ConditionType.LoudUpAndDown => "Noise in both neighbor rooms",
                ConditionType.LoudUpOrDown => "Noise in neighbor rooms",
                ConditionType.DrinkComparedToFoodInHostel => "Drink compared to food in hostel",
                ConditionType.DrinkComparedToFoodOnBar => "Drink compared to food in bar",
                ConditionType.FloorInHostel => "Hostel floor",
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
            };
        }
        
        private static string GetStringForOperator(Operator @operator)
        {
            return @operator switch
            {
                Operator.Less => "<",
                Operator.LessOrEqual => "<=",
                Operator.Equal => "==",
                Operator.GreaterOrEqual => ">=",
                Operator.Greater => ">",
                Operator.First => "is first",
                Operator.BeforeLast => "is pre-last",
                Operator.Last => "is last",
                _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null)
            };
        }
        
        private static string GetLeaveEffectString(CardData data)
        {
            return data.LeaveWith switch
            {
                LeaveWithCondition.Nobody => "Leave",
                LeaveWithCondition.AllGoblins => "Leave with all goblins",
                LeaveWithCondition.ClientBeneath => "Make client beneath leave",
                LeaveWithCondition.ClientBeneath2Floor => "Make client beneath 2 floor leave and activate his leave effect",
                LeaveWithCondition.ThoseWhoDontLikeFight => "Make those who don't like fight leave",
                LeaveWithCondition.ThoseWhoDontLikeLoud => "Make those who don't like loud leave",
                LeaveWithCondition.ThoseWhoDontLikeSmell => "Make those who don't like smell leave",
                LeaveWithCondition.DrawCardFromTankAndSwitch => "Take client from tank and switch it with this client",
                LeaveWithCondition.Take3ClientBeneathAndReplaceThem => "Take the 3 clients beneath and replace them",
                LeaveWithCondition.ShuffleBarAndPlaceBackToTank => "Shuffle bar clients and place back to tank",
                LeaveWithCondition.DrawFromTankToTopOfHostel => "Draw client from tank to top of hostel",
                LeaveWithCondition.DrawFromBarToTopOfHostel => "Draw client from bar to top of hostel",
                LeaveWithCondition.DrawFirstInBarToCardHostelFlor => "Draw first client in bar to card hostel floor",
                LeaveWithCondition.DestroyNextCardInTank => "Make next client in tank leave",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}