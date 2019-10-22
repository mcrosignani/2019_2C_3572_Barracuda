using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Sprites;

namespace TGC.Group.Model
{
    public class HUDModel
    {
        private string MediaDir;
        private Device Device;

        private float originalWidth = 1366f;
        private float originalHeigth = 768f;

        private float correctFactorWidth;
        private float correctFactorHeigth;

        private Drawer2D spriteDrawer = new Drawer2D();
        private List<CustomSprite> sprites = new List<CustomSprite>();

        private CustomBitmap bitmapTomasFace;
        private CustomSprite spriteTomasFace = new CustomSprite();
        private CustomBitmap bitmapOxygenBar;
        private CustomSprite spriteOxygenBar = new CustomSprite();
        private CustomBitmap bitmapHealthBar;
        private CustomSprite spriteHealthBar = new CustomSprite();

        public HUDModel(string mediaDir, Device device)
        {
            MediaDir = mediaDir;
            Device = device;
        }

        public void Init()
        {
            correctFactorWidth = Device.Viewport.Width / originalWidth;
            correctFactorHeigth = Device.Viewport.Height / originalHeigth;

            bitmapTomasFace = new CustomBitmap(MediaDir + "Level1\\Textures\\" + "tomasFace.png", Device);
            spriteTomasFace.Bitmap = bitmapTomasFace;
            spriteTomasFace.Scaling = new TGCVector2(0.25f * correctFactorWidth, 0.2f * correctFactorHeigth);
            spriteTomasFace.Position = new TGCVector2(Device.Viewport.Width / 1.3f, Device.Viewport.Height / 30f);
            sprites.Add(spriteTomasFace);

            bitmapOxygenBar = new CustomBitmap(MediaDir + "Level1\\Textures\\" + "oxygenBar.png", Device);
            spriteOxygenBar.Bitmap = bitmapOxygenBar;
            spriteOxygenBar.Scaling = new TGCVector2(0.4f * correctFactorWidth, 1f * correctFactorHeigth);
            spriteOxygenBar.Position = new TGCVector2(Device.Viewport.Width / 1.2f, Device.Viewport.Height / 30f);
            sprites.Add(spriteOxygenBar);

            bitmapHealthBar = new CustomBitmap(MediaDir + "Level1\\Textures\\" + "healthBar.png", Device);
            spriteHealthBar.Bitmap = bitmapHealthBar;
            spriteHealthBar.Scaling = new TGCVector2(0.4f * correctFactorWidth, 1f * correctFactorHeigth);
            spriteHealthBar.Position = new TGCVector2(Device.Viewport.Width / 1.2f, Device.Viewport.Height / 12f);
            sprites.Add(spriteHealthBar);
        }

        public void Update(float elapsedTime)
        { }

        public void Render()
        {
            spriteDrawer.BeginDrawSprite();
            sprites.ForEach(sprite => spriteDrawer.DrawSprite(sprite));
            spriteDrawer.EndDrawSprite();
        }

        public void Dispose()
        {
            sprites.ForEach(x => x.Dispose());
        }
    }
}
