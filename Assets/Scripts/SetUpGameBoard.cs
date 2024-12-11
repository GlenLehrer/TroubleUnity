using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using static Classes;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Threading.Tasks;
using System.Linq;
using TMPro;
using UnityEngine.Rendering;
using UnityEditor;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SetUpGameBoard : MonoBehaviour
{
    public Button btnShowChat;

    public Game game;
    public GameObject gameBoard;
    public GameObject Yellow1;
    public GameObject Yellow2;
    public GameObject Yellow3;
    public GameObject Yellow4;

    public GameObject Blue1;
    public GameObject Blue2;
    public GameObject Blue3;
    public GameObject Blue4;

    public GameObject Red1;
    public GameObject Red2;
    public GameObject Red3;
    public GameObject Red4;

    public GameObject Green1;
    public GameObject Green2;
    public GameObject Green3;
    public GameObject Green4;

    public TMP_InputField txtStartSquare;
    public TMP_InputField txtEndSquare;

    public TMP_Text CurrentPlayer;
    public TMP_Text DieRoll;

    public TMP_Text Player1;
    public TMP_Text Player2;
    public TMP_Text Player3;
    public TMP_Text Player4;

    public TMP_Text DisplayMessage;
    public TMP_InputField EnterMessage;

    List<Classes.Game> gameList; //List of all games
    List<Classes.PlayerGame> pgList; //List of all PlayerGames
    List<Classes.Player> playerList; //List of all players
    List<(Classes.Player, string)> PlayersInThisGame; //List of players playing in the current game

    HubConnection _connection;
    string groupName = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Classes.APILinks.GameID == null || Classes.APILinks.GameID == Guid.Empty)
        {
            ResetToNewGameBoard();
        }
        else
        {
            StartCoroutine("FillLabels");            
            if(game != null)
            {
                SetUpBoard(game);
                ConnectToChannel();
            }
        }
    }
    public async Task FillLabels()
    {
        HttpClient client = new HttpClient();
        string responseBody = await client.GetStringAsync(new Uri(Classes.APILinks.APIAddress + "game/"));
        string responseBody2 = await client.GetStringAsync(new Uri(Classes.APILinks.APIAddress + "playergame/"));
        string responseBody3 = await client.GetStringAsync(new Uri(Classes.APILinks.APIAddress + "player/"));
        if (responseBody != null && responseBody2 != null && responseBody3 != null)
        {
            
            gameList = JsonConvert.DeserializeObject<List<Classes.Game>>(responseBody);
            pgList = JsonConvert.DeserializeObject<List<Classes.PlayerGame>>(responseBody2);
            playerList = JsonConvert.DeserializeObject<List<Classes.Player>>(responseBody3);

            PlayersInThisGame = new List<(Classes.Player, string)>();

            game = gameList.Where(g => g.Id == Classes.APILinks.GameID).First(); //Get the current game, based on the global GameID property.  App.GameID was set on the JoinGameRoomPage.
            for (int i = 0; i < pgList.Count; i++)
            {
                if (pgList[i].GameId == game.Id)
                {   //Determine which players are playing in the game
                    PlayersInThisGame.Add((playerList.FirstOrDefault(p => p.Id == pgList[i].PlayerId), pgList[i].PlayerColor.ToString()));
                }
            }
            DieRoll.text = "Die Roll: " + game.DieRoll.ToString(); //Set the Die Roll Text Box based on the game.DieRoll Property
            switch (game.PlayerTurn.ToString().ToLower().Substring(0, 1))
            {
                case "y":  //Set the color of the CurrentPlayer label, to show whose turn it is.
                    CurrentPlayer.color = Color.yellow;
                    break;
                case "b":
                    CurrentPlayer.color = Color.blue;
                    break;
                case "r":
                    CurrentPlayer.color = Color.red;
                    break;
                case "g":
                    CurrentPlayer.color = Color.green;
                    break;
            }
            for (int i = 0; i < PlayersInThisGame.Count; i++)
            {
                switch (PlayersInThisGame[i].Item2.ToLower().Substring(0, 1))
                {
                    case "y": //Change the Player1 Label Text, to say who is Playing as yellow
                        Player1.text = PlayersInThisGame[i].Item1.UserName;
                        break;
                    case "b": //Change the Player2 Label Text, to say who is Playing as blue
                        Player2.text = PlayersInThisGame[i].Item1.UserName;
                        break;
                    case "r": //Change the Player3 Label Text, to say who is Playing as red
                        Player3.text = PlayersInThisGame[i].Item1.UserName;
                        break;
                    case "g": //Change the Player4 Label Text, to say who is Playing as green
                        Player4.text = PlayersInThisGame[i].Item1.UserName;
                        break;
                }
            }
        }
    }
    public void Move()
    {
        StartCoroutine("MakeMove");
    }
    public void Skip()
    {
        StartCoroutine("SkipTurn");
    }
    private async Task MakeMove() //Add logic so only the player whose turn it is can move
    {
        if (Classes.APILinks.GameID == Guid.Empty)
        {
            EditorUtility.DisplayDialog("Error", "No game is started!", "Ok");
            return;
        }
        if (Classes.APILinks.PlayerID == Guid.Empty)
        {
            EditorUtility.DisplayDialog("Error", "Cannot move unless part of game!", "Ok");
            return;
        }
        HttpClient client = new HttpClient();

        int startSquare; 
        int endSquare; 
        if(txtStartSquare != null && txtEndSquare != null)
        {
            if (!int.TryParse(txtStartSquare.text, out startSquare) || !int.TryParse(txtEndSquare.text, out endSquare))
            {
                EditorUtility.DisplayDialog("Error", "Your Move is not Valid!", "Ok");
                return;
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Your Move is not Valid!", "Ok");
            return;
        }

        
        game.Id = Classes.APILinks.GameID;
        game = gameList.Where(g => g.Id == Classes.APILinks.GameID).First();
        if (game.GameComplete == 1)
        {
            SetUpBoard(game);
            EditorUtility.DisplayDialog("Winner", "Game has been won!", "Ok");
            return;
        }
        bool keepGoing = false;
        foreach (var item in PlayersInThisGame) //Make sure only the player whose is part of the game and of the right color can make a move
        {
            if (item.Item1.Id == Classes.APILinks.PlayerID)
            {
                if (game.PlayerTurn.Substring(0, 1).ToLower() == item.Item2.Substring(0, 1).ToLower())
                {
                    keepGoing = true;
                }
            }

        }
        if (!keepGoing)
        {
            EditorUtility.DisplayDialog("Error", "You can't move for someone else", "Ok");
            return;
        }

        string serializedObject = JsonConvert.SerializeObject(game);
        var content = new StringContent(serializedObject);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = client.PutAsync(new Uri(Classes.APILinks.APIAddress + "Game/" + startSquare + "/" + endSquare + "/1"), content).Result;
        string result = response.Content.ReadAsStringAsync().Result;

        if (response.StatusCode == System.Net.HttpStatusCode.OK && result == "true")
        {
            string responseBody = await client.GetStringAsync(new Uri(Classes.APILinks.APIAddress + "game/"));
            if (responseBody != null)
            {

                gameList = JsonConvert.DeserializeObject<List<Game>>(responseBody);
                game = gameList.Where(g => g.Id == Classes.APILinks.GameID).First();
           
                StartCoroutine("FillLabels");
                if (game != null)
                    SetUpBoard(game);

               UpdateOtherGameBoards();
                if (game.GameComplete == 1)
                {
                    EditorUtility.DisplayDialog("Winner", "Game has been won!", "Ok");
                    return;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Errror", "Data Not Retrieved", "Ok");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Errror", "Your Move is not Valid!", "Ok");
        }

    }
    public void AddComp()
    {
        StartCoroutine("AddComputer");
    }
    private async Task AddComputer()
    {
        if (Classes.APILinks.GameID == Guid.Empty)
        {
            EditorUtility.DisplayDialog("Error", "No game created to add a computer to", "Ok");
            return;
        }
        else if (game.GameComplete == 1)
        {
            EditorUtility.DisplayDialog("Winner", "Game has been won!", "Ok");
            return;
        }
        else if (Classes.Game.hasGameStarted(game))
        {
            EditorUtility.DisplayDialog("Error", "Game has started.  Cannot Add new players.", "Ok");
            return;
        }
        else if (Classes.APILinks.PlayerID == Guid.Empty)
        {
            EditorUtility.DisplayDialog("Error", "Must be logged in to add a computer player!", "Ok");
            return;
        }
        else
        {
            bool keepGoing = false;
            foreach (var item in PlayersInThisGame) //Make sure only the player whose is part of the game and of the right color can make a move
            {
                if (item.Item1.Id == Classes.APILinks.PlayerID)
                {
                    if (game.PlayerTurn.Substring(0, 1).ToLower() == item.Item2.Substring(0, 1).ToLower())
                    {
                        keepGoing = true;
                    }
                }

            }
            if (!keepGoing)
            {
                EditorUtility.DisplayDialog("Error", "Must be part of game to add a computer player", "Ok");
                return;
            }
        }



        string ColorsInUse = "";
        string ComputerColor = "";
        for (int i = 0; i < PlayersInThisGame.Count; i++)
        {
            ColorsInUse += PlayersInThisGame[i].Item2.Trim().Substring(0, 1).ToLower();
        }
        if (ColorsInUse.Length >= 4)
        {
            EditorUtility.DisplayDialog("Error", "Already 4 players.  Cannot Add a Computer.", "Ok");
            return;
        }
        if (!ColorsInUse.Contains("y"))
        {
            ComputerColor = "y";
        }
        else if (!ColorsInUse.Contains("b"))
        {
            ComputerColor = "b";
        }
        else if (!ColorsInUse.Contains("r"))
        {
            ComputerColor = "r";
        }
        else if (!ColorsInUse.Contains("g"))
        {
            ComputerColor = "g";
        }
        //Add Computer PlayerID and GameID to PlayerGameTable, so new Game has you assigned as a player
        PlayerGame pg = new PlayerGame
        {
            IsComputerPlaying = true,
            GameId = Classes.APILinks.GameID,
            PlayerId = new Guid("11111111-1111-1111-1111-111111111111"),
            PlayerColor = ComputerColor
        };

        HttpClient client = new HttpClient();
        bool rollback = false;
        var serializedObject = JsonConvert.SerializeObject(pg);
        var content = new StringContent(serializedObject);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri(Classes.APILinks.APIAddress + "playergame/" + rollback), content);
        string result = response.Content.ReadAsStringAsync().Result;
        //result = result.Replace("\'", "").Replace("\\", "").Replace("\"", "").Trim();

        StartCoroutine("FillLabels");
        UpdateOtherGameBoards();
    }

    private async Task SkipTurn()
    {
        if (Classes.APILinks.GameID == Guid.Empty)
        {
            EditorUtility.DisplayDialog("Error", "Must be in a game to skip a move!", "Ok");
            return;
        }
        else if (Classes.APILinks.GameID == Guid.Empty)
        {
            EditorUtility.DisplayDialog("Error", "Must be logged in to skip a move!", "Ok");
            return;
        }
        if (game != null && game.GameComplete == 1)
        {
            if (game != null)
                SetUpBoard(game);
            EditorUtility.DisplayDialog("Winner", "Game has been won!", "Ok");
            return;
        }

        game.Id = Classes.APILinks.GameID;
        game = gameList.Where(g => g.Id == Classes.APILinks.GameID).First();

        bool keepGoing = false;
        foreach (var item in PlayersInThisGame) //Make sure only the player whose is part of the game and of the right color can skip a turn
        {
            if (item.Item1.Id == Classes.APILinks.PlayerID)
            {
                if (game.PlayerTurn.Substring(0, 1).ToLower() == item.Item2.Substring(0, 1).ToLower())
                {
                    keepGoing = true;
                }
            }

        }
        if (!keepGoing)
        {
            EditorUtility.DisplayDialog("Error", "You can't skip someone else's move", "Ok");
            return;
        }

        HttpClient client = new HttpClient();
        game.Id = Classes.APILinks.GameID;
        game = gameList.Where(g => g.Id == Classes.APILinks.GameID).First();

        string serializedObject = JsonConvert.SerializeObject(game);
        var content = new StringContent(serializedObject);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = client.PutAsync(new Uri(Classes.APILinks.APIAddress + "Game/skip"), content).Result;
        string result = response.Content.ReadAsStringAsync().Result;

        if (response.StatusCode == System.Net.HttpStatusCode.OK && result == "true")
        {
            string responseBody = await client.GetStringAsync(new Uri(Classes.APILinks.APIAddress + "game/"));
            if (responseBody != null)
            {
                gameList = JsonConvert.DeserializeObject<List<Game>>(responseBody);
                game = gameList.Where(g => g.Id == Classes.APILinks.GameID).First();
                StartCoroutine("FillLabels");
                if (game != null)
                    SetUpBoard(game);
                UpdateOtherGameBoards();
            }
        }
    }
    //Update Make Move, Skip Turn, Play Computer, OnAppearing, OnDisappearing
    //*******************************
    //*******************************

    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************//*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************
    //*******************************

    //*******************************
    //*******************************
    //*******************************

    // Update is called once per frame
    void Update()
    {
        //Testing Move function
        //if (!MoveSquare(Yellow1, BoardPositions.RedHomeSquare4)) {}

    }
    bool MoveSquare(GameObject piece, Vector3 EndSpot)
    {
        float speed = 150;
        Vector3 EndingPosition = EndSpot;

        if (piece.transform.position != EndingPosition)
        {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, EndingPosition, Time.deltaTime * speed);
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ResetToNewGameBoard()
    {
            Yellow1.transform.position = BoardPositions.YellowHomeSquare1;
            Yellow2.transform.position = BoardPositions.YellowHomeSquare2;
            Yellow3.transform.position = BoardPositions.YellowHomeSquare3;
            Yellow4.transform.position = BoardPositions.YellowHomeSquare4;

            Blue1.transform.position = BoardPositions.BlueHomeSquare1;
            Blue2.transform.position = BoardPositions.BlueHomeSquare2;
            Blue3.transform.position = BoardPositions.BlueHomeSquare3;
            Blue4.transform.position = BoardPositions.BlueHomeSquare4;

            Red1.transform.position = BoardPositions.RedHomeSquare1;
            Red2.transform.position = BoardPositions.RedHomeSquare2;
            Red3.transform.position = BoardPositions.RedHomeSquare3;
            Red4.transform.position = BoardPositions.RedHomeSquare4;

            Green1.transform.position = BoardPositions.GreenHomeSquare1;
            Green2.transform.position = BoardPositions.GreenHomeSquare2;
            Green3.transform.position = BoardPositions.GreenHomeSquare3;
            Green4.transform.position = BoardPositions.GreenHomeSquare4;
    }
    public void SetUpBoard(Game game)
    {
        if (game != null) //Draw Rectangles/Circles for each square is filled, determined by Game data
        {
            String[] GameState = new String[60];
            GameState = MakeGameState(game); //Make game object into an array

            int CurrentYellowPieceIndex = 0;
            GameObject[] YellowPieces = new GameObject[4] {Yellow1, Yellow2, Yellow3, Yellow4 };

            int CurrentBluePieceIndex = 0;
            GameObject[] BluePieces = new GameObject[4] { Blue1, Blue2, Blue3, Blue4 };

            int CurrentRedPieceIndex = 0;
            GameObject[] RedPieces = new GameObject[4] { Red1, Red2, Red3, Red4 };

            int CurrentGreenPieceIndex = 0;
            GameObject[] GreenPieces = new GameObject[4] { Green1, Green2, Green3, Green4 };

            Vector3[] Coordinates = FillCoordinates();
            for (int i = 0; i < GameState.Length; i++) //For each square in the game object
            {
                if (GameState[i].Length >= 1) //If a square is full, must not be an emptyu string
                {
                    switch (GameState[i].Substring(0, 1).ToLower()) //first character for color of game square
                    {
                        case "y": //Game Square has a yellow piece in it
                                  //Put a Yellow piece on the right square
                            if (CurrentYellowPieceIndex <= 3)
                            {
                                YellowPieces[i].transform.position = Coordinates[i];
                            }
                            CurrentYellowPieceIndex++;
                            break;
                        case "b"://Game Square has a blue piece in it
                                 //Put a Blue piece on the right square
                            if (CurrentBluePieceIndex <= 3)
                            {
                                BluePieces[i].transform.position = Coordinates[i];
                            }
                            CurrentBluePieceIndex++;
                            break;
                        case "r"://Game Square has a red piece in it
                                 //Put a Red piece on the right square
                            if (CurrentRedPieceIndex <= 3)
                            {
                                RedPieces[i].transform.position = Coordinates[i];
                            }
                            CurrentRedPieceIndex++;
                            break;
                        case "g"://Game Square has a green piece in it
                                 //Put a Green piece on the right square
                            if(CurrentGreenPieceIndex <= 3)
                            {
                                GreenPieces[i].transform.position = Coordinates[i];
                            }
                            
                            CurrentGreenPieceIndex++;
                            break;
                        default: break;
                    }
                }
            }
        }
    }
    public String[] MakeGameState(Game game) //Game object with its squares listed in an Array Form.
    {
        String[] gameState = new String[60];
        gameState[0] = game.YellowStartSquare;
        gameState[1] = game.Square1;
        gameState[2] = game.Square2;
        gameState[3] = game.Square3; //Protected Square
        gameState[4] = game.Square4;
        gameState[5] = game.Square5;
        gameState[6] = game.Square6;

        gameState[7] = game.BlueStartSquare;
        gameState[8] = game.Square7;
        gameState[9] = game.Square8;
        gameState[10] = game.Square9;  //Protected Square
        gameState[11] = game.Square10;
        gameState[12] = game.Square11;
        gameState[13] = game.Square12;

        gameState[14] = game.RedStartSquare;
        gameState[15] = game.Square13;
        gameState[16] = game.Square14;
        gameState[17] = game.Square15;  //Protected Square
        gameState[18] = game.Square16;
        gameState[19] = game.Square17;
        gameState[20] = game.Square18;

        gameState[21] = game.GreenStartSquare;
        gameState[22] = game.Square19;
        gameState[23] = game.Square20;
        gameState[24] = game.Square21;  //Protected Square
        gameState[25] = game.Square22;
        gameState[26] = game.Square23;
        gameState[27] = game.Square24;

        gameState[28] = game.YellowHomeSquare1 == 1 ? "y" : "";
        gameState[29] = game.YellowHomeSquare2 == 1 ? "y" : "";
        gameState[30] = game.YellowHomeSquare3 == 1 ? "y" : "";
        gameState[31] = game.YellowHomeSquare4 == 1 ? "y" : "";
        gameState[32] = game.GreenHomeSquare1 == 1 ? "g" : "";
        gameState[33] = game.GreenHomeSquare2 == 1 ? "g" : "";
        gameState[34] = game.GreenHomeSquare3 == 1 ? "g" : "";
        gameState[35] = game.GreenHomeSquare4 == 1 ? "g" : "";
        gameState[36] = game.RedHomeSquare1 == 1 ? "r" : "";
        gameState[37] = game.RedHomeSquare2 == 1 ? "r" : "";
        gameState[38] = game.RedHomeSquare3 == 1 ? "r" : "";
        gameState[39] = game.RedHomeSquare4 == 1 ? "r" : "";
        gameState[40] = game.BlueHomeSquare1 == 1 ? "b" : "";
        gameState[41] = game.BlueHomeSquare2 == 1 ? "b" : "";
        gameState[42] = game.BlueHomeSquare3 == 1 ? "b" : "";
        gameState[43] = game.BlueHomeSquare4 == 1 ? "b" : "";

        gameState[44] = game.YellowCenterSquare1 == 1 ? "y" : "";
        gameState[45] = game.YellowCenterSquare2 == 1 ? "y" : "";
        gameState[46] = game.YellowCenterSquare3 == 1 ? "y" : "";
        gameState[47] = game.YellowCenterSquare4 == 1 ? "y" : "";
        gameState[48] = game.GreenCenterSquare1 == 1 ? "g" : "";
        gameState[49] = game.GreenCenterSquare2 == 1 ? "g" : "";
        gameState[50] = game.GreenCenterSquare3 == 1 ? "g" : "";
        gameState[51] = game.GreenCenterSquare4 == 1 ? "g" : "";
        gameState[52] = game.RedCenterSquare1 == 1 ? "r" : "";
        gameState[53] = game.RedCenterSquare2 == 1 ? "r" : "";
        gameState[54] = game.RedCenterSquare3 == 1 ? "r" : "";
        gameState[55] = game.RedCenterSquare4 == 1 ? "r" : "";
        gameState[56] = game.BlueCenterSquare1 == 1 ? "b" : "";
        gameState[57] = game.BlueCenterSquare2 == 1 ? "b" : "";
        gameState[58] = game.BlueCenterSquare3 == 1 ? "b" : "";
        gameState[59] = game.BlueCenterSquare4 == 1 ? "b" : "";

        return gameState;
    }
    private Vector3[] FillCoordinates() //Coordinates for each circle/square on the game Image
    {
        Vector3[] XYvalues = new Vector3[60];
        XYvalues[0] = new Vector3(-232, 36, 248);
        XYvalues[1] = new Vector3(-132, 36, 261);
        XYvalues[2] = new Vector3(-59, 36, 259);
        XYvalues[3] = new Vector3(12, 36, 259);
        XYvalues[4] = new Vector3(92, 36, 259);
        XYvalues[5] = new Vector3(169, 36, 259);
        XYvalues[6] = new Vector3(235, 36, 240);
        XYvalues[7] = new Vector3(295, 36, 162);
        XYvalues[8] = new Vector3(299, 36, 105);
        XYvalues[9] = new Vector3(297, 36, 53);
        XYvalues[10] = new Vector3(297, 36, 2);
        XYvalues[11] = new Vector3(297, 36, -62);
        XYvalues[12] = new Vector3(297, 36, -125);
        XYvalues[13] = new Vector3(275, 36, -173);
        XYvalues[14] = new Vector3(233, 36, -216);
        XYvalues[15] = new Vector3(157, 36, -225);
        XYvalues[16] = new Vector3(75, 36, -225);
        XYvalues[17] = new Vector3(-1, 36, -223);
        XYvalues[18] = new Vector3(-76, 36, -223);
        XYvalues[19] = new Vector3(-139, 36, -225);
        XYvalues[20] = new Vector3(-230, 36, -217);
        XYvalues[21] = new Vector3(-288, 36, -147);
        XYvalues[22] = new Vector3(-295, 36, -97);
        XYvalues[23] = new Vector3(-295, 36, -38);
        XYvalues[24] = new Vector3(-295, 36, 22);
        XYvalues[25] = new Vector3(-295, 36, 82);
        XYvalues[26] = new Vector3(-295, 36, 135);
        XYvalues[27] = new Vector3(-293, 36, 187);
        XYvalues[28] = new Vector3(-362, 36, 204);
        XYvalues[29] = new Vector3(-361, 36, 241);
        XYvalues[30] = new Vector3(-325, 36, 234);
        XYvalues[31] = new Vector3(-321, 36, 277);
        XYvalues[32] = new Vector3(-328, 36, -270);
        XYvalues[33] = new Vector3(-306, 36, -223);
        XYvalues[34] = new Vector3(-355, 36, -232);
        XYvalues[35] = new Vector3(-385, 36, -196);
        XYvalues[36] = new Vector3(357, 36, -195);
        XYvalues[37] = new Vector3(351, 36, -240);
        XYvalues[38] = new Vector3(314, 36, -236);
        XYvalues[39] = new Vector3(314, 36, -270);
        XYvalues[40] = new Vector3(314, 36, 280);
        XYvalues[41] = new Vector3(346, 36, 240);
        XYvalues[42] = new Vector3(333, 36, 197);
        XYvalues[43] = new Vector3(374, 36, 204);

        XYvalues[44] = new Vector3(-195, 36, 178);
        XYvalues[45] = new Vector3(-154, 36, 150);
        XYvalues[46] = new Vector3(-111, 36, 109);
        XYvalues[47] = new Vector3(-66, 36, 78);

        XYvalues[48] = new Vector3(-191, 36, -130);
        XYvalues[49] = new Vector3(-145, 36, -95);
        XYvalues[50] = new Vector3(-100, 36, -67);
        XYvalues[51] = new Vector3(-56, 36, -27);
        XYvalues[52] = new Vector3(216, 36, -152);
        XYvalues[53] = new Vector3(169, 36, -122);
        XYvalues[54] = new Vector3(125, 36, -76);
        XYvalues[55] = new Vector3(85, 36, -40);
        XYvalues[56] = new Vector3(217, 36, 174);
        XYvalues[57] = new Vector3(178, 36, 140);
        XYvalues[58] = new Vector3(117, 36, 100);
        XYvalues[59] = new Vector3(82, 36, 67);
        return XYvalues;
    }

        private async void ConnectToChannel()
        {

            _connection = new HubConnectionBuilder()
                .WithUrl(Classes.APILinks.hubAddress)
                .Build();
            await _connection.StartAsync();
            groupName = "g" + Classes.APILinks.GameID.ToString();
            await _connection.InvokeAsync("JoinGroup", groupName);
            _connection.On<string, string>("ReceiveMessage", (s1, s2) => UpdateMessages(s1, s2));
            _connection.On("UpdateGameBoard", async () => await UpdateGameBoard());
            _connection.On("GroupJoined", async () => await GroupJoined());

        //_connection.InvokeAsync("UpdateGameBoard", groupName);
        //_connection.InvokeAsync("SendMessage", groupName, Classes.APILinks.PlayerUserName, "Message Text");
    }

    public void ShowChat()
    {
        btnShowChat.gameObject.SetActive(!btnShowChat.IsActive());
        GameObject CanvasParent = GameObject.Find("CanvasParent");
        GameObject ChatMenu = CanvasParent.transform.Find("ChatMenu").gameObject;
        ChatMenu.gameObject.SetActive(!ChatMenu.active);
    }
    private async Task GroupJoined() //When I recieve messages
    {
        await FillLabels();
    }
    public void BtnChat()
    {
        if (Classes.APILinks.PlayerID != Guid.Empty && !string.IsNullOrEmpty(Classes.APILinks.PlayerUserName) && Classes.APILinks.GameID != Guid.Empty && _connection != null)
        {
            StartCoroutine("SendMessageToChannel", EnterMessage.text);
            EnterMessage.text = "";
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Must be Logged in & at a game to Chat!", "Ok");
        }
    }
    private async Task UpdateOtherGameBoards() //When I change my GameBoard
    {
        await _connection.InvokeAsync("UpdateGameBoard", groupName);
    }
    private async void SendMessageToChannel(string message) //When I write a message
    {
        string groupName = "g" + Classes.APILinks.GameID.ToString();
        await _connection.InvokeAsync("SendMessage", groupName, Classes.APILinks.PlayerUserName, message);
    }

    private void UpdateMessages(string name, string message) //When I recieve messages
    {
        try
        {          
            DisplayMessage.text += name + " : " + message + " \n";

        }
        catch (Exception ex)
        {
        }
    }
    private async Task UpdateGameBoard() //When I receive changes to Game Board from other players.
    {
        HttpClient client = new HttpClient();
        string responseBody = await client.GetStringAsync(new Uri(Classes.APILinks.APIAddress + "game/"));
        if (responseBody != null)
        {
            List<Classes.Game> gameList = JsonConvert.DeserializeObject<List<Classes.Game>>(responseBody).ToList(); ;
            Classes.Game game = gameList.Where(g => g.Id == Classes.APILinks.GameID).First();

            StartCoroutine("FillLabels");
            if (game != null)
                SetUpBoard(game);
        }
    }


}

public static class BoardPositions  //Coordinates of each square on the game board
{
public static Vector3 YellowHomeSquare1 = new Vector3(-362, 36, 204); //Square 28
public static Vector3 YellowHomeSquare2 = new Vector3(-361, 36, 241); //Square 29
public static Vector3 YellowHomeSquare3 = new Vector3(-325, 36, 234); //Square 30
public static Vector3 YellowHomeSquare4 = new Vector3(-321, 36, 277); //Square 31

public static Vector3 BlueHomeSquare1 = new Vector3(314, 36, 280);  //Square 40
public static Vector3 BlueHomeSquare2 = new Vector3(346, 36, 240);  //Square 41
public static Vector3 BlueHomeSquare3 = new Vector3(333, 36, 197);  //Square 42
public static Vector3 BlueHomeSquare4 = new Vector3(374, 36, 204);  //Square 43

public static Vector3 RedHomeSquare1 = new Vector3(357, 36, -195);  //Square 36
public static  Vector3 RedHomeSquare2 = new Vector3(351, 36, -240); //Square 37
public static Vector3 RedHomeSquare3 = new Vector3(314, 36, -236);  //Square 38
public static Vector3 RedHomeSquare4 = new Vector3(314, 36, -270);  //Square 39

public static Vector3 GreenHomeSquare1 = new Vector3(-328, 36, -270); //Square 32
public static Vector3 GreenHomeSquare2 = new Vector3(-306, 36, -223); //Square 33
public static Vector3 GreenHomeSquare3 = new Vector3(-355,36, -232);  //Square 34
public static Vector3 GreenHomeSquare4 = new Vector3(-385, 36, -196); //Square 35

public static Vector3 Square0 = new Vector3(-232, 36, 248); //Yellow Start Square
public static Vector3 Square1 = new Vector3(-132, 36, 261);
public static Vector3 Square2 = new Vector3(-59, 36, 259);
public static Vector3 Square3 = new Vector3(12, 36, 259);  //Protected Square
public static Vector3 Square4 = new Vector3(92, 36, 259);
public static Vector3 Square5 = new Vector3(169, 36, 259);
public static Vector3 Square6 = new Vector3(235, 36, 240);

public static Vector3 Square7 = new Vector3(295, 36, 162); //Blue Start Square
public static Vector3 Square8 = new Vector3(299, 36, 105);
public static Vector3 Square9 = new Vector3(297, 36, 53);
public static Vector3 Square10 = new Vector3(297, 36, 2);  //Protected Square
public static Vector3 Square11= new Vector3(297, 36, -62);
public static Vector3 Square12 = new Vector3(297, 36, -125);
public static Vector3 Square13 = new Vector3(275, 36, -173);

public static Vector3 Square14 = new Vector3(233, 36, -216);  //Red Start Square
public static Vector3 Square15 = new Vector3(157, 36, -225);
public static Vector3 Square16 = new Vector3(75, 36, -225);
public static Vector3 Square17 = new Vector3(-1, 36, -223);  //Protected Square
public static Vector3 Square18 = new Vector3(-76, 36, -223);
public static Vector3 Square19 = new Vector3(-139, 36, -225);
public static Vector3 Square20 = new Vector3(-230, 36, -217);

public static Vector3 Square21 = new Vector3(-288, 36, -147);  //Green Start Square
public static Vector3 Square22 = new Vector3(-295, 36, -97);
public static Vector3 Square23 = new Vector3(-295, 36, -38);
public static Vector3 Square24 = new Vector3(-295, 36, 22); //Protected Square
public static Vector3 Square25 = new Vector3(-295, 36, 82);
public static Vector3 Square26 = new Vector3(-295, 36, 135);
public static Vector3 Square27 = new Vector3(-293, 36, 187);

//Yellow Center Squares
public static Vector3 Squar44 = new Vector3(-195, 36, 178);
public static Vector3 Squar45 = new Vector3(-154, 36, 150);
public static Vector3 Square46 = new Vector3(-111, 36, 109);
public static Vector3 Square47 = new Vector3(-66, 36, 78);

//Blue Center Squares
public static Vector3 Square56 = new Vector3(217, 36, 174);
public static Vector3 Square57 = new Vector3(178, 36, 140);
public static Vector3 Square58 = new Vector3(117, 36, 100);
public static Vector3 Square59 = new Vector3(82, 36, 67);

//Red Center Squares
public static Vector3 Square52 = new Vector3(216, 36, -152);
public static Vector3 Square53 = new Vector3(169, 36, -122);
public static Vector3 Square54 = new Vector3(125, 36, -76);
public static Vector3 Square55 = new Vector3(85, 36, -40);

//Green Center Squares
public static Vector3 Square48 = new Vector3(-191, 36, -130);
public static Vector3 Square49 = new Vector3(-145, 36, -95);
public static Vector3 Square50 = new Vector3(-100, 36, -67);
public static Vector3 Square51 = new Vector3(-56, 36, -27);


}






