using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SwitchScene : MonoBehaviour
{
    public string Username = "";
    public string Password = "";

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
        else
        {
            return;
        }
    }
    public void Login()
    {
        
        Username = myInputs[0].text;
        Password = myInputs[1].text;
        if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
        {
            IsLoggedIn.text = $"Welcome, {Username}";
        }
        else 
        {
            IsLoggedIn.text = "Log In Failed";
        }
            
    }
    public void CreateAccount()
    {

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