using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System;
using UnityEditor;
using UnityEngine;
using static Classes;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class GameData //Model to display Data in Game Lobby.
{
    public Guid GameID { get; set; }
    public int GameNumber { get; set; }
    public string UserNamesAndColor { get; set; } = "";
    public string IsGameStarted { get; set; } = "";

    public class GameDataCompare : IComparer<GameData>
    { 
        public int Compare(GameData x, GameData y)
        {
            return x.GameID.ToString().CompareTo(y.GameID.ToString());
        }
    }


}
public class JoinGame : MonoBehaviour
{
    public TMP_Text NumberOfGamesLabel;
    public TMP_InputField LoadGamesInput;
    public UnityEngine.UI.Button NewGame;

    public TMP_Text GameInfo1;
    public UnityEngine.UI.Button JoinAGame1;
    public UnityEngine.UI.Button Delete1;

    public TMP_Text GameInfo2;
    public UnityEngine.UI.Button JoinAGame2;
    public UnityEngine.UI.Button Delete2;

    public TMP_Text GameInfo3;
    public UnityEngine.UI.Button JoinAGame3;
    public UnityEngine.UI.Button Delete3;

    public TMP_Text GameInfo4;
    public UnityEngine.UI.Button JoinAGame4;
    public UnityEngine.UI.Button Delete4;


    public List<GameData> entities = new List<GameData>();

    List<Game> gameList;
    List<PlayerGame> pgList;
    List<Player> playerList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Classes.APILinks.GameID = Guid.Empty;
        StartCoroutine("Reload"); //Reload Page elements in case there are any new changes
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void btnGo() //Displays games
    {
        StartCoroutine("Reload");
    }
    public void BtnJoinGame(Guid GameID)
    {
        Classes.APILinks.GameID = GameID;
        StartCoroutine("PlayGame");
    }

    public void BtnCreateGame()
    {
        Classes.APILinks.GameID = Guid.Empty;
        StartCoroutine("CreateGame");
    }
    public void BtnDeleteGame(Guid GameID)
    {
        Classes.APILinks.GameID = GameID;
        Game game = gameList.Where(data => data.Id == Classes.APILinks.GameID).First(); //Need to set GameID
        //Game game = gameList.Where(data => data.Id == Classes.APILinks.GameID).First(); //Need to set GameID
        HttpClient client = new HttpClient();
        HttpResponseMessage response = client.DeleteAsync(Classes.APILinks.APIAddress + "game/" + Classes.APILinks.GameID + "/false").Result;
        Classes.APILinks.GameID = Guid.Empty;
        StartCoroutine("Reload");
    }
    public async Task CreateGame()
    {
        if (Classes.APILinks.PlayerID != Guid.Empty)
        {
            Game game = new Game()  //New Game.  All pieces start on their home squares.
            {
                Id = Guid.NewGuid(),
                PlayerTurn = "y",
                DieRoll = 6,
                GameStartDate = DateTime.Now,
                GameComplete = 0,
                YellowHomeSquare1 = 1,
                YellowHomeSquare2 = 1,
                YellowHomeSquare3 = 1,
                YellowHomeSquare4 = 1,
                BlueHomeSquare1 = 1,
                BlueHomeSquare2 = 1,
                BlueHomeSquare3 = 1,
                BlueHomeSquare4 = 1,
                RedHomeSquare1 = 1,
                RedHomeSquare2 = 1,
                RedHomeSquare3 = 1,
                RedHomeSquare4 = 1,
                GreenHomeSquare1 = 1,
                GreenHomeSquare2 = 1,
                GreenHomeSquare3 = 1,
                GreenHomeSquare4 = 1,
                BlueStartSquare = "",
                YellowStartSquare = "",
                RedStartSquare = "",
                GreenStartSquare = "",
                Square1 = "",
                Square2 = "",
                Square3 = "",
                Square4 = "",
                Square5 = "",
                Square6 = "",
                Square7 = "",
                Square8 = "",
                Square9 = "",
                Square10 = "",
                Square11 = "",
                Square12 = "",
                Square13 = "",
                Square14 = "",
                Square15 = "",
                Square16 = "",
                Square17 = "",
                Square18 = "",
                Square19 = "",
                Square20 = "",
                Square21 = "",
                Square22 = "",
                Square23 = "",
                Square24 = "",
                YellowCenterSquare1 = 0,
                YellowCenterSquare2 = 0,
                YellowCenterSquare3 = 0,
                YellowCenterSquare4 = 0,
                BlueCenterSquare1 = 0,
                BlueCenterSquare2 = 0,
                BlueCenterSquare3 = 0,
                BlueCenterSquare4 = 0,
                RedCenterSquare1 = 0,
                RedCenterSquare2 = 0,
                RedCenterSquare3 = 0,
                RedCenterSquare4 = 0,
                GreenCenterSquare1 = 0,
                GreenCenterSquare2 = 0,
                GreenCenterSquare3 = 0,
                GreenCenterSquare4 = 0,
            };
            
            //Create new game object in API
            HttpClient client = new HttpClient();
            bool rollback = false;
            string serializedObject = JsonConvert.SerializeObject(game);
            var content = new StringContent(serializedObject);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(new Uri(Classes.APILinks.APIAddress + "game/" + rollback), content);
            string result = response.Content.ReadAsStringAsync().Result;
            result = result.Replace("\'", "").Replace("\\", "").Replace("\"", "").Trim();
            Classes.APILinks.GameID = new Guid(result); //Assign GameID so that PlayGame page will load correct game

            //Add PlayerID and GameID to PlayerGameTable, so new Game has you assigned as a player.  Default PlayerColor is Yellow
            rollback = false;
            serializedObject = JsonConvert.SerializeObject(new PlayerGame { GameId = new Guid(result), PlayerId = Classes.APILinks.PlayerID, PlayerColor = "yellow" });
            content = new StringContent(serializedObject);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await client.PostAsync(new Uri(Classes.APILinks.APIAddress + "playergame/" + rollback), content);
            result = response.Content.ReadAsStringAsync().Result;
            //result = result.Replace("\'", "").Replace("\\", "").Replace("\"", "").Trim();

            SceneManager.LoadScene("Play");

        }
    }

