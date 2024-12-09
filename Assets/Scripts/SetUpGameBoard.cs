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

public static class BoardPositions
{
    public static Vector3 YellowHomeSquare1 = new Vector3(-362, 36, 204);
    public static Vector3 YellowHomeSquare2 = new Vector3(-361, 36, 241);
    public static Vector3 YellowHomeSquare3 = new Vector3(-325, 36, 234);
    public static Vector3 YellowHomeSquare4 = new Vector3(-321, 36, 277);

    public static Vector3 BlueHomeSquare1 = new Vector3(314, 36, 280);
    public static Vector3 BlueHomeSquare2 = new Vector3(346, 36, 240);
    public static Vector3 BlueHomeSquare3 = new Vector3(333, 36, 197);
    public static Vector3 BlueHomeSquare4 = new Vector3(374, 36, 204);

    public static Vector3 RedHomeSquare1 = new Vector3(357, 36, -195);
    public static  Vector3 RedHomeSquare2 = new Vector3(351, 36, -240);
    public static Vector3 RedHomeSquare3 = new Vector3(314, 36, -236);
    public static Vector3 RedHomeSquare4 = new Vector3(314, 36, -270);

    public static Vector3 GreenHomeSquare1 = new Vector3(-328, 36, -270);
    public static Vector3 GreenHomeSquare2 = new Vector3(-306, 36, -223);
    public static Vector3 GreenHomeSquare3 = new Vector3(-355,36, -232);
    public static Vector3 GreenHomeSquare4 = new Vector3(-385, 36, -196);

}






