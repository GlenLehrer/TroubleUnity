using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Xml.Linq;
using UnityEngine;
using static Classes;
using TMPro;
using System.Linq;
using UnityEditor;
using System.Threading.Tasks;

public class Classes : MonoBehaviour
{
    public static class APILinks
    {
        public static string hubAddress = "https://bigprojectapi-300083331.azurewebsites.net/uihub/"; 

        public static string APIAddress = "https://bigprojectapi-300083331.azurewebsites.net/api/";
        public static Guid GameID { get; set; } = Guid.Empty;
        public static Guid PlayerID { get; set; } = Guid.Empty;
        public static string PlayerUserName { get; set; } = "";

    }
    public class Player
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int NumberOfWins { get; set; }
        public DateTime DateJoined { get; set; }

    }

    public class PlayerGame
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public string PlayerColor { get; set; }
        public bool IsComputerPlaying { get; set; } = false;

    }
    public class Game
    {
        public Guid Id { get; set; }
        public string PlayerTurn { get; set; }  //Turn Order:  Yellow, Blue, Red, Green
        public int DieRoll { get; set; }
        public DateTime GameStartDate { get; set; }
        public int GameComplete { get; set; } //0 incomplete, 1 complete

        //SQUARES ON a single Game Board
        public int YellowHomeSquare1 { get; set; }//0 nothing in square, 1 yellow piece in square
        public int YellowHomeSquare2 { get; set; }
        public int YellowHomeSquare3 { get; set; }
        public int YellowHomeSquare4 { get; set; }

        public string YellowStartSquare { get; set; }
        //Squares Starting after Yellow Start Square
        public string Square1 { get; set; }
        public string Square2 { get; set; }
        public string Square3 { get; set; }
        public string Square4 { get; set; }
        public string Square5 { get; set; }
        public string Square6 { get; set; }
        //-------------------------------------
        public int YellowCenterSquare1 { get; set; } //0 nothing in square, 1 yellow piece in square
        public int YellowCenterSquare2 { get; set; }
        public int YellowCenterSquare3 { get; set; }
        public int YellowCenterSquare4 { get; set; }

        public int GreenHomeSquare1 { get; set; } //0 nothing in square, 1 green piece in square
        public int GreenHomeSquare2 { get; set; }
        public int GreenHomeSquare3 { get; set; }
        public int GreenHomeSquare4 { get; set; }

        public string GreenStartSquare { get; set; }
        //Squares Starting after Green Start Square
        public string Square7 { get; set; }
        public string Square8 { get; set; }
        public string Square9 { get; set; }
        public string Square10 { get; set; }
        public string Square11 { get; set; }
        public string Square12 { get; set; }
        //-------------------------------------
        public int GreenCenterSquare1 { get; set; } //0 nothing in square, 1 green piece in square
        public int GreenCenterSquare2 { get; set; }
        public int GreenCenterSquare3 { get; set; }
        public int GreenCenterSquare4 { get; set; }

        public int RedHomeSquare1 { get; set; } //0 nothing in square, 1 red piece in square
        public int RedHomeSquare2 { get; set; }
        public int RedHomeSquare3 { get; set; }
        public int RedHomeSquare4 { get; set; }

        public string RedStartSquare { get; set; }
        //Squares Starting after Red Start Square
        public string Square13 { get; set; }
        public string Square14 { get; set; }
        public string Square15 { get; set; }
        public string Square16 { get; set; }
        public string Square17 { get; set; }
        public string Square18 { get; set; }
        //-------------------------------------
        public int RedCenterSquare1 { get; set; } //0 nothing in square, 1 red piece in square
        public int RedCenterSquare2 { get; set; }
        public int RedCenterSquare3 { get; set; }
        public int RedCenterSquare4 { get; set; }

        public int BlueHomeSquare1 { get; set; } //0 nothing in square, 1 blue piece in square
        public int BlueHomeSquare2 { get; set; }
        public int BlueHomeSquare3 { get; set; }
        public int BlueHomeSquare4 { get; set; }

        public string BlueStartSquare { get; set; }

        //Squares Starting after Blue Start Square
        public string Square19 { get; set; }
        public string Square20 { get; set; }
        public string Square21 { get; set; }
        public string Square22 { get; set; }
        public string Square23 { get; set; }
        public string Square24 { get; set; }
        //-------------------------------------
        public int BlueCenterSquare1 { get; set; } //0 nothing in square, 1 blue piece in square
        public int BlueCenterSquare2 { get; set; }
        public int BlueCenterSquare3 { get; set; }
        public int BlueCenterSquare4 { get; set; }

    }

    public class SignalRClient
    {
        //Fix code to work in Unity
        /*
        HubConnection _connection;
        string groupName = "";
        public TMP_Text IsLoggedIn;
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

            //_connection.InvokeAsync("UpdateGameBoard", groupName);
            //_connection.InvokeAsync("SendMessage", groupName, Classes.APILinks.PlayerUserName, "Message Text");
        }
        private async void BtnChat_Clicked(object? sender, EventArgs e)
        {
            if (Classes.APILinks.PlayerID != Guid.Empty && !string.IsNullOrEmpty(Classes.APILinks.PlayerUserName) && Classes.APILinks.GameID != Guid.Empty && _connection != null)
            {
                await SendMessageToChannel(TypeMessage.Text);
                TypeMessage.Text = "";
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Must be Logged in & at a game to Chat!", "Ok");
            }
        }
        private async void UpdateOtherGameBoards() //When I change my GameBoard
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
                //Without running in the main thread, SignalR callback methods (or any method) cannot access the UI on MAUI               
                //ShowChatMessage.Text += name + " : " + message + " \n";

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
                IsLoggedIn.text = "Game Data Retrieved";

                List<Classes.Game> gameList = JsonConvert.DeserializeObject<List<Classes.Game>>(responseBody).ToList(); ;
                Classes.Game game = gameList.Where(g => g.Id == Classes.APILinks.GameID).First();

                await FillLabels();
                //Without running in the main thread, SignalR callback methods (or any method) cannot access the UI on maui
                {
                    GraphicsDrawable.Game = game;
                    graphicsView.Drawable = new GraphicsDrawable();//Must make new GraphicsDrawable() and assign it the the xaml element graphicsView every time I want to change the Game Board. 
                });
            }
        }
        */
    }
}
