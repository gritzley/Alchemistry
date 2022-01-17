using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

[ExecuteInEditMode]
public class TextDisplay : Interactible
{
    public Texture2D FontTex;
    public Sprite ErrorCharacterSprite;
    [TextArea]
    public string CharacterString;
    public GameObject LetterPrefab;

    public Vector3 Offset;
    public float letterSpacing = 0.01f;
    public float lineSpacing = 0.1f;
    public float textSpeed = 30.0f;
    public float maxLineWidth = 5.0f;

    public Action OnClickCallback;
    public bool ClickActive = true;
    private Sprite[] sprites;
    public float PPU = 80;

    void Awake()
    {
        // Creating the letters in EditMode aswell means, that there is no cleanup. Therefore we have to clean them up ourselves
        // We must do this in Awake because otherwise, if we try to display somethign in OnEnable, from this parent, it gets erased immdiately
        ClearLetters();
        sprites = Resources
            .LoadAll<Sprite>(FontTex.name)
            .Select(e => Sprite.Create(FontTex, e.textureRect, Vector2.one / 2, PPU ))
            .ToArray();
    }

    void OnEnable()
    {
        // create a single letter for a single frame to get height of a sprite (all letters actually have the same height, despite appearance)
        GameObject go = PrefabUtility.InstantiatePrefab(LetterPrefab) as GameObject;
        Letter letter = go.GetComponent<Letter>();
        letter.Sprite = sprites[0];
        float spriteHeight = letter.Height;
        DestroyImmediate(go);

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.center = Offset + Vector3.up / 2 + Vector3.down * spriteHeight / 2;
            collider.size = new Vector3(maxLineWidth, 1, 1);
        }
    }

    public void ClearLetters()
    {
        StopAllCoroutines();
        foreach(Letter letter in GetComponentsInChildren<Letter>(true))
        {
            letter.transform.parent = null;
            DestroyImmediate(letter.gameObject);
        }
    }

    public override bool OnInteract(PlayerController _)
    {
        if (ClickActive) OnClickCallback?.Invoke();
        return true;
    }

    public void DisplayText(string text, Action callback = null)
    {
        ClearLetters();
        StartCoroutine(DisplayTextCoroutine(text, callback));
    }

    IEnumerator DisplayTextCoroutine(string text, Action callback = null)
    {

        // This filters the text into a MatchCollection. Each Match contains either a command or a single letter that is to be displayed
        // These matches can be used to iterate over the text letter by letter
        // MatchGroups: "command" and "character"
        // Note: The Characterstring contains all characters contained in the FontTexture and acts as a filter for unsupported characters.s
        Regex regex = new Regex($"((<(?<command>\\w*?)(=(?<value>\\d+(\\.\\d+)?))?>)|(?<character>[{CharacterString}\\n]+?))");
        MatchCollection matches = regex.Matches(text);

        // Flags for how letters are rendered
        // These are the standard settings for text
        Color color = Color.white;
        bool isBobbing = false;
        bool isWiggling = false;

        List<Letter> letters = new List<Letter>();
        foreach (Match match in matches)
        {
            if (match.Groups["character"].Success)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(LetterPrefab) as GameObject;
                go.transform.parent = transform;
                go.SetActive(false);
                Letter letter = go.GetComponent<Letter>();
                letter.Character = match.Groups["character"].Value[0];
                if (letter.Character != '\n') letter.Sprite = GetSpriteForCharacter(letter.Character);
                letters.Add(letter);
            }
        }
        int charIndex = 0;
        int lastBreakIndex = 0;
        int nextSpaceIndex = letters.FindIndex(0, letters.Count, e => e.Character == ' ' || e.Character == '\n');
        float lineWidth = 0;
        foreach (Match match in matches)
        {
            if (match.Groups["character"].Success)
            {
                Letter letter = letters[charIndex];
                letter.Color = color;

                if (charIndex++ == nextSpaceIndex && charIndex < letters.Count)
                {
                    nextSpaceIndex = letters.FindIndex(charIndex, letters.Count - charIndex, e => e.Character == ' ' || e.Character == '\n');
                    if (nextSpaceIndex == -1) nextSpaceIndex = letters.Count;
                    float wordWidth = letters
                    .GetRange(charIndex, nextSpaceIndex - charIndex)
                    .Select( e => e.Width + letterSpacing)
                    .Sum();

                    if (lineWidth + wordWidth > maxLineWidth || letters[charIndex - 1].Character == '\n')
                    {
                        letters.GetRange(0, charIndex - 1).ForEach( e => e.Position += Vector3.up * lineSpacing);
                        lastBreakIndex = charIndex;
                        lineWidth = 0;
                    }
                    lineWidth += wordWidth;
                }

                float leftCursor = letters.GetRange(lastBreakIndex, charIndex - lastBreakIndex).Select( e => e.Width + letterSpacing).Sum() / 2;
                letters.GetRange(lastBreakIndex, charIndex - lastBreakIndex).ForEach( e => {
                    e.Position = Vector3.left * leftCursor + Vector3.right * e.Width / 2 + Offset;
                    leftCursor -= e.Width + letterSpacing;
                });

                if (isBobbing) letter.StartBobbing();
                if (isWiggling) letter.StartWiggle();
                letter.gameObject.SetActive(true);
                if (EditorApplication.isPlaying) yield return new WaitForSeconds(1.0f / textSpeed);
            }
            if (match.Groups["command"].Success)
            {
                float value;
                Single.TryParse(match.Groups["value"]?.Value, out value);
                switch (match.Groups["command"].Value.ToLower())
                {
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
                    case "textspeed":
                        textSpeed = value;
                        break;
                }
            }
        }

        callback?.Invoke();
        yield return null;
    }

    Sprite GetSpriteForCharacter(char c)
    {
        if (CharacterString.Contains(char.ToLower(c)))
        {
            // The CharacterString is the "text" of the Font Texture, when written as a word. That way, the nth letter in the string
            // is the nth sprite in the spritesheet.
            int i = CharacterString.IndexOf(char.ToLower(c));
            return sprites[i];
        }
        else {
            return ErrorCharacterSprite;
        }
    }
}
