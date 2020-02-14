﻿using BulletSharp;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Core.Text;
using TGC.Core.Textures;
using TGC.Group.Camera;
using TGC.Group.Entities;
using TGC.Group.Helpers;
using TGC.Group.Physics;
using Effect = Microsoft.DirectX.Direct3D.Effect;

namespace TGC.Group.Model.Levels
{
    public class Level1Model : CustomModel
    {
        private float time;
        private string currentHeightmap;
        private float currentScaleXZ;
        private float currentScaleY;
        TGCVector3 initialLookAt;
        TGCVector3 initialPosition;
        //private Texture terrainTexture;
        private List<ItemModel> meshes;
        //private HeightmapModel hmModel;
        private TgcSkyBox skyBox;
        private TgcSkyBox skyBoxUndersea;
        private TgcSimpleTerrain surfacePlane;
        private TGCVector3 surfacePosition = new TGCVector3(0, 4000, 0);
        //private CollisionManager collisionManager;
        TgcSimpleTerrain underseaTerrain;
        private Surface g_pDepthStencil;
        private Texture g_pRenderTarget;
        private VertexBuffer g_pVBV3D;

        private Effect wavesEffect;
        private Effect recoltableItemEffect;

        //HUD
        HUDModel hudModel;

        //Shaders
        private Effect effect;

        // Physics
        BulletSharpManager bulletManager;

        // Player
        PlayerModel playerModel;

        // Collect
        CollectModel collectModel;

        public Level1Model(UnderseaModel gameModel, TgcCamera camera, TgcD3dInput input, string mediaDir, string shadersDir, TgcFrustum frustum, TgcText2D drawText)
            : base(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText)
        {
            hudModel = new HUDModel(MediaDir, D3DDevice.Instance.Device);
            
            initialLookAt = new TGCVector3(6000, 4120f, 6600f);
            initialPosition = new TGCVector3(6000, 4120f, 6600f);
            
            //Camara
            Camera = new FpsCamera(initialLookAt, Input);

            //Player
            playerModel = new PlayerModel(surfacePosition.Y, initialPosition, gameModel, camera, input, mediaDir, shadersDir, frustum, drawText);

            //Collect Model
            collectModel = new CollectModel(gameModel, camera, input, mediaDir, shadersDir, frustum, drawText);
        }

        public override void Init()
        {
            time = 0;

            playerModel.Init();

            collectModel.Init();
            collectModel.Player = playerModel;

            //Skybox
            LoadSkyBox();
            LoadSkyBoxUndersea();

            //Heightmap
            currentHeightmap = MediaDir + "\\Level1\\Heigthmap\\" + "hm_level1.jpg";
            var currentTexture = MediaDir + "\\Level1\\Textures\\" + "level1.PNG";
            currentScaleXZ = 50f;
            currentScaleY = 2f;

            underseaTerrain = new TgcSimpleTerrain();
            underseaTerrain.loadHeightmap(currentHeightmap, currentScaleXZ, currentScaleY, new TGCVector3(129, 0, 129));
            underseaTerrain.loadTexture(currentTexture);
            underseaTerrain.AlphaBlendEnable = true;

            bulletManager = new BulletSharpManager(initialPosition, Input, Camera, playerModel);

            bulletManager.Init(underseaTerrain);

            //Surface
            LoadSurface();

            //Meshes
            LoadMeshes();

            //Collectables
            LoadCollectableMeshes();

            //HUD
            hudModel.Init();

            //Shaders
            LoadShaders();
        }

        private void LoadCollectableMeshes()
        {
            LoadMask();
            LoadFatherNote();
            LoadHammer();
            LoadRope();
            LoadWood();
            LoadCopper();
        }

        private void LoadMask()
        {
            recoltableItemEffect = TGCShaders.Instance.LoadEffect(ShadersDir + "\\RecolectableItemShader.fx");
            recoltableItemEffect.Technique = "RecolectableItemTechnique";

            string pathMask = MediaDir + "\\Meshes\\mask\\mask_floor-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var meshes = loader.loadSceneFromFile(pathMask).Meshes;

            int i = 0;
            foreach (var mesh in meshes)
            {
                mesh.Position = new TGCVector3(6939, 4105, 6585);
                mesh.Scale = new TGCVector3(1f, 1f, 1f);
                mesh.Rotation = new TGCVector3(0, 90, 0);

                mesh.Effect = recoltableItemEffect;
                mesh.Technique = "RecolectableItemTechnique";

                mesh.Name = "maskfloor_" + i.ToString();
                i++;

                collectModel.CollisionMeshes.Add(new ItemModel { Mesh = mesh, IsCollectable = true, CollectablePosition = mesh.Position });
            }
        }

