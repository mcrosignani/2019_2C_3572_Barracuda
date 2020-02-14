using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Levels;

namespace TGC.Group.Model
{
    public class PlayerModel : CustomModel
    {
        private float surfaceYPosition;

        public TGCVector3 Position { get; set; }
        public bool WithoutHelmet { get; set; }
        public InventoryModel InventoryModel { get; set; }

        public PlayerModel(float surfacePosition, TGCVector3 initialPosition, UnderseaModel gameModel, TgcCamera camera, TgcD3dInput input, string mediaDir, string shadersDir, TgcFrustum frustum, TgcText2D drawText)
            : base(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText)
        {
            surfaceYPosition = surfacePosition;
            Position = initialPosition;
            WithoutHelmet = true;
            InventoryModel = new InventoryModel(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText);
        }

        public override void Init()
        {
            InventoryModel.Init();
        }

        public override void Update(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
            {
                InventoryModel.ShowInventory();
            }

            InventoryModel.Update(elapsedTime);
        }

        public override void Render(float elapsedTime)
        {
            InventoryModel.Render(elapsedTime);
        }

        public override void Dispose()
        {
            InventoryModel.Dispose();
        }

        public bool UnderSurface()
        {
            return Position.Y < surfaceYPosition;
        }
    }
}
