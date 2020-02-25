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
        private TgcText2D text;
        private TgcText2D noUseShipHelmText = new TgcText2D();
        private bool showNoUseShipHelmMsg = false;
        private float msgTime = 0;

        public PlayerModel PlayerModel { get; set; }
        public bool ShowShipHelm { get; set; } = false;
        public bool RenderShipHelm { get; set; } = false;

        public InventoryModel(UnderseaModel gameModel, TgcCamera camera, TgcD3dInput input, string mediaDir, string shadersDir, TgcFrustum frustum, TgcText2D drawText)
            : base(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText)
        {
            
        }

        public override void Init()
        {
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;

            collectedItems = new List<ItemModel>();

            text = new TgcText2D();
            text.Text = "INVENTARIO";
            text.Color = Color.White;
            text.Align = TgcText2D.TextAlign.RIGHT;
            text.Position = new Point(300, 40);
            text.Size = new Size(300, 100);
            text.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));

            noUseShipHelmText.Text = "Acercate a la proa";
            noUseShipHelmText.Color = Color.Red;
            noUseShipHelmText.Align = TgcText2D.TextAlign.RIGHT;
            noUseShipHelmText.Position = new Point((W / 2) - 180, (H / 2) - 5);
            noUseShipHelmText.Size = new Size(300, 100);
            noUseShipHelmText.changeFont(new Font("TimesNewRoman", 14));

            gui.Create(MediaDir);
            gui.InitDialog(false, false);
        }

        public override void Update(float elapsedTime)
        {
            if (!active)
            {
                gui.Reset();
            }
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
            GuiMessage msg = gui.Update(elapsedTime, Input);
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:

                    switch (msg.id)
                    {
                        case 1:
                            active = false;
                            PlayerModel.ShowInventory = false;
                            PlayerModel.ShowFatherNote = true;
                            break;
                        case 2:
                            if (PlayerModel.CanUseShipHelm)
                            {
                                active = false;
                                PlayerModel.ShowInventory = false;
                                ShowShipHelm = false;
                                RenderShipHelm = true;
                            }
                            else
                            {
                                showNoUseShipHelmMsg = true;
                                msgTime = 0;
                            }
                            break;
                    }

                    break;
            }

            gui.Render();
            text.render();

            if (showNoUseShipHelmMsg && msgTime < 3)
            {
                msgTime += elapsedTime;
                noUseShipHelmText.render();
            }
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
                bool hammerAdded = false;
                int cantWood = 0, cantRope = 0;
                GUIItem woodGUIText = new GUIItem();
                GUIItem ropeGUIText = new GUIItem();

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

                GUIItem frame = gui.InsertIFrame("", posEnX, posEnY, dx, dy, Color.FromArgb(92, 43, 43));
                frame.c_font = Color.FromArgb(0, 0, 0);
                frame.scrolleable = true;

                collectedItems.ForEach(item =>
                {
                    GUIItem itemGui = new GUIItem();

                    if (item.Mesh.Name.Contains("wood"))
                    {
                        if (cantWood == 0)
                        {
                            itemAdded = true;
                            itemGui = gui.InsertImage("wood2.png", x1, y1 + 30, MediaDir);
                            woodGUIText = gui.InsertItem("Madera " + "(x" + ++cantWood + ")", x1 += 50, y1 + 20);
                        }
                        else
                        {
                            woodGUIText.text = "Madera " + "(x" + ++cantWood + ")";
                        }
                    }
                    else if (item.Mesh.Name.Contains("hammer") && !hammerAdded)
                    {
                        hammerAdded = true;
                        itemAdded = true;
                        itemGui = gui.InsertImage("hammer2.png", x1, y1 + 30, MediaDir);
                        gui.InsertItem("Martillo", x1 += 50, y1 + 20);
                    }
                    else if (item.Mesh.Name.Contains("rope"))
                    {
                        if (cantRope == 0)
                        {
                            itemAdded = true;
                            itemGui = gui.InsertImage("rope.png", x1, y1 + 30, MediaDir);
                            ropeGUIText = gui.InsertItem("Soga " + "(x" + ++cantRope + ")", x1 += 50, y1 + 20);
                        }
                        else
                        {
                            ropeGUIText.text = "Soga " + "(x" + ++cantRope + ")";
                        }
                    }
                    else if (item.Mesh.Name.Contains("fatherNote"))
                    {
                        itemAdded = true;
                        itemGui = gui.InsertImage("note.png", x1, y1 + 30, MediaDir);
                        gui.InsertItem("Nota de papá", x1 += 50, y1 + 20);
                        gui.InsertButton(1, "Usar", x1 += 300, y1, 120, 60);
                    }

                    if (itemAdded)
                    {
                        x1 = x0;
                        y1 = y1 + 100;
                        itemAdded = false;
                    }
                });

                if (ShowShipHelm)
                {
                    GUIItem itemGui = new GUIItem();

                    itemAdded = true;
                    itemGui = gui.InsertImage("timon.jpg", x1, y1 + 30, MediaDir);
                    gui.InsertItem("Timon", x1 += 50, y1 + 20);
                    gui.InsertItem("Se puede usar en la proa del barco", x1, y1 + 70);
                    gui.InsertButton(2, "Usar", x1 += 300, y1, 120, 60);
                }
            }
        }

        public int CantWood()
        {
            return collectedItems.Count(x => x.Mesh.Name.Contains("wood"));
        }

        public int CantRope()
        {
            return collectedItems.Count(x => x.Mesh.Name.Contains("rope"));
        }

        public bool HasHammer()
        {
            return collectedItems.Any(x => x.Mesh.Name.Contains("hammer"));
        }

        public void RemoveCraftElements()
        {
            for (int i = 0; i < 3; i++)
            {
                var woodItem = collectedItems.FirstOrDefault(x => x.Mesh.Name.Contains("wood"));
                collectedItems.Remove(woodItem);
            }

            var ropeItem = collectedItems.FirstOrDefault(x => x.Mesh.Name.Contains("rope"));
            collectedItems.Remove(ropeItem);
        }

        public void ResetGUI()
        {
            gui.Reset();
        }
    }
}
