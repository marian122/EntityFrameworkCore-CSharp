namespace Prototype
{
    using Prototype.Data;

    public class StartUp
    {
        static void Main(string[] args)
        {
            SandwichMenu sandiwchMenu = new SandwichMenu();

            sandiwchMenu["BLT"] = new Sandwich("Wheat", "Bacon", "", "Lettuce, Tomato");
            sandiwchMenu["PB&J"] = new Sandwich("White", "", "", "Peanut Butter, Jelly");
            sandiwchMenu["Turkey"] = new Sandwich("Rye", "Turkey", "Swiss", "Lettuce, Onion, Tomato");

            sandiwchMenu["Vegetarian"] = new Sandwich("Wheat", "", "", "Lettuce, Onion, Tomato, Olive, Spinach");
            sandiwchMenu["ThreeMeatCombo"] = new Sandwich("Rye", "Turkey, Ham, Salami", "Provolone", "Lettuce, Onion");

            Sandwich firstSandwich = sandiwchMenu["BLT"].Clone() as Sandwich;
            Sandwich secondSandwitch = sandiwchMenu["ThreeMeatCombo"].Clone() as Sandwich;
            Sandwich thirdSandwich = sandiwchMenu["Turkey"].Clone() as Sandwich;

        }
    }
}
