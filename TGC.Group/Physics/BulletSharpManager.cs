using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BulletPhysics;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Camera;
using TGC.Group.Model;

namespace TGC.Group.Physics
{
    public class BulletSharpManager
    {
        private TgcD3dInput Input;
        private TgcCamera Camera;
        private PlayerModel PlayerModel;

        DiscreteDynamicsWorld dynamicsWorld;
        CollisionDispatcher dispatcher;
        BroadphaseInterface broadphaseInterface;
        SequentialImpulseConstraintSolver constraintSolver;
        DefaultCollisionConfiguration collisionConfiguration;
        private TGCVector3 initialPosition;
        private Vector3 gravity = new Vector3(0, -250f, 0);
        private Vector3 gravityUnderSurface = new Vector3(0, -5f, 0);

        public RigidBody RigidCamera { get; private set; }

        public BulletSharpManager(TGCVector3 initialPosition, TgcD3dInput input, TgcCamera camera, PlayerModel playerModel)
        {
            this.initialPosition = initialPosition;
            Input = input;
            Camera = camera;
            PlayerModel = playerModel;
        }

        public void Init(TgcSimpleTerrain terrain)
        {
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            broadphaseInterface = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphaseInterface, constraintSolver, collisionConfiguration);
            dynamicsWorld.Gravity = gravity;

            var ballShape = new SphereShape(50);
            var ballTransform = TGCMatrix.Identity;
            ballTransform.Origin = initialPosition;
            var ballMotionState = new DefaultMotionState(ballTransform.ToBsMatrix);
            var ballInertia = ballShape.CalculateLocalInertia(1f);
            var ballInfo = new RigidBodyConstructionInfo(1, ballMotionState, ballShape, ballInertia);

            RigidCamera = new RigidBody(ballInfo);
            RigidCamera.SetDamping(0.9f, 0.9f);

            //esto es para que no le afecte la gravedad al inicio de la partida
            //RigidCamera.ActivationState = ActivationState.IslandSleeping;
            dynamicsWorld.AddRigidBody(RigidCamera);

            var heighmapRigid = BulletRigidBodyFactory.Instance.CreateSurfaceFromHeighMap(terrain.getData());
            dynamicsWorld.AddRigidBody(heighmapRigid);
        }

        public void Update(float elapsedTime)
        {
            dynamicsWorld.StepSimulation(1 / 60f, 100);

            var director = Camera.LookAt - Camera.Position;
            director.Normalize();
            var strength = 10f;

            if (Camera.Position.Y > 4119)
            {
                director.Y = 0;
            }

            if (Input.keyDown(Key.W))
            {
                RigidCamera.ActivationState = ActivationState.ActiveTag;

                RigidCamera.ApplyCentralImpulse(strength * director.ToBulletVector3());

            }
            if (Input.keyUp(Key.W))
            {
                //op1
                //RigidCamera.ActivationState = ActivationState.IslandSleeping;
                //op2
                //rigidCamera.ActivationState = ActivationState.DisableSimulation;
            }

            if (Input.keyDown(Key.A))
            {

                RigidCamera.ActivationState = ActivationState.ActiveTag;

                var left = new TGCVector3();
                left.X = director.X * FastMath.Cos(FastMath.PI_HALF) - director.Z * FastMath.Sin(FastMath.PI_HALF);
                left.Z = director.X * FastMath.Sin(FastMath.PI_HALF) + director.Z * FastMath.Cos(FastMath.PI_HALF);

                RigidCamera.ApplyCentralImpulse(strength * left.ToBulletVector3());

            }
            if (Input.keyUp(Key.A))
            {
                //op1
                //RigidCamera.ActivationState = ActivationState.IslandSleeping;
                //op2
                //rigidCamera.ActivationState = ActivationState.DisableSimulation;
            }

            if (Input.keyDown(Key.D))
            {

                RigidCamera.ActivationState = ActivationState.ActiveTag;

                var right = new TGCVector3();
                right.X = director.X * FastMath.Cos(FastMath.PI + FastMath.PI_HALF) - director.Z * FastMath.Sin(FastMath.PI + FastMath.PI_HALF);
                right.Z = director.X * FastMath.Sin(FastMath.PI + FastMath.PI_HALF) + director.Z * FastMath.Cos(FastMath.PI + FastMath.PI_HALF);

                RigidCamera.ApplyCentralImpulse(strength * right.ToBulletVector3());

            }
            if (Input.keyUp(Key.D))
            {
                //op1
                //RigidCamera.ActivationState = ActivationState.IslandSleeping;
                //op2
                //rigidCamera.ActivationState = ActivationState.DisableSimulation;
            }

            if (Input.keyDown(Key.S))
            {
                RigidCamera.ActivationState = ActivationState.ActiveTag;
                RigidCamera.ApplyCentralImpulse(-strength * director.ToBulletVector3());

            }
            if (Input.keyUp(Key.S))
            {
                //op1
                //RigidCamera.ActivationState = ActivationState.IslandSleeping;
                //op2
                //rigidCamera.ActivationState = ActivationState.DisableSimulation;
            }

            ((FpsCamera)Camera).SetPosicion(new TGCVector3(RigidCamera.CenterOfMassPosition));

            PlayerModel.Position = Camera.Position;

            dynamicsWorld.Gravity = gravityUnderSurface;
            if (!PlayerModel.UnderSurface())
            {
                dynamicsWorld.Gravity = gravity;
            }
        }

