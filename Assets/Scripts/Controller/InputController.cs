using UnityEngine;
public class InputController : MonoBehaviour
{
    PlayerController pc;

    bool inputDisabled;
    void OnEnable()
    {
        pc = GetComponent<PlayerController>();
    }

    void Update()
    { 
        // INPUTS
        if (!inputDisabled)
        {
            // On Turn Buttons (A and D)
            if (Input.GetButtonDown("Turn"))
            {
                pc.Turn();
            }

            // On Move Buttons (W and S)
            if (Input.GetButtonDown("Move"))
            {
                pc.Move(Mathf.Sign(Input.GetAxis("Move")));
            }

            // Handle Mouseclick Event
            if (Input.GetMouseButtonDown(0))
            {
                pc.Interact();
            }


            if (Input.GetButtonDown("1"))
            {
                GameManager.Instance.CurrentCustomer.ReceiveAnswer(0);
            }
            if (Input.GetButtonDown("2"))
            {
                GameManager.Instance.CurrentCustomer.ReceiveAnswer(1);
            }
        }
    }
}