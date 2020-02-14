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
using TGC.Core.Text;
using TGC.Group.Camera;
using TGC.Group.Entities;
using TGC.Group.Model.Levels;

namespace TGC.Group.Model
{
    public class InventoryModel : CustomModel
    {
        private List<ItemModel> collectedItems;
        private bool active = false;
        private DXGui gui = new DXGui();

        public InventoryModel(UnderseaModel gameModel, TgcCamera camera, TgcD3dInput input, string mediaDir, string shadersDir, TgcFrustum frustum, TgcText2D drawText)
            : base(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText)
        {
        }

        public override void Init()
        {
            collectedItems = new List<ItemModel>();

            gui.Create(MediaDir);
            gui.InitDialog(false, false);
        }

        public override void Update(float elapsedTime)
        {
            //if (active)
            //{
            //    var camaraInterna = (FpsCamera)Camera;
            //    camaraInterna.LockCam = false;
            //}
        }

        public override void Render(float elapsedTime)
        {
            if (active)
            {
                RenderGUI(elapsedTime);
            }
            else
            {
                gui.Reset();
            }
        }

        public void RenderGUI(float elapsedTime)
        {
            //GuiMessage msg = gui.Update(elapsedTime, Input);
            //switch (msg.message)
            //{
            //    case MessageType.WM_COMMAND:

            //        switch (msg.id)
            //        {
            //            case 1:
            //                incrementarOxigeno();
            //                break;
            //            case 2:
            //                incrementarSalud();
            //                break;
            //            case 3:
            //                utilizarArma();
            //                break;
            //            case 4:
            //                utilizarRed();
            //                break;
            //        }

            //        //Luego de usar un item cierro el inventario
            //        renderizoTextoExito = true;
            //        huboUtilizacionExitosa = true;
            //        acumuloTiempo = 0;
            //        var camaraInterna = (TgcFpsCamera)GModel.Camara;
            //        camaraInterna.LockCam = true;
            //        recienActivo = false;
            //        activo = false;
            //        gui.Reset();

            //        GModel.Escenario.hacerSonarUsoElementoInventario();

            //        break;
            //}

            gui.Render();
        }

        public override void Dispose()
        {
            collectedItems.ForEach(item => item.Dispose());
        }

        public void AddItem(ItemModel item)
        {
            collectedItems.Add(item);
        }

        public void ShowInventory()
        {
            bool itemAdded = false;
            active = !active;

            if (active)
            {
                int W = D3DDevice.Instance.Width;
                int H = D3DDevice.Instance.Height;

                int dy = H - 50;
                int dy2 = dy;
                int dx = W / 2;

                int posEnX = (W / 2) - (dx / 2);
                int posEnY = (H / 2) - (dy / 2);
                int x0 = posEnX + 150;
                int y0 = posEnY + 50;
                int x1 = x0;
                int y1 = y0;

                GUIItem frame = gui.InsertIFrame("", posEnX, posEnY, dx, dy, Color.FromArgb(140, 240, 140));
                frame.c_font = Color.FromArgb(0, 0, 0);
                frame.scrolleable = true;

                collectedItems.ForEach(item =>
                {
                    GUIItem itemGui = new GUIItem();

                    if (item.Mesh.Name.Contains("wood"))
                    {
                        itemGui = gui.InsertImage("wood2.png", x1, y1 + 30, MediaDir);
                        itemAdded = true;
                    }
                    else if (item.Mesh.Name.Contains("hammer"))
                    {
                        itemGui = gui.InsertImage("hammer.jpg", x1, y1 + 30, MediaDir);
                        itemAdded = true;
                    }
                    else if (item.Mesh.Name.Contains("rope"))
                    {
                        itemGui = gui.InsertImage("rope.jpg", x1, y1 + 30, MediaDir);
                        itemAdded = true;
                    }

                    if (itemAdded)
                    {
                        x1 = x0;
                        y1 = y1 + itemGui.image_height + 20;
                        itemAdded = false;
                    }

                });
            }
        }
    }
}
