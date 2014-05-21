using UnityEngine;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System;
using System.Diagnostics;
using System.Threading;

namespace Swarch {
public class Sockets : MonoBehaviour {
	
	
	public string SERVER_LOCATION = "128.195.11.138"; 
	const int SERVER_PORT = 7589;
	
	public TcpClient client; 
	
	public NetworkStream nws;
	public int clientNumber;
	public bool startGame;
	public bool connected;
	
	public DateTime dt;
	
	public Thread t = null; 
	
	protected static bool threadState = false;
	
	public Queue recvBuffer;
	
	
	public Sockets() {
		
		connected = false;
		recvBuffer = new Queue();
		
		//uniClock = new Stopwatch();
		//dt = NTPTime.getNTPTime (dt, ref uniClock);
		
	}
	
	public bool Connect() {
		//********* COMPLETE THE FOLLOWING CODE
		//********* ESTABLISH CONNECTION THEN MAKE THREAD TO READ BYTES FROM STREAM
		try {
			client = new TcpClient(SERVER_LOCATION,SERVER_PORT);
			nws = client.GetStream();
			t = new Thread(new ThreadStart(DoThread));
			t.Start();
			connected = true;
		}
		catch (Exception ex) {
			print (ex.Message + " : OnConnect");
			
		}
		
		if (client == null) return false;
		return client.Connected;
	}
	
	public bool Disconnect() {
		//********* COMPLETE THE FOLLOWING CODE
		
		try {	
			nws.Close();
			client.Close();
			connected = false;
		}
		catch (Exception ex) {
			print (ex.Message + " : OnDisconnect");
			return false;
			
		}
		return true;
	}
	
	public void SendTCPPacket (byte[] toSend) {
		//********* COMPLETE THE FOLLOWING CODE
		try {	
			nws.Write (toSend,0,toSend.Length);
		}
		catch (Exception ex) {
			print (ex.Message + ": OnTCPPacket");
		}	
	}
	
	public void measureLatency() { //UN-NECESSARY
	}
	
	public int returnLatency(){//UN-NECESSARY
		//return latency;
		return 0;
	}
	
	
	public void endThread(){
		threadState = false;
	}
	
	public void testThread() {
		
		try {
			if (t!= null && !threadState) {
				print ("thread aborted");
				t.Abort();
				threadState = !threadState;	
			}
		}
		catch (Exception ex) {
			print (ex.Message + " : testThread ");
		}
	}
	
	public void DoThread() {
		
		//	ThreadSock th = (ThreadSock)gameObject.AddComponent("ThreadSock");
		//	th.setStuff(nws,this);
		ThreadSock th = new ThreadSock(nws,this);
		th.Service();
	}	
}
}