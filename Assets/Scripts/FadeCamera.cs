using UnityEngine;
 
public class FadeCamera : MonoBehaviour
{
    public AnimationCurve FadeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.6f, 0.7f, -1.8f, -1.2f), new Keyframe(1, 0));
 
    public bool InAnimation => (_direction > 0 ? _time < 1 : _time > 0);
    private float _alpha = 1;
    private float _direction = 1;
    private Texture2D _texture;
    private float _time = 0;
 
    public void In()
    {
        _direction = 1;
    }
    public void Out()
    {
        _direction = -1;
    }
 
    public void OnGUI()
    {
        if (_texture == null) _texture = new Texture2D(1, 1);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);
        _texture.SetPixel(0, 0, new Color(0, 0, 0, _alpha));
        _texture.Apply();
 
        if (!InAnimation) return;
        _time += Time.deltaTime * _direction;
        _alpha = FadeCurve.Evaluate(Mathf.Max(0, _time));
    }
}
