using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using MobileGridGames.Services;
using Xamarin.Essentials;
using System.IO;

namespace MobileGridGames.ViewModels
{
    public class SquaresViewModel : BaseViewModel
    {
        public int MoveCount { get; set; }

        private ObservableCollection<Square> squareList;
        public ObservableCollection<Square> SquareListCollection
        {
            get { return squareList; }
            set { this.squareList = value; }
        }

        private int numberHeight;
        public int NumberHeight
        {
            get { return numberHeight; }
            set
            {
                SetProperty(ref numberHeight, value);
            }
        }

        private bool showNumbers;
        public bool ShowNumbers
        {
            get { return showNumbers; }
            set
            {
                SetProperty(ref showNumbers, value);
            }
        }

        private bool showPicture;
        public bool ShowPicture
        {
            get { return showPicture; }
            set
            {
                SetProperty(ref showPicture, value);
            }
        }

        private string picturePathSquares;
        public string PicturePathSquares
        {
            get { return picturePathSquares; }
            set
            {
                SetProperty(ref picturePathSquares, value);
            }
        }

        private bool firstRunSquares = true;
        public bool FirstRunSquares
        {
            get
            {
                return firstRunSquares;
            }
            set
            {
                if (firstRunSquares != value)
                {
                    SetProperty(ref firstRunSquares, value);

                    Preferences.Set("FirstRunSquares", firstRunSquares);
                }
            }
        }

        public SquaresViewModel()
        {
            Title = "Squares";

            squareList = new ObservableCollection<Square>();

            this.CreateDefaultSquares();

            // If we won't be loading pictures into the squares, shuffle them now.
            ShowPicture = Preferences.Get("ShowPicture", false);
            PicturePathSquares = Preferences.Get("PicturePathSquares", "");

            if (!ShowPicture || !IsImageFilePathValid(PicturePathSquares))
            {
                GameIsLoading = false;

                ResetGrid();
            }
        }

        public bool IsImageFilePathValid(string imageFilePath)
        {
            bool ImageFilePathIsValid = false;

            var fileExists = File.Exists(imageFilePath);
            if (fileExists)
            {
                // The ImageEditor documentation states that only png, jpg and bmp formats
                // are supported, so check the extension suggests that the file is supported.
                var extension = Path.GetExtension(imageFilePath).ToLower();
                if ((extension == ".jpg") || (extension == ".png") || (extension == ".bmp"))
                {
                    ImageFilePathIsValid = true;
                }
            }

            return ImageFilePathIsValid;
        }

        public void RestoreEmptyGrid()
        {
            squareList.Clear();
            
            this.CreateDefaultSquares();
        }

        public bool AttemptMoveBySelection(object currentSelection)
        {
            if (currentSelection == null)
            {
                return false;
            }

            Square selectedSquare = currentSelection as Square;

            int currentSelectionIndex = -1;
            for (int i = 0; i < 16; ++i)
            {
                if (squareList[i] == selectedSquare)
                {
                    currentSelectionIndex = i;
                    break;
                }
            }

            if (currentSelectionIndex < 0)
            {
                return false;
            }

            return AttemptToMoveSquare(currentSelectionIndex);
        }

        public bool AttemptToMoveSquare(int SquareIndex)
        {
            bool gameIsWon = false;

            Square adjacentSquare = null;
            int emptySquareIndex = -1;
            string direction = "";

            var resManager = Resource1.ResourceManager;

            // Is the empty square adjacent to this square?

            // Check the square to the left.
            if (SquareIndex % 4 > 0)
            {
                adjacentSquare = squareList[SquareIndex - 1];
                if (adjacentSquare.TargetIndex == 15)
                {
                    emptySquareIndex = SquareIndex - 1;

                    direction = resManager.GetString("Left");
                }
            }

            // Check the square above.
            if ((emptySquareIndex == -1) && (SquareIndex > 3))
            {
                adjacentSquare = squareList[SquareIndex - 4];
                if (adjacentSquare.TargetIndex == 15)
                {
                    emptySquareIndex = SquareIndex - 4;

                    direction = resManager.GetString("Up");
                }
            }

            // Check the square to the right.
            if ((emptySquareIndex == -1) && (SquareIndex % 4 < 3))
            {
                adjacentSquare = squareList[SquareIndex + 1];
                if (adjacentSquare.TargetIndex == 15)
                {
                    emptySquareIndex = SquareIndex + 1;

                    direction = resManager.GetString("Right");
                }
            }

            // Check the square below.
            if ((emptySquareIndex == -1) && (SquareIndex < 12))
            {
                adjacentSquare = squareList[SquareIndex + 4];
                if (adjacentSquare.TargetIndex == 15)
                {
                    emptySquareIndex = SquareIndex + 4;

                    direction = resManager.GetString("Down");
                }
            }

            // Make an announcement regardless of whether a square is moved.
            var service = DependencyService.Get<IMobileGridGamesPlatformAction>();

            // If we found an adjacent empty square, swap the clicked square with the empty square.
            if (emptySquareIndex != -1)
            {
                ++MoveCount;

                var clickedSquareName = squareList[SquareIndex].AccessibleName;

                Square temp = squareList[SquareIndex];
                squareList[SquareIndex] = squareList[emptySquareIndex];
                squareList[emptySquareIndex] = temp;

                // Has the game been won?
                gameIsWon = GameIsWon(squareList);
                if (!gameIsWon)
                {
                    string announcement = resManager.GetString("Moved") +
                        " " + clickedSquareName + " " + direction + ".";
                    service.ScreenReaderAnnouncement(announcement);
                }
            }
            else
            {
                string announcement = resManager.GetString("NoMovePossible");
                service.ScreenReaderAnnouncement(announcement);
            }

            return gameIsWon;
        }