        private void LoadHammer()
        {
            string pathHammer = MediaDir + "\\Meshes\\hammer\\hammer-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMeshes = loader.loadSceneFromFile(pathHammer).Meshes;

            int i = 0;
            foreach (var mesh in originalMeshes)
            {
                var hammer = mesh.createMeshInstance($"hammer_{i++}");

                var position = new TGCVector3(6643, 4105, 7041);
                var scale = new TGCVector3(.5f, .5f, .5f);

                hammer.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

                hammer.Effect = recoltableItemEffect;
                hammer.Technique = "RecolectableItemTechnique";

                //hammer.createBoundingBox();

                collectModel.CollisionMeshes.Add(new ItemModel { Mesh = hammer, IsCollectable = true, CollectablePosition = position });
            }

            //int i = 0;
            //foreach (var mesh in originalMeshes)
            //{
            //    mesh.Position = new TGCVector3(6188, 4105, 6879);
            //    mesh.Scale = new TGCVector3(1f, 1f, 1f);

            //    mesh.Effect = recoltableItemEffect;
            //    mesh.Technique = "RecolectableItemTechnique";

            //    mesh.Name = "hammer_" + i.ToString();
            //    i++;

            //    collectModel.CollisionMeshes.Add(new ItemModel { Mesh = mesh, IsCollectable = true });
            //}
        }

        private void LoadCopper()
        {
            
        }

        private void LoadWood()
        {
            string pathWood = MediaDir + "\\Meshes\\recolectableWood\\recolectableWood-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathWood).Meshes[0];

            var wood1 = originalMesh.createMeshInstance(originalMesh.Name + $"_wood1");

            var position = new TGCVector3(5573, 4105, 7134);
            var scale = new TGCVector3(2f, 1f, 2f);

            wood1.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

            wood1.Effect = recoltableItemEffect;
            wood1.Technique = "RecolectableItemTechnique";

            collectModel.CollisionMeshes.Add(new ItemModel { Mesh = wood1, IsCollectable = true, CollectablePosition = position });

            var wood2 = originalMesh.createMeshInstance(originalMesh.Name + $"_wood2");

            position = new TGCVector3(2516, 50, 2472);

            wood2.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

            wood2.Effect = recoltableItemEffect;
            wood2.Technique = "RecolectableItemTechnique";

            collectModel.CollisionMeshes.Add(new ItemModel { Mesh = wood2, IsCollectable = true, CollectablePosition = position });

            var wood3 = originalMesh.createMeshInstance(originalMesh.Name + $"_wood3");

            position = new TGCVector3(3461, 200, 3595);

            wood3.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

            wood3.Effect = recoltableItemEffect;
            wood3.Technique = "RecolectableItemTechnique";

            collectModel.CollisionMeshes.Add(new ItemModel { Mesh = wood3, IsCollectable = true, CollectablePosition = position });
        }

        private void LoadRope()
        {
            string pathRope = MediaDir + "\\Meshes\\sogaenrollada\\SogaEnrollada-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathRope).Meshes[0];

            var rope1 = originalMesh.createMeshInstance($"rope1");

            var position = new TGCVector3(12000, 500, 12000);
            var scale = new TGCVector3(0.1f, 0.1f, 0.1f);

            rope1.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

            collectModel.CollisionMeshes.Add(new ItemModel { Mesh = rope1, IsCollectable = true, CollectablePosition = position });

            var rope2 = originalMesh.createMeshInstance($"rope2");

            position = new TGCVector3(5886, 3000, 4000);
            scale = new TGCVector3(0.09f, 0.09f, 0.09f);

            rope2.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.RotationYawPitchRoll(-180, 30, 0) * TGCMatrix.Translation(position);

            collectModel.CollisionMeshes.Add(new ItemModel { Mesh = rope2, IsCollectable = true, CollectablePosition = position });

            var rope3 = originalMesh.createMeshInstance($"rope3");

            position = new TGCVector3(1500, 60, 9886);
            scale = new TGCVector3(0.1f, 0.1f, 0.1f);

            rope3.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.RotationYawPitchRoll(0, -180, 0) * TGCMatrix.Translation(position);

            collectModel.CollisionMeshes.Add(new ItemModel { Mesh = rope3, IsCollectable = true, CollectablePosition = position });
        }