        public void Render()
        {
            
        }

        public void Dispose()
        {
            RigidCamera.Dispose();
            dynamicsWorld.Dispose();
        }

        public RigidBody AddRigidBody(TgcMesh mesh, TGCVector3 position, TGCVector3 scale, int mass = 0)
        {
            //var rigidBody = BulletRigidBodyFactory.Instance.CreateBox(size, mass, mesh.Position, 0, 0, 0, 0.55f, false);

            var triangleMesh = BuildTriangleMeshShape(mesh);
            var rigidBody = BuildRigidBodyFromTriangleMeshShape(triangleMesh, mass, position, scale);

            dynamicsWorld.AddRigidBody(rigidBody);

            return rigidBody;
        }

        private BvhTriangleMeshShape BuildTriangleMeshShape(TgcMesh mesh)
        {
            var vertexCoords = mesh.getVertexPositions();

            TriangleMesh triangleMesh = new TriangleMesh();
            for (int i = 0; i < vertexCoords.Length; i = i + 3)
            {
                triangleMesh.AddTriangle(vertexCoords[i].ToBulletVector3(), vertexCoords[i + 1].ToBulletVector3(), vertexCoords[i + 2].ToBulletVector3());
            }
            return new BvhTriangleMeshShape(triangleMesh, false);
        }

        private RigidBody BuildRigidBodyFromTriangleMeshShape(BvhTriangleMeshShape triangleMeshShape, int mass, TGCVector3 position, TGCVector3 scale)
        {
            var transformationMatrix = TGCMatrix.RotationYawPitchRoll(0, 0, 0);
            transformationMatrix.Origin = position;
            DefaultMotionState motionState = new DefaultMotionState(transformationMatrix.ToBsMatrix);

            var bulletShape = new ScaledBvhTriangleMeshShape(triangleMeshShape, scale.ToBulletVector3());
            var boxLocalInertia = bulletShape.CalculateLocalInertia(0);

            var bodyInfo = new RigidBodyConstructionInfo(mass, motionState, bulletShape, boxLocalInertia);
            var rigidBody = new RigidBody(bodyInfo);
            rigidBody.Friction = 0.4f;
            rigidBody.RollingFriction = 1;
            rigidBody.Restitution = 1f;

            return rigidBody;
        }
    }
}
