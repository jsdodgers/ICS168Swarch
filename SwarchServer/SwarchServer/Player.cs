using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace SwarchServer
{
    class Player
    {
        public static System.Random rand = new Random();
        public Stopwatch stopWatch;
        public long lastTime, currentTime;
        public string playerName;
        public int playerNumber;
        public Rectangle playerRect;
        public int dir;
        public int score = 0;
        public float size, x, y, speedX, speedY;
        private float maxSpeed = 4.0f;
        public bool isDead = false;

        private bool isConnected = true;
        public GameState gs;
        private Thread mThread;
        private NetworkStream mStream;
        private TcpClient mClient;
        public Queue<Command> writeQueue;
        public Queue<Command> readQueue;
        public Queue<Command> readQueueGameState;

        public Player(TcpClient client, NetworkStream stream, int number)
        {
            playerNumber = number;
            mClient = client;
            mStream = stream;
            stopWatch = Stopwatch.StartNew();
            lastTime = stopWatch.ElapsedMilliseconds;
            writeQueue = new Queue<Command>();
            readQueue = new Queue<Command>();
            readQueueGameState = new Queue<Command>();
            mThread = new Thread(new ThreadStart(netUpdate));
            resetPosition();

            playerRect = new Rectangle((int)(x * 10), (int)(y * 10), (int)(Math.Sqrt(size * 10)), (int)(Math.Sqrt(size * 10)));
            playerRect.X -= playerRect.Width / 2;
            playerRect.Y -= playerRect.Height / 2;
            mThread.Start();
        }

        public void setPosition(int ndir, float nx, float ny)
        {
            //size = 1;
            System.Random rand = new Random();
            dir = ndir;
            x = nx;
            y = ny;
            playerRect.X = (int)(nx * 10) - playerRect.Width / 2;
            playerRect.Y = (int)(ny * 10) - playerRect.Height / 2;
            speedX = (dir % 2 == 1 ? getCurrentSpeed() * (dir - 2) : 0);
            speedY = (dir % 2 == 0 ? getCurrentSpeed() * (dir - 3) : 0);
            //Console.WriteLine("SPEEDX: " + speedX + "  SPEEDY: " + speedY);
        }

        public void resetPosition()
        {
            size = 1;
            dir = rand.Next(1, 5);
            x = rand.Next(-30, 30) / 10.0f;
            y = rand.Next(-30, 30) / 10.0f;
            speedX = (dir % 2 == 1 ? getCurrentSpeed() * (dir - 2) : 0);
            speedY = (dir % 2 == 0 ? getCurrentSpeed() * (dir - 3) : 0);
        }

        public void increaseSize(float s)
        {
            size += s;
            playerRect.Width = (int)(Math.Sqrt(size * 10));
            playerRect.Height = (int)(Math.Sqrt(size * 10));
            playerRect.X = (int)(x * 10) - playerRect.Width / 2;
            playerRect.Y = (int)(y * 10) - playerRect.Height / 2;
        }

        public void sendCommand(Command c)
        {
            writeQueue.Enqueue(c);
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

        float getCurrentSpeed()
        {
            return maxSpeed / (float)Math.Atan(size);
        }

        public void disconnect()
        {
            if (mClient.Connected || isConnected)
            {
                mThread.Abort();
                isConnected = false;
                Thread.Sleep(1000);
                mStream.Close(20);
                mClient.Close();
                GameManager.removePlayer(this);
                if(gs != null)
                {
                    gs.removePlayer(this);
                }
                //Thread.Sleep(1000);
            }
        }

        private void netUpdate()
        {
            while (true)
            {
                if (!mClient.Connected || !isConnected)
                {
                    disconnect();
                    break;
                }

                if (gs != null && gs.gameStarted)
                {
                    currentTime = stopWatch.ElapsedMilliseconds;
                    long deltaTime = currentTime - lastTime;
                    x += deltaTime * speedX/1000;
                    y += deltaTime * speedY/1000;
                    playerRect.X = (int)(x * 10) - playerRect.Width/2;
                    playerRect.Y = (int)(y * 10) - playerRect.Height/2;
                    lastTime = currentTime;

                    foreach(Pellet pellet in gs.pelletList)
                    {
                        Rectangle r = Rectangle.Intersect(pellet.pelletRect, playerRect);
                        if (!r.IsEmpty)
                        {
                            gs.eatPellet(pellet.id, this);
                        }
                    }

                    if (!isDead && (playerRect.X + playerRect.Width >= 70 || playerRect.X <= -70 || playerRect.Y + playerRect.Height >= 40 || playerRect.Y <= -40))
                    {
                        isDead = true;
                        gs.death(this);
                    }
                }
                
                if (mStream.DataAvailable)
                {
                    byte[] bytes = new byte[mClient.ReceiveBufferSize];
                    Int32 cmdLength = mStream.Read(bytes, 0, bytes.Length);
                    StringBuilder sBuilder = new StringBuilder();
                    sBuilder.Append(Encoding.UTF8.GetString(bytes, 0, cmdLength));
                    string str = sBuilder.ToString();
                    string[] stra = str.Split(new char[] { ';' });
                    for(int i = 0; i < stra.Length - 1; i++)
                    {
                        lock(readQueue)
                        {
                            Command tempComm = Command.unwrap(stra[i]);
                            readQueue.Enqueue(tempComm);
                            readQueueGameState.Enqueue(tempComm);
                        }
                    }
                }

                lock (writeQueue)
                {
                    while(writeQueue.Count() > 0)
                    {
                        Command newCommand = writeQueue.Dequeue();
                        if (newCommand!=null) {
                            string message = newCommand.message;
                            byte[] bytes = Encoding.UTF8.GetBytes(message);
                            mStream.Write(bytes, 0, bytes.Length);
                            mStream.Flush();
                            if (newCommand.cType == CType.EatPlayer || newCommand.cType == CType.Death)
                            {
                                isDead = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
