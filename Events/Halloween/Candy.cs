
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    internal class Candy
    {
        internal static Dictionary<int, CandyObject> candys = new Dictionary<int, CandyObject>()
        {
            {1, new CandyObject(1, "Fruchtgummi-Tütchen", 10) },
            {2, new CandyObject(2, "Karamellbonbons", 10) },
            {3, new CandyObject(3, "Lutschbonbons", 5) },
            {4, new CandyObject(4, "Kaubonbon-Streifen", 10) },
            {5, new CandyObject(5, "Brausebonbons", 5) },
            {6, new CandyObject(6, "Schaumzucker-Figuren", 10) },
            {7, new CandyObject(7, "Kaugummikugeln", 10) },
            {8, new CandyObject(8, "Mini-Kaubonbons", 10) },
            {9, new CandyObject(9, "Pfefferminzbonbons", 10) },
            {10, new CandyObject(10, "Brausepulver-Tütchen", 10) },
            {11, new CandyObject(11, "Kakaodragees", 10) },
            {12, new CandyObject(12, "Zuckerstangen", 5) },
            {13, new CandyObject(13, "Geleefrüchte", 10) },
            {14, new CandyObject(14, "Lakritzrollen", 10) },
            {15, new CandyObject(15, "Pralinen mit Cremefüllung", 10) },
            {16, new CandyObject(16, "Marshmallow-Spieße", 10) },
            {17, new CandyObject(17, "Knisterbonbons", 10) },
            {18, new CandyObject(18, "Fruchtleder-Streifen", 10) },
            {19, new CandyObject(19, "Nougatriegel", 10) },
            {20, new CandyObject(20, "Mini-Tafelschokoladen", 10) },
            {21, new CandyObject(21, "Keks-Schokoriegel", 10) },
            {22, new CandyObject(22, "Karamellriegel", 10) },
            {23, new CandyObject(23, "Haselnuss-Riegel", 10) },
            {24, new CandyObject(24, "Kokosriegel", 10) },
            {25, new CandyObject(25, "Getreideriegel mit Honig", 10) },
            {26, new CandyObject(26, "Proteinriegel", 10) },
            {27, new CandyObject(27, "Müsliriegel mit Schokolade", 10) },
            {28, new CandyObject(28, "Fruchtriegel", 10) },
            {29, new CandyObject(29, "Mini-Butterkekse", 10) },
            {30, new CandyObject(30, "Schokokekse", 10) },
            {31, new CandyObject(31, "Waffelriegel mit Cremefüllung", 10) },
            {32, new CandyObject(32, "Mini-Donuts", 10) },
            {33, new CandyObject(33, "Zimtgebäck", 10) },
            {34, new CandyObject(34, "Mini-Chipstüten", 10) },
            {35, new CandyObject(35, "Salzstangen-Päckchen", 10) },
            {36, new CandyObject(36, "Käsecracker", 10) },
            {37, new CandyObject(37, "Erdnussflips", 10) },
            {38, new CandyObject(38, "Maischips", 10) },
            {39, new CandyObject(39, "Popcorn-Tütchen", 10) },
            {40, new CandyObject(40, "Snackbrezeln", 10) },
            {41, new CandyObject(41, "Reiswaffeln mit Schokoüberzug", 10) },
            {42, new CandyObject(42, "Pizzasnacks", 10) },
            {43, new CandyObject(43, "Überraschungstüten mit Süßigkeiten", 30) },
            {44, new CandyObject(44, "Halloween-Lollis", 20) },
            {45, new CandyObject(45, "Halloween-Gummifiguren", 20) },
            {46, new CandyObject(46, "Snackbox mit Mini-Snacks", 30) }
        };
    }



    internal class CandyObject
    {
        internal int Id { get; set; }
        internal string Name { get; set; }
        internal int Points { get; set; }

        internal CandyObject(int id, string name, int points)
        {
            Id = id;
            Name = name;
            Points = points;
        }
    }
}
