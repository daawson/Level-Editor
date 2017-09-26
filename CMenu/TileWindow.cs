using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
using LevelEditor;

namespace CMenu
{
    // NOWA KLASA OKNA W SILNKU
    // Dodaje okienko które można przesuwać
    // TODO: Naprawić bo sie psuje :(
    class TileWindow:Entity
    {
        //public static TileWindow Instance = new TileWindow(Akcja.TILESET_PATH);

        Image mainbg;
        Image topbar;
        Image tileset;
        Vector2 cameraPos;
        RichText windowName;

        int topbarh = 40;
        enum windowCollider
        {
            TopBar
        }

        public TileWindow()
        {
            InitWindow(Color.Gray);
            Layer = -1;
        }

        // instancja z kolore
        public void InitWindow(Color c)
        {
            c.A = 0.9f;

            int w = Akcja.TILESET_PATH.Width + 6;
            int h = Akcja.TILESET_PATH.Height + 6;

            mainbg = Image.CreateRectangle(w, h, c);
            windowName = new RichText("Tiles", 13);
            topbar = Image.CreateRectangle(w, topbarh, new Color(c.R - 0.4f, c.G - 0.4f, c.B - 0.4f, c.A));
            tileset = new Image(Akcja.TILESET_STRING);

            AddGraphics(mainbg);
            AddGraphic(tileset);            
            AddGraphics(topbar);
            AddGraphic(windowName);

            mainbg.SetPosition(0, 0 - 10 + topbarh);
            tileset.SetPosition(0+3, 0 - 10 + topbarh);
            topbar.SetPosition(0, 0 - 10);
            windowName.SetPosition(0 + 8, 0 - 10 + 11);
        }

        // ustawia pozycję względem kamery
        void FollowCamera()
        {
            SetPosition(cameraPos);
        }

        // kiedy isDragged, podąża za kursorem
        void FollowMouse(Vector2 mPos)
        {
            if (isVectorInside(mPos, 0 + topbar.HalfWidth, 0 + topbar.HalfHeight, Program.screenWidth - topbar.HalfWidth, Program.screenHeight - topbar.HalfHeight))
            {
                mainbg.SetPosition(mPos.X - mainbg.HalfWidth, mPos.Y - 6 - 10 + topbarh);
                tileset.SetPosition(mPos.X - topbar.HalfWidth + 3, mPos.Y - 6 - 10 + topbarh+3);
                topbar.SetPosition(mPos.X - topbar.HalfWidth, mPos.Y - 6 - 10);
                windowName.SetPosition(mPos.X - windowName.Width / 2, mPos.Y - 6 - 10 + 11);
            }
            //mainbg.SetOrigin(mPos);
            //topbar.SetOrigin(mPos);
        }

        void CheckTileClick()
        {
            Vector2 mPos = new Vector2(Scene.Instance.Input.MouseScreenX - cameraPos.X, Scene.Instance.Input.MouseScreenY - cameraPos.Y);
        }

        // sprawdza czy pasek okna został kliknięty (drag'n drop)
        void CheckDragDrop()
        {
            Vector2 mPos = new Vector2(Scene.Instance.Input.MouseScreenX-cameraPos.X, Scene.Instance.Input.MouseScreenY-cameraPos.Y);
            
            if (isVectorInside(mPos, topbar.X, topbar.Y, topbar.Width, topbar.Height))
            {
                Akcja.isDragging = Game.Instance.Input.MouseButtonDown(MouseButton.Left) ? true : false;
                if(Akcja.isDragging)
                    FollowMouse(mPos);
            }
        }

        public override void Added()
        {
            base.Added();
        }
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

        public bool isVectorInside(Vector2 pos, float x, float y, float w, float h)
        {
            if (pos.X > x && pos.X < x + w && pos.Y > y && pos.Y < y + h) return true;
            else return false;
        }
    }
}
