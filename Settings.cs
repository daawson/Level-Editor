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
    // Klasa okna z opcjami //TODO: Poczyścić kod
    class Settings:Form
    {
        #region FIELDS

        Akcja akszyn;
        TabControl tc = new TabControl();

        Czek[] cb;
        //Button[] block;
        //Button save, load, quit, newLevel, help;

        ListBox resolutionsBox;

        static SaveFileDialog sfd = new SaveFileDialog();        
        static OpenFileDialog ofd = new OpenFileDialog();

        public static Boolean[] collider;
        public static Boolean[] wall;

        //TabPage wallPage;

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

        private void Tc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        // MAIN
        public Settings(Akcja a)
        {
            this.Text = "LevelEditor";
            this.Focus();
            akszyn = a;
            sfd.FileOk += new CancelEventHandler(sfd_FileOk);
            //this.FormClosing += new FormClosingEventHandler(quitClick);
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            //this.LocationChanged += Window_LocationChanged();


            collider = new Boolean[Akcja.indexTiles+1];
            wall = new Boolean[Akcja.indexTiles + 1];

            this.ClientSize = new System.Drawing.Size(300, 720);
            this.BackColor = System.Drawing.Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            tc.Location = new System.Drawing.Point(0, 0);
            tc.ClientSize = new System.Drawing.Size(this.Width, this.Height);

            Controls.Add(tc);
            //tc.Controls.Add(InitTab1());
            //tc.Controls.Add(InitTab2());
            //wallPage = InitTab3();
            //tc.Controls.Add(wallPage);
            //tc.Controls.Add(InitTab4());
            tc.SelectedIndexChanged += Tc_SelectedIndexChanged;

      
        }        

        /*
        #region TABPAGES
        // Generuje zakładki okna opcji.
        TabPage InitTab1()
        {
            TabPage tp = new TabPage("Tiles");

            PictureBox pb1 = new PictureBox();
            //pb1.Enabled = false;
            pb1.ImageLocation = Akcja.TILESET_STRING;
            pb1.SizeMode = PictureBoxSizeMode.AutoSize;
            pb1.Location = new System.Drawing.Point((tc.Width / 2) - (Akcja.indexX * 32) / 2, 10);
            pb1.BringToFront();

            tp.Controls.Add(pb1);
            block = new Button[Akcja.indexTiles];

            int x = 0, y = 0;
            for (int i = 0; i < Akcja.indexTiles; i++)
            {

                if (x == Akcja.indexX)
                {
                    x = 0;
                    y++;
                }

                block[i] = new Button();
                //cb[i].Parent = pb1;

                block[i].Location = new System.Drawing.Point(x * 32, y * 32);
                block[i].Size = new System.Drawing.Size(32, 32);

                block[i].FlatStyle = FlatStyle.Popup;
                block[i].UseVisualStyleBackColor = false;
                block[i].BackColor = System.Drawing.Color.Transparent;
                block[i].ForeColor = System.Drawing.Color.FromArgb(25, 25, 25, 25);

                block[i].Font = new System.Drawing.Font(Font.FontFamily, 7f);
                block[i].Text = i.ToString();
                block[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                block[i].Click += new EventHandler(changeCurrent);

                x++;

                pb1.Controls.Add(block[i]);
                //cb[i].BringToFront();
            }

            return tp;
        }

        TabPage InitTab2()
        {
            TabPage tp = new TabPage("Collider");
            
            PictureBox pb1 = new PictureBox();
            //pb1.Enabled = false;
            pb1.ImageLocation = Akcja.TILESET_STRING;
            pb1.SizeMode = PictureBoxSizeMode.AutoSize;
            pb1.Location = new System.Drawing.Point((tc.Width/2) - (Akcja.indexX*32)/2, 10);
            pb1.BringToFront();

            tp.Controls.Add(pb1);
            cb = new Czek[Akcja.indexTiles];

            int x = 0, y = 0;
            for (int i = 0; i < Akcja.indexTiles; i++)
            {

                if (x == Akcja.indexX)
                {
                    x = 0;
                    y++;
                }

                cb[i] = new Czek();
                //cb[i].Parent = pb1;

                cb[i].Location = new System.Drawing.Point(x * 32, y * 32);
                cb[i].Size = new System.Drawing.Size(32, 32);

                cb[i].Appearance = Appearance.Button;
                cb[i].FlatStyle = FlatStyle.Popup;
                cb[i].BackColor = System.Drawing.Color.Transparent;

                cb[i].Font = new System.Drawing.Font(Font.FontFamily, 7f);
                cb[i].Text = i.ToString();
                cb[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                cb[i].ID = i;
                cb[i].CheckedChanged += new EventHandler(checkChange);
                collider[i] = false;

                x++;

                pb1.Controls.Add(cb[i]);
                //cb[i].BringToFront();
            }
            return tp;
        }

        TabPage InitTab3()
        {
            TabPage tp = new TabPage("Walls");

            PictureBox pb1 = new PictureBox();
            //pb1.Enabled = false;
            pb1.ImageLocation = Akcja.TILESET_STRING;
            pb1.SizeMode = PictureBoxSizeMode.AutoSize;
            pb1.Location = new System.Drawing.Point((tc.Width / 2) - (Akcja.indexX * 32) / 2, 10);
            pb1.BringToFront();

            tp.Controls.Add(pb1);
            block = new Button[Akcja.indexTiles];

            int x = 0, y = 0;
            for (int i = 0; i < Akcja.indexTiles; i++)
            {

                if (x == Akcja.indexX)
                {
                    x = 0;
                    y++;
                }

                block[i] = new Button();
                //cb[i].Parent = pb1;

                block[i].Location = new System.Drawing.Point(x * 32, y * 32);
                block[i].Size = new System.Drawing.Size(32, 32);

                block[i].FlatStyle = FlatStyle.Popup;
                block[i].BackColor = System.Drawing.Color.Transparent;

                block[i].Font = new System.Drawing.Font(Font.FontFamily, 7f);
                block[i].Text = i.ToString();
                block[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                block[i].Click += new EventHandler(changeCurrent);

                x++;

                pb1.Controls.Add(block[i]);
            }

            return tp;
        }
        /*
        TabPage InitTab4()
        {
            TabPage tp = new TabPage("Options");
            save = new Button();
            save.FlatStyle = FlatStyle.System;
            save.Text = "Save Level";
            save.Location = new System.Drawing.Point(10, 10);
            save.Size = new System.Drawing.Size(80, 24);
            save.Click += new EventHandler(saveClick);
            tp.Controls.Add(save);

            load = new Button();
            load.FlatStyle = FlatStyle.System;
            load.Text = "Load Level";
            load.Location = new System.Drawing.Point(10 + 80 + 20, 10);
            load.Size = new System.Drawing.Size(80, 24);
            load.Click += new EventHandler(loadClick);
            tp.Controls.Add(load);

            quit = new Button();
            quit.FlatStyle = FlatStyle.System;
            quit.Text = "Quit";
            quit.Location = new System.Drawing.Point(10 + 80 + 20 + 80 + 20, 10+30+10+30);
            quit.Size = new System.Drawing.Size(80, 24);
            quit.Click += new EventHandler(quitClick);
            tp.Controls.Add(quit);

            newLevel = new Button();
            newLevel.FlatStyle = FlatStyle.System;
            newLevel.Text = "Create New";
            newLevel.Location = new System.Drawing.Point(10+80+20+80+20, 10);
            newLevel.Size = new System.Drawing.Size(80, 24);
            newLevel.Click += new EventHandler(newClick);
            tp.Controls.Add(newLevel);

            help = new Button();
            help.FlatStyle = FlatStyle.System;
            help.Text = "Help";
            help.Location = new System.Drawing.Point(10+80+20, 10 + 30+10+30);
            help.Size = new System.Drawing.Size(80, 24);
            help.Click += new EventHandler(helpClick);
            tp.Controls.Add(help);

            Label lb = new Label();
            lb.BorderStyle = BorderStyle.Fixed3D;
            lb.AutoSize = false;
            lb.Height = 2;
            lb.Width = tp.Width+70;
            lb.Location = new System.Drawing.Point(10, 10 + 30 + 10+10);
            tp.Controls.Add(lb);

            Label lb2 = new Label();
            lb2.BorderStyle = BorderStyle.Fixed3D;
            lb2.AutoSize = false;
            lb2.Height = 2;
            lb2.Width = tp.Width+70;
            lb2.Location = new System.Drawing.Point(10, 10 + 30 + 10 + 30+10+30);
            tp.Controls.Add(lb2);

            // resolutions
            List<string> resolutions = new List<string>();

            resolutions.Add("1366 x 768");
            resolutions.Add("1280 x 720");
            resolutions.Add("1024 x 576");

            resolutionsBox = new ListBox();
            resolutionsBox.DataSource = resolutions;

            resolutionsBox.Location = new System.Drawing.Point(10, 10 + 30 + 10 + 30 + 10 + 30+20);

            resolutionsBox.DoubleClick += ResolutionsBox_DoubleClick;
            tp.Controls.Add(resolutionsBox);

            
            return tp;

        }

        private void ResolutionsBox_DoubleClick(object sender, EventArgs e)
        {
            string item = resolutionsBox.SelectedItem.ToString();
            if (item.Equals("1366 x 768"))
            {
                Program.RestartWindow(1366, 768);
                akszyn.Game.Surface.ScaledWidth = 1280;
                akszyn.Game.Surface.ScaledHeight = 720;
                this.ClientSize = new System.Drawing.Size(300, 768);
                Akcja.currentRes = 0;
            }
            else if (item.Equals("1280 x 720"))
            {
                Program.RestartWindow(1280, 720);
                akszyn.Game.Surface.ScaledWidth = 1280;
                akszyn.Game.Surface.ScaledHeight = 720;
                this.ClientSize = new System.Drawing.Size(300, 720);
                Akcja.currentRes = 1;
            }
            else if (item.Equals("1024 x 576"))
            {
                Program.RestartWindow(1024, 576);
                akszyn.Game.Surface.ScaledWidth = 1280;
                akszyn.Game.Surface.ScaledHeight = 720;
                this.ClientSize = new System.Drawing.Size(300, 576);
                Akcja.currentRes = 2;
            }
            Akcja.updatePreview = true;
        }
        #endregion

        */
    }
    class Czek : CheckBox
    {
        public int ID;
    }
}
