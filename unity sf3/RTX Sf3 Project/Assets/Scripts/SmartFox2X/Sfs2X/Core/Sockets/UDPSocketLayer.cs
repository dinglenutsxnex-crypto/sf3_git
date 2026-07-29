using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Sfs2X.Bitswarm;
using Sfs2X.Logging;

namespace Sfs2X.Core.Sockets
{
	public class UDPSocketLayer : ISocketLayer
	{
		private Logger log;

		private BitSwarmClient bitSwarm;

		private int socketPollSleep;

		private int socketNumber;

		private IPAddress ipAddress;

		private UdpClient connection;

		private IPEndPoint sender;

		private volatile bool isDisconnecting = false;

		private Thread thrSocketReader;

		private byte[] byteBuffer;

		private bool connected = false;

		private OnDataDelegate onData = null;

		private OnErrorDelegate onError = null;

		public bool IsConnected
		{
			get
			{
				return connected;
			}
		}

		public OnDataDelegate OnData
		{
			get
			{
				return onData;
			}
			set
			{
				onData = value;
			}
		}

		public OnErrorDelegate OnError
		{
			get
			{
				return onError;
			}
			set
			{
				onError = value;
			}
		}

		public ConnectionDelegate OnConnect
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public ConnectionDelegate OnDisconnect
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public bool RequiresConnection
		{
			get
			{
				return false;
			}
		}

		public int SocketPollSleep
		{
			get
			{
				return socketPollSleep;
			}
			set
			{
				socketPollSleep = value;
			}
		}

		public UDPSocketLayer(SmartFox sfs)
		{
			if (sfs != null)
			{
				log = sfs.Log;
				bitSwarm = sfs.BitSwarm;
			}
		}

		private void LogWarn(string msg)
		{
			if (log != null)
			{
				log.Warn("UDPSocketLayer: " + msg);
			}
		}

		private void LogError(string msg)
		{
			if (log != null)
			{
				log.Error("UDPSocketLayer: " + msg);
			}
		}

		private void HandleError(string err, SocketError se)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["err"] = err;
			hashtable["se"] = se;
			bitSwarm.ThreadManager.EnqueueCustom(HandleErrorCallback, hashtable);
		}

		private void HandleError(string err)
		{
			HandleError(err, SocketError.NotSocket);
		}

		private void HandleErrorCallback(object state)
		{
			Hashtable hashtable = state as Hashtable;
			string msg = (string)hashtable["err"];
			SocketError se = (SocketError)hashtable["se"];
			if (!isDisconnecting)
			{
				CloseConnection();
				LogError(msg);
				CallOnError(msg, se);
			}
		}

		private void WriteSocket(byte[] buf)
		{
			try
			{
				connection.Send(buf, buf.Length);
			}
			catch (SocketException ex)
			{
				string err = "Error writing to socket: " + ex.Message;
				HandleError(err, ex.SocketErrorCode);
			}
			catch (Exception ex2)
			{
				string err = "General error writing to socket: " + ex2.Message + " " + ex2.StackTrace;
				HandleError(err);
			}
		}

		private static void Sleep(int ms)
		{
			Thread.Sleep(10);
		}

		private void Read()
		{
			connected = true;
			while (connected)
			{
				try
				{
					if (socketPollSleep > 0)
					{
						Sleep(socketPollSleep);
					}
					byteBuffer = connection.Receive(ref sender);
					if (byteBuffer != null && byteBuffer.Length != 0)
					{
						HandleBinaryData(byteBuffer);
					}
				}
				catch (SocketException ex)
				{
					HandleError("Error reading data from socket: " + ex.Message, ex.SocketErrorCode);
				}
				catch (ThreadAbortException)
				{
					break;
				}
				catch (Exception ex3)
				{
					HandleError("General error reading data from socket: " + ex3.Message + " " + ex3.StackTrace);
				}
			}
		}

		private void HandleBinaryData(byte[] buf)
		{
			CallOnData(buf);
		}

		public void Connect(IPAddress adr, int port)
		{
			socketNumber = port;
			ipAddress = adr;
			try
			{
				connection = new UdpClient(ipAddress.ToString(), socketNumber);
				sender = new IPEndPoint(IPAddress.Any, 0);
				thrSocketReader = new Thread(Read);
				thrSocketReader.Start();
			}
			catch (SocketException ex)
			{
				string err = "Connection error: " + ex.Message + " " + ex.StackTrace;
				HandleError(err, ex.SocketErrorCode);
			}
			catch (Exception ex2)
			{
				string err = "General exception on connection: " + ex2.Message + " " + ex2.StackTrace;
				HandleError(err);
			}
		}

		public void Disconnect()
		{
			isDisconnecting = true;
			CloseConnection();
			isDisconnecting = false;
		}

		public void Disconnect(string reason)
		{
		}

		private void CloseConnection()
		{
			try
			{
				connection.Client.Shutdown(SocketShutdown.Both);
				connection.Close();
			}
			catch (Exception)
			{
			}
			connected = false;
		}

		public void Kill()
		{
			throw new NotSupportedException();
		}

		private void CallOnData(byte[] data)
		{
			if (onData != null)
			{
				bitSwarm.ThreadManager.EnqueueDataCall(onData, data);
			}
		}

		private void CallOnError(string msg, SocketError se)
		{
			if (onError != null)
			{
				onError(msg, se);
			}
		}

		public void Write(byte[] data)
		{
			WriteSocket(data);
		}
	}
}
