using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Otter;
using CMenu;

using System.Diagnostics;

namespace LevelEditor
{    
    // klasa 'handlująca', zawiera główne opcje
    // TODO: Usunąć niepotrzebne publiki. Wyczyścić kod.
    class Akcja : Entity
    {

        #region TILESET DATA

        public static Image TILESET_PATH;
        public static string TILESET_STRING;
        //string LEVELDATA = "../../Assets/LevelData.xml";

            
        public static bool IsEven(int value)
        {
            return value % 2 == 0;
        }

        // wyciąga fragment 32x32 z tekstury
        // TODO: Dodać funkcje wyboru szerokości fragmentu np. 64x64 itd.
        public Rectangle GetTexture(int id)
        {
            int usedID = id;
            int x = usedID % indexX;
            int y = usedID / indexX;
            return new Rectangle(x * 32, y * 32, 32, 32);
        }

        #endregion

        #region FIELDS        

        public static Scene usedScene;

        public static int maxGridW = 64;
        public static int maxGridH = 64;

        //int tileSize = 32;

        public static int CurrentTile { get; set; } = 0;

        public static int[,] tholder;

        Tilemap edges;

        //GraphicList LAYER0;
        Tilemap MAIN_TILEMAP;

        public static bool updatePos { get; set; } = true;
        public static bool updatePreview { get; set; } = true;

        public Image preview;
        public Text tt,ttpos;
        public Image underbox, ub2;

        int prevX;
        int prevY = 0;

        public int mouseCol { get; set; } = 1337;

        public enum Kolider
        {
            mysz, guzik
        }

        public static int indexTiles;
        public static int indexX;
        public static int indexY;

        public Settings set;

        public static int currentRes = 1;
        #endregion        

        #region METHODS
        public Akcja(Scene s, string st, Image img)
        {
            usedScene = s;
            TILESET_PATH = img;
            TILESET_STRING = st;

            SetupEdges();

            //LAYER0 = new GraphicList();
            MAIN_TILEMAP = new Tilemap(st, 64 * 32, 32);
            //LAYER1 = new GraphicList();
            SetupTiles();
            SetupPreviewInfo();


            

            set = new Settings(this);
            set.Show();

            updatePreview = true;
            updatePos = true;

            usedScene.Add(OtterMenu.Instance);
        }

        //dodaje krawędzie wokół głównej mapy.
        void SetupEdges()
        {
            edges = new Tilemap("../../Assets/edges_tileset.png", 66 * 32, 32);
            edges.SetPosition(-32, -32);
            AddGraphic(edges);
            for (int x = -1; x <= maxGridW; x++)
            {

                for (int y = -1; y <= maxGridH; y++)
                {
                    //boki
                    if(x == -1 && y > -1)
                    {
                        edges.SetTile(x, y, 3);
                    }
                    else if (y == -1 && x > -1)
                    {
                        edges.SetTile(x, y, 1);
                    }
                    else if (x == maxGridW && y > -1)
                    {
                        edges.SetTile(x+1, y, 5);
                    }
                    else if (y == maxGridH && x > -1)
                    {
                        edges.SetTile(x+1, y+1, 7);
                    }

                    //narozniki
                    if (x == 0 && y == 0)
                    {
                        edges.SetTile(x, y, 0);
                    }
                    else if (x == maxGridW && y == 0)
                    {
                        edges.SetTile(x + 1, y, 2);
                    }
                    else if (x == 0 && y == maxGridH)
                    {
                        edges.SetTile(x, y + 1, 6);
                    }
                    else if (x == maxGridW && y == maxGridH)
                    {
                        edges.SetTile(x + 1, y + 1, 8);
                    }

                }
            }

            Grid g = new Grid((maxGridW) * 32, (maxGridH) * 32, 32, 32, Color.FromBytes(176, 224, 230), Color.FromBytes(230, 230, 250));
            //g.SetPosition(-(5 * 32), -(5 * 32));
            AddGraphic(g);
        }

        //zmienia podgląd aktualnie wybranego fragmentu tilesetu.
        void SetupPreviewInfo()
        {
            ub2 = Image.CreateRectangle(110, 180, Color.FromBytes(100, 0, 0, 90));
            AddGraphic(ub2);

            underbox = Image.CreateRectangle(100, 170, Color.FromBytes(100, 0, 0, 90));
            AddGraphic(underbox);

            ttpos = new Text();
            ttpos.Color = Color.White;
            ttpos.SetPosition(0, 0);
            AddGraphic(ttpos);

            tt = new Text();
            tt.Color = Color.White;
            tt.SetPosition(0, 0);
            AddGraphic(tt);
            
            preview = new Image(TILESET_PATH.Texture);
            preview.SetPosition(prevX - 34, prevY);
            preview.ClippingRegion = GetTexture(1);
            preview.AtlasRegion = GetTexture(0);
            AddGraphic(preview);


            
        }
        
