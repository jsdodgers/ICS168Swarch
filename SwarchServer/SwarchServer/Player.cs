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
        public string playerName;
        public int playerNumber;
        private int score = 0;
        private float size, x, y;
        private bool ready = false;

        private bool isConnected = true;
        public GameState gs;
        private Thread mThread;
        private NetworkStream mStream;
        private TcpClient mClient;
        public Queue<Command> writeQueue;
        public Queue<Command> readQueue;

        public Player(TcpClient client, NetworkStream stream, int number)
        {
            playerNumber = number;
            mClient = client;
            mStream = stream;
            writeQueue = new Queue<Command>();
            readQueue = new Queue<Command>();
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
                            readQueue.Enqueue(Command.unwrap(stra[i]));
                        }
                    }
                }

                if (writeQueue.Count() > 0)
                {
                    lock (writeQueue)
                    {
                        Command newCommand = writeQueue.Dequeue();
                        string message = newCommand.message;
                        if(message == null)
                        {
                            
                        }
                        byte[] bytes = Encoding.UTF8.GetBytes(message);
                        mStream.Write(bytes, 0, bytes.Length);
                        mStream.Flush();
                    }
                }
            }
        }
    }
}
