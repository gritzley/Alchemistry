using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendClickEvent : MonoBehaviour
{
    public Clickable Recipient;
    void OnMouseDown() => Recipient.OnClick();
}
