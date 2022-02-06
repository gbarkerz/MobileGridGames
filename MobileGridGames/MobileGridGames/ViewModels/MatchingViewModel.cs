using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using MobileGridGames.Services;
using System.IO;
using System.Reflection;

namespace MobileGridGames.ViewModels
{
    public class Card : INotifyPropertyChanged
    {
        public int Index { get; set; }

        private string accessibleName;
        public string AccessibleName
        {
            get
            {
                string name = "";
                if (faceUp)
                {
                    name = (Matched ? "Matched " : "") + accessibleName;
                }
                else
                {
                    name = "Face down";
                }

                return name;
            }
            set
            {
                SetProperty(ref accessibleName, value);
            }
        }

        private string accessibleDescription;
        public string AccessibleDescription
        {
            get
            {
                return (FaceUp ? accessibleDescription : "");
            }
            set
            {
                SetProperty(ref accessibleDescription, value);
            }
        }

        private bool faceUp;
        public bool FaceUp
        {
            get
            {
                return faceUp;
            }
            set
            {
                SetProperty(ref faceUp, value);

                // Other properties may change as a result of this.
                OnPropertyChanged("AccessibleName");
                OnPropertyChanged("AccessibleDescription");
            }
        }

        private bool matched;
        public bool Matched
        {
            get
            {
                return matched;
            }
            set
            {
                SetProperty(ref matched, value);

                // Other properties may change as a result of this.
                OnPropertyChanged("AccessibleName");
            }
        }

        private ImageSource pictureImageSource;
        public ImageSource PictureImageSource
        {
            get
            {
                return pictureImageSource;
            }
            set
            {
                SetProperty(ref pictureImageSource, value);
            }
        }

        private bool playSoundOnMatch;
        public bool PlaySoundOnMatch
        {
            get { return playSoundOnMatch; }
            set
            {
                SetProperty(ref playSoundOnMatch, value);
            }
        }

