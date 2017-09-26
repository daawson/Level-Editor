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
    class CurrentWindow:Entity
    {
        Image mainbg;
        Image topbar;
        Image preview;
        Vector2 cameraPos;
        RichText windowName;
        RichText t_prev;

        int _currentTile;
        //public static bool isDragging = false;

        enum windowCollider
        {
            TopBar
        }

        public CurrentWindow()
        {
            InitWindow(100,160,Color.Red);
            Layer = -1;
        }
        // instancja z kolore
        public void InitWindow(int w, int h, Color c)
        {
            c.A = 0.5f;
            mainbg = Image.CreateRectangle(w, h, c);
            topbar = Image.CreateRectangle(w, 30, new Color(c.R - 0.4f, c.G - 0.4f, c.B - 0.4f, c.A));
            windowName = new RichText("Info", 13);
            t_prev = new RichText(" ", 12);

            preview = new Image(Akcja.TILESET_STRING);
            preview.ClippingRegion = Akcja.Instance.GetTexture(1);
            preview.AtlasRegion = Akcja.Instance.GetTexture(0);
            preview.SetOrigin(0, 0);
            preview.Scale = 2f;

            t_prev.String = "ID: " + Akcja.CurrentTile + "\nCC: " + Settings.collider[Akcja.CurrentTile];

            AddGraphics(mainbg);
            AddGraphic(preview);
            AddGraphics(topbar);
            AddGraphic(windowName);
            AddGraphic(t_prev);
            SetPosition(0, 0);
            //FollowMouse(new Vector2(1280-w/2,0));
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
                mainbg.SetPosition(mPos.X - mainbg.HalfWidth, mPos.Y - 10);
                //preview.SetPosition(mPos.X - (topbar.HalfWidth + 46), mPos.Y + 24);
                preview.SetPosition(mPos.X - topbar.HalfWidth, mPos.Y + 24);
                topbar.SetPosition(mPos.X - topbar.HalfWidth, mPos.Y - 10);
                windowName.SetPosition(mPos.X - windowName.Width / 2, mPos.Y - 6 - 10 + 11);
                t_prev.SetPosition(mPos.X - t_prev.Width / 2, mPos.Y + 94 );
            }
            //mainbg.SetOrigin(mPos);
            //topbar.SetOrigin(mPos);
        }

        // sprawdza czy pasek okna został kliknięty (drag'n drop)
        void CheckDragDrop()
        {
            Vector2 mPos = new Vector2(Scene.Instance.Input.MouseScreenX-cameraPos.X, Scene.Instance.Input.MouseScreenY-cameraPos.Y);
            
            if (isVectorInside(mPos, topbar.X, topbar.Y, topbar.Width, topbar.Height))
            {
                Akcja.isDragging = Game.Instance.Input.MouseButtonDown(MouseButton.Left) ? true : false;
                if (Akcja.isDragging)
                    FollowMouse(mPos);
            }
        }

        public override void Update()
        {
            base.Update();
            CheckDragDrop();

            if(_currentTile != Akcja.CurrentTile)
            {
                _currentTile = Akcja.CurrentTile;
                //preview.TextureRegion = Akcja.Instance.GetTexture(0);
                //preview.ClippingRegion = Akcja.Instance.GetTexture(_currentTile);
                //preview.AtlasRegion = Akcja.Instance.GetTexture(0);
                float _x = preview.X, _y = preview.Y;
                RemoveGraphics(preview);
                preview = new Image(Akcja.TILESET_PATH.Texture);
                preview.AtlasRegion = Akcja.Instance.GetTexture(_currentTile);
                preview.ClippingRegion = Akcja.Instance.GetTexture(0);
                preview.SetPosition(_x, _y);
                preview.Scale = 2.0f;
                AddGraphic(preview);
            }
        }

        public override void UpdateLast()
        {
            base.UpdateLast();
            cameraPos = new Vector2(Scene.Instance.CameraX, Scene.Instance.CameraY);
            FollowCamera();
            Point snap = new Point((int)Otter.Util.SnapToGrid(Akcja.usedScene.Input.MouseScreenX, 32) / 32, (int)Otter.Util.SnapToGrid(Akcja.usedScene.Input.MouseScreenY, 32) / 32);
            t_prev.String = "ID: " + Akcja.CurrentTile + "\nCC: " + Settings.collider[Akcja.CurrentTile] + "\nX:"+snap.X+" |Y:"+snap.Y;
            preview.ClippingRegion = Akcja.Instance.GetTexture(Akcja.CurrentTile);
            //CheckDragDrop();
        }

        public bool isVectorInside(Vector2 pos, float x, float y, float w, float h)
        {
            if (pos.X > x && pos.X < x + w && pos.Y > y && pos.Y < y + h) return true;
            else return false;
        }
    }
}
