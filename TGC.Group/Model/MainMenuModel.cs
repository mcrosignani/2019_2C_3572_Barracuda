using System.Drawing;
using System.Windows.Forms;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Entities;
using TGC.Group.Model.Levels;
using TGC.Group.Sprites;

namespace TGC.Group.Model
{
    public class MainMenuModel : CustomModel
    {
        private DXGui gui = new DXGui();
        public bool msg_box_app_exit = false;
        public bool msg_box_nueva_mision = false;
        private Drawer2D drawer2D;
        private CustomSprite backgroundSprite;
        private TgcText2D text;

        public const int IDOK = 0;
        public const int IDCANCEL = 1;
        public const int ID_NUEVA_PARTIDA = 102;
        public const int ID_CONTROLES = 103;
        public const int ID_APP_EXIT = 105;

        public MainMenuModel(UnderseaModel gameModel, TgcCamera camera, TgcD3dInput input, string mediaDir, string shadersDir, TgcFrustum frustum, TgcText2D drawText)
            : base(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText)
        {
        }

        public override void Init()
        {
            drawer2D = new Drawer2D();

            //Crear Sprite
            backgroundSprite = new CustomSprite();
            backgroundSprite.Bitmap = new CustomBitmap(MediaDir + "\\Bitmaps\\main3.jpg", D3DDevice.Instance.Device);

            text = new TgcText2D();
            text.Text = "CARIBBEAN´S DEATH";
            text.Color = Color.Red;
            text.Align = TgcText2D.TextAlign.RIGHT;
            text.Position = new Point(D3DDevice.Instance.Width - 500, 100);
            text.Size = new Size(300, 100);
            text.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold | FontStyle.Italic));

            //Ubicarlo centrado en la pantalla
            var textureSize = backgroundSprite.Bitmap.Size;
            backgroundSprite.Position = new TGCVector2(0, 0);
            backgroundSprite.Scaling = new TGCVector2(0.75f, 0.35f);

            gui.Create(MediaDir);

            // menu principal
            gui.InitDialog(false, false);
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;
            int x0 = W - 200;
            int y0 = H - 300;
            int dy = 80;
            int dy2 = dy;
            int dx = 200;

            gui.InsertMenuItem(ID_NUEVA_PARTIDA, "  Jugar", "play.png", x0, y0, MediaDir, dx, dy);
            gui.InsertMenuItem(ID_CONTROLES, "  Controles", "navegar.png", x0, y0 += dy2, MediaDir, dx, dy);
            gui.InsertMenuItem(ID_APP_EXIT, "  Salir", "salir.png", x0, y0 += dy2, MediaDir, dx, dy);
        }

        public override void Update(float elapsedTime)
        {
        }

        public override void Render(float elapsedTime)
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(backgroundSprite);
            drawer2D.EndDrawSprite();

            text.render();

            GuiMessage msg = gui.Update(elapsedTime, Input);
            // proceso el msg
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:
                    switch (msg.id)
                    {
                        case IDOK:
                            if (msg_box_app_exit)
                            {

                                Application.Exit();
                            }
                            if (msg_box_nueva_mision)
                            {
                                GameModel.ChangeLevel();
                            }
                            break;
                        case IDCANCEL:
                            gui.EndDialog();
                            break;
                        case ID_NUEVA_PARTIDA:
                            gui.MessageBox("Nueva Partida", "TGC Gui Demo");
                            msg_box_nueva_mision = true;
                            msg_box_app_exit = false;
                            break;
                        case ID_APP_EXIT:
                            gui.MessageBox("Desea Salir?", "TGC Gui Demo");
                            msg_box_app_exit = true;
                            msg_box_nueva_mision = false;
                            break;
                        case ID_CONTROLES:
                            gui.MessageBoxControles("Controles", "TGC Gui Demo");
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
            text.render();
        }
    }
}
