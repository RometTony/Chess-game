using Domain;


namespace CheckersBrain;

public class CheckersBrainMain
{
    public CheckersState State;

    public int Width;
    public int Height;
    

    public CheckersBrainMain(CheckersOption option, CheckersGameState? state)
    {
        if (state == null)
        {
            Width = option.Width - 1;
            Height = option.Height - 1;
            State = new CheckersState();
            InitialNewGame(option);
        }
        else
        {
            Width = option.Width - 1;
            Height = option.Height - 1;
            State = System.Text.Json.JsonSerializer.Deserialize<CheckersState>(state.SerializedGameState)!;
        }
    }

    public string GetSerializedGameState()
    {
        return System.Text.Json.JsonSerializer.Serialize(State);
    }

    // Creating new game with black and white buttons
    private void InitialNewGame(CheckersOption option)
    {
        // Initialize the jagged array
        State.GameBoard = new EGamePiece?[option.Width][];
        for (int i = 0; i < option.Width; i++)
        {
            State.GameBoard[i] = new EGamePiece?[option.Height];
        }

        for (int i = 0; i <= (option.Height - 2) / 2 - 1; i++)
        {
            for (int j = 0; j < option.Width; j++)
            {
                // paaritud kohad
                if ((i + 1) % 2 != 0 && (j + 1) % 2 == 0)
                {
                    State.GameBoard[j][i] = EGamePiece.Black;
                }
                // paaris kohad
                else if (((i + 1) % 2 == 0 && (j + 1) % 2 != 0))
                {
                    State.GameBoard[j][i] = EGamePiece.Black;
                }
            }
        }

        for (int i = (option.Width) / 2 + 1; i <= option.Height - 1; i++)
        {
            for (int j = 0; j < option.Width; j++)
            {
                // paaris kohad
                if ((i + 1) % 2 == 0 && (j + 1) % 2 != 0)
                {
                    State.GameBoard[j][i] = EGamePiece.White;
                }
                // paaritud kohad
                else if ((i + 1) % 2 != 0 && (j + 1) % 2 == 0)
                {
                    State.GameBoard[j][i] = EGamePiece.White;
                }
            }
        }
    }
    

    public int initialButtonCount = 0;

    //checks if the game is over
    public int ButtonChecker(String var)
    {
        int buttonCount = 0;
        foreach (var element in var)
        {
            if (element.Equals("1") || element.Equals("2"))
            {
                buttonCount += 1;
            }
        }

        return buttonCount;
    }

    public string CurrentState = "";
    public bool PreviousState = false;


    public EGamePiece?[][] GetBoard()
    {
        var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);
        // Setting up the initial button count for the checker
        initialButtonCount = ButtonChecker(jsonStr);

        CurrentState = jsonStr;

