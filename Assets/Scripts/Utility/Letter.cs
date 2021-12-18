using UnityEngine;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class Letter : MonoBehaviour
{
    public char Character;
    SpriteRenderer spriteRenderer;
    float bobbingAmount = 0.03f;
    float bobbingSpeed;
    float bobbingStartTime;
    
    float wiggleMaxAngle = 20f;
    float wiggleSpeed;
    float wiggleStartTime;

    public Color Color
    {
        get { return spriteRenderer.color; }
        set { spriteRenderer.color = value; }
    }

    public Sprite Sprite
    {
        get { return spriteRenderer.sprite; }
        set { spriteRenderer.sprite = value; }
    }

    public float Width
    {
        get { return spriteRenderer.bounds.size.x; }
    }

    float time
    {
        get { return Time.time * inPlay; }
    }

    public Vector3 Position;

    int inPlay = 0;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        inPlay = (!Application.isEditor || EditorApplication.isPlaying) ? 1 : 0;
    }

    [ExecuteInEditMode]
    void Update()
    {
        transform.position = transform.parent.position + transform.parent.rotation * Position;
        transform.position += Mathf.Sin((time - bobbingStartTime) * bobbingSpeed) * Vector3.up * bobbingAmount;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin((time - wiggleStartTime) * wiggleSpeed) * wiggleMaxAngle);
    }

    public void StartBobbing(float speed = 3.0f)
    {
        bobbingStartTime = time;
        bobbingSpeed = speed;
    }

    public void StartWiggle(float speed = 3.0f)
    {
        wiggleStartTime = time;
        wiggleSpeed = speed;
    }

}
