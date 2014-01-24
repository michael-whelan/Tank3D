using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CityShooter
{
    class BuildingManager
    {
        public static string buildingSymbols = "1 2 3 4 5 6 7 8 9";
        public static string build1 = "1";
        public static string build2 = "2 3";
        public static string build3 = "4";
        public static string build4 = "5 6";
        public static string build5 = "7";
        public static string build6 = "8 9";
        static Rectangle blockSize;


        static Texture2D build1T;
        static Texture2D build2T;
        static Texture2D build3T;
        static Texture2D build4T;
        static Texture2D build5T;
        static Texture2D build6T;

        static Texture2D roof1;
        static Texture2D roof2;
        static Texture2D roof3;
        static Texture2D roof4;

        // public static Street makeStreet(char c,Vector2 position)
        public static Building makeBuilding(char c, Vector2 position)
        {
            Building b = new Building(theGame);
            int buildingType = buildingSymbols.IndexOf(c);

            if (build1.Contains(c))
            {
                int i = build1.IndexOf(c);
                b.texture = build1T;
                b.roof = roof1;
                b.roofH = 0.5f;
            }

            if (build2.Contains(c))
            {
                int i = build2.IndexOf(c);
                b.texture = build2T;
                b.roof = roof4;
                b.roofH = 5;
            }
            if (build3.Contains(c))
            {
                int i = build3.IndexOf(c);
                b.texture = build3T;
                b.roof = roof4;
                b.roofH = 5;
            }
            if (build4.Contains(c))
            {
                int i = build4.IndexOf(c);
                b.texture = build4T;
                b.roof = roof1;
                b.roofH = 18;
            }
            if (build5.Contains(c))
            {
                int i = build5.IndexOf(c);
                b.texture = build5T;
                b.roof = roof2;
                b.roofH = 8;
            }

            if (build6.Contains(c))
            {
                int i = build6.IndexOf(c);
                b.texture = build6T;
                b.roof = roof3;
                b.roofH = 8;
            }

            //uses gameObject inheritence
            b.Position = new Vector3(position.X * blockSize.Width, 0, position.Y * blockSize.Height);
            b.size = blockSize;
            b.initialize();

            return b;
        }


        static Game theGame;
        public static void Initialize(Game game, Rectangle bS)
        {
            theGame = game;
            LoadBuildingTextures();
            blockSize = bS;
        }

        static void LoadBuildingTextures()
        {
            ContentManager contentManger = (ContentManager)theGame.Services.GetService(typeof(ContentManager));
            build1T = contentManger.Load<Texture2D>("building1");
            build2T = contentManger.Load<Texture2D>("building2");
            build3T = contentManger.Load<Texture2D>("building3");
            build4T = contentManger.Load<Texture2D>("building6");
            build5T = contentManger.Load<Texture2D>("building7");
            build6T = contentManger.Load<Texture2D>("building8");

            roof1 = contentManger.Load<Texture2D>("roof1");
            roof2 = contentManger.Load<Texture2D>("roof2");
            roof3 = contentManger.Load<Texture2D>("roof3");
            roof4 = contentManger.Load<Texture2D>("roof4");
        }
    }
}
