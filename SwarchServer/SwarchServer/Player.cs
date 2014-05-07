using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SwarchServer
{
    class Player
    {
        private string playerName;
        private int playerNumber;
        private int score = 0;
        private float size, x, y;
        private bool ready = false;
        
        private Thread mThread;
        private NetworkStream mStream;
        private TcpClient mClient;
        private Queue<Command> writeQueue;
        private Queue<Command> readQueue;

        public Player(TcpClient client, NetworkStream stream, int number)
        {
            playerNumber = number;
            mClient = client;
            mStream = stream;
            writeQueue = new Queue<Command>();
            writeQueue = new Queue<Command>();
            mThread = new Thread(new ThreadStart(netUpdate));
            setResetPosition();

            mThread.Start();
        }

        public void setResetPosition()
        {
            size = 1;
            System.Random rand = new Random();
            int dir = rand.Next(1, 5);
            x = rand.Next(-30, 30) / 10.0f;
            y = rand.Next(-30, 30) / 10.0f;
        }

        public float getX()
        {
            return x;
        }

        public float getY()
        {
            return y;
        }

        public float getSize()
        {
            return size;
        }

        public int getScore()
        {
            return score;
        }

        private void netUpdate()
        {
            /*while (false)   // disabled the loop for now
            {
                // read one update from queue
                if (mStream.DataAvailable)
                {
                    // read and parse a command from the stream
                }
                // write one update to stream
                if (commandQueue.Count() > 0)
                {
                    // write a command (just one) out to the stream
                }
            }*/
        }
    }
}
