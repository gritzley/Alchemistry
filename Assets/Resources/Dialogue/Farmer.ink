// This is a story knot.
// You can call them however you want, but it's good to give them
// numbers so we can search for them however we want.
== Farmer_Start_01
    // The text here will be displayed as the farmers dialogue
    Hello good lady. I need myself a potion for my cow.
    It has a stomach ache and won't give no milk no more.
    + [Nod] -> Farmer_Start_02

// This is a potion knot. It is written with three equal signs.
// For inky, story knots and potion knots are the same.
// In unity they are handled differently.
=== Farmer_Start_02
    Well, do you have anything for me?
    // Write down a list of all the potions and where they lead
    // If a Potion is not on this list, unity will link it to the next
    // knot in this file.
    + [Potion 1] -> Farmer_Start_03
    
== Farmer_Start_03
    Thank you. I hope this works.
    + [Wait till he returns] -> Farmer_End
    
== Farmer_End
    Thank you, my cow is now healthy.
    // Link to main when the characters story is finished
    + [Go back to character selection] -> main