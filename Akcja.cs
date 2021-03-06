﻿using System;
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
        // pozwala na randomowe korzystanie z funkcji bez tworzenia niepotrzebnych innych instancji klasy
        public static Akcja Instance;

        #region TILESET DATA

        public static Image TILESET_PATH;
        public static string TILESET_STRING;
            
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

        //TODO OGARNĄC NIEPOTRZEBNE ZMIENNE
        public static Scene usedScene;

        public static int maxGridW = 64;
        public static int maxGridH = 64;

        public static int CurrentTile { get; set; } = 0;

        public static int[,] tholder;

        Tilemap edges;
        Tilemap MAIN_TILEMAP;

        public static bool updatePos { get; set; } = true;
        public static bool updatePreview { get; set; } = true;

        int prevX;

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

       
        /// ZMIENNE OKIENEK
        Image currentTile; // INSTANCJA IMAGE AKTYWNEGO FRAGMENTU
        CWindow tilePreview; // OKNO Z PODGLADEM AKTEWNEGO FRAGMENTU
        RichText tilePreviewInfo;

        public static bool isDragging = false;
        public static bool isInsideWindow = false;
        #endregion        

        #region METHODS
        // main public m8
        public Akcja(Scene s, string st, Image img)
        {
            usedScene = s;
            TILESET_PATH = img;
            TILESET_STRING = st;

            SetupEdges();

            MAIN_TILEMAP = new Tilemap(st, 64 * 32, 32);
            SetupTiles();

            Instance = this;                       

            //TODO: edytować nazwy klas i ogarnąć kod \//\//\/\/\ w klasie Settings
            set = new Settings(this);

            //forcuje odswierzenie podgladu
            updatePreview = true;
            updatePos = true;

            // dodaje okno wyboru fragmentów 
            CWindow tilesChooser = new CWindow(Akcja.TILESET_PATH.Width+6, Akcja.TILESET_PATH.Height+6, 5,10, Color.Gray, "Tiles");
            tilesChooser.AddGraphicsToWindow(3,3,Akcja.TILESET_PATH); // funcja dodająca grafike
            tilesChooser.AddButtonsForTiles(); // TODO: Połączyć tą i funkcje wyzej, tak jak jest to w CWindow-options.
            usedScene.Add(tilesChooser);

            // okienko z informacjami fragmentu/podgląd
            tilePreview = new CWindow(100, 140, Akcja.TILESET_PATH.Width + 6+6, 10, Color.Gray, "Info");
            usedScene.Add(tilePreview);

            // ...
            SetupPreviewInfo();

            // okno opcji, zapis/wczytaj/pomoc/quit/clear
            CWindow options = new CWindow(184, 54, 5, Akcja.TILESET_PATH.Height + 6+1+45, Color.Red, "Options");
            options.AddButtonsForOptions();
            options.AddTextToWindow(4, 40, new RichText("Made by Daawson",10));//witam
            usedScene.Add(options);

        }

        //dodaje krawędzie wokół mapy edytora
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

        //funkcja startowa podglądu aktualnie wybranego fragmentu tilesetu.
        void SetupPreviewInfo()
        {
            //dodaje startowy podgląd
            currentTile = new Image(TILESET_PATH.Texture);
            currentTile.ClippingRegion = GetTexture(1);
            currentTile.AtlasRegion = GetTexture(0);
            tilePreview.AddGraphicsToWindow(3,3,currentTile);

            tilePreviewInfo = new RichText("", 12);
            tilePreview.AddTextToWindow(3, 96, tilePreviewInfo);         
        }
        
        //GŁÓWNY FUNKCJA USTAWIAJĄCA TILEMAP DO UŻYTKU
        //generuje mape pół ustawiając przy okazji wszystkie pola na ID0
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

        //zeruje wszystkie pola tj. ustawia id na 0.
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

            //set.ClearColliders();
        }

        //Wczytanie poziomy
        public void LoadLevel(String t)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(t);

            var layer0 = doc.SelectSingleNode("/leveldata/level/layer0");
            var settings = doc.SelectSingleNode("/leveldata/settings");


            int tilescount = doc.GetElementsByTagName("collision").Count;
            //set.ClearColliders();
            // wczytywanie koliderów
            for (int i = 0; i < tilescount-1 ; i++)
            {
                var cc = settings.ChildNodes[i];

                int id = Int32.Parse(cc.Attributes["id"].Value);
                bool hasCC = Boolean.Parse(cc.Attributes["cc"].Value);

                if (hasCC)
                {
                    //Console.WriteLine(hasCC + " " + cc.Attributes["cc"].Value);
                    //set.SetCollider(id);
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

        // forseqłit do pulpitu
        public static void QuitEditor()
        {
            Game.Instance.Close();
        }

        #endregion

        #region ANODER MITODS

        // przesuwanie kamery WSAD'em
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
        }        

        // czitersko ustawia okno z fragmentami obok okna z grą. //nieużywane od przeniesienia opcji do głównego okna.
        void SetOptionsPos()
        {
            //Form m = Application.OpenForms[0];
            //m.BringToFront();
            //m.Location = new System.Drawing.Point(Game.WindowX-m.Size.Width+20, Game.WindowY);
        }

        #endregion

        // aktualizacja GUI // TODO: Skrócić kod
        public override void UpdateLast()
        {
            base.UpdateLast();
            //snapujemy do gridu 32x32
            Point snap = new Point((int)Otter.Util.SnapToGrid(usedScene.Input.MouseScreenX, 32) / 32, (int)Otter.Util.SnapToGrid(usedScene.Input.MouseScreenY, 32) / 32);

            // zmiana obrazka podglądu aktualnego fragmentu tilesetu
            if (updatePreview)
            {
                tilePreview.RemoveGraphicsFromWindow(currentTile);
                currentTile = new Image(TILESET_PATH.Texture);
                currentTile.AtlasRegion = GetTexture(CurrentTile);
                currentTile.ClippingRegion = GetTexture(0);
                currentTile.Scale = 2.0f;
                tilePreview.AddGraphicsToWindow(18, 9, currentTile);

            }
        }

        //główna funkcja rysowania z tilesetu oraz inne zadania wykonywanye co update.
        public override void Update()
        {
            base.Update();

            CheckPlayerKeys();

            //snap to grid 32/32
            Point snap = new Point((int)Otter.Util.SnapToGrid(usedScene.Input.MouseScreenX, 32) / 32, (int)Otter.Util.SnapToGrid(usedScene.Input.MouseScreenY, 32) / 32);

            //aktualizuje tekst z okienka podglądu
            tilePreviewInfo.String = "ID: " + CurrentTile + "\nCC: " + Settings.collider[CurrentTile] + "\nX:" + snap.X + "|Y:" + snap.Y;

            if (!isDragging && !isInsideWindow) //jeśli nie przesuwa okna lub jeśli kursor nie jest w oknie z jakimikolwiek guzikami
            {      

                //TODO: Przerobić na vectory, zoptymalizować :_: bo tragedia
                float mX = Game.Instance.Input.MouseScreenX;
                float mY = Game.Instance.Input.MouseScreenY;

                //usuwanie z mapy
                if (Game.Instance.Input.MouseButtonPressed(MouseButton.Right) || Game.Instance.Input.MouseButtonDown(MouseButton.Right) && Game.Instance.HasFocus && !isDragging && !isInsideWindow)
                {
                    int snapX = (int)Util.SnapToGrid(mX, 32) / 32;
                    int snapY = (int)Util.SnapToGrid(mY, 32) / 32;

                    if (snapX < 0 || snapY < 0 || snapX >= maxGridW || snapY >= maxGridH) { }
                    else
                    {
                        tholder[snapX, snapY] = 0;
                        MAIN_TILEMAP.SetTile(snapX, snapY, 0);
                    }
                }
                //rysowanie fragmentu
                else if (Game.Instance.Input.MouseButtonPressed(MouseButton.Left) || Game.Instance.Input.MouseButtonDown(MouseButton.Left) && Game.Instance.HasFocus && !isDragging && !isInsideWindow)
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

