using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CityShooter
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Tank playerTank;
        
        Rocket missile;
        CollisionManager colManage;
        List<ParticleSystem> sflares;
        ParticleSystem playerSmoke;
        ParticleSystem rocketSmoke;
        ParticleSystem rocketFire;

        String[] map ={ 
                    "123834561449",
                    "7╔═╦═╦════╗1",
                    "6║1║3║1231║2",
                    "8║2║4╠════╣1",
                    "2╠═╩═╣7447║3",
                    "3║595║7817║5",
                    "6╠═╦═╬════╣9",
                    "5║2║9║1818║4",
                    "3║1╚═╬═╗63║1",
                    "1║343╠═╝93║9",
                    "4╚═══╩════╝8",
                    "818131954321"
        };

        String[] npcMap ={  
                             "",
                    " ╔═╦═╦════╗ ",
                    " ║1║3*1231║ ",
                    " ║2║4╠════╣ ",
                    " ╠═╩═╣7447* ",
                    " ║595║7817║ ",
                    " ╠═╦═╬═*══╣ ",
                    " ║2║9║1818* ",
                    " ║1╚═╬═╗63║ ",
                    " ║343╠═╝93║ ",
                    " ╚══*╩════* ",
                    ""
        };
        
        List<Street> streets;
        Rectangle blockSize;
        List <Building> buildings;
        List<Enemy> enemies;
        Camera camera;
        


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            playerTank = new Tank(this);
            missile = new Rocket(this);
            camera = new Camera();

            Services.AddService(typeof(ContentManager), Content);
            Services.AddService(typeof(Camera), camera);
        }

        protected override void Initialize()
        {
            playerTank.Initialize();
            missile.Initilaize();
            
            blockSize = new Rectangle(0, 0, 15, 15);

            colManage = new CollisionManager();
            streets = new List<Street>();
            buildings = new List<Building>();
            enemies = new List<Enemy>();

            StreetFactory.Init(this, blockSize);
            BuildingManager.Initialize(this, blockSize);
            NPCManager.Initialize(this);

            for (int z = 0; z < map.Length; z++)
            {
                for (int x = 0; x < map[z].Length; x++)
                {
                    char c = map[z][x];
                    if (BuildingManager.buildingSymbols.Contains(c))
                    {
                        Building b = BuildingManager.makeBuilding(map[z][x], new Vector2(x, z));
                        buildings.Add(b);
                    }
                }
            }

            for (int z = 0; z < map.Length; z++)
            {
                for (int x = 0; x < map[z].Length; x++)
                {
                    char c = map[z][x];
                    if (StreetFactory.streetSymbols.Contains(c))
                    {
                        Street s = StreetFactory.makeStreet(map[z][x], new Vector2(x, z));
                        streets.Add(s);
                    }
                }
            }

            for (int z = 0; z < npcMap.Length; z++)
            {
                for (int x = 0; x < npcMap[z].Length; x++)
                {
                    char c = npcMap[z][x];
                    if(NPCManager.npcSymbol.Contains(c))
                    {
                        Enemy e = NPCManager.MakeEnemies(npcMap[z][x], new Vector2(x, z));
                        enemies.Add(e);
                    }
                }
            }
            InitializeParticles();
            camera = new Camera();
            camera.Init(new Vector3(0, 10, 0), new Vector3(50, 10, 50), Vector3.Up,0.6f,graphics.GraphicsDevice.Viewport.AspectRatio,1,1000);

            base.Initialize();
        }

        public void InitializeParticles()
        {
            Texture2D flareTexture = Content.Load<Texture2D>("fireball2");
            sflares = new List<ParticleSystem>();
            Vector3 position = new Vector3(0, 1000, 0);

            // ParticleSystem sps = new ParticleSystem(this);
            playerSmoke = new ParticleSystem(this);
            playerSmoke.Texture = flareTexture;
            playerSmoke.Position = position;
            playerSmoke.name = "player";
            playerSmoke.Init();

            rocketSmoke = new ParticleSystem(this);
            rocketSmoke.Texture = flareTexture;
            rocketSmoke.Position = position;
            rocketSmoke.name = "smoke";
            rocketSmoke.ParticleLife = 50;
            rocketSmoke.Init();


            Texture2D fireTexture = Content.Load<Texture2D>("fire");
            rocketFire = new ParticleSystem(this);
            rocketFire.Texture = fireTexture;
            rocketFire.Position = position;
            rocketFire.name = "fire";
            rocketFire.Velocity = 3;
            rocketFire.ParticleLife = 10;
            rocketFire.Init();


            foreach (Enemy e in enemies)
            {
                ParticleSystem enemyParts = new ParticleSystem(this);

                enemyParts.Texture = flareTexture;
                enemyParts.Position = position;
                enemyParts.name = "enemy";
                enemyParts.Init();
                sflares.Add(enemyParts);
            }
        }

        protected override void LoadContent()
        {       
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            KeyboardState kb = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || kb.IsKeyDown(Keys.Escape))
            { this.Exit(); }

            camera.Update(gameTime, playerTank.TankPos, playerTank.AimerYaw);
            playerTank.Update(gameTime);
            missile.Update(playerTank.AimerYaw,playerTank.TankPos,playerTank.shoot);
            

            foreach (Building b in buildings)
            {
                if (colManage.SphereVsBox(playerTank.Sphere, b.HitBox) == true)
                {
                    playerTank.Collision();
                }
                if (colManage.SphereVsBox(missile.Sphere, b.HitBox) == true)
                {
                    missile.Alive = false;
                }
            }

            foreach (Enemy e in enemies)
            {
                e.Update(gameTime);
                if (e.Alive)
                {
                    if (missile.Alive == true)
                    {
                        if (colManage.SphereVsShere(missile.Sphere, e.Sphere) == true)
                        {
                            e.Health -= 50;
                            missile.Alive = false;
                        }
                    }
                    if (e.enemyBullet.Alive == true)
                    {
                        if (colManage.SphereVsShere(e.enemyBullet.Sphere, playerTank.Sphere) == true)
                        {
                            playerTank.Health -= 50;
                            e.enemyBullet.Alive = false;
                        }
                    }

                    if (colManage.SphereVsShere(e.Proximity, playerTank.Sphere) == true)
                    {
                        e.SeekPlayer(playerTank.TankPos);
                        e.seekPlayer = true;
                    }
                    else { e.seekPlayer = false; }


                    foreach (Building b in buildings)
                    {
                        if (colManage.SphereVsBox(e.Sphere, b.HitBox) == true)
                        {
                            e.Collision();
                        }
                        if (colManage.SphereVsBox(e.enemyBullet.Sphere, b.HitBox) == true)
                        {
                            e.enemyBullet.Alive = false;
                        }
                        if (colManage.SphereVsBox(camera.Sphere, b.HitBox) == true)
                        {
                            b.clipping = true;
                        }
                        else { b.clipping = false; }
                    }
                }
            }
            UpdateParticles(gameTime);
            base.Update(gameTime);
        }


        public void UpdateParticles(GameTime gameTime)
        {
            //player particles for the smoke
            Vector3 partPos = new Vector3(0, 1000, 0);//sets a hidden position when no drawing is wanted
            bool smokeDark = true;
            if (playerTank.Health <= 400 && playerTank.Health > 300)
            {
                smokeDark = false;
                partPos = playerTank.TankPos;
            }
            else if (playerTank.Health <= 300 && playerTank.Health > 100)
            {
                smokeDark = true;
                partPos = playerTank.TankPos;
                playerSmoke.MaxParts = 30;
            }
            else if (playerTank.Health <= 100 && playerTank.Health > 0)
            {
                smokeDark = true;
                partPos = playerTank.TankPos;
                playerSmoke.MaxParts = 200;
            }
            else
            {
                partPos = new Vector3(0, 1000, 0);
            }
            playerSmoke.Update(gameTime, camera, partPos, smokeDark);

            foreach (Enemy e in enemies)
            {
                Vector3 pos;
                if (missile.Alive)
                {
                    pos = new Vector3(missile.Position.X, missile.Position.Y-1, missile.Position.Z);
                }
                else { pos = new Vector3(0, 1000, 0); }
                rocketSmoke.Update(gameTime, camera, pos, true);
                rocketFire.Update(gameTime, camera, pos, false);

                //runs through all particle syatems for each enemy 
                foreach (ParticleSystem b in sflares)
                {
                    Vector3 enemyPos = new Vector3(0, 1000, 0);
                    if (e.Health < 100 && e.Health > 0)
                    {
                        enemyPos = e.position;
                    }
                    else if (e.Health <= 0)
                    {
                        enemyPos = new Vector3(0, 1000, 0);
                    }
                    b.Update(gameTime, camera, enemyPos, true);
                }
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

         
            playerTank.Draw(gameTime, camera.View,camera.Projection);
            missile.Draw(gameTime, camera.View, camera.Projection);

            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp; // need to do this on reach devices to allow non 2^n textures
            RasterizerState rs = RasterizerState.CullNone;
            
            GraphicsDevice.RasterizerState = rs;
            
            foreach (Street s in streets)
            {
                s.Draw(gameTime, camera);
            }

            foreach (Building b in buildings)
            {
               b.Draw( camera);
            }
            foreach (Enemy e in enemies)
            {
                e.Draw(gameTime, camera.View, camera.Projection);
            }


            foreach (ParticleSystem ps in sflares)
            {
                ps.Draw(gameTime, camera);
            }

            playerSmoke.Draw(gameTime, camera);

            rocketSmoke.Draw(gameTime, camera);

            rocketFire.Draw(gameTime,camera);
            base.Draw(gameTime);
        }
    }
}