        // Reset the grid to an initial game state.
        public void ResetGrid()
        {
            MoveCount = 0;

            Shuffle(squareList);

            RaiseNotificationEvent("Game is ready to play.");
        }

        private void CreateDefaultSquares()
        {
            var resManager = Resource1.ResourceManager;

            // Future: If feedback suggests that exposing accessible HelpText for each
            // item would be helpful when playing the game, set that here through the
            // AccessibleDescription property.

            // Note: This app assumes the total count of cards is 16.
            squareList.Add(
                new Square
                {
                    TargetIndex = 0,
                    VisualLabel = resManager.GetString("DefaultSquare1Name"),
                    AccessibleName = resManager.GetString("DefaultSquare1Name")
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 1,
                    VisualLabel = resManager.GetString("DefaultSquare2Name"),
                    AccessibleName = resManager.GetString("DefaultSquare2Name")
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 2,
                    VisualLabel = resManager.GetString("DefaultSquare3Name"),
                    AccessibleName = resManager.GetString("DefaultSquare3Name")
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 3,
                    VisualLabel = resManager.GetString("DefaultSquare4Name"),
                    AccessibleName = resManager.GetString("DefaultSquare4Name")
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 4,
                    VisualLabel = resManager.GetString("DefaultSquare5Name"),
                    AccessibleName = resManager.GetString("DefaultSquare5Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 5,
                    VisualLabel = resManager.GetString("DefaultSquare6Name"),
                    AccessibleName = resManager.GetString("DefaultSquare6Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 6,
                    VisualLabel = resManager.GetString("DefaultSquare7Name"),
                    AccessibleName = resManager.GetString("DefaultSquare7Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 7,
                    VisualLabel = resManager.GetString("DefaultSquare8Name"),
                    AccessibleName = resManager.GetString("DefaultSquare8Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 8,
                    VisualLabel = resManager.GetString("DefaultSquare9Name"),
                    AccessibleName = resManager.GetString("DefaultSquare9Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 9,
                    VisualLabel = resManager.GetString("DefaultSquare10Name"),
                    AccessibleName = resManager.GetString("DefaultSquare10Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 10,
                    VisualLabel = resManager.GetString("DefaultSquare11Name"),
                    AccessibleName = resManager.GetString("DefaultSquare11Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 11,
                    VisualLabel = resManager.GetString("DefaultSquare12Name"),
                    AccessibleName = resManager.GetString("DefaultSquare12Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 12,
                    VisualLabel = resManager.GetString("DefaultSquare13Name"),
                    AccessibleName = resManager.GetString("DefaultSquare13Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 13,
                    VisualLabel = resManager.GetString("DefaultSquare14Name"),
                    AccessibleName = resManager.GetString("DefaultSquare14Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 14,
                    VisualLabel = resManager.GetString("DefaultSquare15Name"),
                    AccessibleName = resManager.GetString("DefaultSquare15Name"),
                });

            squareList.Add(
                new Square
                {
                    TargetIndex = 15,
                    VisualLabel = "",
                    AccessibleName = resManager.GetString("DefaultSquareEmpty"),
                });
        }

        private bool GameIsWon(ObservableCollection<Square> collection)
        {
            bool gameIsWon = true;

            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].TargetIndex != i)
                {
                    gameIsWon = false;

                    break;
                }
            }
            
            return gameIsWon;
        }

        public class Square : INotifyPropertyChanged
        {
            public int TargetIndex { get; set; }
            public string VisualLabel { get; set; }
            public string AccessibleName { get; set; }
            public string AccessibleDescription { get; set; }

            private ImageSource pictureImageSource;
            public ImageSource PictureImageSource
            {
                get { return pictureImageSource; }
                set
                {
                    SetProperty(ref pictureImageSource, value);
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

        public void Shuffle(ObservableCollection<Square> collection)
        {
            var shuffler = new Shuffler();

            // Keep shuffling until the arrangement of squares can be solved.
            bool gameCanBeSolved;

            do
            {
                shuffler.Shuffle(collection);

                gameCanBeSolved = CanGameBeSolved(collection);
            }
            while (!gameCanBeSolved);
        }

        private bool CanGameBeSolved(ObservableCollection<Square> collection)
        {
            int parity = 0;
            int emptySquareRow = 0;

            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].TargetIndex == 15)
                {
                    // The empty square row is one-based.
                    emptySquareRow = (i / 4) + 1;

                    continue;
                }

                for (int j = i + 1; j < collection.Count; j++)
                {
                    if ((collection[j].TargetIndex != 15) &&
                        (collection[i].TargetIndex > collection[j].TargetIndex))
                    {
                        parity++;
                    }
                }
            }

            // The following only applies to an grid with an even number of rows and columns.
            bool gridCanBeSolved = false;

            if (emptySquareRow % 2 == 0)
            {
                gridCanBeSolved = (parity % 2 == 0);
            }
            else
            {
                gridCanBeSolved = (parity % 2 != 0);
            }

            return gridCanBeSolved;
        }

        public class Shuffler
        {
            private readonly Random random;

            public Shuffler()
            {
                this.random = new Random();
            }

            public void Shuffle(ObservableCollection<Square> collection)
            {
                for (int i = collection.Count; i > 1;)
                {
                    int j = this.random.Next(i);

                    --i;

                    Square temp = collection[i];
                    collection[i] = collection[j];
                    collection[j] = temp;
                }
            }
        }
    }
}
