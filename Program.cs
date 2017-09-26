using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
using System.Configuration;
namespace LevelEditor
{
    /// <summary>
    /// EDYTOR POZIOMU BY DAAWSON
    /// </summary>
    class Program
    {
        public static int screenWidth = 1280;
        public static int screenHeight = 720;

        //bez tego nie odpalą się dialogi otwierania/zapisywania plików :( //TODO zrobić resercz co to je kurdebele
        [STAThread]        
        static void Main(string[] args)
        {
            //samotłumaczące się komendy i nazwy.
            Game game = new Game("Level Editor by Daawson", screenWidth, screenHeight, 60, false);
            game.SetIcon("../../Assets/icon.png");
            game.WindowResize = false;
            game.ReleaseModeDebugger = false;

            game.MouseVisible = true;
            Scene scene = new Scene();

            // Startowe okno otwierania pliku tilesetu (PNG)
            new Handler(scene);
            game.Start(scene);
        }

        // Funkcja zmieniająca wielkość okna gry wg. szerokości i wysokości. //nieaktualne cuz wszystkie funkcjie są w oknie głównym
        public static void RestartWindow(int width, int height)
        {
            Game.Instance.SetWindow(width, height);
            screenWidth = width;
            screenHeight = height;
            /*
            var settings = ConfigurationManager.AppSettings;
            settings.Set("windowW", width.ToString());
            settings.Set("windowH", height.ToString());
            */    
        }
    }
}
