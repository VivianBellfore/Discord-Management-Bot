
using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    internal class WinterManager
    {
        /// <summary>
        /// Contains different poems and gifs for each advent day.
        /// </summary>
        internal static Dictionary<int, AdventObject> AdventItems = new Dictionary<int, AdventObject>()
        {
            {1, new AdventObject("Bald ist Weihnacht, wie freu ich mich drauf,\r\nda putzt uns die Mutter ein Bäumlein schön auf,\r\nes glänzen " +
                "die Äpfel, es funkeln die Stern,\r\nwie hab´n wir doch alle das Weihnachtsfest gern.",
                "https://lost-city.net/advent_images/advent_1.png") },
            {2, new AdventObject("Strahlend, wie ein schöner Traum,\r\nsteht vor uns der Weihnachtsbaum.\r\nSeht nur, wie sich goldenes Licht\r\nauf der zarten Kugeln bricht." +
                "\r\n“Frohe Weihnacht” klingt es leise\r\nund ein Stern geht auf die Reise.\r\nLeuchtet hell vom Himmelszelt -\r\nhinunter auf die ganze Welt.",
                "https://lost-city.net/advent_images/advent_2.png") },
            {3, new AdventObject("1. Advent\nIm Winter, wenn es stürmt und schneit\r\nUnd's Weihnachtsfest ist nicht mehr weit.\r\n\r\nDa kommt weit her aus dunklem Tann'\r\n" +
                "Der liebe, gute Weihnachtsmann.",
                "https://lost-city.net/advent_images/advent_3.png") },
            {4, new AdventObject("Nun leuchten wieder die Weihnachtskerzen\r\nund wecken Freude in allen Herzen.\r\nIhr lieben Eltern, in diesen Tagen,\r\nwas sollen wir " +
                "singen, was sollen wir sagen?\r\nWir wollen euch wünschen zum heiligen Feste\r\nvom Schönen das Schönste, vom Guten das Beste!\r\nWir wollen euch danken " +
                "für alle Gaben\r\nund wollen euch immer noch lieber haben.",
                "https://lost-city.net/advent_images/advent_4.png") },
            {5, new AdventObject("Vom Himmel bis in die tiefsten Klüfte\r\nein milder Stern herniederlacht;\r\nvom Tannenwalde steigen Düfte\r\nund kerzenhelle wird die " +
                "Nacht.\r\n\r\nMir ist das Herz so froh erschrocken,\r\ndas ist die liebe Weihnachtszeit!\r\nIch höre fernher Kirchenglocken,\r\nin märchenstiller " +
                "Herrlichkeit.\r\n\r\nEin frommer Zauber hält mich nieder,\r\nanbetend, staunend muß ich stehn,\r\nes sinkt auf meine Augenlider,\r\nich fühl's, ein " +
                "Wunder ist geschehn.",
                "https://lost-city.net/advent_images/advent_5.png") },
            {6, new AdventObject("Nikolaus\nWird es dunkel vor dem Haus,\r\nkommt zu uns der Nikolaus.\r\nHat uns etwas mitgebracht,\r\nschöner als wir ja gedacht." +
                "\r\n\r\nSteht der Baum im Lichterschein,\r\ngehen wir zu Tür hinein.\r\nWeihnacht, Weihnacht - es ist wahr,\r\nist das schönste Fest im Jahr.",
                "https://lost-city.net/advent_images/advent_6.png")},
            {7, new AdventObject("Plätzchenduft zieht durch das Haus,\r\nversperrt sind manche Schränke.\r\nes weihnachtet, man kennt sich aus\r\nund wohlsortiert sind " +
                "die Geschenke.\r\n\r\nMan freut sich auf das Kinderlachen\r\nund auf ein paar Tage - ruhig und still,\r\nandern `mal eine Freude machen,\r\ndas ist es, " +
                "was man will.\r\n\r\nWeihnachtskarten trudeln ein\r\nvon allen Ecken und Kanten,\r\ndie meisten sind, so soll es ein\r\nvon den Lieben und Verwandten.",
                "https://lost-city.net/advent_images/advent_7.png") },
            {8, new AdventObject("Schöne Lieder, warme Worte,\r\ntiefe Sehnsucht, ruhige Orte,\r\nGedanken, die voll Liebe klingen,\r\nWeihnachten möcht' ich nur mit " +
                "dir verbringen.",
                "https://lost-city.net/advent_images/advent_8.png") },
            {9, new AdventObject("Lieber guter Weihnachtsmann,\r\nschau mich nicht so böse an!\r\nPacke deine Rute ein,\r\nich will auch immer lieb und artig sein.",
                "https://lost-city.net/advent_images/advent_9.png") },
            {10, new AdventObject("2. Advent\nWenn's Licht brennt heller,\r\nWenn's Herz schlägt schneller,\r\nDann weiß ich ganz genau:\r\nWheinachten steht vor " +
                "der Tür mit einem Plätzchenteller.",
                "https://lost-city.net/advent_images/advent_10.png") },
            {11, new AdventObject("Lieber guter Weihnachtsmann,\r\nschenk mir doch ein Schokomann.\r\nNicht zu groß, nicht zu klein...\r\naber LECKER muss er sein!",
                "https://lost-city.net/advent_images/advent_11.png") },
            {12, new AdventObject("Oh, wie lieb ich die Gerüche\r\naus der warmen Weihnachtsküche!\r\nZieht der süße Duft hinaus,\r\nriecht man ihn im ganzen " +
                "Haus.\r\n\r\nHörnchen, Herzen, Zuckerkringel,\r\nPfefferkuchen, Schokoringel,\r\nBrezeln, Sterne und noch mehr -\r\nPlätzchenbacken ist nicht " +
                "schwer.\r\n\r\nBesser noch als die vom Bäcker\r\nschmecken sie - so köstlich, lecker!\r\nKeiner könnte widerstehn,\r\nwenn sie auf dem Festtisch stehn.",
                "https://lost-city.net/advent_images/advent_12.png") },
            {13, new AdventObject("Ihr heller, leuchtend warmer Schein,\r\nlädt uns zur Besinnung ein.\r\nDer Heiligabend ist nicht mehr fern.\r\nWir warten in " +
                "Hoffnung, begrüßen den Herrn.",
                "https://lost-city.net/advent_images/advent_13.png") },
            {14, new AdventObject("Es treibt der Wind im Winterwalde\r\nDie Flockenherde wie ein Hirt,\r\nUnd manche Tanne ahnt, wie balde\r\nSie fromm und " +
                "lichterheilig wird.\r\n\r\nSie lauscht hinaus. Den weissen Wegen\r\nStreckt sie die Zweige hin bereit\r\nUnd wehrt dem Wind und wächst entgegen\r\nDer " +
                "einen Nacht der Herrlichkeit.",
                "https://lost-city.net/advent_images/advent_14.png") },
            {15, new AdventObject("Der Winter ist ein karger Mann, \r\ner hat von Schnee ein Röcklein an;\r\nzwei Schuh von Eis \r\nsind nicht zu heiß;\r\nvon rauhem " +
                "Reif eine Mütze \r\nmacht auch nur wenig Hitze. \r\n Er klagt: „Verarmt ist Feld und Flur!\" \r\nDen grünen Christbaum hat er nur;\r\nden trägt er aus " +
                "in jedes Haus, \r\nin Hütten und Königshallen:\r\nden schönsten Strauß von allen!",
                "https://lost-city.net/advent_images/advent_15.png") },
            {16, new AdventObject("Es war einmal ein Tännelein \r\nmit braunen Kuchenherzlein \r\nund Glitzergold und Äpflein fein \r\nund vielen bunten Kerzlein: " +
                "\r\nDas war am Weihnachtsfest so grün \r\nals fing es eben an zu blüh'n. \r\n\r\nDoch nach nicht gar zu langer Zeit, \r\nda stand's im Garten unten, " +
                "\r\nund seine ganze Herrlichkeit \r\nwar, ach, dahingeschwunden. \r\nDie grünen Nadeln war'n verdorrt, \r\ndie Herzlein und die Kerzlein fort. \r\n " +
                "Bis eines Tags der Gärtner kam, \r\nden fror zu Haus im Dunkeln, \r\nund es in seinen Ofen nahm -\r\nHei! Tat's da sprüh'n und funkeln! \r\nUnd flammte " +
                "jubelnd himmelwärts \r\nin hundert Flämmlein an Gottes Herz.",
                "https://lost-city.net/advent_images/advent_16.png") },
            {17, new AdventObject("3. Advent\nDraußen leuchten Sterne der heiligen Nacht,\r\nUnd drinnen glänzt der Weihnachtsbaum in strahlender Pracht.\r\nDer " +
                "Weihnachtsbraten ist aus dem Ofen ganz frisch,\r\nman stellt ihn gerade auf den Tisch.\r\nDie Kinder packen fröhlich die Geschenke aus,\r\nDrum wünsch' " +
                "ich euch 'nen schönen Weihnachtsrausch.",
                "https://lost-city.net/advent_images/advent_17.png") },
            {18, new AdventObject("Nun leuchten wieder die Weihnachtskerzen \r\nund wecken Freude in allen Herzen.\r\nIhr lieben Eltern, in diesen Tagen, \r\nwas " +
                "sollen wir singen, was sollen wir sagen? \r\n Wir wollen euch wünschen zum heiligen Feste \r\nvom Schönen das Schönste, vom Guten das Beste!\r\nWir " +
                "wollen euch danken für alle Gaben \r\nund wollen euch immer noch lieber haben.",
                "https://lost-city.net/advent_images/advent_18.png") },
            {19, new AdventObject("Sind Weihnachtsmann und Christkind da,\r\nFür Kinder einfach wunderbar.\r\ndoch ob Groß und Klein,\r\njeder soll heut glücklich " +
                "sein.\r\nMit Rotkohl, Gans und nem Rosè,\r\ntun die Streitereien kaum weh.\r\nDas Weihnachtsfest als Pulverfass,\r\nWort des Tages lautet: Hass.\r\nDie " +
                "Mutter weint, der Vater voll.\r\nWeihnachten ist doch wundervoll.",
                "https://lost-city.net/advent_images/advent_19.png") },
            {20, new AdventObject("Draußen weht es bitterkalt, \r\nwer kommt da durch den Winterwald?\r\nStippstapp, stippstapp und huckepack.\r\nKnecht Ruprecht " +
                "ist’s mit seinem Sack. \r\nWas ist denn in dem Sacke drin? \r\nÄpfel, Mandeln und Rosin’ \r\nund schöne Zuckerrosen, \r\nauch Pfeffernüss’ fürs gute " +
                "Kind; \r\ndie andern, die nicht artig sind, \r\nklopft er auf die Hosen.",
                "https://lost-city.net/advent_images/advent_20.png") },
            {21, new AdventObject("Eine blütenweiße Decke schwebt herab vom Himmelszelt \r\nzaubert sanft in aller Stille eine zarte Märchenwelt. \r\nHörst du auch " +
                "im Wald die Tannen? \r\nEine raunt der anderen zu, \r\n\"Schon sehr bald ist wieder Weihnacht, \r\nendlich kommt das Land zur Ruh!\" \r\nFenster " +
                "strahlen hell erleuchtet, Feuer knistert im Kamin. \r\nSpür das Glück ganz tief im Herzen, weil ich hier zu Hause bin.",
                "https://lost-city.net/advent_images/advent_21.png") },
            {22, new AdventObject("Glöckchen klingen leise -\r\nder Weihnachtsstern geht auf seine Reise.\r\nLeuchtet hell vom Himmelszelt -\r\nhinunter auf die ganze " +
                "Welt.\r\nEr führt uns durch die Dunkelheit\r\nund kündet von der nahen Weihnachtszeit\r\nSeht nur, wie er golden strahlt\r\nund Hoffnung in die " +
                "Gesichter der Menschen malt.",
                "https://lost-city.net/advent_images/advent_22.png") },
            {23, new AdventObject("Die Nacht vor dem Heiligen Abend, da liegen die Kinder im Traum. \r\nSie träumen von schönen Sachen und von dem Weihnachtsbaum.\n" +
                "Und während sie schlafen und träumen, wird es am Himmel klar, \r\nund durch den Himmel fliegen drei Engel wunderbar. ",
                "https://lost-city.net/advent_images/advent_23.png") },
            {24, new AdventObject("4. Advent Heiligabend\nGesegnet sei die heilige Nacht, \r\ndie uns das Licht der Welt gebracht! - wohl unterm lieben " +
                "Himmelszelt \r\ndie Hirten lagen auf dem Feld. Ein Engel Gottes, licht und klar, \r\nmit seinem Gruß tritt auf sie dar. Vor Angst sie decken ihr " +
                "Angesicht,\r\nda spricht der Engel: „Fürchtet euch nicht!\" \"Ich verkünd euch große Freud: \r\nDer Heiland ist geboren heut.\"\r\nDa gehn die " +
                "Hirten hin in Eil, \r\nzu schaun mit Augen das ewig Heil;\r\nzu singen dem süßen Gast Willkomm, \r\nzu bringen ihm ein Lämmlein fromm. " +
                "Bald kommen auch gezogen fern \r\ndie heilgen drei König' mit ihrem Stern. Sie knieen vor dem Kindlein hold, \r\nschenken ihm Myrrhen, " +
                "Weihrauch, Gold. Vom Himmel hoch der Engel Heer \r\nfrohlocket: \"Gott in der Höh sei Ehr!\" ",
                "https://lost-city.net/advent_images/advent_24.png") }
        };

        /// <summary>
        /// Checks if enough time has past untill the last use. Also adding the new time stamp to data base.
        /// </summary>
        internal static async Task<(bool, string)> CanUserDoWinterWorkNow(ulong userId)
        {
            object result = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `date` FROM `user_event_date` WHERE `user_id` = @user_id AND `event_type` = @event_type",
                new Dictionary<string, object> { { "user_id", userId }, { "event_type", "winterwork" } });

            if (result == null)
            {
                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `user_event_date` (`user_id`, `event_type`, `date`) VALUES (@user_id, @event_type, @date)",
                    new Dictionary<string, object> { { "user_id", userId }, { "event_type", "winterwork" }, { "date", DateTime.Now.ToString("o") } });

                return (true, "");
            }
            
            if (DateTime.Parse(Convert.ToString(result)) <= DateTime.Now.AddMinutes(15))
            {
                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "UPDATE `user_event_date` SET `date` = @date WHERE `user_id` = @user_id AND `event_type` = @event_type",
                    new Dictionary<string, object> { { "user_id", userId }, { "event_type", "winterwork" }, { "date", DateTime.Now.ToString("o") } });

                return (true, "");
            }

            return (false, DateTime.Parse(Convert.ToString(result)).ToString());
        }
    }



    /// <summary>
    /// Contains the dictionary with christmas poems and gifs.
    /// </summary>
    public class AdventObject
    {
        public string Poem { get; set; }
        public string Gif { get; set; }

        public AdventObject(string poem, string gif)
        {
            Poem = poem;
            Gif = gif;
        }
    }
}
