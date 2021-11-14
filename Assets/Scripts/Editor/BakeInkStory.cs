using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BakeInkStory : EditorWindow
{

    ////////// THIS IS CURRENTLY UNUSED

    // [MenuItem("Window/Bake Ink Story")]
    // public static void ShowWindow () {
    //     GetWindow<BakeInkStory>("Bake Ink Story");
    // }

    void OnGUI()
    {

        if (GUILayout.Button("Bake Story"))
        {
            /////// CLEAR OLD ASSETS ///////

            // Clear old Dialogue
            string[] paths = AssetDatabase.GetSubFolders("Assets/Dialogue");
            AssetDatabase.DeleteAssets(paths, new List<string>());

            /////// CREATE NEW ASSETS ///////
            
            // Read main.ink
            string mainPath = @$"{Directory.GetCurrentDirectory()}\\Assets\\Resources\\Dialogue\\";
            string mainInk = System.IO.File.ReadAllText(mainPath + @"main.ink");



            // CHARACTERS
            
            // Filter main.ink for all includes
            Regex characterRegex = new Regex(@"INCLUDE (?<name>\w+).ink");
            MatchCollection characterMatches = characterRegex.Matches(mainInk);

            // Create List to store the names of the characters
            List<string> characters = new List<string>();

            // Iterate through matches
            foreach (Match match in characterMatches)
            {
                // Get the name of the current match
                string name = match.Groups["name"].Value;

                // Add character to list
                characters.Add(name);

                if (!AssetDatabase.IsValidFolder($"Assets/Dialogue/{name}"))
                {
                    // Create a directory corresponding to the character
                    AssetDatabase.CreateFolder("Assets/Dialogue", name);
                    AssetDatabase.CreateFolder($"Assets/Dialogue/{name}", "Bits");
                }
            }

            

            //// DIALOGUE BITS

            // REGEX DEFS
            // Catches lines that indicate the beginning of a story knot
            // Capture group "title" contains the title of the story knot
            Regex dialogueStartRegex = new Regex(@"^===? (?<title>\w+)");
            // Catches all content after initial spaces and before double slashes in capture group "content"
            Regex contentRegex = new Regex(@"^\s*(?<content>[^/]*)(//.*)?");
            // Catches choices in the inky story knots
            // Capture group "qualifier" catches + or * characters at the beginning, thus indicating whether the choice is sticky
            // Capture group "text" catches the text of the choice
            // Capture group "link" catches the name of the next knot/bit
            Regex choiceRegex = new Regex(@"(?<qualifier>\+|\*)\s+\[(?<text>[\w\s]+)\]\s+->\s+(?<link>\w+)");

            // Iterate throuh all the characters and create all the story bits they require
            foreach (string character in characters)
            {
                // Create two dictionaries to temporarily store the data for each bit
                // These effectively map the Raw content, as a list of strings, to the dialogue bit, with a key for ease of access
                Dictionary<string, List<string>> dialogueBitsRaw = new Dictionary<string, List<string>>();
                Dictionary<string, DialogueBit> dialogueBits = new Dictionary<string, DialogueBit>();

                // Now we want to read through the ink file and save all the content in one dictionary and create new bits in the other
                // We can't immediately parse the content into the story bits because it may contain references to story bits that are not
                // yet created.
                // Thus we store the content first and iterate over it again, once all the story bits are there.

                // Read character.ink line by line
                string characterPath = @$"{Directory.GetCurrentDirectory()}\\Assets\\Resources\\Dialogue\\{character}.ink";
                foreach (string line in System.IO.File.ReadLines(characterPath))
                {  
                    // Filter out non-content
                    string content = contentRegex.Match(line).Groups["content"].Value;
                    if (content == "") continue;

                    // Filter for the lines where dialogue starts
                    if (dialogueStartRegex.IsMatch(content))
                    {
                        // Get the title from the capture group
                        string title = dialogueStartRegex.Match(content).Groups["title"].Value;
                        
                        // Create a new instance of the scriptable object for dialogue bits
                        DialogueBit dialogueBit = ScriptableObject.CreateInstance<DialogueBit>();

                        // Add it to the dictionary of dialogue bits
                        dialogueBits.Add(title, dialogueBit);

                        // Add an empty list of strings to the content dictionary
                        dialogueBitsRaw.Add(title, new List<string>());
                    }
                    // If content is not a new knot and not before the first knot
                    else if (dialogueBits.Count > 0)
                    {
                        // Add the content to the latest bits raw content
                        dialogueBitsRaw.Values.Last().Add(content);
                    }
                }

                // Iterate through the raw dialogue content
                foreach (var entry in dialogueBitsRaw)
                {
                    // Create easy references to the bit and the content
                    string[] content = entry.Value.ToArray();
                    DialogueBit bit = dialogueBits[entry.Key];

                    // Go through each line of content
                    foreach (string line in content)
                    {
                        // If it is a choice, add a choice
                        if (choiceRegex.IsMatch(line))
                        {
                            GroupCollection captureGroups = choiceRegex.Match(line).Groups;

                            DialogueBit.Choice choice = new DialogueBit.Choice();
                            string nextName = captureGroups["link"].Value;

                            choice.Next = nextName != "main" ? dialogueBits[nextName] : null;
                            choice.Text = captureGroups["text"].Value;

                            bit.Choices.Add(choice);
                        }
                        else
                        {
                            bit.Text += $"{line}\n";
                        }
                    }

                    AssetDatabase.CreateAsset(bit, $"Assets/Dialogue/{character}/Bits/{entry.Key}.asset");
                }
            }

            


            // SAVE ALL ASSETS
            AssetDatabase.SaveAssets();
        }
    }
}
