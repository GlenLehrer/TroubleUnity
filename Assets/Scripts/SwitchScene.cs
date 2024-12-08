using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
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

    //GameObject loaded
    void Start()
    {

    }

    //Everytime frames are updated
    void Update()
    {
    }
}