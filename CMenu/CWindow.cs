using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
using LevelEditor;

namespace CMenu
{
    // klasa customowego okna w grze, można tworzyć instancje!    
    struct CButton
    {
        public int x, y, w, h, id;
        public CButton(int _x, int _y, int _w, int _h, int _id)
        {
            this.x = _x;
            this.y = _y;
            this.w = _w;
            this.h = _h;
            this.id = _id;
        }

        public int GetID()
        {
            return this.id;
        }
    }

    // TODO: Skomentować bardziej klase, ogarnąć niepotrzebny kod, rozdzielić ten syf na moduły XD
    class CWindow :Entity
    {
        #region FIELDS

        //podstawa tło i pasek u góry
        Image mainbg;
        Image topbar;

        //nazwa okienka
        RichText windowName;

        //wysokość paska z tytułem
        int topbarh = 30;

        //pozycja kamery w świecie
        Vector2 cameraPos;
       
        //grafiki okna
        GraphicList windowGraphics = new GraphicList();

        //lista guzików
        CButton[] buttonsList;
        bool hasButtons = false;
        bool tilesChooser = false;
        bool optionsButtons = false;
        //
        public int _w, _h;        


        #endregion

        /// <summary>
        /// TWORZY INSTANCEJ OKNA
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="x">X Pos</param>
        /// <param name="y">Y Pos</param>
        /// <param name="c">Main Color</param>
        /// <param name="windowName">Window Name</param>
        public CWindow(int w, int h, int x, int y, Color c, String windowName)
        {
            InitWindow(w,h,c, windowName);
            //SetPosition(x, y);

            this._w = w;
            this._h = h+topbarh;

            Layer = -1;

            windowGraphics.SetPosition(x, y);

        }

        #region METHODS
        // główny void tworzący wszystkie dane okna.
        void InitWindow(int width, int height, Color c, String wname)
        {
            c.A = 0.9f;
            this.mainbg = Image.CreateRectangle(width, height, c);
            this.windowName = new RichText(wname, 12);
            this.topbar = Image.CreateRectangle(width, topbarh, new Color(c.R - 0.4f, c.G - 0.4f, c.B - 0.4f, c.A));
            //tileset = new Image(Akcja.TILESET_STRING);

            this.windowGraphics.Add(mainbg);
            this.windowGraphics.Add(topbar);
            this.windowGraphics.Add(windowName);

            this.mainbg.SetPosition(0, 0 - 10 + topbarh);
            this.topbar.SetPosition(0, 0 - 10);
            this.windowName.SetPosition(0 + topbar.Width/2 -  windowName.Width / 2, -3);

            this.AddGraphic(windowGraphics);
        }

        //if is dragged
        void FollowMouse(Vector2 mPos)
        {
            if (isVectorInside(mPos, 0 + this.topbar.HalfWidth, 0 + this.topbar.HalfHeight, Program.screenWidth - this.topbar.HalfWidth, Program.screenHeight - this.topbar.HalfHeight))
            {
                this.windowGraphics.SetPosition(mPos.X - this.mainbg.HalfWidth, mPos.Y-10);
            }
            //mainbg.SetOrigin(mPos);
            //topbar.SetOrigin(mPos);
        }
        //podąrza przyczepione do kamery
        void FollowCamera()
        {
            this.SetPosition(cameraPos);
        }        

        void CheckDragDrop()
        {
            Vector2 mPos = new Vector2(Scene.Instance.Input.MouseScreenX - cameraPos.X, Scene.Instance.Input.MouseScreenY - cameraPos.Y);

            if (isVectorInside(mPos, this.windowGraphics.X+this.topbar.X, this.windowGraphics.Y + this.topbar.Y, topbar.Width, topbar.Height))
            {
                Akcja.isDragging = Game.Instance.Input.MouseButtonDown(MouseButton.Left) ? true : false;
                if (Akcja.isDragging)
                    FollowMouse(mPos);
            }
        }       

        void CheckForButtonClick()
        {
            Vector2 mPos = new Vector2(Scene.Instance.Input.MouseScreenX - cameraPos.X, Scene.Instance.Input.MouseScreenY - cameraPos.Y);
            bool isClicked = Game.Instance.Input.MouseButtonDown(MouseButton.Left) ? true : false;
            if (hasButtons && tilesChooser)
            {
                if (isClicked)
                {
                    foreach (CButton button in buttonsList)
                    {                        
                        if(isVectorInside(mPos, windowGraphics.X + button.x, windowGraphics.Y + button.y, button.w, button.h))
                        {
                            Akcja.CurrentTile = button.GetID();
                        }
                    }
                }
            }

            if (hasButtons && optionsButtons)
            {
                if (isClicked)
                {
                    foreach (CButton button in buttonsList)
                    {
                        if (isVectorInside(mPos, windowGraphics.X + button.x, windowGraphics.Y + button.y, button.w, button.h))
                        {
                            switch (button.id)
                            {
                                case 0:
                                    Settings.saveClick();
                                    break;

                                case 1:
                                    Settings.loadClick();
                                    break;

                                case 2:
                                    Settings.helpClick();
                                    break;

                                case 3:
                                    Settings.quitClick();
                                    break;

                                case 4:
                                    Settings.newClick();
                                    break;

                            }
                        }
                    }
                }
            }
        }

