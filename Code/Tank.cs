using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CityShooter
{
    class Tank
    {
        Model tank;
        Vector3 position = Vector3.Zero;
        Vector3 direction = Vector3.Backward;
        BoundingSphere sphere = new BoundingSphere();
        float yaw = 0;
        float turretYaw = 0;//set seperately for turret rotation
        float maxYawSpeed = 0.05f;// radiansperframe
        float maxHorzSpeed = 0.2f;
        Vector3 rotation = Vector3.Zero;
        Game game;
        Matrix world = Matrix.Identity;
        Matrix turretTransform;
        Matrix[] bonesM;
        ModelBone turretBone;
        Matrix turretRotation;
        ModelBone frWheelBone;
        ModelBone flWheelBone;
        ModelBone brWheelBone;
        ModelBone blWheelBone;
        Matrix wheelTrans;
        Matrix wheelRot;
        float wheelSpin = 0;
        float aimerYaw;//turretYaw +tankYaw
        public bool shoot = false;
        //direction variables
        int forward = 1;
        int backward = -1;
        int movementDirection;
        bool alive = true;
        int health = 500;
        bool collisionF = false;//is collision was in the forward or backward direction
        bool collisionB = false;

        public float AimerYaw
        {
            get { return aimerYaw; }
        }

        public BoundingSphere Sphere
        {
            get { return sphere; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public Vector3 TankPos
        {
            get { return position; }
            set { position = value; }
        }

        public Tank(Game theGame)
        {
            game = theGame;
        }

        public void Initialize()
        {
            ContentManager Content = (ContentManager)game.Services.GetService(typeof(ContentManager));
            tank = Content.Load<Model>("tank");
            position = new Vector3(20,0,20);
            bonesM = new Matrix[tank.Bones.Count];
            turretBone = tank.Bones[9];//sets the turret bone as the turret
            frWheelBone = tank.Bones[4];
            flWheelBone = tank.Bones[8];//2
            blWheelBone = tank.Bones[6];
            brWheelBone = tank.Bones[2];
            turretTransform = turretBone.Transform;
            sphere.Radius = 3;//3

        }

        public void Update(GameTime gt)
        {
            KeyboardState ks = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            aimerYaw = turretYaw + yaw;
            sphere.Center =new Vector3(position.X-(sphere.Radius/2),position.Y,position.Z+(sphere.Radius/2));//half

            if (alive)
            {
                if (ks.IsKeyDown(Keys.D) || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
                {
                    if (collisionB == false && collisionF == false)
                    {
                        yaw -= maxYawSpeed;
                    }
                }
                if (ks.IsKeyDown(Keys.A) || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
                {
                    if (collisionB == false && collisionF == false)
                    {
                        yaw += maxYawSpeed;
                    }
                }

                if (ks.IsKeyDown(Keys.W) || gamePadState.IsButtonDown(Buttons.LeftThumbstickUp))
                {
                    if (collisionF == false)
                    {
                        position -= direction * maxHorzSpeed;
                        wheelSpin = 0.3f;
                        movementDirection = forward;
                        EndCollision();
                    }
                }
                else if (ks.IsKeyDown(Keys.S) || gamePadState.IsButtonDown(Buttons.LeftThumbstickDown))
                {
                    if (collisionB == false)
                    {
                        position += direction * maxHorzSpeed;
                        wheelSpin = -0.3f;
                        movementDirection = backward;
                        EndCollision();
                    }
                }
                else { wheelSpin = 0; }

                if (ks.IsKeyDown(Keys.Left) || gamePadState.IsButtonDown(Buttons.RightThumbstickLeft))
                {
                    turretYaw += maxYawSpeed;
                }

                if (ks.IsKeyDown(Keys.Right) || gamePadState.IsButtonDown(Buttons.RightThumbstickRight))
                {
                    turretYaw -= maxYawSpeed;
                }


                if (ks.IsKeyDown(Keys.Space))
                {
                    shoot = true;
                }
                else { shoot = false; }

                if (health == 0)
                {
                    alive = false;
                }

            }

            

            Matrix rot = Matrix.CreateRotationY(yaw);
            direction = Vector3.Transform(Vector3.Forward, rot);
            world = Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(position);
            turretRotation = Matrix.CreateRotationY(turretYaw);
            turretBone.Transform = turretRotation * turretTransform;
            wheelRot = Matrix.CreateRotationX(wheelSpin);
            WheelSpin(frWheelBone);
            WheelSpin(flWheelBone);
            WheelSpin(brWheelBone);
            WheelSpin(blWheelBone);
        }

        public void WheelSpin(ModelBone wheelBone)
        {
            wheelTrans = wheelBone.Transform;
            wheelBone.Transform = wheelRot * wheelTrans;
        }

        public void Collision()
        {
            if (movementDirection == forward)
            {
                collisionF = true;
                position += direction * maxHorzSpeed;
            }
            else if (movementDirection == backward)
            {
                collisionB = true; 
                position -= direction * maxHorzSpeed;
            }
        }

        public void EndCollision()
        {
                collisionB = false;
                collisionF = false;
        }

        public void Draw(GameTime gt, Matrix view, Matrix proj)
        {
            foreach (ModelMesh mesh in tank.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //Effect Settings Goes here
                    effect.LightingEnabled = false;
                    effect.World = bonesM[mesh.ParentBone.Index] * world;
                    effect.Projection = proj;
                    effect.View = view;
                }
                tank.CopyAbsoluteBoneTransformsTo(bonesM);
                if (alive)
                mesh.Draw();
            }
        }

    }
}
