//Justin Saletta 38006614
//Jonathan Stevens 61356189

using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Swarch {
public class ThreadSock : MonoBehaviour {
	private NetworkStream nws;
	private byte[] streamBuffer;
	private byte byteBuffer;
	private byte tempBuffer;
	private Sockets socks;

	public ThreadSock (NetworkStream nwsIn, Sockets inSocket) {
		nws = nwsIn;
		socks = inSocket;
	}
	
	public void setStuff(NetworkStream nwsIn, Sockets inSocket) {
		nws = nwsIn;
		socks = inSocket;
	}


	public void Service() {	
		try {
		 	while (socks.connected) {
				byte[] data = new byte[socks.client.ReceiveBufferSize];
				Int32 bytes = nws.Read(data,0,data.Length);
				lock (socks.recvBuffer) {
					StringBuilder str = new StringBuilder();
					str.Append(Encoding.UTF8.GetString(data,0,bytes));
					string stri = str.ToString();
					string[] s = stri.Split(new char[]{';'});
					for (int n=0;n<s.Length-1;n++) {
						socks.recvBuffer.Enqueue(s[n]);
					}
				}
			}
		}
		catch (Exception ex) {
			print (ex.Message + " : Thread loop");
			
		}
		
	}
	
}
}