    public async Task PlayGame()
    {

        entities = new List<GameData>(); //reset values put onto JoinGamePage rows/cols

        if (Classes.APILinks.GameID != Guid.Empty) //&& App.PlayerID != Guid.Empty)
        {
            Game game = gameList.Where(data => data.Id == Classes.APILinks.GameID).First();
            List<PlayerGame> limit = pgList.Where(pg => pg.GameId == Classes.APILinks.GameID).ToList();
            List<Player> CurrentPlayers = new List<Player>();
            string currentColors = "";
            foreach (PlayerGame pg in limit)
            {
                CurrentPlayers.Add(playerList.Where(pl => pl.Id == pg.PlayerId).First());
                currentColors += pg.PlayerColor.Substring(0, 1).ToLower();
            }
            if (CurrentPlayers.Count >= 4 || Classes.Game.hasGameStarted(game) || Classes.APILinks.PlayerID == Guid.Empty)
            {
                //Unable to Join Game.  4 Player Max.  Can't join a started game.  Join as Observer
                SceneManager.LoadScene("Play");
            }
            else if (CurrentPlayers.Count < 4 && Classes.APILinks.PlayerID != Guid.Empty && CurrentPlayers.Find(p => p.Id == Classes.APILinks.PlayerID) == null) //Only add to player Game table if player is not already part of game
            {
                PlayerGame pg = new PlayerGame
                {
                    IsComputerPlaying = false,
                    GameId = Classes.APILinks.GameID,
                    PlayerId = Classes.APILinks.PlayerID
                };
                //Join game.  Need to join with an appropriate unused color
                if (!currentColors.Contains("y"))
                {
                    pg.PlayerColor = "y";
                }
                else if (!currentColors.Contains("b"))
                {
                    pg.PlayerColor = "b";
                }
                else if (!currentColors.Contains("r"))
                {
                    pg.PlayerColor = "r";
                }
                else if (!currentColors.Contains("g"))
                {
                    pg.PlayerColor = "g";
                }
                //Add PlayerID and GameID to PlayerGameTable, so new Game has you assigned as a player
                HttpClient client = new HttpClient();
                bool rollback = false;
                var serializedObject = JsonConvert.SerializeObject(pg);
                var content = new StringContent(serializedObject);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(new Uri(Classes.APILinks.APIAddress + "playergame/" + rollback), content);
                string result = response.Content.ReadAsStringAsync().Result;
                //result = result.Replace("\'", "").Replace("\\", "").Replace("\"", "").Trim();

            }
            SceneManager.LoadScene("Play");
        }
    }
    private async Task Reload()
    {
        entities = new List<GameData>();
        HttpClient client = new HttpClient();
        string responseBody = await client.GetStringAsync(new Uri(Classes.APILinks.APIAddress + "game/"));
        string responseBody2 = await client.GetStringAsync(new Uri(Classes.APILinks.APIAddress + "playergame/"));
        string responseBody3 = await client.GetStringAsync(new Uri(Classes.APILinks.APIAddress + "player/"));
        if (responseBody != null && responseBody2 != null && responseBody3 != null)
        {
            gameList = JsonConvert.DeserializeObject<List<Game>>(responseBody);
            pgList = JsonConvert.DeserializeObject<List<PlayerGame>>(responseBody2);
            playerList = JsonConvert.DeserializeObject<List<Player>>(responseBody3);

            for (int i = 0; i < pgList.Count; i++) //Filling entities will all the data to display on the Page
            {
                //GameData object was made for displaying the data on the grid rows/cols on the JoinGameRoompage
                GameData dataInEntities = entities.Where(data => data.GameID == pgList[i].GameId).FirstOrDefault() ?? new GameData();
                if (dataInEntities.UserNamesAndColor != "")
                {
                    entities.Remove(dataInEntities); //Remove entity, to add more data to the UserNames and Color.  The same entity (row), will have UserNamesAndColors adde multiple times
                    string userName = playerList.Where(data => data.Id == pgList[i].PlayerId).First().UserName;
                    dataInEntities.UserNamesAndColor += userName + " :" + pgList[i].PlayerColor + " | "; //Adding to the UserNamesAndColors
                    entities.Add(dataInEntities);
                }
                else
                {
                    GameData gameData = new GameData();

                    gameData.GameID = pgList[i].GameId;
                    gameData.IsGameStarted = "false";
                    Game game = gameList.Where(g => g.Id == pgList[i].GameId).FirstOrDefault();
                    gameData.IsGameStarted = Classes.Game.hasGameStarted(game) ? "true" : "false";
                    string userName = playerList.Where(data => data.Id == pgList[i].PlayerId).First().UserName;
                    gameData.UserNamesAndColor += userName + " :" + pgList[i].PlayerColor + " | ";
                    entities.Add(gameData);
                }
            }
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].GameNumber = i + 1;
            }
        }
        entities.Sort(new GameData.GameDataCompare());
        int start = 0;
        if(!int.TryParse(LoadGamesInput.text, out start))
        {
            Rebind();
        }
        else if(start > entities.Count - 1 || start < entities.Count - 1)
        {
            EditorUtility.DisplayDialog("Rebind", "Game Loading Value is set out of range!", "OK");
            Rebind();
        }
        else
        {
            Rebind(start);
        }

    }
    private void Rebind(int startItem = 0) //Starts at a 0 Index //Uses public List<GameData> entities, for the data it displays
    {
        NumberOfGamesLabel.text = $"Go to Games(0-{entities.Count - 1})";
        LoadGamesInput.text = "";
        int endCount = startItem + 3 < entities.Count - 1? startItem + 3 : entities.Count - 1;
        int count = 0;
        for (int i = startItem; i < endCount; i++) //Add these controls for each row
        {
            string label = entities[i].GameNumber.ToString() + " " +
                entities[i].UserNamesAndColor +
                "\n" + entities[i].IsGameStarted == "true" ? "Started" : "Unstarted";

            if (count == 0)
            {
                GameInfo1.text = label;
                JoinAGame1.onClick.AddListener(() => BtnJoinGame(entities[i].GameID));
                Delete1.onClick.AddListener(() => BtnDeleteGame(entities[i].GameID));
            }
            else if (count == 1)
            {
                GameInfo2.text = label;
                JoinAGame2.onClick.AddListener(() => BtnJoinGame(entities[i].GameID));
                Delete2.onClick.AddListener(() => BtnDeleteGame(entities[i].GameID));
            }
            else if (count == 2)
            {
                GameInfo3.text = label;
                JoinAGame3.onClick.AddListener(() => BtnJoinGame(entities[i].GameID));
                Delete3.onClick.AddListener(() => BtnDeleteGame(entities[i].GameID));
            }
            else if (count == 3)
            {
                GameInfo4.text = label;
                JoinAGame4.onClick.AddListener(() => BtnJoinGame(entities[i].GameID));
                Delete4.onClick.AddListener(() => BtnDeleteGame(entities[i].GameID));
                return;
            }
            count++;
        }
        NewGame.onClick.AddListener(() => BtnCreateGame());

    }
}