        //wypełnia wszystkie pola ID0
        void SetupTiles()
        {


            Image prev = new Image(TILESET_PATH.Texture);

            indexTiles = (prev.Width / 32) * (prev.Height / 32);
            indexX = prev.Width / 32;
            indexY = prev.Height / 32;
            prevX = 1280 - indexX * 32;

            //Console.WriteLine("index X:" + indexX + " index Y:" + indexY + " total: " + indexTiles);

            tholder = new int[maxGridW, maxGridH];

            //LAYER0.AutoClear = false;
            //LAYER1.AutoClear = false;

            

            AddGraphics(
                MAIN_TILEMAP
                );

            for (int x = 0; x < maxGridW; x++)
            {
                for (int y = 0; y < maxGridH; y++)
                {
                    tholder[x, y] = 0;
                    MAIN_TILEMAP.SetTile(x, y, 0);
                }
            }
        }

        // CZITERSKIE ZAPISANIE PLIKU
        public static String SaveFile()
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load(LEVELDATA);

            String format = "";
            format += "<leveldata>\n\t<settings>\n\t";

            int c = 0;
            foreach(bool b in Settings.collider)
            {

                format += "<collision id='" + c + "' cc='" + b + "'></collision>\n";
                c++;
            }


            format += "</settings>\n\t<level>\n\t<layer0>\n\t";
            for (int x = 0; x < maxGridW; x++)
            {
                for (int y = 0; y < maxGridH; y++)
                {
                    format += "\t<tile x='" + x + "' y='" + y + "' id='" + tholder[x, y] + "'></tile>\n";
                }
            }

            format += "</layer0>\n";

            format += "\t</level>\n</leveldata>";
            //Console.Write(format);

            return format;

        }

        public void ClearLevel()
        {
            for(int x =0; x < maxGridW; x++)
            {
                for(int y=0; y < maxGridH; y++)
                {
                    tholder[x, y] = 0;                    
                    MAIN_TILEMAP.SetTile(x, y, 0);

                }
            }

            set.ClearColliders();
        }

        //Wwczytanie poziomy
        public void LoadLevel(String t)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(t);

            var layer0 = doc.SelectSingleNode("/leveldata/level/layer0");
            var settings = doc.SelectSingleNode("/leveldata/settings");


            int tilescount = doc.GetElementsByTagName("collision").Count;
            set.ClearColliders();
            // wczytywanie koliderów
            for (int i = 0; i < tilescount-1 ; i++)
            {
                var cc = settings.ChildNodes[i];

                int id = Int32.Parse(cc.Attributes["id"].Value);
                bool hasCC = Boolean.Parse(cc.Attributes["cc"].Value);

                if (hasCC)
                {
                    //Console.WriteLine(hasCC + " " + cc.Attributes["cc"].Value);
                    set.SetCollider(id);
                }
            }

            int count = doc.GetElementsByTagName("tile").Count;
            ClearLevel();

            // wczytywanie tilesetów
            for (int i = 0; i < count; i++)
            {               

                var childNode = layer0.ChildNodes[i];
                int x = Int32.Parse(childNode.Attributes["x"].Value);
                int y = Int32.Parse(childNode.Attributes["y"].Value);
                int id = Int32.Parse(childNode.Attributes["id"].Value);
                tholder[x, y] = id;
                MAIN_TILEMAP.SetTile(x, y, id);
                
                
            }

