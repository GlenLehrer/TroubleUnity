using UnityEngine;
using UnityEngine.UIElements;
// Controls player movement and rotation.
public class MoveCamera : MonoBehaviour
{
    public float speed = 10000f; // Set player's movement speed.
    private Vector2 motion;

    void Start()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
        motion = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.Translate(motion * speed);
    }

}