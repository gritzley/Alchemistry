public class Ingredient : Pickupable
{
    public enum Type
    {
        APPLE,
        PEPPER,
    }
    public Type type;
    // The name of the ingredient
    public string Name;
    // If this is true, putting this in a kettle will destroy the gameobject
    public bool DestroyOnUse;
}
