using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;

using MobileGridGames.ResX;

namespace MobileGridGames.ViewModels
{
    public class WheresCard : INotifyPropertyChanged
    {
        public int Index { get; set; }

        private string accessibleName;
        public string AccessibleName
        {
            get
            {
                string name = "";

                if (Index == 15)
                {
                    name = "Get a tip";
                }
                else
                {
                    name = WCAGNumber;

                    if (isFound)
                    {
                        name += " " + WCAGName;
                    }
                }

                return name;
            }
            set
            {
                SetProperty(ref accessibleName, value);
            }
        }

        public string WCAGName { get; set; }

        private string wcagNumber;
        public string WCAGNumber
        {
            get
            {
                return wcagNumber;
            }
            set
            {
                SetProperty(ref wcagNumber, value);
            }
        }

        private bool isFound;
        public bool IsFound
        {
            get
            {
                return isFound;
            }
            set
            {
                SetProperty(ref isFound, value);

                // Other properties may change as a result of this.
                OnPropertyChanged("AccessibleName");
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class WheresViewModel : BaseViewModel
    {
        // Barker: Unify the use of the two arrays here.

        private string[] wcagNames =
        {
            "Perceivable",
            "Text Alternatives",
            "Time-based Media",
            "Adaptable",
            "Distinguishable",

            "Operable",
            "Keyboard Accessible",
            "Enough Time",
            "Seizures and Physical Reactions",
            "Navigable",
            "Input Modalities",

            "Understandable",
            "Readable",
            "Predictable",
            "Input Assistance"
        };

        string[] wcagNumbers =
        {
            "1",
            "1.1",
            "1.2",
            "1.3",
            "1.4",

            "2",
            "2.1",
            "2.2",
            "2.3",
            "2.4",
            "2.5",

            "3",
            "3.1",
            "3.2",
            "3.3",
        };

        public WheresViewModel()
        {
            Title = AppResources.ResourceManager.GetString("Wheres");

            wheresList = new ObservableCollection<WheresCard>();

            TryAgainCount = 0;
        }

        public int TryAgainCount { get; set; }
        public int CurrentQuestionIndex { get; set; }

        private string currentQuestionWCAG;
        public string CurrentQuestionWCAG
        {
            get
            {
                return currentQuestionWCAG;
            }
            set
            {
                SetProperty(ref currentQuestionWCAG, value);
            }
        }


        private bool firstRunWheres = true;
        public bool FirstRunWheres
        {
            get
            {
                return firstRunWheres;
            }
            set
            {
                if (firstRunWheres != value)
                {
                    SetProperty(ref firstRunWheres, value);

                    Preferences.Set("FirstRunWheres", firstRunWheres);
                }
            }
        }

        public void SetupDefaultWheresCardList()
        {
            var resManager = AppResources.ResourceManager;

            wheresList.Clear();

            // We have 8 pairs of cards.
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    // The card index runs from 0 to 15.
                    var cardIndex = (i * 2) + j;

                    wheresList.Add(
                        new WheresCard
                        {
                            Index = cardIndex,
                            WCAGNumber = (cardIndex < 15 ? wcagNumbers[cardIndex] : "?"),
                            WCAGName = (cardIndex < 15 ? wcagNames[cardIndex] : "Tip")
                        });
                }
            }

            var shuffler = new Shuffler();
            shuffler.Shuffle(wcagNames);

            CurrentQuestionIndex = 0;
            CurrentQuestionWCAG = wcagNames[CurrentQuestionIndex];
        }

        public void SetupCustomWheresCardList(Collection<WheresCard> cards)
        {
            wheresList.Clear();

            for (int i = 0; i < cards.Count; i++)
            {
                wheresList.Add(cards[i]);
            }
        }

        private ObservableCollection<WheresCard> wheresList;
        public ObservableCollection<WheresCard> WheresListCollection
        {
            get { return wheresList; }
            set { this.wheresList = value; }
        }

        public bool AttemptToAnswerQuestion(int squareIndex)
        {
            bool gameIsWon = false;

            // Take no action if the click is on a cell that's already face-up.
            var card = wheresList[squareIndex];
            if (card.IsFound)
            {
                return false;
            }

            // Does this card's WCAG match the current question?
            if (card.WCAGName == currentQuestionWCAG)
            {
                card.IsFound = true;

                // Has the game been won?
                gameIsWon = GameIsWon();
                if (!gameIsWon)
                {
                    ++CurrentQuestionIndex;

                    CurrentQuestionWCAG = wcagNames[CurrentQuestionIndex];

                    var message = String.Format(
                        AppResources.ResourceManager.GetString("CorrectWCAG"),
                        card.WCAGName, CurrentQuestionWCAG);

                    RaiseNotificationEvent(message);
                }
            }
            else if (card.WCAGName != "Tip")
            {
                ++TryAgainCount;

                var message = String.Format(
                    AppResources.ResourceManager.GetString("IncorrectWCAG"),
                    card.WCAGNumber, CurrentQuestionWCAG);

                RaiseNotificationEvent(message);
            }

            return gameIsWon;
        }

        public bool AttemptTurnUpBySelection(object currentSelection)
        {
            if (currentSelection == null)
            {
                return false;
            }

            WheresCard selectedCard = currentSelection as WheresCard;

            int currentSelectionIndex = -1;
            for (int i = 0; i < 16; ++i)
            {
                if (wheresList[i] == selectedCard)
                {
                    currentSelectionIndex = i;
                    break;
                }
            }

            if (currentSelectionIndex < 0)
            {
                return false;
            }

            return AttemptToAnswerQuestion(currentSelectionIndex);
        }

        private bool GameIsWon()
        {
            for (int i = 0; i < this.wheresList.Count - 1; i++)
            {
                if (!this.wheresList[i].IsFound)
                {
                    return false;
                }
            }

            return true;
        }

        public void ResetGrid(bool shuffle)
        {
            TryAgainCount = 0;

            if (shuffle)
            {
                var shuffler = new Shuffler();
                shuffler.Shuffle(wcagNames);
            }

            CurrentQuestionIndex = 0;
            CurrentQuestionWCAG = wcagNames[CurrentQuestionIndex];

            for (int i = 0; i < this.wheresList.Count; i++)
            {
                this.wheresList[i].IsFound = false;
            }
        }

        public class Shuffler
        {
            private readonly Random random;

            public Shuffler()
            {
                this.random = new Random();
            }

            public void Shuffle(string[] titles)
            {
                for (int i = titles.Length; i > 1;)
                {
                    int j = this.random.Next(i);

                    --i;

                    string temp = titles[i];
                    titles[i] = titles[j];
                    titles[j] = temp;
                }
            }
        }
    }
}
