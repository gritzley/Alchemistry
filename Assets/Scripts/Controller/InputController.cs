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
            if (Input.GetButtonDown("Forward")) pc.Move(Vector3.forward);

            if (Input.GetButtonDown("Left")) pc.Move(Vector3.left);

            if (Input.GetButtonDown("Back")) pc.Move(Vector3.back);

            if (Input.GetButtonDown("Right")) pc.Move(Vector3.right);

            // Handle Mouseclick Event
            if (Input.GetMouseButtonDown(0)) pc.Interact();

            if (Input.GetButtonDown("Pause")) pc.Paused = !pc.Paused;

            if (Input.GetButtonDown("1"))
                if (!GameManager.Instance.CurrentCustomer.CurrentDialogueLine.IsReceivingState)
                    GameManager.Instance.CurrentCustomer.ReceiveAnswer(0);

            if (Input.GetButtonDown("2"))
                if (GameManager.Instance.CurrentCustomer.CurrentDialogueLine.HasAnswers)
                    GameManager.Instance.CurrentCustomer.ReceiveAnswer(1);
        }
    }
}