using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HyperspaceCheeseBattle
{
    class Program
    {


        // Variable and array declarations

        static bool sixPower = false;

        static int consecutiveSixPowerRolls = 0;

        static int NumPlayers;

        static int boardWidth = 8;

        static Player[] players;

        static Random randomGenerator = new Random((int)DateTime.Now.Ticks);

        static bool godDice = false;


        // Creation of the playing board with an array, the numbers represent directions.
        static Tile[,] board = new Tile[,] 
        { 

         {Tile.Up,Tile.Up,Tile.Up,Tile.Up,Tile.Up,Tile.Up,Tile.Up,Tile.Up}, // row 0 
         {Tile.Right,Tile.Right,Tile.Up,Tile.Down,Tile.CheeseUp,Tile.Up,Tile.Left,Tile.Left}, // row 1 
         {Tile.Right,Tile.Right,Tile.Up,Tile.Right,Tile.Left,Tile.Right,Tile.Left,Tile.Left}, // row 2 
         {Tile.CheeseRight,Tile.Right,Tile.Up,Tile.Right,Tile.Up,Tile.Up,Tile.Left,Tile.Left}, // row 3 
         {Tile.Right,Tile.Right,Tile.Up,Tile.Right,Tile.Up,Tile.Up,Tile.CheeseLeft,Tile.Left}, // row 4 
         {Tile.Right,Tile.Right,Tile.Right,Tile.CheeseRight,Tile.Up,Tile.Up,Tile.Left,Tile.Left}, // row 5 
         {Tile.Right,Tile.Right,Tile.Up,Tile.Down,Tile.Up,Tile.Right,Tile.Left,Tile.Left}, // row 6 
         {Tile.Down,Tile.Right,Tile.Right,Tile.Right,Tile.Right,Tile.Right,Tile.Down,Tile.Win}  // row 7 

        };


        //The storage of player directions 
        enum Tile : int
        {
            Down = 0,
            Up = 1,
            Right = 2,
            Left = 3,
            Win = 4,
            CheeseUp = 5,
            CheeseRight = 6,
            CheeseLeft = 7
        };


        //A place to hold the players' names and board position variables 
        public class Player
        {
            public string Name;
            public int X;
            public int Y;

            public Player()
            {
                X = 0;
                Y = 0;
            }
        }
        //Classes inherting from the main player class, which hold either AI or Human players.
        public class HumanPlayer : Player
        {

        }

        public class ComputerPlayer : Player
        {

        }


        /// <summary>
        /// A method that asks a simple yes or no question and can be used within logic [e.g if statements]
        /// </summary>
        /// <param name="question"></param>
        /// <returns>It will return with a true or false based on yes or no</returns>
        static bool AskYesNoQuestion(string question)
        {
            while (true)
            {
                Console.Write(question);
                Console.Write(" (Y/N) - ");
                string questionAnswer = Console.ReadLine().ToLower().Trim();

                if (questionAnswer.StartsWith("y"))
                {
                    return true;
                }
                else if (questionAnswer.StartsWith("n"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please enter (Y/N)");
                }
            }
        }

        /// <summary>
        /// A method that asks a question involving numbers, expecting an answer between two pre-set numbers
        /// </summary>
        /// <param name="question"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Returns the number the user has input that must be between two numbers</returns>
        static int AskNumberQuestion(string question, int from, int to)
        {
            while (true)
            {
                Console.Write(question);
                Console.Write(" (number between " + from + " and " + to + ") - ");

                string answer = Console.ReadLine();

                try
                {
                    int number = int.Parse(answer);

                    if (number >= from && number <= to)
                    {
                        // number is within specified range

                        return number;
                    }
                    else
                    {
                        Console.WriteLine("-- Ensure the number is between " + from + " and " + to + " --");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("-- Please enter a valid number --");
                }
            }
        }

       /// <summary>
       /// A method that asks a question after text, expeting as string to be entered
       /// </summary>
       /// <param name="question"></param>
       /// <returns>Returns the text which the user has input and stores it as a string, but will ensure the user does not put in blank spaces or gaps</returns>
        static string AskStringQuestion(string question)
        {
            while (true)
            {
                Console.Write(question);
                Console.Write(" - ");

                string playerName = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(playerName))
                {
                    return playerName;
                }

                Console.WriteLine("Enter a valid name");
            }
        }


        //Method that creates X amount of players, adds them to an arry and sets their values.
        static void ResetGame()
        {
            Console.ForegroundColor = ConsoleColor.White;

            sixPower = AskYesNoQuestion("Do you want the six power rule active (Re-roll when you roll six)?");

            godDice = AskYesNoQuestion("Do you want to play in debug mode? (specify dice rolls)");

            if (AskYesNoQuestion("Do you want to play vs computer?"))
            {
                NumPlayers = 4;
                players = new Player[NumPlayers];

                // create the only human player
                players[0] = new HumanPlayer();
                players[0].Name = AskStringQuestion("Enter the players name");

                // create the computer players

                players[1] = new ComputerPlayer();
                players[1].Name = "Angry Allan";

                players[2] = new ComputerPlayer();
                players[2].Name = "Speedy Steve";

                players[3] = new ComputerPlayer();
                players[3].Name = "Clever Trevor";

            }
            else
            {
                // create all the players as human

                NumPlayers = AskNumberQuestion("How many players will be battling?", 2, 4);
                players = new Player[NumPlayers];

                for (int i = 0; i < NumPlayers; i++)
                {
                    players[i] = new HumanPlayer();
                    players[i].Name = AskStringQuestion("Enter the name for player " + (i + 1));
                }
            }
        }


        /// <summary>
        /// Dice roll method
        /// </summary>
        /// <returns>Returns a number between 1-6 based on system clock [so it's different each time] </returns>
        static int DiceThrow()
        {
            return randomGenerator.Next(1, 7);
        }

        static Player RandomPlayer()
        {
            return players[randomGenerator.Next(0, NumPlayers)];
        }


        /// <summary>
        /// Method containing each player's movement on the board, as well as options for debug/testing mode and sixpower
        /// </summary>
        /// <param name="playerNo"></param>
        /// <returns>Rolls the dice and moves the player based on the tile they are on</returns>
        private static bool PlayerTurn(Player currentPlayer)
        {
            int diceThrow;

            if (godDice)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                diceThrow = AskNumberQuestion(currentPlayer.Name + ", what would you like your dice roll to be?\n\rYou sly little cheater!", 1, 6);
            }
            else
            {
                diceThrow = DiceThrow();
            }

            Console.ForegroundColor = ConsoleColor.White;

            Tile tileType = board[currentPlayer.Y, currentPlayer.X];


            if (tileType == Tile.Up || tileType == Tile.CheeseUp) //1 Means up
            {

                if (currentPlayer.Y + diceThrow < boardWidth - 1)
                {
                    // Move is valid - move to new position
                    currentPlayer.Y = currentPlayer.Y + diceThrow;
                }
                else
                {
                    //Move is invalid - move to end of board
                    currentPlayer.Y = boardWidth - 1;
                }

                // this is a really good place to check if the position is valid

                while (CheckIfPlayerCollision(currentPlayer))
                {
                    // player already has space to move up one more

                    currentPlayer.Y += 1;

                    if (currentPlayer.Y > boardWidth - 1)
                    {
                        // player is off the edge of the board so move them down to an empty space

                        currentPlayer.Y -= 1;

                        while (CheckIfPlayerCollision(currentPlayer))
                        {
                            currentPlayer.Y -= 1;

                            // assumes that the entire column is not full of players
                            // and that there is some space somewhere
                        }
                    }
                }

                Console.WriteLine(currentPlayer.Name + " has moved up " + diceThrow + " spaces, you are now at position: " + currentPlayer.X + "," + currentPlayer.Y);

            }

            else if (tileType == Tile.Right || tileType == Tile.CheeseRight) // 2 = Movement right 
            {
                if (currentPlayer.X + diceThrow < boardWidth - 1)
                {
                    // Move is valid - move to new position
                    currentPlayer.X = currentPlayer.X + diceThrow;

                }
                else
                {
                    //Move is invalid - move to end of board
                    currentPlayer.X = boardWidth - 1;
                }

                Console.WriteLine(currentPlayer.Name + " has moved right " + diceThrow + " spaces, you are now at position: " + currentPlayer.X + "," + currentPlayer.Y);

            }

            else if (tileType == Tile.Left || tileType == Tile.CheeseLeft) // 3 = Movement left
            {

                if (currentPlayer.X - diceThrow >= 0)
                {
                    // Move is valid - move to new position
                    currentPlayer.X = currentPlayer.X - diceThrow;

                }
                else
                {
                    //Move is invalid - move to end of board
                    currentPlayer.X = 0;
                }

                Console.WriteLine(currentPlayer.Name + " has moved left " + diceThrow + " spaces, you are now at position: " + currentPlayer.X + "," + currentPlayer.Y);

            }
            else if (tileType == Tile.Down) // 0 = Movement down
            {
                if (currentPlayer.Y - diceThrow >= 0)
                {
                    //Move is valid - move to new position
                    currentPlayer.Y = currentPlayer.Y - diceThrow;
                }
                else
                {
                    //Move is invalid - move to end of board
                    currentPlayer.Y = 0;
                }
                Console.WriteLine(currentPlayer.Name + " has moved down " + diceThrow + " spaces, you are now at position: " + currentPlayer.X + "," + currentPlayer.Y);
            }

            int cursorX = Console.CursorLeft;
            int cursorY = Console.CursorTop;

            Console.SetCursorPosition(0, 0);
            DrawBoard();

            Console.SetCursorPosition(cursorX, cursorY);

            Tile newTileType = board[currentPlayer.Y, currentPlayer.X];

            if (newTileType == Tile.Win) //4 = Victory position 
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Congratulations " + currentPlayer.Name + " you are victirious in the Hyperspace Cheese Battle!");
                Console.ForegroundColor = ConsoleColor.White;
                return true;

            }
            else if (newTileType == Tile.CheeseRight || newTileType == Tile.CheeseLeft || newTileType == Tile.CheeseUp)
            {
                CheeseSquare(currentPlayer);
            }

            bool hasWon = false;

            if (currentPlayer.Name == "supersix" || (sixPower == true && diceThrow == 6))
            {
                consecutiveSixPowerRolls++;

                if (consecutiveSixPowerRolls == 3)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You have rolled SIXPOWER three times, your engines cannot handle the speed\r\nand blow up! You have been placed at the start of the board.");
                    currentPlayer.X = 0;
                    currentPlayer.Y = 0;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You rolled a six! SIXPOWER is in effect, roll again!");
                    hasWon = PlayerTurn(currentPlayer);
                }
            }

            if (consecutiveSixPowerRolls > 0)
            {
                consecutiveSixPowerRolls--;
            }

            return hasWon;

        }

        /// <summary>
        /// Player colision detection
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <returns>Returns whether or not the square has a player on it</returns>
        private static bool CheckIfPlayerCollision(Player currentPlayer)
        {
            foreach (Player otherPlayer in players)
            {
                if (currentPlayer.X == otherPlayer.X && currentPlayer.Y == otherPlayer.Y && currentPlayer.Name != otherPlayer.Name)
                {
                    return true;
                }
            }

            return false;
        }



        // Method that runs if you land on a cheese power square, and has player decisions and AI logic


        static void CheeseSquare(Player currentPlayer)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine(currentPlayer.Name + ", You have come into the orbit of a giant block of cheese,\r\nthe cheese empowers your rocket, giving you the choice between two powers:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Power 1: The cheese powers up your weapons system, allowing you to fire\r\na cheesy beam of death at a target of your choice!");
            Console.WriteLine("Power 2: Let the cheese flow through you! Your engines will be replenished,\r\nallowing you to have a second turn.");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;

            int cheesePowerChoice = 0;
            Player playerToShoot = null;

            if (currentPlayer is HumanPlayer)
            {
                cheesePowerChoice = AskNumberQuestion("Select which power to use", 1, 2);

                if (cheesePowerChoice == 1)
                {
                    // ask who to shoot

                    for (int i = 0; i < NumPlayers; i++)
                    {
                        Console.WriteLine((i + 1) + ". " + players[i].Name);
                    }

                    playerToShoot = players[AskNumberQuestion("Which player would you like to shoot?", 1, NumPlayers)];
                }
            }
            else
            {
                switch (currentPlayer.Name)
                {
                    //Angry Allan always shoots a random player
                    case "Angry Allan":
                        cheesePowerChoice = 1;

                        do
                        {        
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Angry Allan: 'GRRRRRR I HATE YOU ALL! I don't care who gets hit,\r\nFIRE THE DEATH BEAM AT RANDOM!!!'");
                            Console.ForegroundColor = ConsoleColor.White;
                            playerToShoot = RandomPlayer();
                        } while (playerToShoot.Name == "Angry Allan");
                        break;
                        //Speedy Steve always powers up his engines
                    case "Speedy Steve":
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Speedy Steve: 'I want more speed! None you of heathens can keep up with me!\r\nI choose to let the mighty cheese refuel my engines!'");
                        Console.ForegroundColor = ConsoleColor.White;
                        cheesePowerChoice = 2;
                        break;
                        //Clever Trevor checks to see if he's closest
                    case "Clever Trevor":
                        Player closestPlayer = null;

                        foreach (var player in players)
                        {
                            if (closestPlayer == null)
                            {
                                closestPlayer = player;
                            }

                            if (player.X + player.Y > closestPlayer.X + closestPlayer.Y)
                            {
                                closestPlayer = player;
                            }
                        }

                        if (closestPlayer.Name == "Clever Trevor")
                        {
                            // I am the closest, so roll again
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Clever Trevor: 'You are all BAD and should FEEL BAD! Why? Because I am ahead, and as such I am clearly the better Pilot. I've chosen to roll again!");
                            Console.ForegroundColor = ConsoleColor.White;
                            cheesePowerChoice = 2;

                        }
                        else
                        {
                            // I am not the closest, so shoot the closest 
                            cheesePowerChoice = 1;
                            playerToShoot = closestPlayer;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Clever Trevor: '" + closestPlayer.Name + ", you are ahead of me you devilish rogue!\r\nSo I've chosen to blast YOU with my cheesy beam of death!'");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        break;
                    default: break;
                }
            }

            //Player selects to blast another player
            if (cheesePowerChoice == 1)
            {
                playerToShoot.Y = 0;
                //Asks human players to select a position if they have been shot
                if (playerToShoot is HumanPlayer)
                {
                    while (true)
                    {
                        playerToShoot.X = AskNumberQuestion(playerToShoot.Name + ", you have been shot at, which position on the bottom row would you like to move to?", 1, 8) - 1;

                        if (CheckIfPlayerCollision(playerToShoot))
                        {
                            Console.WriteLine("That space is already taken, please choose another");
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    playerToShoot.X = 7;

                    while (CheckIfPlayerCollision(playerToShoot))
                    {
                        playerToShoot.X -= 1;
                    };
                }

                Console.WriteLine(playerToShoot.Name + " has been blasted to position: " + playerToShoot.X + "," + playerToShoot.Y);
            }
                //Player selects to roll again
            else if (cheesePowerChoice == 2)
            {           
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(currentPlayer.Name + ", the power of cheese flows through your engines, roll again!");
                Console.WriteLine();

                PlayerTurn(currentPlayer);
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        static void DrawBoard()
        {
            Console.WriteLine("   #  #  #  #  #  #  #  #");
            Console.WriteLine();

            for (int y = boardWidth - 1; y >= 0; y--)
            {
                Console.Write("#  ");
                for (int x = 0; x < boardWidth; x++)
                {
                    string playerMarker = string.Empty;

                    for (int i = 0; i < NumPlayers; i++)
                    {
                        Player player = players[i];

                        if (player.X == x && player.Y == y)
                        {
                            playerMarker = (i + 1).ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(playerMarker))
                    {
                        Tile tile = board[y, x];

                        switch (tile)
                        {
                            case Tile.Down:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write(".  ");
                                break;
                            case Tile.Up:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write("^  ");
                                break;
                            case Tile.Right:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write(">  ");
                                break;
                            case Tile.Left:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write("<  ");
                                break;
                            case Tile.Win:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write("@  ");
                                break;
                            case Tile.CheeseUp:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("#  ");
                                break;
                            case Tile.CheeseRight:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("#  ");
                                break;
                            case Tile.CheeseLeft:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("#  ");
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(playerMarker + "  ");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine("#");
                Console.WriteLine();
            }

            Console.WriteLine("   #  #  #  #  #  #  #  #");
        }


        static void Main()
        {
            do
            {
                //Introductory text
                Console.WriteLine(" * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("               HYPERSPACE CHEESE BATTLE IS ABOUT TO COMMENCE!");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You have the honor of becoming a new pilot in the Cheese Wars of 2013, your\r\nfriends can join you, but there are some budding young pilots ready to join you if you're alone! But be wary of Clever Trevor, he has some really cheesy tactics");
                Console.ForegroundColor = ConsoleColor.White;
                ResetGame();

                Console.WriteLine("");

                int turn = 1;

                //Loop which cycles player turns, checks if the players have won and draws/updates the board appropriately
                do
                {
                    Console.Clear();
                    DrawBoard();

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" Turn " + turn);
                    Console.WriteLine(" -------");
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;

                    bool hasWon = false;

                    foreach (Player player in players)
                    {
                        hasWon = PlayerTurn(player);

                        if (hasWon == true)
                        {
                            // Breaks from player loop                     
                            break;
                        }

                    }

                    Console.WriteLine();




                    if (hasWon == true)
                    {
                        // breaks from turn loop
                        break;
                    }

                    turn++;

                    Console.ReadLine();

                }
                while (true);

                //Asks the player if they want to play again, if so they game restarts, if not the game ends.
                if (AskYesNoQuestion("Do you want to play again?"))
                {
                    Console.Clear();

                }
                else
                {
                    break;
                }
            }
            while (true);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("End.");
            Console.ReadLine();

        }



    }
}

/* Developer: Elliott Revell  */






