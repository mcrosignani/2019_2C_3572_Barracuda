using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Levels;

namespace TGC.Group.Model
{
    public class CollectModel : CustomModel
    {
        private TgcBoundingCylinder collisionCylinder;
        private float leftrightRot;
        private float updownRot;
        private float rotationSpeed;

        public List<ItemModel> CollisionMeshes { get; set; }
        public PlayerModel Player { get; set; }

        public CollectModel(UnderseaModel gameModel, TgcCamera camera, TgcD3dInput input, string mediaDir, string shadersDir, TgcFrustum frustum, TgcText2D drawText)
            : base(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText)
        {
            rotationSpeed = 0.1f;
        }

        public override void Init()
        {
            CollisionMeshes = new List<ItemModel>();

            collisionCylinder = new TgcBoundingCylinder(Camera.LookAt, 50f, 200f);
            updownRot = Geometry.DegreeToRadian(90f) + (FastMath.PI / 10.0f);
            collisionCylinder.rotateZ(updownRot);
            collisionCylinder.setRenderColor(Color.LimeGreen);
            collisionCylinder.updateValues();
        }

        public override void Update(float elapsedTime)
        {
            collisionCylinder.Center = Camera.LookAt;
            collisionCylinder.move(Player.Position - Camera.LookAt);
            leftrightRot -= -Input.XposRelative * rotationSpeed;
            updownRot -= -Input.YposRelative * rotationSpeed;
            collisionCylinder.Rotation = new TGCVector3(0, leftrightRot, updownRot);
            collisionCylinder.updateValues();

            if (Input.keyPressed(Key.E))
            {
                DetectCollision();
            }
        }

        public override void Render(float elapsedTime)
        {
            //Render Collectables
            foreach (var item in CollisionMeshes)
            {
                item.Mesh.Render();
            }
        }

        public override void Dispose()
        {
            CollisionMeshes.ForEach(item => item.Dispose());
            collisionCylinder.Dispose();
        }

        public void DetectCollision()
        {
            List<ItemModel> collidesItems = CollisionMeshes.Where(item => CollidesWithCylinder(item)).ToList();

            if (collidesItems.Any())
            {
                //var collectedItem = collidesItems.First();

                foreach (var collectedItem in collidesItems)
                {
                    Player.InventoryModel.AddItem(collectedItem);

                    if (collectedItem.Mesh.Name.Contains("mask"))
                    {
                        Player.WithoutHelmet = false;
                    }

                    CollisionMeshes.Remove(collectedItem);
                }
            }
        }

        private bool CollidesWithCylinder(ItemModel item)
        {
            var collisionSphere = new TgcBoundingSphere(item.CollectablePosition, 5);
            return TgcCollisionUtils.testSphereCylinder(collisionSphere, collisionCylinder);
        }
    }
}
