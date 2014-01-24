using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CityShooter
{
    class ParticleSystem:GameObject
    {
        public static int MaxParticles = 20;
        Texture2D texture;
        int timerMax = 30;
        public string name;
        bool smoke;
        float particleVelocity = 0.6f;
        int lifeMax = 200;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public int MaxParts
        {
            set { MaxParticles = value; }
        }

        public float Velocity
        {
            get { return particleVelocity; }
            set { particleVelocity = value; }
        }

        public int ParticleLife
        {
            get { return lifeMax; }
            set { lifeMax = value; }
        }

         public ParticleSystem(Game game)
            : base(game)
        {
        

        }

        SimpleParticleEmmiter emmiter=new SimpleParticleEmmiter();

        List<SimpleParticle> particles = new List<SimpleParticle>();

        public override void Init()
        {

            emmiter.Init();
            emmiter.Position = position;
            emmiter.TimerMax = timerMax;
            Effect = new BasicEffect(graphicsDevice);
            Effect.VertexColorEnabled = true;
            Effect.TextureEnabled = true;

 
        }

        public void Update(GameTime gametime, Camera camera,Vector3 pos, bool smokeCol)
        {
            smoke = smokeCol;
            emmiter.Update(gametime, particles,this,pos,particleVelocity);

            foreach (SimpleParticle p in particles)
            {
                p.Update(gametime,camera, smoke);
                if (p.LifeTime >= lifeMax)
                {
                    p.Position = new Vector3(pos.X,pos.Y+1,pos.Z+1);
                    p.LifeTime = 0;
                }
                p.LifeTime++;
            }


        }

        override public  void Draw(GameTime gametime, Camera camera)
        { 

            foreach(SimpleParticle sp in particles){
                sp.Draw(camera,graphicsDevice);
            }
        }
    
    }



    class SimpleParticleEmmiter
    {

        Vector3 position;
        Random random;
        int timer;
        int timerMax;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public int TimerMax
        {
            set { timerMax = value; }
        }

        public SimpleParticleEmmiter()
        {
        }
        public void Init()
        {
            random = new Random();
        }

        public void Update(GameTime gameTime, List<SimpleParticle> particleList, ParticleSystem sps,Vector3 pos,float particleVelocity)
        {
            timer++;
            if (timer >= timerMax)//changes the rete the particles appear
            {
                if (ParticleSystem.MaxParticles > particleList.Count)
                {
                    float maxVel = particleVelocity; //0.6f;
                    SimpleParticle p = new SimpleParticle();
                    p.Init();
                    p.Position = position;
                    p.velocity = new Vector3((float)random.NextDouble() * maxVel, Math.Abs((float)random.NextDouble()) * 3.0f, (float)random.NextDouble() * maxVel);
                    p.Texture = sps.Texture;
                    p.Effect = sps.Effect;
                    particleList.Add(p);
                }
                timer = 0;
            }
        }
    }



    class SimpleParticle
    {

        BasicEffect effect;
        public bool alive = true;
        int lifeTime;
        bool smoke;

        public BasicEffect Effect
        {
            get { return effect; }
            set { effect = value; }
        }
        public Vector3 velocity;
        private Vector3 position;

        public int LifeTime
        {
            get { return lifeTime; }
            set { lifeTime = value; }
        }
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }



        VertexPositionColorTexture[] verts = new VertexPositionColorTexture[4];


        public void Init()
        {
            float s = 0.5f;  // initial size of a particle
            verts[0].Position = new Vector3(-s, -s, 0);
            verts[1].Position = new Vector3(+s, -s, 0);
            verts[2].Position = new Vector3(-s, +s, 0);
            verts[3].Position = new Vector3(+s, +s, 0);

            verts[0].TextureCoordinate = new Vector2(0, 0);
            verts[1].TextureCoordinate = new Vector2(1, 0);
            verts[2].TextureCoordinate = new Vector2(0, 1);
            verts[3].TextureCoordinate = new Vector2(1, 1);

            verts[0].Color = Color.White;
            verts[1].Color = Color.White;
            verts[2].Color = Color.White;
            verts[3].Color = Color.White;
        }


        public void Update(GameTime gametime, Camera camera,bool smoke_)
        {
            float time = (float)gametime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            smoke = smoke_;//checks if the smoke should be black or white
            position += velocity * time;
        }


        public void Draw(Camera camera, GraphicsDevice g)
        {
            Effect.View = camera.View;
            Effect.Projection = camera.Projection;
            Effect.World = Matrix.CreateBillboard(position, camera.pos, camera.up, camera.dir);
            Effect.Texture = texture;
            g.DepthStencilState = DepthStencilState.None;//allows particles

            if (smoke == true)
            {
                g.BlendState = BlendState.AlphaBlend;//makes the smoke black
            }
            else if(smoke == false)//makes the smoke white
            { g.BlendState = BlendState.Additive; }

            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                g.DrawUserPrimitives(PrimitiveType.TriangleStrip, verts, 0, 2);
            }
            g.DepthStencilState = DepthStencilState.Default;//stops the textures from ghosting/becoming transparent
            g.BlendState = BlendState.Opaque;           //same use ^^^^^^^^^

        }

    }
}
