
using System;
using System.Runtime.InteropServices;

namespace Framework.Network
{
	#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA_10_0
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int KcpOutput(IntPtr buf, int len, IntPtr kcp, IntPtr user);
	#else
	public delegate int KcpOutput(IntPtr buf, int len, IntPtr kcp, IntPtr user);
	#endif
	

	public class CKcp
	{
	#if UNITY_IPHONE && !UNITY_EDITOR
		const string KcpDLL = "__Internal";
	#else
		const string KcpDLL = "kcp";
	#endif

		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern uint ikcp_check(IntPtr kcp, uint current);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern IntPtr ikcp_create(uint conv, IntPtr user);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ikcp_flush(IntPtr kcp);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern uint ikcp_getconv(IntPtr ptr);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ikcp_input(IntPtr kcp, byte[] data, int size);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ikcp_nodelay(IntPtr kcp, int nodelay, int interval, int resend, int nc);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ikcp_peeksize(IntPtr kcp);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ikcp_recv(IntPtr kcp, byte[] buffer, int len);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ikcp_release(IntPtr kcp);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ikcp_send(IntPtr kcp, byte[] buffer, int len);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ikcp_setmtu(IntPtr kcp, int mtu);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ikcp_setoutput(IntPtr kcp, IntPtr output);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ikcp_update(IntPtr kcp, uint current);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ikcp_waitsnd(IntPtr kcp);
		[DllImport(KcpDLL, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ikcp_wndsize(IntPtr kcp, int sndwnd, int rcvwnd);
		
		public static uint KcpCheck(IntPtr kcp, uint current)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_check(kcp, current);
		}
		
		public static IntPtr KcpCreate(uint conv, IntPtr user)
		{
			return ikcp_create(conv, user);
		}

		public static void KcpFlush(IntPtr kcp)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			ikcp_flush(kcp);
		}

		public static uint KcpGetconv(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_getconv(ptr);
		}

		public static int KcpInput(IntPtr kcp, byte[] data, int size)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_input(kcp, data, size);
		}

		public static int KcpNodelay(IntPtr kcp, int nodelay, int interval, int resend, int nc)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_nodelay(kcp, nodelay, interval, resend, nc);
		}

		public static int KcpPeeksize(IntPtr kcp)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_peeksize(kcp);
		}

		public static int KcpRecv(IntPtr kcp, byte[] buffer, int len)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_recv(kcp, buffer, len);
		}

		public static void KcpRelease(IntPtr kcp)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			ikcp_release(kcp);
		}

		public static int KcpSend(IntPtr kcp, byte[] buffer, int len)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_send(kcp, buffer, len);
		}

		public static int KcpSetmtu(IntPtr kcp, int mtu)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_setmtu(kcp, mtu);
		}

		public static void KcpSetoutput(IntPtr kcp, KcpOutput output)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			IntPtr fn = Marshal.GetFunctionPointerForDelegate(output);
			ikcp_setoutput(kcp, fn);
		}

		public static void KcpUpdate(IntPtr kcp, uint current)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			ikcp_update(kcp, current);
		}

		public static int KcpWaitsnd(IntPtr kcp)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_waitsnd(kcp);
		}

		public static int KcpWndsize(IntPtr kcp, int sndwnd, int rcvwnd)
		{
			if (kcp == IntPtr.Zero)
			{
				throw new Exception("kcp error, kcp point is zero");
			}
			return ikcp_wndsize(kcp, sndwnd, rcvwnd);
		}
	}
}

