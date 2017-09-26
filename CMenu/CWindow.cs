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
    class CWindow:Entity
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
            SetPosition(x, y);

            this._w = w;
            this._h = h;

            Layer = -1;
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

        //public void AddButtons(int x, int y, int w, int h, ButtonAction ba)
        //{
        //}


        #endregion

        #region UPDATES
        public override void Update()
        {
            base.Update();
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

        //struct Button
    }
}
