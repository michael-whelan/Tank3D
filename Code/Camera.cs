using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CityShooter
{
    public class Camera
    {
        public Vector3 pos;
        public Vector3 dir;
        public Vector3 up;

        int viewType;
        int sky = 1;
        int fps = 2;
        int thirdP = 3;
        int free = 0;
        Vector3 tankPos;
        float fieldOfView;
        float aspectRatio;
        float near;
        float far;
        float yaw;
        Matrix view;
        BoundingSphere sphere;


        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }
        Matrix projection;

        public BoundingSphere Sphere
        {
            get { return sphere; }
        }

        public Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }

        float speed = 0.050f;
        float angVel = 0.0025f; // set max angular velocity for rotating camera

        public void Init(Vector3 p, Vector3 lookat, Vector3 u,float FOV,float ar,float n, float f)
        {
            pos = p;
            dir = (lookat - p);
            dir.Normalize();
            up = u;
            sphere.Radius = 2;
            fieldOfView = FOV;
            aspectRatio = ar;
            near = n;
            far = f;
            UpdateView();
            UpdateProj();
        }

        void UpdateView()
        {
            if (viewType == sky)
            {
                pos = new Vector3(tankPos.X, 100, tankPos.Z - 10);
                view = Matrix.CreateLookAt(pos, tankPos, up);
            }
            else if (viewType == fps)
            {
                Matrix rotationMatrix = Matrix.CreateRotationY(yaw);
                Vector3 transformedReference = Vector3.Transform(Vector3.Backward, rotationMatrix);

                pos = new Vector3(tankPos.X, tankPos.Y + 4f, tankPos.Z);
                Vector3 cameraLookat = pos + transformedReference;
                view = Matrix.CreateLookAt(pos, new Vector3(cameraLookat.X, cameraLookat.Y, cameraLookat.Z), up);
            }
            else if (viewType == thirdP)
            {
                //cameraPosition = Vector3.Transform(cameraPosition - cameraTarget, Matrix.CreateFromAxisAngle(axis, angle)) + cameraTarget;
                pos = new Vector3(tankPos.X, tankPos.Y + 6f, tankPos.Z - 20);
                pos = Vector3.Transform(pos - tankPos, Matrix.CreateFromAxisAngle(new Vector3(0,1,0),yaw)) + tankPos;
                view = Matrix.CreateLookAt(pos, new Vector3(tankPos.X,tankPos.Y+3f,tankPos.Z), up);
            }
            else if (viewType == free)
            {
                view = Matrix.CreateLookAt(pos, pos + dir, up);
            }
        }

        void UpdateProj()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio,near, far);
        }

        public void Update(GameTime gameTime,Vector3 tankP,float aimerYaw)
        {
            tankPos = tankP;
            yaw = aimerYaw;// -9.5f;
            sphere.Center = new Vector3(pos.X - (sphere.Radius / 2), pos.Y, pos.Z + (sphere.Radius / 2));

            KeyboardState kb = Keyboard.GetState();

            float distanceTravelled = speed * gameTime.ElapsedGameTime.Milliseconds;
            
            Vector3 right = Vector3.Cross(dir, up);

 
            if (kb.IsKeyDown(Keys.Left))
            {// pan
                pos -= right * distanceTravelled;
            }
            if (kb.IsKeyDown(Keys.Right))
            {// pan
                pos += right * distanceTravelled;
            }

            if (kb.IsKeyDown(Keys.Up))
            {// forward
                pos += dir * distanceTravelled;
            }
            if (kb.IsKeyDown(Keys.Down))
            {// Back
                pos -= dir * distanceTravelled;
            }
            if (kb.IsKeyDown(Keys.D1))
            {
                viewType = sky;
       
            }
            if (kb.IsKeyDown(Keys.D2))
            {
                viewType = fps;
            }
            if (kb.IsKeyDown(Keys.D3))
            {
                
                viewType = thirdP;
            }
            if (kb.IsKeyDown(Keys.D0))
            {
                viewType = free;
            }
            UpdateView();
            UpdateProj();
        }
    }
}
