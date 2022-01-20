using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class DiegeticText : MonoBehaviour
{
    [SerializeField] private Canvas Canvas;
    [SerializeField] private string Text;
    [SerializeField] private Font Font;
    [SerializeField] private int FontSize = 50;
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

    public void DisplayText(string text)
    {
        ClearLetters();
        CreateLetters(text);
        AlignLetters();
    }
    private void ClearLetters()
    {
        if (letters != null)
            foreach(Text letter in Canvas.GetComponentsInChildren<Text>())
                DestroyImmediate(letter.gameObject);
        
        letters = new List<Text>();
    }

    private void CreateLetters(IEnumerable<char> characters)
    {
        foreach (char character in characters)
        {
            GameObject ngo = new GameObject("Letter");
            ngo.transform.SetParent(Canvas.transform);
            ngo.transform.position = Canvas.transform.position;
            ngo.transform.localScale = Vector3.one;

            Text text = ngo.AddComponent<Text>();
            text.text = character.ToString();
            text.alignment = TextAnchor.MiddleCenter;
            if (Font) text.font = Font;
            text.fontSize = FontSize;

            letters.Add(text);
        }
    }

    private void AlignLetters()
    {
        float GetLetterWidth(Text letter) => letter.preferredWidth * Canvas.transform.localScale.x;
        float GetWordWidth(Text[] word) => word.Select(e => GetLetterWidth(e) + letterSpacing).Sum() - letterSpacing;        

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

        int wordIndex = 0;
        while (wordIndex < words.Count)
        {
            List<Text> line = new List<Text>();

            float lineWidth = 0;
            do
            {
                foreach(Text letter in words[wordIndex])
                    line.Add(letter);
                lineWidth += GetWordWidth(words[wordIndex]);

                wordIndex++;

            }
            while (wordIndex < words.Count && lineWidth + GetWordWidth(words[wordIndex]) < maxLineWidth);

            float cursor = lineWidth / -2;
            foreach(Text letter in line)
            {
                letter.transform.position += letter.transform.right * (cursor + GetLetterWidth(letter) / 2);
                cursor += GetLetterWidth(letter) + letterSpacing;
            }
            
            float lineHeight = letters[0].preferredHeight * Canvas.transform.localScale.x + lineSpacing;
            lines.ForEach(e => e.ForEach(e => e.transform.position += e.transform.up * lineHeight));
            lines.Add(line);
        }
    }
}
