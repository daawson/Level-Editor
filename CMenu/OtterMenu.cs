using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;

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

        enum windowCollider
        {
            TopBar
        }

        private OtterMenu()
        {
            InitWindow(100, 500, Color.White);
            Layer = -1;
        }

        public void InitWindow(int w, int h) {
            InitWindow(w, h, Color.Cyan);
        }

        public void InitWindow(int w, int h, Color c)
        {
            mainbg = Image.CreateRectangle(w, h, c);
            topbar = Image.CreateRectangle(w, 20, Color.Black);

            AddGraphics(mainbg);
            AddGraphics(topbar);
            SetPosition(0, 0);
        }

        void FollowCamera()
        {
            SetPosition(cameraPos);
        }

        void FollowMouse(Vector2 mPos)
        {
            mainbg.SetPosition(mPos.X - mainbg.HalfWidth, mPos.Y - 10);
            topbar.SetPosition(mPos.X - topbar.HalfWidth, mPos.Y - 10);

            //mainbg.SetOrigin(mPos);
            //topbar.SetOrigin(mPos);
        }

        void CheckDragDrop()
        {
            Vector2 mPos = new Vector2(Scene.Instance.Input.MouseScreenX, Scene.Instance.Input.MouseScreenY);
            bool isDragging = Game.Instance.Input.MouseButtonDown(MouseButton.Left) ? true : false;
            if (isDragging && isVectorInside(mPos, topbar.X, topbar.Y, topbar.Width, topbar.Height))
            {
                Console.WriteLine("Drag: {0}", isDragging);
                FollowMouse(mPos);
            }
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
            CheckDragDrop();
        }

        public bool isVectorInside(Vector2 pos, float x, float y, float w, float h)
        {
            if (pos.X > x && pos.X < x + w && pos.Y > y && pos.Y < y + h) return true;
            else return false;
        }
    }
}
