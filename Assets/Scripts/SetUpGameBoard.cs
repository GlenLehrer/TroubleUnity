using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetUpGameBoard : MonoBehaviour
{
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Classes.APILinks.GameID == null || Classes.APILinks.GameID == Guid.Empty)
        {
            ResetToNewGameBoard();

        }

    }

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

    public static Vector3 RedHomeSquare1 = new Vector3(357, 36, -195);  //Square 37
    public static  Vector3 RedHomeSquare2 = new Vector3(351, 36, -240); //Square 38
    public static Vector3 RedHomeSquare3 = new Vector3(314, 36, -236);  //Square 39
    public static Vector3 RedHomeSquare4 = new Vector3(314, 36, -270);  //Square 40

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