        return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
    }

    //New board for white input and draws out the board for the whites. Check if all the conditions are good for white

    public EGamePiece?[][] NewWhiteState(int sxAxis, int syAxis, int fxAxis, int fyAxis)
    {
        if (
            State.GameBoard[sxAxis][syAxis].Equals(null) ||
            State.GameBoard[sxAxis][syAxis].Equals(EGamePiece.Black) ||
            State.GameBoard[fxAxis][fyAxis].Equals(EGamePiece.Black) ||
            State.GameBoard[fxAxis][fyAxis].Equals(EGamePiece.White) ||
            // sxAxis > width || syAxis > height || fxAxis > width || fyAxis > height ||
            sxAxis.Equals(fxAxis) ||
            syAxis.Equals(fyAxis))
        {
            State.NextMoveByBlack = false;
            PreviousState = true;
            var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);
            return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
            // return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(State.GameBoard)!;
        }
        // kas valge peab votma
        while (Stepchecker())
        {
            int startX = 0;
            int startY = 0;
            int middleX = 0;
            int middleY = 0;
            int finishX = 0;
            int finishY = 0;
            foreach (List<int> element in StepCheckerWhite())
            {
                if (element[4] == fxAxis && element[5] == fyAxis)
                {
                    startX += element[0];
                    startY += element[1];
                    middleX += element[2];
                    middleY += element[3];
                    finishX += element[4];
                    finishY += element[5];
                }
            }

            if (sxAxis == startX && syAxis == startY && fxAxis == finishX && fyAxis == finishY)
            {
                if (fyAxis == 0 || State.GameBoard[startX][startY] == EGamePiece.WhiteQueen)
                {
                    State.GameBoard[sxAxis][syAxis] = null;
                    State.GameBoard[middleX][middleY] = null;
                    State.GameBoard[fxAxis][fyAxis] = EGamePiece.WhiteQueen;
                }
                else
                {
                    State.GameBoard[sxAxis][syAxis] = null;
                    State.GameBoard[middleX][middleY] = null;
                    State.GameBoard[fxAxis][fyAxis] = EGamePiece.White;
                }


                var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);

                PreviousState = false;
                CurrentState = jsonStr;
                State.NextMoveByBlack = true;

                return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
            }
            else
            {
                State.NextMoveByBlack = false;
                PreviousState = true;

                var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);
                return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
            }
        }

        // moving with white normal
        if (((fxAxis - sxAxis == 1 && fxAxis - sxAxis == -1) || fyAxis - syAxis == -1) &&
            State.GameBoard[sxAxis][syAxis] == EGamePiece.White)
        {
            State.GameBoard[sxAxis][syAxis] = null;

            if (fyAxis == 0)
            {
                State.GameBoard[fxAxis][fyAxis] = EGamePiece.WhiteQueen;
            }
            else
            {
                State.GameBoard[fxAxis][fyAxis] = EGamePiece.White;
            }


            var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);

            PreviousState = false;
            CurrentState = jsonStr;
            State.NextMoveByBlack = true;


            return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
        }

        // white moving for the queen piece
        if (State.GameBoard[sxAxis][syAxis] == EGamePiece.WhiteQueen)
        {
            if (Math.Abs(sxAxis - fxAxis) == Math.Abs(syAxis - fyAxis))
            {
                State.GameBoard[sxAxis][syAxis] = null;

                State.GameBoard[fxAxis][fyAxis] = EGamePiece.WhiteQueen;


                var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);

                PreviousState = false;
                CurrentState = jsonStr;
                State.NextMoveByBlack = true;


                return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
            }
            else
            {
                State.NextMoveByBlack = false;
                PreviousState = true;
                var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);
                return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
            }
        }

        return null!;
    }


    //New board for black input
    //New board for black input and draws out the board for the blacks. Check if all the conditions are good for black
    public EGamePiece?[][] NewBlackState(int sxAxis, int syAxis, int fxAxis, int fyAxis)
    {
        if (State.GameBoard[sxAxis][syAxis].Equals(null) ||
            State.GameBoard[sxAxis][syAxis].Equals(EGamePiece.White) ||
            State.GameBoard[fxAxis][fyAxis].Equals(EGamePiece.White) ||
            State.GameBoard[fxAxis][fyAxis].Equals(EGamePiece.Black) ||
            // sxAxis > width || syAxis > height || fxAxis > width || fyAxis > height ||
            sxAxis.Equals(fxAxis) ||
            syAxis.Equals(fyAxis))
        {
            PreviousState = true;
            State.NextMoveByBlack = true;

            var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);
            return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
        }

        //if you have to take    
        while (Stepchecker())
        {
            int startX = 0;
            int startY = 0;
            int middleX = 0;
            int middleY = 0;
            int finishX = 0;
            int finishY = 0;
            foreach (List<int> element in StepCheckerBlack())
            {
                if (element[4] == fxAxis && element[5] == fyAxis)
                {
                    startX = element[0];
                    startY = element[1];
                    middleX = element[2];
                    middleY = element[3];
                    finishX = element[4];
                    finishY = element[5];
                }
            }

            if (sxAxis == startX && syAxis == startY && fxAxis == finishX && fyAxis == finishY)
            {
                if (fyAxis == Height || State.GameBoard[startX][startY] == EGamePiece.BlackQueen)
                {
                    State.GameBoard[sxAxis][syAxis] = null;
                    State.GameBoard[middleX][middleY] = null;
                    State.GameBoard[fxAxis][fyAxis] = EGamePiece.BlackQueen;
                }
                else
                {
                    State.GameBoard[sxAxis][syAxis] = null;
                    State.GameBoard[middleX][middleY] = null;
                    State.GameBoard[fxAxis][fyAxis] = EGamePiece.Black;
                }


                var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);

                PreviousState = false;
                CurrentState = jsonStr;
                State.NextMoveByBlack = false;

                return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
            }
            else
            {
                PreviousState = true;
                State.NextMoveByBlack = true;

                var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);
                return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
            }
        }


        //moving with black normal
        if (((fxAxis - sxAxis == 1 || fxAxis - sxAxis == -1) && syAxis - fyAxis == -1) &&
            State.GameBoard[sxAxis][syAxis] == EGamePiece.Black)
        {
            State.GameBoard[sxAxis][syAxis] = null;

            if (fyAxis == Height)
            {
                State.GameBoard[fxAxis][fyAxis] = EGamePiece.BlackQueen;
            }
            else
            {
                State.GameBoard[fxAxis][fyAxis] = EGamePiece.Black;
            }


            var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);

            PreviousState = false;
            CurrentState = jsonStr;
            State.NextMoveByBlack = false;


            return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
        }

        // black moving for the queen piece
        if (State.GameBoard[sxAxis][syAxis] == EGamePiece.BlackQueen)
        {
            if (Math.Abs(sxAxis - fxAxis) == Math.Abs(syAxis - fyAxis))
            {
                State.GameBoard[sxAxis][syAxis] = null;

                State.GameBoard[fxAxis][fyAxis] = EGamePiece.BlackQueen;


                var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);

                PreviousState = false;
                CurrentState = jsonStr;
                State.NextMoveByBlack = false;


                return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
            }
            else
            {
                PreviousState = true;
                State.NextMoveByBlack = true;

                var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);
                return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
            }
        }

        return null!;
    }

    //Check whether game-over requirement is there or not

    public bool CheckGameState()
    {
        var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);
        if (jsonStr.Contains("1") && jsonStr.Contains("2") || jsonStr.Contains("3") && jsonStr.Contains("4") ||
            jsonStr.Contains("3") && jsonStr.Contains("2") || jsonStr.Contains("4") && jsonStr.Contains("1"))
        {
            return true;
        }

        return false;
    }


    public bool NextMoveByBlack() => State.NextMoveByBlack;


    public bool GetWinner()
    {
        // Checking if the game is over or not, false is over and true is not over
        if (CheckGameState() == false)
        {
            var jsonStr = System.Text.Json.JsonSerializer.Serialize(State.GameBoard);
            if (jsonStr.Contains("1") || jsonStr.Contains("3"))
            {
                //white win
                return true;
            }

            if (jsonStr.Contains("2") || jsonStr.Contains("4"))
            {
                //black win
                return false;
            }
        }

        return false;
    }

    //Check if you have to take with the white button
    public List<List<int>> StepCheckerWhite()
    {
        List<List<int>> hasToTake = new List<List<int>>();
        for (int i = 0; i <= Width; i++)
        {
            for (int j = 0; j <= Height; j++)
            {
                if (State.NextMoveByBlack == false)
                {
                    if (State.GameBoard[i][j] == EGamePiece.White)
                    {
                        if (j - 2 > -1)
                        {
                            if (i + 2 <= Width)
                            {
                                if (State.GameBoard[i + 1][j - 1] == EGamePiece.Black ||
                                    State.GameBoard[i + 1][j - 1] == EGamePiece.BlackQueen)
                                {
                                    if (State.GameBoard[i + 2][j - 2] == null)
                                    {
                                        List<int> button = new List<int> { i, j, i + 1, j - 1, i + 2, j - 2 };
                                        hasToTake.Add(button);
                                    }
                                }
                            }

                            if (i - 2 > -1)
                            {
                                if (State.GameBoard[i - 1][j - 1] == EGamePiece.Black ||
                                    State.GameBoard[i - 1][j - 1] == EGamePiece.BlackQueen)
                                {
                                    if (State.GameBoard[i - 2][j - 2] == null)
                                    {
                                        List<int> button = new List<int> { i, j, i - 1, j - 1, i - 2, j - 2 };
                                        hasToTake.Add(button);
                                    }
                                }
                            }
                        }
                    }

                    //shows the buttons what queens have to take
                    if (State.GameBoard[i][j] == EGamePiece.WhiteQueen)
                    {
                        // up taking
                        for (int n = j; n >= 0; n--)
                        {
                            //up and left
                            for (int m = i; m >= 0; m--)
                            {
                                if (State.GameBoard[m][n] == EGamePiece.Black &&
                                    Math.Abs(i - m) == Math.Abs(j - n) ||
                                    State.GameBoard[m][n] == EGamePiece.BlackQueen &&
                                    Math.Abs(i - m) == Math.Abs(j - n))
                                {
                                    if (m - 1 >= 0 && n - 1 >= 0)
                                    {
                                        if (State.GameBoard[m - 1][n - 1] == null)
                                        {
                                            List<int> button = new List<int> { i, j, m, n, m - 1, n - 1 };
                                            hasToTake.Add(button);
                                        }
                                    }
                                }
                            }

                            //up and right
                            for (int m = i; m <= Width; m++)
                            {
                                if (State.GameBoard[m][n] == EGamePiece.Black &&
                                    Math.Abs(i - m) == Math.Abs(j - n) ||
                                    State.GameBoard[m][n] == EGamePiece.BlackQueen &&
                                    Math.Abs(i - m) == Math.Abs(j - n))
                                {
                                    if (m + 1 <= Width && n - 1 >= 0)
                                    {
                                        if (State.GameBoard[m + 1][n - 1] == null)
                                        {
                                            List<int> button = new List<int> { i, j, m, n, m + 1, n - 1 };
                                            hasToTake.Add(button);
                                        }
                                    }
                                }
                            }
                        }

                        //down taking
                        for (int n = j; n <= Height; n++)
                        {
                            //down and left
                            for (int m = i; m >= 0; m--)
                            {
                                if (State.GameBoard[m][n] == EGamePiece.Black &&
                                    Math.Abs(i - m) == Math.Abs(j - n) ||
                                    State.GameBoard[m][n] == EGamePiece.BlackQueen &&
                                    Math.Abs(i - m) == Math.Abs(j - n))
                                {
                                    if (m - 1 >= 0 && n + 1 <= Height)
                                    {
                                        if (State.GameBoard[m - 1][n + 1] == null)
                                        {
                                            List<int> button = new List<int> { i, j, m, n, m - 1, n + 1 };
                                            hasToTake.Add(button);
                                        }
                                    }
                                }
                            }

                            // down and right
                            for (int m = i; m <= Width; m++)
                            {
                                if (State.GameBoard[m][n] == EGamePiece.Black &&
                                    Math.Abs(i - m) == Math.Abs(j - n) ||
                                    State.GameBoard[m][n] == EGamePiece.BlackQueen &&
                                    Math.Abs(i - m) == Math.Abs(j - n))
                                {
                                    if (m + 1 <= Width && n + 1 <= Height)
                                    {
                                        if (State.GameBoard[m + 1][n + 1] == null)
                                        {
                                            List<int> button = new List<int> { i, j, m, n, m + 1, n + 1 };
                                            hasToTake.Add(button);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return hasToTake;
    }

    //Check if you have to take with the black button
    public List<List<int>> StepCheckerBlack()
    {
        List<List<int>> hasToTake = new List<List<int>>();

        for (int i = 0; i <= Width; i++)
        {
            for (int j = 0; j <= Height; j++)
            {
                if (State.NextMoveByBlack)
                {
                    if (State.GameBoard[i][j] == EGamePiece.Black)
                    {
                        if (j + 2 <= Height)
                        {
                            if (i + 2 <= Width)
                            {
                                if (State.GameBoard[i + 1][j + 1] == EGamePiece.White ||
                                    State.GameBoard[i + 1][j + 1] == EGamePiece.WhiteQueen)
                                {
                                    if (State.GameBoard[i + 2][j + 2] == null)
                                    {
                                        List<int> button = new List<int> { i, j, i + 1, j + 1, i + 2, j + 2 };
                                        hasToTake.Add(button);
                                    }
                                }
                            }

                            if (i - 2 > -1)
                            {
                                if (State.GameBoard[i - 1][j + 1] == EGamePiece.White ||
                                    State.GameBoard[i - 1][j + 1] == EGamePiece.WhiteQueen)
                                {
                                    if (State.GameBoard[i - 2][j + 2] == null)
                                    {
                                        List<int> button = new List<int> { i, j, i - 1, j + 1, i - 2, j + 2 };
                                        hasToTake.Add(button);
                                    }
                                }
                            }
                        }
                    }

                    //shows the buttons what queens have to take
                    if (State.GameBoard[i][j] == EGamePiece.BlackQueen)
                    {
                        // //up taking
                        for (int n = j; n >= 0; n--)
                        {
                            //up and left
                            for (int m = i; m >= 0; m--)
                            {
                                if (State.GameBoard[m][n] == EGamePiece.White &&
                                    Math.Abs(i - m) == Math.Abs(j - n) ||
                                    State.GameBoard[m][n] == EGamePiece.WhiteQueen &&
                                    Math.Abs(i - m) == Math.Abs(j - n))
                                {
                                    if (m - 1 >= 0 && n - 1 >= 0)
                                    {
                                        if (State.GameBoard[m - 1][n - 1] == null)
                                        {
                                            List<int> button = new List<int> { i, j, m, n, m - 1, n - 1 };
                                            hasToTake.Add(button);
                                        }
                                    }
                                }
                            }

                            //up and right
                            for (int m = i; m <= Width; m++)
                            {
                                if (State.GameBoard[m][n] == EGamePiece.White &&
                                    Math.Abs(i - m) == Math.Abs(j - n) ||
                                    State.GameBoard[m][n] == EGamePiece.WhiteQueen &&
                                    Math.Abs(i - m) == Math.Abs(j - n))
                                {
                                    if (m + 1 <= Width && n - 1 >= 0)
                                    {
                                        if (State.GameBoard[m + 1][n - 1] == null)
                                        {
                                            List<int> button = new List<int> { i, j, m, n, m + 1, n - 1 };
                                            hasToTake.Add(button);
                                        }
                                    }
                                }
                            }
                        }

                        //down taking
                        for (int n = j; n <= Height; n++)
                        {
                            //down and left
                            for (int m = i; m >= 0; m--)
                            {
                                if (State.GameBoard[m][n] == EGamePiece.White &&
                                    Math.Abs(i - m) == Math.Abs(j - n) ||
                                    State.GameBoard[m][n] == EGamePiece.WhiteQueen &&
                                    Math.Abs(i - m) == Math.Abs(j - n))
                                {
                                    if (m - 1 >= 0 && n + 1 <= Height)
                                    {
                                        if (State.GameBoard[m - 1][n + 1] == null)
                                        {
                                            List<int> button = new List<int> { i, j, m, n, m - 1, n + 1 };
                                            hasToTake.Add(button);
                                        }
                                    }
                                }
                            }

                            // down and right
                            for (int m = i; m <= Width; m++)
                            {
                                if (State.GameBoard[m][n] == EGamePiece.White &&
                                    Math.Abs(i - m) == Math.Abs(j - n) ||
                                    State.GameBoard[m][n] == EGamePiece.WhiteQueen &&
                                    Math.Abs(i - m) == Math.Abs(j - n))
                                {
                                    if (m + 1 <= Width && n + 1 <= Height)
                                    {
                                        if (State.GameBoard[m + 1][n + 1] == null)
                                        {
                                            List<int> button = new List<int> { i, j, m, n, m + 1, n + 1 };
                                            hasToTake.Add(button);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return hasToTake;
    }


    //checks if there is another take needed for the button
    public bool Stepchecker()
    {
        if (State.NextMoveByBlack == false)
        {
            int counter = 0;
            foreach (var element in StepCheckerWhite())
            {
                if (element != null)
                {
                    counter++;
                }
            }

            if (counter != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (State.NextMoveByBlack)
        {
            int counter = 0;
            foreach (var element in StepCheckerBlack())
            {
                if (element != null)
                {
                    counter++;
                }
            }

            if (counter != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public List<int> AiMoveChooser()
    {
        List<List<int>> validMoves = new List<List<int>>();

        for (int i = 0; i <= Width; i++)
        {
            for (int j = 0; j <= Height; j++)
            {
                //White
                if (State.NextMoveByBlack == false)
                {
                    if (Stepchecker() == false)
                    {
                        //Moves up height -
                        if (State.GameBoard[i][j] == EGamePiece.White)
                        {
                            if (i + 1 <= Width && j - 1 >= 0 &&
                                State.GameBoard[i + 1][j - 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i + 1, j - 1 };
                                validMoves.Add(move);
                            }

                            if (i - 1 >= 0 && j - 1 >= 0 &&
                                State.GameBoard[i - 1][j - 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i - 1, j - 1 };
                                validMoves.Add(move);
                            }
                        }

                        //AI movement for the queen
                        if (State.GameBoard[i][j] == EGamePiece.WhiteQueen)
                        {
                            if (i + 1 <= Width && j + 1 <= Height &&
                                State.GameBoard[i + 1][j + 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i + 1, j + 1 };
                                validMoves.Add(move);
                            }

                            if (i - 1 >= 0 && j + 1 <= Height &&
                                State.GameBoard[i - 1][j + 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i - 1, j + 1 };
                                validMoves.Add(move);
                            }

                            if (i + 1 <= Width && j - 1 >= 0 &&
                                State.GameBoard[i + 1][j - 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i + 1, j - 1 };
                                validMoves.Add(move);
                            }

                            if (i - 1 >= 0 && j - 1 >= 0 &&
                                State.GameBoard[i - 1][j - 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i - 1, j - 1 };
                                validMoves.Add(move);
                            }
                        }
                    }

                    if (Stepchecker())
                    {
                        foreach (var element in StepCheckerWhite())
                        {
                            List<int> move = new List<int> { element[0], element[1], element[4], element[5] };
                            validMoves.Add(move);
                        }
                    }
                }

                //Black AI
                else if (State.NextMoveByBlack)
                {
                    if (Stepchecker() == false)
                    {
                        //Moves down height +
                        if (State.GameBoard[i][j] == EGamePiece.Black)
                        {
                            if (i + 1 <= Width && j + 1 <= Height &&
                                State.GameBoard[i + 1][j + 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i + 1, j + 1 };
                                validMoves.Add(move);
                            }

                            if (i - 1 >= 0 && j + 1 <= Height &&
                                State.GameBoard[i - 1][j + 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i - 1, j + 1 };
                                validMoves.Add(move);
                            }
                        }
                        // AI movement for black queen
                        if (State.GameBoard[i][j] == EGamePiece.BlackQueen)
                        {
                            if (i + 1 <= Width && j + 1 <= Height &&
                                State.GameBoard[i + 1][j + 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i + 1, j + 1 };
                                validMoves.Add(move);
                            }

                            if (i - 1 >= 0 && j + 1 <= Height &&
                                State.GameBoard[i - 1][j + 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i - 1, j + 1 };
                                validMoves.Add(move);
                            }

                            if (i + 1 <= Width && j - 1 >= 0 &&
                                State.GameBoard[i + 1][j - 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i + 1, j - 1 };
                                validMoves.Add(move);
                            }

                            if (i - 1 >= 0 && j - 1 >= 0 &&
                                State.GameBoard[i - 1][j - 1] == null)
                            {
                                List<int> move = new List<int> { i, j, i - 1, j - 1 };
                                validMoves.Add(move);
                            }
                        }
                    }


                    if (Stepchecker())
                    {
                        foreach (var element in StepCheckerBlack())
                        {
                            List<int> move = new List<int> { element[0], element[1], element[4], element[5] };
                            validMoves.Add(move);
                        }
                    }
                }
            }
        }

        Random random = new Random();
        int index = random.Next(validMoves.Count);
        return validMoves[index];
    }
    
}
