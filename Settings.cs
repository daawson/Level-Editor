using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Otter;

namespace LevelEditor
{
    // Potrzebna pozozostałość po windowsowym oknie opcji, zawiera wszystkie funkcjie wyłapywanie otwierania/zapisywania pliku itd.
    class Settings
    {
        #region FIELDS

        Akcja akszyn;

        static SaveFileDialog sfd = new SaveFileDialog();        
        static OpenFileDialog ofd = new OpenFileDialog();

        //TODO DODAC Z POWROTEM SWITCHE KOLIZJI
        Czek[] cb; //?
        public static Boolean[] collider;
        public static Boolean[] wall;

        #endregion

        #region VOID METHODS /EVENTS

        public static void saveClick()
        {
            sfd.Filter = "XML Files | *.xml";
            DialogResult dr = sfd.ShowDialog();
        }

        void sfd_FileOk(Object sender, CancelEventArgs e)
        {
            string name = sfd.FileName;
            File.WriteAllText(name, Akcja.SaveFile());
        }

        public static void loadClick()
        {
            ofd.Filter = "XML Files | *.xml";
            DialogResult dr = ofd.ShowDialog();
            //int size = -1;
            if (dr == DialogResult.OK)
            {
                string file = ofd.FileName;
                try
                {
                    Akcja.Instance.LoadLevel(file);
                }
                catch (IOException)
                {
                }
            }

        }

        public static void quitClick()
        {
            Akcja.QuitEditor();
        }

        public static void newClick()
        {
            Akcja.Instance.ClearLevel();
        }

        public static void helpClick()
        {
            string message = "All the functions are shown in movable inEditor windows!!";
            message += "\nTiles: Chose a tile to draw on the map. LMB to place. RMB to remove.";
            message += "\nInfo: Shows the currently choosen tile, also displays current mouse position according to grid";
            message += "\nOptions: Basic options for your level. Save/Load/Create New/Quit";
            string caption = "Help?";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

        }
        
        // wczytywanie colliderów dla funkcji wczytania
        public void SetCollider(int id)
        {
            cb[id].Checked = true;
            collider[id] = true;
            //collider[cb[id].ID] = true;
        }
        // używane podczas ClearLevel
        public void ClearColliders()
        {
            for(int i = 0; i < cb.Count(); i++)
            {
                cb[i].Checked = false;
                collider[i] = false;
            }
        }

         #endregion

        // MAIN
        public Settings(Akcja a)
        {
            akszyn = a;
            sfd.FileOk += new CancelEventHandler(sfd_FileOk);
            collider = new Boolean[Akcja.indexTiles+1];
            wall = new Boolean[Akcja.indexTiles + 1];      
        }   
    }

    // klasa z ID do czekboxa koliderów //TODO: usunąć/naprawić/przerobić
    class Czek : CheckBox
    {
        public int ID;
    }
}
