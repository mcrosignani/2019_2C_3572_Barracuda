using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Camera;
using TGC.Group.Entities;
using TGC.Group.Model.Levels;
using TGC.Group.Sprites;

namespace TGC.Group.Model
{
    public class HistoryModel : CustomModel
    {
        private DXGui gui = new DXGui();
        private Drawer2D drawer2D;
        private CustomSprite backgroundSprite;

        private const int ID_CLOSE = 2;

        public bool ShowHistory { get; set; } = true;

        public HistoryModel(UnderseaModel gameModel, TgcCamera camera, TgcD3dInput input, string mediaDir, string shadersDir, TgcFrustum frustum, TgcText2D drawText)
            : base(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText)
        {

        }

        public override void Init()
        {
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;
            int x0 = W / 2;
            int y0 = H / 2;
            int dy = 80;
            int dy2 = dy;
            int dx = 200;

            drawer2D = new Drawer2D();

            backgroundSprite = new CustomSprite();
            backgroundSprite.Bitmap = new CustomBitmap(MediaDir + "\\Bitmaps\\history.PNG", D3DDevice.Instance.Device);
            backgroundSprite.Position = new TGCVector2(W * 0.1f, H * 0.1f);
            backgroundSprite.Scaling = new TGCVector2(1, 1);

            gui.Create(MediaDir);
            gui.InitDialog(false, false);

            //gui.InsertMenuItem(ID_CLOSE, "Cerrar", "close.png", x0, y0, MediaDir, dx, dy);
            gui.InsertButton(ID_CLOSE, "Cerrar", x0 - 150, y0 + 250, dx, dy, Color.DarkRed);
        }

        public override void Update(float elapsedTime)
        {
            
        }

        public override void Render(float elapsedTime)
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(backgroundSprite);
            drawer2D.EndDrawSprite();

            GuiMessage msg = gui.Update(elapsedTime, Input);
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:
                    switch (msg.id)
                    {
                        case ID_CLOSE:
                            ShowHistory = false;
                            break;
                    }
                    break;
                default:
                    break;
            }

            gui.Render();
        }

        public override void Dispose()
        {
            gui.Dispose();
            backgroundSprite.Dispose();
        }
    }
}
