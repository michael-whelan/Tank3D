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
    class CollisionManager
    {


        public bool SphereVsBox(BoundingSphere sphere, BoundingBox box)
        {
            if (sphere.Intersects(box))
            {
                return true;
            }
            else { return false; }
        }

        public bool SphereVsShere(BoundingSphere sphere1,BoundingSphere sphere2)
        {
            if (sphere1.Intersects(sphere2))
            {
                return true;
            }
            else { return false; }
        }

    }
}