            /*
            for (int i = 0; i < count / 2; i++)
            {


                var childNode = layer1.ChildNodes[i];
                int x = Int32.Parse(childNode.Attributes["x"].Value);
                int y = Int32.Parse(childNode.Attributes["y"].Value);
                int id = Int32.Parse(childNode.Attributes["id"].Value);

                Image tile;
                if (id != 0)
                {
                    tholder[x, y] = id;
                    tile = new Image(TILESET_PATH.Texture);
                    tile.AtlasRegion = GetTexture(id);
                    tile.ClippingRegion = GetTexture(0);
                    tile.SetPosition(y * 32, x * 32);

                    LAYER1.Graphics.Remove(timage[x, y]);
                    timage[x, y] = tile;

                    LAYER1.Graphics.Add(timage[x, y]);
                }

            }
            */
        }        

        public static void QuitEditor()
        {
            Game.Instance.Close();
        }

        #endregion

        #region UPDATE METHODS

        // przesuwanie kamery
        void CheckPlayerKeys()
        {
            if (Game.Instance.Input.KeyDown(Key.W))
            {
                usedScene.CameraY -= 10;
                updatePos = true;
            }
            else if (Game.Instance.Input.KeyDown(Key.S))
            {
                usedScene.CameraY += 10;
                updatePos = true;
            }

            if (Game.Instance.Input.KeyDown(Key.A))
            {
                usedScene.CameraX -= 10;
                updatePos = true;
            }
            else if (Game.Instance.Input.KeyDown(Key.D))
            {
                usedScene.CameraX += 10;
                updatePos = true;
            }

            /*
            if (Game.Instance.Input.KeyPressed(Key.P))
            {
                if (playerPlace)
                {
                    playerPlace = false;
                    CurrentTile = 0;
                }
                else
                {
                    playerPlace = true;
                    CurrentTile = 69;
                }
            }
            */

        }        

        // czitersko ustawia okno z fragmentami obok okna z grą.
        void SetOptionsPos()
        {
            Form m = Application.OpenForms[0];
            //m.BringToFront();
            m.Location = new System.Drawing.Point(Game.WindowX-m.Size.Width+20, Game.WindowY);
        }

        #endregion

        // aktualizacja GUI // TODO: Skrócić kod
        public override void UpdateLast()
        {
            base.UpdateLast();

            Vector2 underboxPos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 100, Scene.Instance.CameraY - 86);
            Vector2 ub2Pos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 110, Scene.Instance.CameraY - 86);
            Vector2 previewPos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 82, Scene.Instance.CameraY + 18 - 86);
            Vector2 ttPos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 90, Scene.Instance.CameraY + 100 - 86);
            Vector2 ttPos2 = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 90, Scene.Instance.CameraY + 140 - 86);

            if (currentRes == 0 || currentRes == 1)
            {
                underboxPos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 100, Scene.Instance.CameraY);
                ub2Pos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 110, Scene.Instance.CameraY);
                previewPos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 82, Scene.Instance.CameraY + 18);
                ttPos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 90, Scene.Instance.CameraY + 100);
                ttPos2 = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 90, Scene.Instance.CameraY + 140);

            }
            else if (currentRes == 2)
            {
                underboxPos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 150 - 77, Scene.Instance.CameraY+67);
                ub2Pos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 160 - 77, Scene.Instance.CameraY + 67);
                previewPos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 132 - 77, Scene.Instance.CameraY + 18 + 67);
                ttPos = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 140 - 77, Scene.Instance.CameraY + 100 + 67);
                ttPos2 = new Vector2(Scene.Instance.CameraX + Game.Instance.Surface.Width - 140 - 77, Scene.Instance.CameraY + 140 + 67);

            }

            Point snap = new Point((int)Otter.Util.SnapToGrid(usedScene.Input.MouseScreenX, 32) / 32, (int)Otter.Util.SnapToGrid(usedScene.Input.MouseScreenY, 32) / 32);

            if (updatePreview)
            {

                underbox.SetPosition(underboxPos);

                RemoveGraphics(preview);
                preview = new Image(TILESET_PATH.Texture);
                preview.AtlasRegion = GetTexture(CurrentTile);
                preview.ClippingRegion = GetTexture(0);
                preview.SetPosition(previewPos);
                tt.String = "ID: " + CurrentTile + "\nCC: " + Settings.collider[CurrentTile];
                tt.SetPosition(ttPos);
                ttpos.SetPosition(ttPos2);
                preview.Scale = 2f;
                AddGraphic(preview);
                updatePreview = false;


            }
            if (updatePos)
            {
                ub2.SetPosition(ub2Pos);
                underbox.SetPosition(underboxPos);

                preview.SetPosition(previewPos);                

                tt.String = "ID: " + CurrentTile + "\nCC: " + Settings.collider[CurrentTile];
                ttpos.SetPosition(ttPos2);

                tt.SetPosition(ttPos);
                //updatePreview = true;
                updatePos = false;
            }
        }
        //główna funkcja ustawiania fragmentów
        public override void Update()
        {
            base.Update();

            CheckPlayerKeys();
            //CheckPlaceMode();
            SetOptionsPos();

            if (OtterMenu.isDragging)
            { 
                Point snap = new Point((int)Otter.Util.SnapToGrid(usedScene.Input.MouseScreenX, 32) / 32, (int)Otter.Util.SnapToGrid(usedScene.Input.MouseScreenY, 32) / 32);
                ttpos.String = "X:" + snap.X + "|Y:" + snap.Y;

                float mX = Game.Instance.Input.MouseScreenX;
                float mY = Game.Instance.Input.MouseScreenY;

                if (Game.Instance.Input.MouseButtonPressed(MouseButton.Right) || Game.Instance.Input.MouseButtonDown(MouseButton.Right) && Game.Instance.HasFocus)
                {
                    int snapX = (int)Util.SnapToGrid(mX, 32) / 32;
                    int snapY = (int)Util.SnapToGrid(mY, 32) / 32;

                    if (snapX < 0 || snapY < 0 || snapX >= maxGridW || snapY >= maxGridH) { }
                    else
                    {

                        tholder[snapX, snapY] = 0;
                        MAIN_TILEMAP.SetTile(snapX, snapY, 0);
                        //LAYER1.Clear();

                    }
                }
                else if (Game.Instance.Input.MouseButtonPressed(MouseButton.Left) || Game.Instance.Input.MouseButtonDown(MouseButton.Left) && Game.Instance.HasFocus)
                {
                    int snapX = (int)Util.SnapToGrid(mX, 32) / 32;
                    int snapY = (int)Util.SnapToGrid(mY, 32) / 32;

                    if (snapX < 0 || snapY < 0 || snapX >= maxGridW || snapY >= maxGridH) { }
                    else
                    {
                        tholder[snapX, snapY] = CurrentTile;
                        MAIN_TILEMAP.SetTile(snapX, snapY, CurrentTile);
                    }
                }
            }
        }
    }
}

