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
    class Enemy
    {
        public Model model;
        Game game;
        Matrix world = Matrix.Identity;
        public Vector3 position = new Vector3();
        Matrix[] bones;
        bool alive = true;
        BoundingSphere sphere = new BoundingSphere();
        Vector3 direction = Vector3.Backward;
        float speed = 0.2f;
        float maxYawSpeed = 0.05f;
        Vector3 enemySpeed = new Vector3(10,0, 10);
        float yaw;
        Random rand = new Random();
        int moveDirect = 1, forward = 1,backward = -1;
        int turnDirect = 0,left = 1,right = 2;
        int timer=0, timerMax = 1000,shootTimer = 0,sTMax = 100;
        BoundingSphere proximity;
        public bool seekPlayer = false;
        public Rocket enemyBullet;
        bool shoot = false;
        int health = 100;

        public Enemy(Game game1)
        { game = game1; }

        

        public BoundingSphere Sphere
        {
            get { return sphere; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public BoundingSphere Proximity
        {
            get { return proximity; }
        }

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

        public void Initialize()
        {
            enemyBullet = new Rocket(game);
            bones = new Matrix[model.Bones.Count];
            sphere.Radius = 3;//3
            proximity.Radius = 18;
            turnDirect = rand.Next(1,2);
            enemyBullet.Initilaize();
            //moveDirect = forward;
        }

        public void Collision()
        {
            int num = rand.Next(1,10);
            SetYaw(num);
            yaw += maxYawSpeed;
            if (moveDirect == forward)
            { position -= direction * 1; }
            if (moveDirect == backward)
            { position += direction * 1; }
            
        }

        public void SetYaw(int num)
        {
            if (turnDirect == right)
            {
                if (num == 1)
                { maxYawSpeed = 0.24f; }
                else if (num == 2)
                { maxYawSpeed = 0.30f; }
                else if (num == 3)
                { maxYawSpeed = 0.30f; }
                else if (num == 4)
                { maxYawSpeed = 0.35f; }
                else if (num == 5)
                { maxYawSpeed = 0.40f; }
                else if (num == 6)
                { maxYawSpeed = 0.45f; }
                else if (num == 7)
                { maxYawSpeed = 0.50f; }
                else if (num == 8)
                { maxYawSpeed = 0.60f; }
                else if (num == 9)
                { maxYawSpeed = 0.65f; }
                else if (num == 10)
                { maxYawSpeed = 0.65f; }
            }
            else if (turnDirect == left)
            {
                if (num == 1)
                { maxYawSpeed = -0.24f; }
                else if (num == 2)
                { maxYawSpeed = -0.30f; }
                else if (num == 3)
                { maxYawSpeed = -0.30f; }
                else if (num == 4)
                { maxYawSpeed = -0.35f; }
                else if (num == 5)
                { maxYawSpeed = -0.40f; }
                else if (num == 6)
                { maxYawSpeed = -0.45f; }
                else if (num == 7)
                { maxYawSpeed = -0.50f; }
                else if (num == 8)
                { maxYawSpeed = -0.60f; }
                else if (num == 9)
                { maxYawSpeed = -0.65f; }
                else if (num == 10)
                { maxYawSpeed = 0.65f; }
            }
        }

        public void Update(GameTime gameTime)
        {
            sphere.Center = new Vector3(position.X - (sphere.Radius / 2), position.Y, position.Z + (sphere.Radius / 2));
            proximity.Center = new Vector3(position.X - (sphere.Radius / 2), position.Y, position.Z + (sphere.Radius / 2));

            if (seekPlayer == false)//only if the enemies arnt within range
            {
                if (moveDirect == forward)
                { position += direction * speed; }

                if (moveDirect == backward)
                {
                    position -= direction * speed;
                }
            }
            if (yaw <= -6.4f||yaw >=6.4f)
            {
                yaw = 0;
            }
            enemyBullet.Update(yaw, position, shoot);
            Timer();

            if (health <= 0)
            { alive = false; }

            Matrix rot = Matrix.CreateRotationY(yaw);
            direction = Vector3.Transform(Vector3.Backward, rot);
            world = Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(position);
        }

        public void Timer()
        {
            timer++;
            if (timer >= timerMax)
            {
                if (turnDirect == left)
                { turnDirect = right; }
                else if (turnDirect == right)
                { turnDirect = left; }
                timer = 0;
            }
        }

        public void SeekPlayer(Vector3 playerPos)
        {
            Vector3 posDifference = new Vector3(playerPos.X - position.X, 0,playerPos.Z - position.Z); // finds the vector for the difference in positions

            float rotation = (float)Math.Atan2(posDifference.X, posDifference.Z);

            if (yaw - 0.08f < rotation)
            {
                yaw += 0.03f;
            }

            if (yaw - 0.08f > rotation)
            {
                yaw -= 0.03f;
            }
            if (shootTimer >= sTMax)
            {
                //enemyBullet.Update(yaw, position,true); //lets the enemy create a bullet at rthe enemies position
                shoot = true;
                shootTimer = 0;
            }
            else { shoot = false; }
            shootTimer++;
        }

        public void Draw(GameTime gt, Matrix view, Matrix proj)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //Effect Settings Goes here
                    effect.LightingEnabled = false;
                    effect.World = bones[mesh.ParentBone.Index] * world;
                    effect.Projection = proj;
                    effect.View = view;
                }
                model.CopyAbsoluteBoneTransformsTo(bones);
                enemyBullet.Draw(gt,view,proj);
                if (alive)
                    mesh.Draw();
            }
        }
    }
}
