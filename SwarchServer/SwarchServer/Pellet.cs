using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SwarchServer
{
    class Pellet
    {
        public static System.Random rand = new Random();
        public int id;
        public float x, y, size;
        public Rectangle pelletRect;

        public Pellet(int mid)
        {
            id = mid;
            size = rand.Next(1, 5)*0.1f;
            x = rand.Next(-67, 67) / 10.0f;
            y = rand.Next(-30, 30) / 10.0f;
            pelletRect = new Rectangle((int)(x * 10), (int)(y * 10), (int)(size * 10 + 1), (int)(size * 10 + 1));
        }
    }
}
