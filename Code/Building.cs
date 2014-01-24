using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CityShooter
{
    class Building : GameObject
    {
        public Texture2D texture;

        float rotation;
        Matrix rotationMatrix;
        BoundingBox hitBox = new BoundingBox();
        VertexPositionNormalTexture[] vertices1;
        VertexPositionNormalTexture[] vertices2;
        VertexPositionNormalTexture[] vertices3;
        VertexPositionNormalTexture[] vertices4;

        VertexPositionNormalTexture[] roofVertices;

        public Texture2D roof;

        public float roofH;
        short[] indices;
        int numTriangles;
        int numVertices;
        public bool clipping = false;


        public Rectangle size;

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public BoundingBox HitBox
        {
            get { return hitBox; }
        }



        public Building(Game game)
            : base(game)
        {

        }

        public void initialize()
        {
            effect = new BasicEffect(graphicsDevice);
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            numVertices = 4;

            vertices1 = new VertexPositionNormalTexture[numVertices];
            vertices2 = new VertexPositionNormalTexture[numVertices];
            vertices3 = new VertexPositionNormalTexture[numVertices];
            vertices4 = new VertexPositionNormalTexture[numVertices];

            roofVertices = new VertexPositionNormalTexture[numVertices];

            vertices1[0].Position = new Vector3(position.X + (size.Width / 2), 0 + 7.5f, position.Z);
            vertices1[0].TextureCoordinate = new Vector2(0, 1);
            vertices1[1].Position = new Vector3(position.X + (size.Width / 2) + size.Width + roofH, size.Y + 7.5f, position.Z);
            vertices1[1].TextureCoordinate = new Vector2(0, 0);
            vertices1[2].Position = new Vector3(position.X + (size.Width / 2) + size.Width + roofH, size.Y + 7.5f, position.Z + size.Width);
            vertices1[2].TextureCoordinate = new Vector2(1, 0);
            vertices1[3].Position = new Vector3(position.X + (size.Width / 2), 0 + 7.5f, position.Z + size.Width);
            vertices1[3].TextureCoordinate = new Vector2(1, 1);


            vertices2[0].Position = new Vector3(position.X + (size.Width / 2), -7.5f, position.Z);
            vertices2[0].TextureCoordinate = new Vector2(0, 1);
            vertices2[1].Position = new Vector3(position.X + (size.Width / 2) + size.Width + roofH, size.Y - 7, position.Z);
            vertices2[1].TextureCoordinate = new Vector2(0, 0);
            vertices2[2].Position = new Vector3(position.X + size.Width + (size.Width / 2) + roofH, size.Y - 7, position.Z + size.Width);
            vertices2[2].TextureCoordinate = new Vector2(1, 0);
            vertices2[3].Position = new Vector3(position.X + (size.Width / 2), 0 - 7.5f, position.Z + size.Width);
            vertices2[3].TextureCoordinate = new Vector2(1, 1);


            vertices3[0].Position = new Vector3(position.X + (size.Width / 2), 0 + 7.5f, position.Z);
            vertices3[0].TextureCoordinate = new Vector2(0, 1);
            vertices3[1].Position = new Vector3(position.X + (size.Width / 2) + size.Width + roofH, size.Y + 7.5f, position.Z);
            vertices3[1].TextureCoordinate = new Vector2(0, 0);
            vertices3[2].Position = new Vector3(position.X + size.Width + (size.Height / 2) + roofH, size.Y - 7, position.Z);
            vertices3[2].TextureCoordinate = new Vector2(1, 0);
            vertices3[3].Position = new Vector3(position.X + (size.Width / 2), 0 - 7.5f, position.Z);
            vertices3[3].TextureCoordinate = new Vector2(1, 1);



            vertices4[0].Position = new Vector3(position.X + (size.Width / 2), 0 - 7.5f, position.Z + size.Width);
            vertices4[0].TextureCoordinate = new Vector2(0, 1);
            vertices4[1].Position = new Vector3(position.X + size.Width + (size.Height / 2) + roofH, size.Y - 7, position.Z + size.Width);
            vertices4[1].TextureCoordinate = new Vector2(0, 0);
            vertices4[2].Position = new Vector3(position.X + (size.Width / 2) + size.Width + roofH, size.Y + 7.5f, position.Z + size.Width);
            vertices4[2].TextureCoordinate = new Vector2(1, 0);
            vertices4[3].Position = new Vector3(position.X + (size.Width / 2), 0 + 7.5f, position.Z + size.Width);
            vertices4[3].TextureCoordinate = new Vector2(1, 1);



            roofVertices[0].Position = new Vector3(position.X + (size.Width / 2) + 14.9f + roofH, 0 + 7.5f, position.Z + size.Width);
            roofVertices[0].TextureCoordinate = new Vector2(0, 0);

            roofVertices[1].Position = new Vector3(position.X + (size.Width / 2) + 14.9f + roofH, 0 - 7.5f, position.Z + size.Width);
            roofVertices[1].TextureCoordinate = new Vector2(1, 0);

            roofVertices[2].Position = new Vector3(position.X + (size.Width / 2) + 14.9f + roofH, -7.5f, position.Z);
            roofVertices[2].TextureCoordinate = new Vector2(1, 1);

            roofVertices[3].Position = new Vector3(position.X + (size.Width / 2) + 14.9f + roofH, 0 + 7.5f, position.Z);
            roofVertices[3].TextureCoordinate = new Vector2(0, 1);

            numTriangles = 2;
            indices = new short[numTriangles + 2];

            int i = 0;
            indices[i++] = 0;
            indices[i++] = 1;
            indices[i++] = 3;
            indices[i++] = 2;

            Vector3 centreDisplace = position + new Vector3(size.Width / 2.0f, 0, size.Height / 2.0f);

            rotationMatrix = Matrix.CreateTranslation(-centreDisplace) * Matrix.CreateRotationZ(1.57f) * Matrix.CreateTranslation(centreDisplace);

            Collision();
        }

        public void Collision()
        {
            hitBox.Min = new Vector3(position.X - 2, position.Y, position.Z + 1);
            hitBox.Max = new Vector3(position.X + (size.Height / 2) + 6.5f, 0 - 7.5f + roofH + 25, position.Z + size.Height + 1);
        }

        public void Draw( Camera camera)
        {

            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.World = rotationMatrix;
            effect.Texture = texture;


            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (!clipping)
                {
                     graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, vertices1, 0, numVertices, indices, 0, numTriangles);
                     graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, vertices2, 0, numVertices, indices, 0, numTriangles);
                     graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, vertices3, 0, numVertices, indices, 0, numTriangles);
                     graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, vertices4, 0, numVertices, indices, 0, numTriangles);
                
                }
            }
            DrawRoof();
        }

        public void DrawRoof()
        {
            effect.Texture = roof;
             foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if(!clipping)
                graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, roofVertices, 0, numVertices, indices, 0, numTriangles);
            }
        }
    }
}

