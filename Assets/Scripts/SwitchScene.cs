using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System;
using UnityEngine.Experimental.Rendering;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public class SwitchScene : MonoBehaviour
{
    public string Username = "";
    public string Password = "";
    public string Password2 = "";
    public string Email = "";

    public TMP_InputField[] myInputs;
    public TMP_Text IsLoggedIn;

    public void switchScenes(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void LoadMenu(string scene)
    {
        GameObject CanvasParent = GameObject.Find("CanvasParent");

        GameObject MainMenu = CanvasParent.transform.Find("Main Menu").gameObject;
        GameObject CreateAccount = CanvasParent.transform.Find("Create Account").gameObject;
        GameObject Login = CanvasParent.transform.Find("Login").gameObject;
        GameObject GameRoom = CanvasParent.transform.Find("Game Room").gameObject;

        if (scene == "MainMenu")
        {
            MainMenu.gameObject.SetActive(true);

            CreateAccount.gameObject.SetActive(false);
            Login.gameObject.SetActive(false);
            GameRoom.gameObject.SetActive(false);
        }
        else if (scene == "CreateAccount")
        {
            CreateAccount.gameObject.SetActive(true);

            MainMenu.gameObject.SetActive(false);
            Login.gameObject.SetActive(false);
            GameRoom.gameObject.SetActive(false);
        }
        else if (scene == "Login")
        {
            Login.gameObject.SetActive(true);

            MainMenu.gameObject.SetActive(false);
            CreateAccount.gameObject.SetActive(false);
            GameRoom.gameObject.SetActive(false);
        }
        else if (scene == "GameRoom")
        {

            GameRoom.gameObject.SetActive(true);

            MainMenu.gameObject.SetActive(false);
            CreateAccount.gameObject.SetActive(false);
            Login.gameObject.SetActive(false);

        }

        //Always run this if Statement, regardless of previous if-else statement results.
        if (IsLoggedIn != null && !string.IsNullOrEmpty(IsLoggedIn.text) && !string.IsNullOrEmpty(Classes.APILinks.PlayerUserName))
        {
            IsLoggedIn.text = Classes.APILinks.PlayerUserName;
        }
        return;
        
    }
    public async void Login()
    {
        
        Username = myInputs[0].text;
        Password = myInputs[1].text;
        if (string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password))
        {
            IsLoggedIn.text = "Fields cannot be empty!";
        }
        else
        {
            Classes.Player player = new Classes.Player
            {
                UserName = this.Username,
                Password = this.Password,
                Email = "12@12.com",
                NumberOfWins = 0,
                DateJoined = DateTime.Now
            };
            HttpClient client = new HttpClient();
            var data = JsonConvert.SerializeObject(player);
            var response = await client.PostAsync(new Uri(Classes.APILinks.APIAddress + "player/login"), new StringContent(data, System.Text.Encoding.UTF8, "application/json"));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                IsLoggedIn.text = $"Welcome, {Username}";
                string result = response.Content.ReadAsStringAsync().Result;
                result = result.Replace("\'", "").Replace("\\", "").Replace("\"", "").Trim(); //Return string has quotes inside the string
                Classes.APILinks.PlayerID = new Guid(result); //Entire app knows the PlayerID that logs in.
                Classes.APILinks.PlayerUserName = player.UserName;
                EditorUtility.DisplayDialog("Logged In!", $"Welcome, {Username}", "OK");
            }
            else
                IsLoggedIn.text = response.StatusCode.ToString();
        }
            
    }
    public async void CreateAccount()
    {
        Username = myInputs[0].text;
        Password = myInputs[1].text;
        Password2 = myInputs[2].text;
        Email = myInputs[3].text;
        if (string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(Password2) && string.IsNullOrEmpty(Email))
        {
            IsLoggedIn.text = "Fields cannot be empty!";
            return;
        }
        else if (Password != Password2)
        {
            IsLoggedIn.text = "Password do not match!";
            return;
        }
        else if(!Email.Contains("@") || Email.LastIndexOf(".") < Email.LastIndexOf("@"))
        {
            IsLoggedIn.text = "Invalid Email!";
            return;
        }
        else
        {
            //Call API to see if login is valid
            Classes.Player player = new Classes.Player
            {
                UserName = this.Username,
                Password = this.Password,
                Email = this.Email,
                NumberOfWins = 0,
                DateJoined = DateTime.Now
            };
            HttpClient client = new HttpClient();
            var data = JsonConvert.SerializeObject(player);
            var response = await client.PostAsync(new Uri(Classes.APILinks.APIAddress + "player/false"), new StringContent(data, System.Text.Encoding.UTF8, "application/json"));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                IsLoggedIn.text = $"Welcome, {Username}";
                string result = response.Content.ReadAsStringAsync().Result;
                result = result.Replace("\'", "").Replace("\\", "").Replace("\"", "").Trim(); //Return string has quotes inside the string
                Classes.APILinks.PlayerID = new Guid(result); //Entire app knows the PlayerID that logs in.
                Classes.APILinks.PlayerUserName = player.UserName;
                EditorUtility.DisplayDialog("Account Created!", $"Welcome, {Username}", "OK");
            }

            else
                IsLoggedIn.text = response.StatusCode.ToString();
        }

    }

    //GameObject loaded
    void Start()
    {

    }

    //Everytime frames are updated
    void Update()
    {
    }
}