        private void LoadShaders()
        {
            var d3dDevice = D3DDevice.Instance.Device;

            //Cargar Shader personalizado
            string compilationErrors;
            effect = Effect.FromFile(d3dDevice, ShadersDir + "\\PostProcess.fx", null, null, ShaderFlags.PreferFlowControl,
                null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }
            //Configurar Technique dentro del shader
            effect.Technique = "PostProcess";

            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth,
                d3dDevice.PresentationParameters.BackBufferHeight,
                DepthFormat.D24S8, MultiSampleType.None, 0, true);

            // inicializo el render target
            g_pRenderTarget = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8,
                Pool.Default);

            effect.SetValue("g_RenderTarget", g_pRenderTarget);

            // Resolucion de pantalla
            effect.SetValue("screen_dx", d3dDevice.PresentationParameters.BackBufferWidth);
            effect.SetValue("screen_dy", d3dDevice.PresentationParameters.BackBufferHeight);

            var texturaCasco = TgcTexture.createTexture(MediaDir + "Level1\\Textures\\mask.png");
            effect.SetValue("texCasco", texturaCasco.D3dTexture);

            CustomVertex.PositionTextured[] vertices =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };
            //vertex buffer de los triangulos
            g_pVBV3D = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                CustomVertex.PositionTextured.Format, Pool.Default);
            g_pVBV3D.SetData(vertices, 0, LockFlags.None);
        }

        private void LoadSurface()
        {
            var surfaceTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "\\Level1\\Textures\\" + "surface.PNG");
            //surfacePlane = new TgcPlane(surfacePosition, new TGCVector3(12800, 0f, 12800), TgcPlane.Orientations.XZplane, surfaceTexture);

            surfacePlane = new TgcSimpleTerrain();
            surfacePlane.loadHeightmap(MediaDir + "Level1\\Heigthmap\\heighmapmar2.jpg", 1000, 1, new TGCVector3(0, 3850, 0));
            surfacePlane.loadTexture(MediaDir + "\\Level1\\Textures\\" + "surface.PNG");
            surfacePlane.AlphaBlendEnable = true;

            wavesEffect = TGCShaders.Instance.LoadEffect(ShadersDir + "\\Waves2.fx");
            wavesEffect.Technique = "WavesScene";

            surfacePlane.Effect = wavesEffect;
            surfacePlane.Technique = "WavesScene";
        }

        private void LoadSkyBox()
        {
            skyBox = new TgcSkyBox();

            skyBox.Center = new TGCVector3(3200, 3000, 3200);
            skyBox.Size = new TGCVector3(12700, 7000, 12700);

            var texturesPath = MediaDir + "\\Level1\\Textures\\SkyBox\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "fup.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "fd.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "f2.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "f4.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "f1.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "f3.PNG");
            skyBox.SkyEpsilon = 25f;
            skyBox.Init();
        }

        private void LoadSkyBoxUndersea()
        {
            skyBoxUndersea = new TgcSkyBox();

            skyBoxUndersea.Center = new TGCVector3(3200, 0, 3200);
            skyBoxUndersea.Size = new TGCVector3(12700, 9000, 12700);

            var texturesPath = MediaDir + "\\Level1\\Textures\\SkyBox\\";
            skyBoxUndersea.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "fd.PNG");
            skyBoxUndersea.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "others.PNG");
            skyBoxUndersea.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "2.PNG");
            skyBoxUndersea.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "4.PNG");
            skyBoxUndersea.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "1.PNG");
            skyBoxUndersea.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "3.PNG");
            skyBoxUndersea.SkyEpsilon = 25f;
            skyBoxUndersea.Init();
        }

        private void LoadMeshes()
        {
            meshes = new List<ItemModel>();

            LoadBoat();
            LoadPillarCorals();
            LoadRocks();
            LoadFishes();
            LoadCorals();
            LoadShip();
            LoadWorkbench();
            LoadDeaths();
        }

        private void LoadFatherNote()
        {
            var noteTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "\\Level1\\Textures\\" + "noteTexture.png");

            var notePosition = new TGCVector3(9820, 30, 880);
            var noteSize = new TGCVector3(20, 40, 20);

            TgcPlane note = new TgcPlane(notePosition, noteSize, TgcPlane.Orientations.XYplane, noteTexture);

            collectModel.CollisionMeshes.Add(new ItemModel { Mesh = note.toMesh("fatherNote"), IsCollectable = true, CollectablePosition = notePosition });
        }

        private void LoadDeaths()
        {
            string pathSkeleton = MediaDir + "\\Meshes\\esqueletohumano\\Esqueleto-TgcScene.xml";
            string pathSkeleton2 = MediaDir + "\\Meshes\\esqueletohumano2\\Esqueleto2-TgcScene.xml";
            string pathSkeleton3 = MediaDir + "\\Meshes\\esqueletohumano3\\Esqueleto3-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathSkeleton).Meshes[0];

            //SKLT 1 (Father)
            var position = new TGCVector3(9886, 30, 987);
            var scale = new TGCVector3(5, 5, 5);

            var sklt1 = originalMesh.createMeshInstance(originalMesh.Name + $"_skt1");

            sklt1.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.RotationYawPitchRoll(-180, 0, 0) * TGCMatrix.Translation(position);

            meshes.Add(new ItemModel { Mesh = sklt1 });

            //SKLT 2 (Mother)
            originalMesh = loader.loadSceneFromFile(pathSkeleton3).Meshes[0];

            position = new TGCVector3(3692, 70, 10286);
            scale = new TGCVector3(4, 4, 4);

            var sklt2 = originalMesh.createMeshInstance(originalMesh.Name + $"_skt2");

            sklt2.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

            meshes.Add(new ItemModel { Mesh = sklt2 });

            //SKLT 3 (Sister)
            originalMesh = loader.loadSceneFromFile(pathSkeleton2).Meshes[0];

            position = new TGCVector3(6712, 3190, 5160);
            scale = new TGCVector3(2, 2, 2);

            var sklt3 = originalMesh.createMeshInstance(originalMesh.Name + $"_skt3");

            sklt3.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

            meshes.Add(new ItemModel { Mesh = sklt3 });
        }

        private void LoadWorkbench()
        {
            string pathWorkbench = MediaDir + "\\Meshes\\workbench\\Workbench-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathWorkbench).Meshes[0];

            var position = new TGCVector3(6900, 4070, 7050);
            var scale = new TGCVector3(0.2f, 0.1f, 0.2f);

            originalMesh.Position = position;
            originalMesh.Scale = scale;

            ItemModel item = new ItemModel { Mesh = originalMesh };

            //The workbench has collision
            item.RigidBody = bulletManager.AddRigidBody(originalMesh, position, scale);

            meshes.Add(item);
        }

        private void LoadBoat()
        {
            string pathBoat = MediaDir + "\\Meshes\\boat2\\boat2-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(pathBoat);

            var position = new TGCVector3(6000, 3800, 6000);
            var scale = new TGCVector3(15, 5, 15);

            foreach (var mesh in scene.Meshes)
            {
                mesh.Position = position;
                mesh.Scale = scale;

                ItemModel item = new ItemModel { Mesh = mesh };

                //The boat has collision
                item.RigidBody = bulletManager.AddRigidBody(mesh, position, scale);

                meshes.Add(item);
            }
        }

        private void LoadShip()
        {
            string pathShip = MediaDir + "\\Meshes\\ship\\ship-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(pathShip);

            var position = new TGCVector3(10000f, 150f, 800f);
            var scale = new TGCVector3(5, 5, 5);
            var rotation = new TGCVector3(-(FastMath.PI / 8), -(FastMath.PI / 4), 0);

            foreach (var mesh in scene.Meshes)
            {
                mesh.Position = position;
                mesh.Scale = scale;
                mesh.Rotation = rotation;

                ItemModel item = new ItemModel { Mesh = mesh };

                //The ship has collision
                item.RigidBody = bulletManager.AddRigidBody(mesh, position, scale);

                meshes.Add(item);
            }
        }

        private void LoadCorals()
        {
            
        }

        private void LoadFishes()
        {
            var redFishesEffect = TGCShaders.Instance.LoadEffect(ShadersDir + "\\RedFishes.fx");
            redFishesEffect.Technique = "RedFishesTechnique";

            var rnd = new Random();
            string pathFish = MediaDir + "\\Meshes\\fish\\fish-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathFish).Meshes[0];

            var xMax = 12000f;
            var zMax = 12000f;
            var yMax = 1500f;
            var cant = 100;

            for (int i = 0; i < cant; i++)
            {
                var posX = xMax * (float)rnd.NextDouble();
                var posZ = zMax * (float)rnd.NextDouble();
                var posY = yMax * (float)rnd.NextDouble() + 20f;

                var position = new TGCVector3(posX, posY * currentScaleY, posZ);
                var scale = new TGCVector3(10, 10, 10);

                var fish = originalMesh.createMeshInstance(originalMesh.Name + $"_{i}");

                fish.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

                fish.Effect = redFishesEffect;
                fish.Technique = "RedFishesTechnique";

                meshes.Add(new ItemModel { Mesh = fish });
            }
        }

        private void LoadRocks()
        {
            var rnd = new Random();
            string pathRock = MediaDir + "\\Meshes\\rock\\Roca-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathRock).Meshes[0];

            var xMax = 12000f;
            var zMax = 12000f;
            var cant = 10;

            for (int i = 0; i < cant; i++)
            {
                var posX = xMax * (float)rnd.NextDouble();
                var posZ = zMax * (float)rnd.NextDouble();
                var posY = underseaTerrain.HeightmapData[Convert.ToInt32(posX/currentScaleXZ), Convert.ToInt32(posZ/currentScaleXZ)];

                var position = new TGCVector3(posX, posY*currentScaleY, posZ);
                var scale = new TGCVector3(i * 1.5f, i, i * 1.5f);

                var rock = originalMesh.createMeshInstance(originalMesh.Name + $"_{i}");

                rock.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

                ItemModel item = new ItemModel { Mesh = rock };

                //The rocks has collision
                item.RigidBody = bulletManager.AddRigidBody(rock, position, scale);

                meshes.Add(item);
            }
        }

        private void LoadPillarCorals()
        {
            string pathPillarCoral = MediaDir + "\\Meshes\\pillar_coral\\pillar_coral-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathPillarCoral).Meshes[0];

            var pillar1 = originalMesh.createMeshInstance(originalMesh.Name + "_1");
            pillar1.Transform = TGCMatrix.Scaling(new TGCVector3(50f, 20f, 50f)) * TGCMatrix.Translation(new TGCVector3(800f, 0f, 500f));
            var itemPillar1 = new ItemModel { Mesh = pillar1 };

            var pillar2 = originalMesh.createMeshInstance(originalMesh.Name + "_2");
            pillar2.Transform = TGCMatrix.Scaling(new TGCVector3(20f, 10f, 20f)) * TGCMatrix.Translation(new TGCVector3(10000f, 360f, 10000f));
            var itemPillar2 = new ItemModel { Mesh = pillar2 };

            meshes.AddRange(new ItemModel[] { itemPillar1, itemPillar2});
        }

        public override void Update(float elapsedTime)
        {
            playerModel.Update(elapsedTime);
            bulletManager.Update(elapsedTime);
            collectModel.Update(elapsedTime);

            effect.SetValue("withoutHelmet", playerModel.WithoutHelmet);
            effect.SetValue("inWater", playerModel.UnderSurface());
        }

        public override void Render(float elapsedTime)
        {
            TexturesManager.Instance.clearAll();

            var device = D3DDevice.Instance.Device;

            var pOldRT = device.GetRenderTarget(0);
            var pSurf = g_pRenderTarget.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            var pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDepthStencil;

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            time += elapsedTime;

            wavesEffect.SetValue("time", time);
            recoltableItemEffect.SetValue("time", time);

            
            bulletManager.Render();
            collectModel.Render(elapsedTime);

            //Render SkyBox
            if (playerModel.UnderSurface())
            {
                skyBoxUndersea.Render();
            }
            else
            {
                skyBox.Render();
            }
            
            underseaTerrain.Render();

            //Render Surface
            surfacePlane.Render();

            //Render Meshes
            foreach (var item in meshes)
            {
                item.Mesh.Render();
            }

            // restuaro el render target y el stencil
            device.DepthStencilSurface = pOldDS;
            device.SetRenderTarget(0, pOldRT);

            effect.Technique = "PostProcess";

            effect.SetValue("time", time);
            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, g_pVBV3D, 0);
            effect.SetValue("g_RenderTarget", g_pRenderTarget);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();

            hudModel.Render();

            playerModel.Render(elapsedTime);

            DrawText.drawText(
                $"Position Camera: {Camera.Position}", 5, 50,
                Color.Yellow);

            DrawText.drawText(
                $"Position Rigid Body: {bulletManager.RigidCamera.CenterOfMassPosition}", 5, 100,
                Color.Red);
        }

        public override void Dispose()
        {
            effect.Dispose();
            recoltableItemEffect.Dispose();
            wavesEffect.Dispose();
            playerModel.Dispose();
            bulletManager.Dispose();
            skyBox.Dispose();
            skyBoxUndersea.Dispose();
            surfacePlane.Dispose();
            underseaTerrain.Dispose();
            collectModel.Dispose();

            //Dispose de Meshes
            meshes.ForEach(x => x.Dispose());
        }
    }
}
