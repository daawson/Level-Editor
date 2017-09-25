using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
using System.Windows.Forms;

namespace LevelEditor
{
    class Handler
    {
        public Scene usedScene;
        OpenFileDialog openFileDialog1;
        public Handler(Scene s)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Wybierz plik tilesetu: ";
            openFileDialog1.Filter = "PNG Files | *.png";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                if (openFileDialog1.FileName == null)
                {
                    Environment.Exit(0);
                }
                else
                {
                    Image file = new Image(openFileDialog1.FileName);

                    usedScene = s;
                    usedScene.Add(new Akcja(usedScene, openFileDialog1.FileName, file));
                }
            }
            else if(result == DialogResult.None || result == DialogResult.Cancel || result == DialogResult.No)
            {
                Environment.Exit(0);
            }
        }
    }
}
