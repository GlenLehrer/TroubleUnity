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

        public static bool hasGameStarted(Game game) //Determines if a game has started.  If a game has started, there should be no new players joining.
        {
            //If HomeSquares are all full, no pieces have been moved.  Technically, a game could have started, but nothing has happened yet.
            //Method returns true if all pieces are on home squares and it is yellow's turn.
            if (
                game.PlayerTurn.ToLower().StartsWith("y") &&
                game.YellowHomeSquare1 == 1 &&
                game.YellowHomeSquare2 == 1 &&
                game.YellowHomeSquare3 == 1 &&
                game.YellowHomeSquare4 == 1 &&
                game.BlueHomeSquare1 == 1 &&
                game.BlueHomeSquare2 == 1 &&
                game.BlueHomeSquare3 == 1 &&
                game.BlueHomeSquare4 == 1 &&
                game.RedHomeSquare1 == 1 &&
                game.RedHomeSquare2 == 1 &&
                game.RedHomeSquare3 == 1 &&
                game.RedHomeSquare4 == 1 &&
                game.GreenHomeSquare1 == 1 &&
                game.GreenHomeSquare2 == 1 &&
                game.GreenHomeSquare3 == 1 &&
                game.GreenHomeSquare4 == 1
            )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }

}