        void CheckCursorInsideWindow()
        {
            if (hasButtons && tilesChooser)
            {
                Vector2 mPos = new Vector2(Scene.Instance.Input.MouseScreenX - cameraPos.X, Scene.Instance.Input.MouseScreenY - cameraPos.Y);
                if (isVectorInside(mPos, this.windowGraphics.X, this.windowGraphics.Y, _w, _h))
                {
                    Akcja.isInsideWindow = true;
                }
                else
                {
                    Akcja.isInsideWindow = false;
                }
            }
            //Console.WriteLine("INSIDE:{0}\nDRAG{1}", Akcja.isInsideWindow, Akcja.isDragging);
        }

        //dodaje grafike do używanej instancji okna
        public void AddGraphicsToWindow(int x, int y, Image img)
        {
            img.SetPosition(mainbg.X+x, mainbg.Y+y);
            windowGraphics.Add(img);
        }

        public void AddTextToWindow(int x, int y, RichText text)
        {
            text.SetPosition(mainbg.X + x, mainbg.Y + y);
            windowGraphics.Add(text);
        }

        public void RemoveGraphicsFromWindow(Image img)
        {
            if (windowGraphics.Graphics.Contains(img))
            {
                windowGraphics.Graphics.Remove(img);
            }
        }

        //Komenda generująca guziki pod wybór tilesetu
        public void AddButtonsForTiles()
        {
            buttonsList = new CButton[Akcja.indexTiles];

            int x = 0, y = 0;
            for (int i = 0; i < Akcja.indexTiles; i++)
            {

                if (x == Akcja.indexX)
                {
                    x = 0;
                    y++;
                }
                CButton custom = new CButton(3 + x * 32, (int)mainbg.Y + 3 + y * 32, 32, 32, i);
                buttonsList[i] = custom;
                x++;                                
            }
            hasButtons = true;
            tilesChooser = true;
        }

        //Komenda tworząca guziki pod opcje.
        public void AddButtonsForOptions()
        {
            buttonsList = new CButton[5];
            Image saveIcon, loadIcon, helpIcon, quitIcon, clearIcon;

            //saveIcon = Image.CreateRectangle(32, 32, Color.Green);
            saveIcon = new Image("../../Assets/save_icon.png");
            saveIcon.SetPosition(4, topbarh / 2 + 10);
            CButton save = new CButton(4, topbarh / 2 + 10, 32, 32, 0);

            //loadIcon = Image.CreateRectangle(32, 32, Color.Orange);
            loadIcon = new Image("../../Assets/load_icon.png");
            loadIcon.SetPosition(4 + 32 + 4, topbarh / 2 + 10);
            CButton load = new CButton(4 + 32 + 4, topbarh / 2 + 10, 32, 32, 1);

            //helpIcon = Image.CreateRectangle(32, 32, Color.Blue);
            helpIcon = new Image("../../Assets/help_icon.png");
            helpIcon.SetPosition(4 + 32 + 4 + 32 + 4, topbarh / 2 + 10);
            CButton help = new CButton(4 + 32 + 4 + 32 + 4, topbarh / 2 + 10,32,32,2);

            //quitIcon = Image.CreateRectangle(32, 32, Color.Red);
            quitIcon = new Image("../../Assets/quit_icon.png");
            quitIcon.SetPosition(4 + 32 + 4 + 32 + 4 + 32 + 4, topbarh / 2 + 10);
            CButton quit = new CButton(4 + 32 + 4 + 32 + 4 + 32 + 4, topbarh / 2 + 10, 32, 32, 3);

            //clearIcon = Image.CreateRectangle(32, 32, Color.White);
            clearIcon = new Image("../../Assets/clear2_icon.png");
            clearIcon.SetPosition(4 + 32 + 4 + 32 + 4 + 32 + 4 + 32 + 4, topbarh / 2 + 10);
            CButton clear = new CButton(4 + 32 + 4 + 32 + 4 + 32 + 4 + 32 + 4, topbarh / 2 + 10, 32, 32, 4);

            windowGraphics.Add(saveIcon);
            windowGraphics.Add(loadIcon);
            windowGraphics.Add(helpIcon);
            windowGraphics.Add(quitIcon);
            windowGraphics.Add(clearIcon);

            buttonsList[0] = save;
            buttonsList[1] = load;
            buttonsList[2] = help;
            buttonsList[3] = quit;
            buttonsList[4] = clear;


            hasButtons = true;
            optionsButtons = true;


        }

        #endregion

        #region UPDATES

        public override void Update()
        {
            base.Update();            
            CheckCursorInsideWindow();
            CheckForButtonClick();
            CheckDragDrop();
        }

        public override void UpdateLast()
        {
            base.UpdateLast();
            cameraPos = new Vector2(Scene.Instance.CameraX, Scene.Instance.CameraY);
            FollowCamera();
            //CheckDragDrop();
        }

        #endregion

        public bool isVectorInside(Vector2 pos, float x, float y, float w, float h)
        {
            if (pos.X > x && pos.X < x + w && pos.Y > y && pos.Y < y + h) return true;
            else return false;
        }
    }
}
