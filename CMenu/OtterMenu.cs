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
    class OtterMenu:Entity
    {
        public static OtterMenu Instance = new OtterMenu();

        Image mainbg;
        Image topbar;

        Vector2 cameraPos;

        public static bool isDragging = false;

        enum windowCollider
        {
            TopBar
        }

        private OtterMenu()
        {
            InitWindow(200, 500, Color.Cyan);
            Layer = -1;
        }

        // instacja bez koloru
        public void InitWindow(int w, int h) {
            InitWindow(w, h, Color.White);
        }

        // instancja z kolorem
        public void InitWindow(int w, int h, Color c)
        {
            mainbg = Image.CreateRectangle(w, h, c);
            topbar = Image.CreateRectangle(w, 30, new Color(c.R - 0.4f, c.G - 0.4f, c.B - 0.4f, c.A));

            AddGraphics(mainbg);
            AddGraphics(topbar);
            SetPosition(0, 0);
        }

        // ustawia pozycję względem kamery
        void FollowCamera()
        {
            SetPosition(cameraPos);
        }

        // kiedy isDragged, podąża za kursorem
        void FollowMouse(Vector2 mPos)
        {
            mainbg.SetPosition(mPos.X - mainbg.HalfWidth, mPos.Y - 10);
            topbar.SetPosition(mPos.X - topbar.HalfWidth, mPos.Y - 10);

            //mainbg.SetOrigin(mPos);
            //topbar.SetOrigin(mPos);
        }

        // sprawdza czy pasek okna został kliknięty (drag'n drop)
        void CheckDragDrop()
        {
            Vector2 mPos = new Vector2(Scene.Instance.Input.MouseScreenX-cameraPos.X, Scene.Instance.Input.MouseScreenY-cameraPos.Y);
            isDragging = Game.Instance.Input.MouseButtonDown(MouseButton.Left) ? true : false;
            if (isDragging && isVectorInside(mPos, topbar.X, topbar.Y, topbar.Width, topbar.Height))
            {
                //Console.WriteLine("Drag: {0}", isDragging);
                FollowMouse(mPos);
            }
        }

        public override void Update()
        {
            base.Update();
            //CheckDragDrop();
        }

        public override void UpdateLast()
        {
            base.UpdateLast();
            cameraPos = new Vector2(Scene.Instance.CameraX, Scene.Instance.CameraY);
            FollowCamera();
            CheckDragDrop();
        }

        public bool isVectorInside(Vector2 pos, float x, float y, float w, float h)
        {
            if (pos.X > x && pos.X < x + w && pos.Y > y && pos.Y < y + h) return true;
            else return false;
        }
    }
}
