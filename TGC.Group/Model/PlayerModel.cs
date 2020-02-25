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
using TGC.Group.Camera;
using TGC.Group.Model.Levels;

namespace TGC.Group.Model
{
    public class PlayerModel : CustomModel
    {
        private float surfaceYPosition;
        private FatherNoteModel fatherNoteModel;
        private CraftModel craftModel;
        private FpsCamera internalCamera;

        public TGCVector3 Position { get; set; }
        public bool WithoutHelmet { get; set; }
        public InventoryModel InventoryModel { get; set; }
        public bool ShowInventory { get; set; }
        public bool ShowFatherNote { get; set; }
        public bool ShowCraft { get; set; }
        public bool CanShowCraft { get; set; }
        public bool CanUseShipHelm { get; set; }
        public bool ShowHistory { get; set; }

        public PlayerModel(float surfacePosition, TGCVector3 initialPosition, UnderseaModel gameModel, TgcCamera camera, TgcD3dInput input, string mediaDir, string shadersDir, TgcFrustum frustum, TgcText2D drawText)
            : base(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText)
        {
            surfaceYPosition = surfacePosition;
            Position = initialPosition;
            WithoutHelmet = true;
            InventoryModel = new InventoryModel(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText);
            InventoryModel.PlayerModel = this;
            fatherNoteModel = new FatherNoteModel(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText);
            fatherNoteModel.PlayerModel = this;
            craftModel = new CraftModel(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText);
            craftModel.PlayerModel = this;
        }

        public override void Init()
        {
            internalCamera = Camera as FpsCamera;

            ShowInventory = false;
            ShowFatherNote = false;
            ShowCraft = false;
            CanShowCraft = false;
            CanUseShipHelm = false;
            ShowHistory = true;

            InventoryModel.Init();
            fatherNoteModel.Init();
            craftModel.Init();
        }

        public override void Update(float elapsedTime)
        {
            if (Input.keyPressed(Key.I) && !ShowCraft)
            {
                ShowInventory = !ShowInventory;
                InventoryModel.ShowInventory();
                InventoryModel.Update(elapsedTime);
            }

            if (Input.keyPressed(Key.C) && CanShowCraft)
            {
                ShowCraft = !ShowCraft;
            }

            if (ShowInventory)
            {
                InventoryModel.Update(elapsedTime);
            }

            if (ShowInventory || ShowFatherNote || ShowCraft || ShowHistory)
            {
                internalCamera.LockCam = false;
            }
            else
            {
                internalCamera.LockCam = true;
            }
        }

        public override void Render(float elapsedTime)
        {
            if (ShowInventory && !ShowCraft)
            {
                InventoryModel.Render(elapsedTime);
            }

            if (ShowFatherNote)
            {
                fatherNoteModel.Render(elapsedTime);
            }

            if (CanShowCraft && ShowCraft)
            {
                craftModel.Render(elapsedTime);
            }
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
