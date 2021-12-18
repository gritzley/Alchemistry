using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

[ExecuteInEditMode]
public class TextDisplay : MonoBehaviour
{
    public Texture2D FontTex;
    [TextArea]
    public string CharacterString;

    public GameObject LetterPrefab;

    float letterSpacing = 0.01f;
    float textSpeed = 12.0f;

    Sprite[] sprites;

    string text;

    void OnEnable()
    {
        sprites = Resources.LoadAll<Sprite>(FontTex.name);

        // Creating the letters in EditMode aswell means, that there is no cleanup. Therefore we have to clean them up ourselves
        foreach(Letter letter in GetComponentsInChildren<Letter>())
        {
            DestroyImmediate(letter.gameObject);
        }
    }

    public void DisplayText(string text)
    {
        this.text = text;
        StartCoroutine(DisplayText());
    }

    IEnumerator DisplayText()
    {
        Regex regex = new Regex($"((<(?<command>\\w*?)(=(?<value>\\d+(\\.\\d+)?))?>)|(?<character>[{CharacterString}]+?))");

        // We calculate the leftmost bound and then draw the letters from there, moving further right with each letter.
        // This is basically like a cursor
        MatchCollection matches = regex.Matches(text);
        Queue<Letter> letters = new Queue<Letter>();
        float leftBound = 0;
        Color color = Color.white;
        bool isBobbing = false;
        bool isWiggling = false;
        foreach (Match match in matches)
        {
            if (match.Groups["character"].Success)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(LetterPrefab) as GameObject;
                go.transform.parent = transform;
                go.SetActive(false);
                Letter letter = go.GetComponent<Letter>();
                letter.Sprite = GetSpriteForCharacter(match.Groups["character"].Value.ToCharArray()[0]);
                leftBound += (letter.Width + letterSpacing) / 2;
                letters.Enqueue(letter);
            }
        }
        foreach (Match match in matches)
        {
            if (match.Groups["character"].Success)
            {

                Letter letter = letters.Dequeue();
                letter.Color = color;
                letter.Position = Vector3.left * leftBound + Vector3.right * letter.Width / 2;
                if (isBobbing) letter.StartBobbing();
                if (isWiggling) letter.StartWiggle();
                letter.gameObject.SetActive(true);
                leftBound -= (letter.Width + letterSpacing);
                if (EditorApplication.isPlaying) yield return new WaitForSeconds(1.0f / textSpeed);
            }
            if (match.Groups["command"].Success)
            {
                switch (match.Groups["command"].Value.ToLower())
                {
                    case "red":
                        color = Color.red;
                        break;
                    case "white":
                        color = Color.white;
                        break;
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
                }
            }
        }
        yield return null;
    }

    Sprite GetSpriteForCharacter(char c)
    {
        // The CharacterString is the "text" of the Font Texture, when written as a word. That way, the nth letter in the string
        // is the nth sprite in the spritesheet.
        int i = CharacterString.IndexOf(char.ToLower(c));
        return sprites[i];
    }
}
