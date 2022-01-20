using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using UnityEditor;


public class DiegeticText : MonoBehaviour
{
    public Action OnClickCallback;
    public bool ClickActive = true;
    [SerializeField] private Canvas Canvas;
    [SerializeField] private string Text;
    [SerializeField] private Font Font;
    [SerializeField] private int FontSize = 50;
    [SerializeField] private Color MainColor = Color.white;
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
        Regex regex = new Regex("((<(?<command>[\\w]*?)(=(?<value>\\d+(\\.\\d+)?))?>)|(?<character>[\\w.,?!'`\\s\\n]+?))");
        Match[] matches = regex.Matches(text).Cast<Match>().ToArray();

        ClearLetters();
        CreateLetters(matches.Where(e => e.Groups["character"].Success).Select(e => e.Groups["character"].Value[0]));
        AlignLetters();
        StartCoroutine(TypeText(matches, callback));
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
            while (wordIndex < words.Count && words[wordIndex][0].text != "\n" && lineWidth + GetWordWidth(words[wordIndex]) < maxLineWidth);

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

    IEnumerator TypeText(IEnumerable<Match> matches, Action onDone = null)
    {
        Color color = MainColor;
        bool isBobbing, isWiggling = false;

        int letterIndex = 0;
        foreach (Match match in matches)
        {
            if (match.Groups["character"].Success)
            {
                letters[letterIndex].color = color;
                letters[letterIndex].enabled = true;
                letterIndex++;
                if (EditorApplication.isPlaying) yield return new WaitForSeconds(1.0f / textSpeed);
            }
            if (match.Groups["command"].Success)
            {
                float value;
                Single.TryParse(match.Groups["value"]?.Value, out value);
                switch (match.Groups["command"].Value.ToLower())
                {
                    // colors
                    case "red":
                        color = Color.red;
                        break;
                    case "white":
                        color = Color.white;
                        break;
                    case "blue":
                        color = Color.blue;
                        break;
                    case "yellow":
                        color = Color.yellow;
                        break;
                    case "green":
                        color = Color.green;
                        break;
                    case "black":
                        color = Color.black;
                        break;
                    case "grey":
                    case "gray":
                        color = Color.grey;
                        break;
                    // Animations
                    case "bob":
                        isBobbing = true;
                        break;
                    case "endbob":
                        isBobbing = false;
                        break;
                    case "wiggle":
                        isWiggling = true;
                        break;
                    case "endwiggle":
                        isWiggling = false;
                        break;
                    // Pacing
                    case "speed":
                    case "textspeed":
                        textSpeed = value;
                        break;
                    case "crawl":
                        textSpeed = 5.0f;
                        break;
                    case "slow":
                        textSpeed = 15.0f;
                        break;
                    case "normal":
                        textSpeed = 30.0f;
                        break;
                    case "fast":
                        textSpeed = 60.0f;
                        break;
                    case "pause":
                        yield return new WaitForSeconds(value);
                        break;
                }
            }
        }
        yield return null;

        onDone?.Invoke();
    }
}
