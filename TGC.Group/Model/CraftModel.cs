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
using TGC.Core.Sound;
using TGC.Core.Text;
using TGC.Group.Entities;
using TGC.Group.Model.Levels;

namespace TGC.Group.Model
{
    public class CraftModel : CustomModel
    {
        private DXGui gui = new DXGui();
        private TgcText2D text;
        private TgcText2D noelementsMsg = new TgcText2D();
        private bool showNoElemMsg = false;
        private TgcText2D craftElementMsg = new TgcText2D();
        private bool showCraftElemMsg = false;
        private float msgTime = 0;
        private GUIItem shipHelmGuiItem;
        private GUIItem shipHelmGuiItemText;
        private gui_button shipHelmGuiItemButton;
        private TgcStaticSound craftSound = new TgcStaticSound();
        private TgcDirectSound DirectSound;

        private const int ID_CRAFT_SHIP_HELM = 1;

        public PlayerModel PlayerModel { get; set; }

        public CraftModel(UnderseaModel gameModel, TgcCamera camera, TgcD3dInput input, string mediaDir, string shadersDir, TgcFrustum frustum, TgcText2D drawText, TgcDirectSound directSound)
            : base(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText)
        {
            DirectSound = directSound;
        }

        public override void Init()
        {
            text = new TgcText2D();

            text.Text = "CRAFTEO";
            text.Color = Color.White;
            text.Align = TgcText2D.TextAlign.RIGHT;
            text.Position = new Point(300, 40);
            text.Size = new Size(300, 100);
            text.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));

            gui.Create(MediaDir);
            gui.InitDialog(false, false);

            CreateInterface();

            craftSound.loadSound(MediaDir + "\\Sounds\\craft.wav", DirectSound.DsDevice);
        }

        private void CreateInterface()
        {
            GUIItem guiText = new GUIItem();

            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;

            int dy = H - 50;
            int dy2 = dy;
            int dx = W / 2;

            int posEnX = (W / 2) - (dx / 2);
            int posEnY = (H / 2) - (dy / 2);
            int x0 = posEnX + 150;
            int y0 = posEnY + 100;
            int x1 = x0;
            int y1 = y0;

            DrawIFrame();

            shipHelmGuiItem = gui.InsertImage("timon.jpg", x1, y1 + 30, MediaDir);
            shipHelmGuiItemText = gui.InsertItem("Timon", x1 += 50, y1 + 20);
            shipHelmGuiItemButton = gui.InsertButton(ID_CRAFT_SHIP_HELM, "Crear", x1 += 300, y1, 120, 60);

            noelementsMsg.Text = "No tienes elementos suficientes";
            noelementsMsg.Color = Color.Red;
            noelementsMsg.Align = TgcText2D.TextAlign.RIGHT;
            noelementsMsg.Position = new Point((W / 2) - 180, (H / 2) - 5);
            noelementsMsg.Size = new Size(300, 100);
            noelementsMsg.changeFont(new Font("TimesNewRoman", 14));

            craftElementMsg.Text = "Creaste un timon. Revisa el inventario";
            craftElementMsg.Color = Color.Green;
            craftElementMsg.Align = TgcText2D.TextAlign.RIGHT;
            craftElementMsg.Position = new Point((W / 2) - 180, (H / 2) - 5);
            craftElementMsg.Size = new Size(300, 100);
            craftElementMsg.changeFont(new Font("TimesNewRoman", 14));
        }

        private void DrawIFrame()
        {
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;

            int dy = H - 50;
            int dy2 = dy;
            int dx = W / 2;

            int posEnX = (W / 2) - (dx / 2);
            int posEnY = (H / 2) - (dy / 2);
            int x0 = posEnX + 150;
            int y0 = posEnY + 100;
            int x1 = x0;
            int y1 = y0;

            GUIItem frame = gui.InsertIFrame("", posEnX, posEnY, dx, dy, Color.FromArgb(96, 78, 130));
            frame.c_font = Color.FromArgb(0, 0, 0);
            frame.scrolleable = true;
        }

        public override void Update(float elapsedTime)
        {
            
        }

        public override void Render(float elapsedTime)
        {
            GuiMessage msg = gui.Update(elapsedTime, Input);
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:

                    switch (msg.id)
                    {
                        case ID_CRAFT_SHIP_HELM:
                            CraftShipHelm();
                            break;
                    }

                    break;
            }

            gui.Render();
            text.render();

            if (showNoElemMsg && msgTime < 5)
            {
                msgTime += elapsedTime;
                noelementsMsg.render();
            }

            if (showCraftElemMsg && msgTime < 5)
            {
                msgTime += elapsedTime;
                craftElementMsg.render();
            }
        }

        public override void Dispose()
        {
            gui.Dispose();
            text.Dispose();
        }

        public void CraftShipHelm()
        {
            var catnWood = PlayerModel.InventoryModel.CantWood();
            var cantRope = PlayerModel.InventoryModel.CantRope();
            var hasHammer = PlayerModel.InventoryModel.HasHammer();

            if (catnWood < 3 ||
                cantRope < 1 ||
                !hasHammer)
            {
                showNoElemMsg = true;
                msgTime = 0;
            }
            else
            {
                PlayCraftSound();

                showCraftElemMsg = true;
                msgTime = 0;

                PlayerModel.InventoryModel.ShowShipHelm = true;
                PlayerModel.InventoryModel.RemoveCraftElements();

                gui.Reset();
                DrawIFrame();
            }
        }

        private void PlayCraftSound()
        {
            craftSound.play();
        }
    }
}
