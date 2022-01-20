using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEditor;


public class DiegeticText : MonoBehaviour
{
    public Action OnClickCallback;
    public bool ClickActive = true;
    [SerializeField] private Canvas Canvas;
    [SerializeField] private string Text;
    [SerializeField] private Font Font;
    [SerializeField] private int FontSize = 50;
    private float textSpeed = 30;
    private float letterSpacing = 0.01f;
    private float lineSpacing = 0.05f;
    private RectTransform _canvasRect;
    private float maxLineWidth
    {
        get
        {
            if (_canvasRect == null)
                _canvasRect = Canvas.GetComponent<RectTransform>();
            
            return _canvasRect.rect.width * Canvas.transform.localScale.x;
        }
    }
    private Vector3 Offset = Vector3.zero;
    private List<Text> letters;
    private void OnEnable()
    {
        DisplayText(Text);
    }

    public void DisplayText(string text, Action callback = null)
    {
        Text = text;
        ClearLetters();
        CreateLetters(text);
        AlignLetters();
        StartCoroutine(TypeText(callback));
    }
    public void ClearLetters()
    {
        StopAllCoroutines();

        foreach(Text letter in Canvas.GetComponentsInChildren<Text>())
        {
            if (EditorApplication.isPlaying) Destroy(letter.gameObject);
            else DestroyImmediate(letter.gameObject);
        }
        
        letters = new List<Text>();
    }

    private void CreateLetters(IEnumerable<char> characters)
    {
        foreach (char character in characters)
        {
            GameObject ngo = new GameObject("Letter");
            ngo.transform.SetParent(Canvas.transform);
            ngo.transform.position = Canvas.transform.position;
            ngo.transform.rotation = Canvas.transform.rotation;
            ngo.transform.localScale = Vector3.one;

            Text text = ngo.AddComponent<Text>();
            text.text = character.ToString();
            text.alignment = TextAnchor.MiddleCenter;
            if (Font) text.font = Font;
            text.fontSize = FontSize;
            text.enabled = false;

            EventTrigger trigger = ngo.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(data => OnPointerDownDelegate((PointerEventData)data));
            trigger.triggers.Add(entry);

            letters.Add(text);
        }
    }

    public void OnPointerDownDelegate(PointerEventData data)
    {
        if (ClickActive) OnClickCallback?.Invoke();
    }

    private void AlignLetters()
    {
        float GetLetterWidth(Text letter) => letter.preferredWidth * Canvas.transform.localScale.x;
        float GetWordWidth(Text[] word) => word.Select(e => GetLetterWidth(e) + letterSpacing).Sum() - letterSpacing;        

        // Split the text into words;
        int charIndex = 0;
        List<Text[]> words = new List<Text[]>();
        List<List<Text>> lines = new List<List<Text>>();
        while (charIndex < letters.Count)
        {
            int nextSpaceIndex = letters.FindIndex(charIndex + 1, letters.Count - charIndex - 1, e => e.text == " " || e.text == "\n");
            if (nextSpaceIndex == -1) nextSpaceIndex = letters.Count;

            Text[] word = letters.GetRange(charIndex, nextSpaceIndex - charIndex).ToArray();

            words.Add(word);
            charIndex = nextSpaceIndex;
        }

        // split the words into lines
        int wordIndex = 0;
        while (wordIndex < words.Count)
        {
            List<Text> line = new List<Text>();

            float lineWidth = 0;
            do
            {
                foreach (Text letter in words[wordIndex])
                    line.Add(letter);

                lineWidth += GetWordWidth(words[wordIndex]);
                wordIndex++;
            }
            while (wordIndex < words.Count && lineWidth + GetWordWidth(words[wordIndex]) < maxLineWidth);

            float cursor = lineWidth / -2;
            foreach (Text letter in line)
            {
                letter.transform.position += letter.transform.right * (cursor + GetLetterWidth(letter) / 2);
                cursor += GetLetterWidth(letter) + letterSpacing;
            }
            
            float lineHeight = letters[0].preferredHeight * Canvas.transform.localScale.x + lineSpacing;
            lines.ForEach(e => e.ForEach(e => e.transform.position += e.transform.up * lineHeight / 2));
            line.ForEach(e => e.transform.position += e.transform.up * lineHeight * lines.Count / -2);
            lines.Add(line);
        }
    }

    IEnumerator TypeText(Action onDone = null)
    {
        foreach (Text letter in letters)
        {
            letter.enabled = true;
            if (EditorApplication.isPlaying) yield return new WaitForSeconds(1.0f / textSpeed);
        }
        yield return null;

        onDone?.Invoke();
    }
}
