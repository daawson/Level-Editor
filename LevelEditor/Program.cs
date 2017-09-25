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
        [STAThread]
        static void Main(string[] args)
        {
            //var settings = ConfigurationManager.AppSettings;

            Game game = new Game("LevelEditor", 1280, 720, 60, false);
            //game.SetIcon("../../Assets/icon.png");
            game.WindowResize = false;
            game.ReleaseModeDebugger = false;

            game.MouseVisible = true;
            Scene scene = new Scene();
            new Handler(scene);
            game.Start(scene);
        }

        // Funkcja zmieniająca wielkość okna gry wg. szerokości i wysokości.
        public static void RestartWindow(int width, int height)
        {
            Game.Instance.SetWindow(width, height);
            /*
            var settings = ConfigurationManager.AppSettings;
            settings.Set("windowW", width.ToString());
            settings.Set("windowH", height.ToString());
            */    
        }
    }
}
