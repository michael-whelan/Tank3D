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
    class Rocket
    {
        Model missile;
        Vector3 position = Vector3.Zero;
        Vector3 direction = Vector3.Backward;
        float yaw = 0;
        Game game;
        Matrix world = Matrix.Identity;
        bool alive = false;
        float maxSpeed = 1.5f;
        BoundingSphere rocketSphere;
        


        public BoundingSphere Sphere
        {
            get { return rocketSphere; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

         public Rocket(Game theGame)
        {
            game = theGame;
        }

         public void Initilaize()
         {
             ContentManager Content = (ContentManager)game.Services.GetService(typeof(ContentManager));
             missile = Content.Load<Model>("missile1");
         }

         public void Update(float aimerYaw,Vector3 tankPos, bool shoot)
         {
             SetBoundingSphere();
             if (!alive)
             {
                 yaw = aimerYaw - (float)Math.PI;
                 position = tankPos;
                 position.Y = 3;
             }
             KeyboardState ks = Keyboard.GetState();

             if (shoot == true)
             {
                 alive = true;
             }
             if (ks.IsKeyDown(Keys.R))
             {
                 alive = false;
             }

             if (alive)
             {
                 position -= direction * maxSpeed;
             }
             Matrix rot = Matrix.CreateRotationY(yaw);
             direction = Vector3.Transform(Vector3.Backward, rot);
             world = Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(position);
         }

         public void SetBoundingSphere()
         {
             rocketSphere.Center = new Vector3(position.X - .2f, position.Y, position.Z + .2f);
             rocketSphere.Radius = .8f;             
         }

         public void Draw(GameTime gt, Matrix view, Matrix proj)
         {
             foreach (ModelMesh mesh in missile.Meshes)
             {
                 foreach (BasicEffect effect in mesh.Effects)
                 {
                     effect.LightingEnabled = false;
                     effect.World = world;
                     effect.Projection = proj;
                     effect.View = view;
                 }
                 if (alive)
                 {
                     mesh.Draw();
                 }
             }
         }
    }
}