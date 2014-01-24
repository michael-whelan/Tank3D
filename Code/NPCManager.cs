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
    class NPCManager
    {
          public static string npcSymbol = "*";
        static Model tank;
        static float blockSize = 16f;


          static Game theGame;
          public static Enemy MakeEnemies(char c, Vector2 position)
          {
              Enemy e = new Enemy(theGame);
              if (npcSymbol.Contains(c))
              {
                  e.model = tank;
              }
              e.position = new Vector3(position.X*blockSize, 0,position.Y*blockSize);
              e.Initialize();
              return e;
          }

          public static void Initialize(Game game)
          {
              theGame = game;
              LoadTexture();
          }

          public static  void LoadTexture()
          {
              ContentManager contentManager = (ContentManager)theGame.Services.GetService(typeof(ContentManager));
              tank = contentManager.Load<Model>("enemyTank");
          }
    }
}