        private bool playSoundOnNotMatch;
        public bool PlaySoundOnNotMatch
        {
            get { return playSoundOnNotMatch; }
            set
            {
                SetProperty(ref playSoundOnNotMatch, value);
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

    public class MatchingViewModel : BaseViewModel
    {
        private Card firstCardInMatchAttempt;
        private Card secondCardInMatchAttempt;

        public int TryAgainCount { get; set; }
        public bool PlaySoundOnMatch { get; set; }
        public bool PlaySoundOnNotMatch { get; set; }
        private string soundOnMatch = "SoundOnMatch.m4a";
        private string soundOnNotMatch = "SoundOnNotMatch.m4a";

        public MatchingViewModel()
        {
            Title = "Matching Game V1.0";

            TryAgainCount = 0;

            this.SetupDefaultMatchingCardList();

            var shuffler = new Shuffler();
            shuffler.Shuffle(squareList);
        }

        private ImageSource GetImageSourceForCard(string cardName)
        {
            return ImageSource.FromResource(
                "MobileGridGames.Resources.DefaultMatchingBackgrounds." + cardName + ".jpg");
        }

        private void SetupDefaultMatchingCardList()
        {
            var resManager = Resource1.ResourceManager;

            // Note: This app assumes the total count of cards is 16.
            squareList = new ObservableCollection<Card>()
            {
                new Card {
                    Index = 1,
                    AccessibleName = resManager.GetString("DefaultMatchingCard1Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard1Description"),
                    PictureImageSource = GetImageSourceForCard("Card1") },
                new Card {
                    Index = 2,
                    AccessibleName = resManager.GetString("DefaultMatchingCard1Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard1Description"),
                    PictureImageSource = GetImageSourceForCard("Card1") },
                new Card {
                    Index = 3,
                    AccessibleName = resManager.GetString("DefaultMatchingCard2Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard2Description"),
                    PictureImageSource = GetImageSourceForCard("Card2") },
                new Card {
                    Index = 4,
                    AccessibleName = resManager.GetString("DefaultMatchingCard2Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard2Description"),
                    PictureImageSource = GetImageSourceForCard("Card2") },
                new Card {
                    Index = 5,
                    AccessibleName = resManager.GetString("DefaultMatchingCard3Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard3Description"),
                    PictureImageSource = GetImageSourceForCard("Card3") },
                new Card {
                    Index = 6,
                    AccessibleName = resManager.GetString("DefaultMatchingCard3Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard3Description"),
                    PictureImageSource = GetImageSourceForCard("Card3") },
                new Card {
                    Index = 7,
                    AccessibleName = resManager.GetString("DefaultMatchingCard4Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard4Description"),
                    PictureImageSource = GetImageSourceForCard("Card4") },
                new Card {
                    Index = 8,
                    AccessibleName = resManager.GetString("DefaultMatchingCard4Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard4Description"),
                    PictureImageSource = GetImageSourceForCard("Card4") },
                new Card {
                    Index = 9,
                    AccessibleName = resManager.GetString("DefaultMatchingCard5Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard5Description"),
                    PictureImageSource = GetImageSourceForCard("Card5") },
                new Card {
                    Index = 10,
                    AccessibleName = resManager.GetString("DefaultMatchingCard5Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard5Description"),
                    PictureImageSource = GetImageSourceForCard("Card5") },
                new Card {
                    Index = 11,
                    AccessibleName = resManager.GetString("DefaultMatchingCard6Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard6Description"),
                    PictureImageSource = GetImageSourceForCard("Card6") },
                new Card {
                    Index = 12,
                    AccessibleName = resManager.GetString("DefaultMatchingCard6Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard6Description"),
                    PictureImageSource = GetImageSourceForCard("Card6") },
                new Card {
                    Index = 13,
                    AccessibleName = resManager.GetString("DefaultMatchingCard7Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard7Description"),
                    PictureImageSource = GetImageSourceForCard("Card7") },
                new Card {
                    Index = 14,
                    AccessibleName = resManager.GetString("DefaultMatchingCard7Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard7Description"),
                    PictureImageSource = GetImageSourceForCard("Card7") },
                new Card {
                    Index = 15,
                    AccessibleName = resManager.GetString("DefaultMatchingCard8Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard8Description"),
                    PictureImageSource = GetImageSourceForCard("Card8") },
                new Card {
                    Index = 16,
                    AccessibleName = resManager.GetString("DefaultMatchingCard8Name"),
                    AccessibleDescription = resManager.GetString("DefaultMatchingCard8Description"),
                    PictureImageSource = GetImageSourceForCard("Card8") },
            };
        }

        public int MoveCount { get; set; }

        private ObservableCollection<Card> squareList;
        public ObservableCollection<Card> SquareListCollection
        {
            get { return squareList; }
            set { this.squareList = value; }
        }

        public bool AttemptToTurnOverSquare(int squareIndex)
        {
            bool gameIsWon = false;

            // If we've already turned over two not-matching cards, turn them back now.
            if (secondCardInMatchAttempt != null)
            {
                ++TryAgainCount;

                firstCardInMatchAttempt.FaceUp = false;
                firstCardInMatchAttempt = null;

                secondCardInMatchAttempt.FaceUp = false;
                secondCardInMatchAttempt = null;

                RaiseNotificationEvent("Unmatched cards turned back.");

                return false;
            }

            // Take no action if the click is on a cell that's already face-up.
            var card = squareList[squareIndex];
            if (card.FaceUp)
            {
                return false;
            }

            // Future: Make appropriate screen reader announcements below.

            // Is this the first card turned over in an attempt to find a match?
            if (firstCardInMatchAttempt == null)
            {
                firstCardInMatchAttempt = card;
                TurnUpCard(card);
            }
            else
            {
                // This must be the second card turned over in an attempt to find a match.
                secondCardInMatchAttempt = card;
                TurnUpCard(card);

                // Has a match been found?
                var cardNameFirst = firstCardInMatchAttempt.AccessibleName;
                var cardNameSecond = secondCardInMatchAttempt.AccessibleName;

                if (cardNameFirst == cardNameSecond)
                {
                    // We have a match!
                    firstCardInMatchAttempt.Matched = true;
                    secondCardInMatchAttempt.Matched = true;

                    RaiseNotificationEvent("That's a match.");

                    firstCardInMatchAttempt = null;
                    secondCardInMatchAttempt = null;

                    PlayCardMatchSound(true);

                    // Has the game been won?
                    gameIsWon = GameIsWon();
                }
                else
                {
                    PlayCardMatchSound(false);
                }
            }

            return gameIsWon;
        }

        public bool AttemptTurnUpBySelection(object currentSelection)
        {
            if (currentSelection == null)
            {
                return false;
            }

            Card selectedCard = currentSelection as Card;

            int currentSelectionIndex = -1;
            for (int i = 0; i < 16; ++i)
            {
                if (squareList[i] == selectedCard)
                {
                    currentSelectionIndex = i;
                    break;
                }
            }

            if (currentSelectionIndex < 0)
            {
                return false;
            }

            return AttemptToTurnOverSquare(currentSelectionIndex);
        }

        private void PlayCardMatchSound(bool cardsMatch)
        {
            if ((PlaySoundOnMatch && cardsMatch) || (PlaySoundOnNotMatch && !cardsMatch))
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                Stream audioStream = assembly.GetManifestResourceStream(
                    "MobileGridGames.Resources." +
                        (cardsMatch ? soundOnMatch : soundOnNotMatch));

                var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                player.Load(audioStream);
                player.Play();
            }
        }

        private void TurnUpCard(Card card)
        {
            card.FaceUp = true;

            RaiseNotificationEvent("Turned up " + card.AccessibleName);
        }

        private bool GameIsWon()
        {
            for (int i = 0; i < this.squareList.Count; i++)
            {
                if (!this.squareList[i].Matched)
                {
                    return false;
                }
            }

            return true;
        }

        public void ResetGrid()
        {
            TryAgainCount = 0;

            firstCardInMatchAttempt = null;
            secondCardInMatchAttempt = null;

            for (int i = 0; i < this.squareList.Count; i++)
            {
                this.squareList[i].FaceUp = false;
                this.squareList[i].Matched = false;
            }

            var shuffler = new Shuffler();
            shuffler.Shuffle(squareList);
        }

        public class Shuffler
        {
            private readonly Random random;

            public Shuffler()
            {
                this.random = new Random();
            }

            public void Shuffle(ObservableCollection<Card> collection)
            {
                for (int i = collection.Count; i > 1;)
                {
                    int j = this.random.Next(i);

                    --i;

                    Card temp = collection[i];
                    collection[i] = collection[j];
                    collection[j] = temp;
                }
            }
        }
    }
}
