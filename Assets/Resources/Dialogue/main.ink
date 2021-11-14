// This is the main Ink file
// In the sidebar, you can see all files used by this file

// Everything written after two slashes will be ignored by ink and unity
// Use this to write comments and notes

////// INCLUDES ///////
// This is the list of included characters.
// When you want to create a new character, click "Add new include"
// at the bottom left of the screen and name it Charactername.ink.
// This list will be updated automatically.
INCLUDE Farmer.ink

////// START //////////
// This is a link. It moves you to another story knot.
// There has to be a link to the first knot, before the knots
// are defined.
// You can alt+click on a link to jump to the corresponding knot
-> knot_example

////// EXAMPLE KNOT ///
// This is a knot. It defines one bit of a story.
// A knot starts with two equal signs and the title
// The title can not have spaces. use an underscore _ instead
== knot_example
This is the text that will be displayed.
    // Answer knots are written with a + and the text in brackets
    // The text CAN have spaces
    // After an answer you can link to a new knot
    + [Answer] -> main

////// MAIN KNOT //////
// This is the main knot for testing
== main
Select a character
// If you add a new character, add a reference to its first dialogue
// bit here.
    + [Farmer] -> Farmer_Start_01