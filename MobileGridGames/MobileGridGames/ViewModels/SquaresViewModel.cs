﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MobileGridGames.ViewModels
{
    public class SquaresViewModel : BaseViewModel
    {
        public SquaresViewModel()
        {
            Title = "Squares";

            squareList = new ObservableCollection<Square>();
            this.CreateDefaultSquares();

            Shuffle(squareList);
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

        private string picturePath;
        public string PicturePath
        {
            get { return picturePath; }
            set
            {
                SetProperty(ref picturePath, value);
            }
        }

        private ObservableCollection<Square> squareList;

        public ObservableCollection<Square> SquareListCollection
        {
            get { return squareList; }
            set { this.squareList = value; }
        }

        public async Task<bool> AttemptMove(object currentSelection)
        {
            bool gameIsWon = false;

            Square adjacentSquare = null;
            int emptySquareIndex = -1;
            //string direction = "";

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

            // Is the empty square adjacent to this square?

            if (currentSelectionIndex % 4 > 0)
            {
                adjacentSquare = squareList[currentSelectionIndex - 1];
                if (adjacentSquare.TargetIndex == 15)
                {
                    emptySquareIndex = currentSelectionIndex - 1;

                    //direction = Resources.ResourceManager.GetString("Left");
                }
            }

            if ((emptySquareIndex == -1) && (currentSelectionIndex > 3))
            {
                adjacentSquare = squareList[currentSelectionIndex - 4];
                if (adjacentSquare.TargetIndex == 15)
                {
                    emptySquareIndex = currentSelectionIndex - 4;

                    //direction = Resources.ResourceManager.GetString("Up");
                }
            }

            if ((emptySquareIndex == -1) && (currentSelectionIndex % 4 < 3))
            {
                adjacentSquare = squareList[currentSelectionIndex + 1];
                if (adjacentSquare.TargetIndex == 15)
                {
                    emptySquareIndex = currentSelectionIndex + 1;

                    //direction = Resources.ResourceManager.GetString("Right");
                }
            }

            if ((emptySquareIndex == -1) && (currentSelectionIndex < 12))
            {
                adjacentSquare = squareList[currentSelectionIndex + 4];
                if (adjacentSquare.TargetIndex == 15)
                {
                    emptySquareIndex = currentSelectionIndex + 4;

                    //direction = Resources.ResourceManager.GetString("Down");
                }
            }

            // If we found an adjacent empty square, swap the clicked square with the empty square.
            if (emptySquareIndex != -1)
            {
                //++moveCount;

                Square temp = squareList[currentSelectionIndex];
                squareList[currentSelectionIndex] = squareList[emptySquareIndex];
                squareList[emptySquareIndex] = temp;

                // Has the game been won?
                gameIsWon = GameIsWon(squareList);
                if (!gameIsWon)
                {
                    ////string announcement = Resources.ResourceManager.GetString("Moved") +
                    ////    " " + clickedSquare.Name + " " + direction + ".";
                    ////AnnounceAction(announcement);
                }
            }
            else
            {
                //string announcement = Resources.ResourceManager.GetString("NoMovePossible");
                //AnnounceAction(announcement);
            }

            return gameIsWon;
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

        // Reset the grid to an initial game state.
        public void ResetGrid()
        {
            //moveCount = 0;

                Shuffle(squareList);
        }

        private void CreateDefaultSquares()
        {
            var resManager = Resource1.ResourceManager;

            // Note: This app assumes the total count of cards is 16.
            squareList.Add(
                    new Square
                    {
                        TargetIndex = 0,
                        Name = resManager.GetString("DefaultSquare1Name"),
                        Description = resManager.GetString("DefaultSquare1Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 1,
                        Name = resManager.GetString("DefaultSquare2Name"),
                        Description = resManager.GetString("DefaultSquare2Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 2,
                        Name = resManager.GetString("DefaultSquare3Name"),
                        Description = resManager.GetString("DefaultSquare3Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 3,
                        Name = resManager.GetString("DefaultSquare4Name"),
                        Description = resManager.GetString("DefaultSquare4Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 4,
                        Name = resManager.GetString("DefaultSquare5Name"),
                        Description = resManager.GetString("DefaultSquare5Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 5,
                        Name = resManager.GetString("DefaultSquare6Name"),
                        Description = resManager.GetString("DefaultSquare6Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 6,
                        Name = resManager.GetString("DefaultSquare7Name"),
                        Description = resManager.GetString("DefaultSquare7Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 7,
                        Name = resManager.GetString("DefaultSquare8Name"),
                        Description = resManager.GetString("DefaultSquare8Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 8,
                        Name = resManager.GetString("DefaultSquare9Name"),
                        Description = resManager.GetString("DefaultSquare9Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 9,
                        Name = resManager.GetString("DefaultSquare10Name"),
                        Description = resManager.GetString("DefaultSquare10Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 10,
                        Name = resManager.GetString("DefaultSquare11Name"),
                        Description = resManager.GetString("DefaultSquare11Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 11,
                        Name = resManager.GetString("DefaultSquare12Name"),
                        Description = resManager.GetString("DefaultSquare12Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 12,
                        Name = resManager.GetString("DefaultSquare13Name"),
                        Description = resManager.GetString("DefaultSquare13Description")
                    });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 13,
                        Name = resManager.GetString("DefaultSquare14Name"),
                        Description = resManager.GetString("DefaultSquare14Description")
                    });

            squareList.Add(
                   new Square
                   {
                       TargetIndex = 14,
                       Name = resManager.GetString("DefaultSquare15Name"),
                       Description = resManager.GetString("DefaultSquare15Description")
                   });

            squareList.Add(
                    new Square
                    {
                        TargetIndex = 15,
                        Name = "", // resManager.GetString("DefaultSquareEmpty"),
                        Description = resManager.GetString("DefaultSquareEmptyDescription")
                    });

        }

        public class Square
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int TargetIndex { get; set; }
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

            // The following applies to an even grid.

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