using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.PInvoke
{
	internal abstract class BaseReferenceHolder : IDisposable
	{
		private HandleRef mSelfPointer;

		public BaseReferenceHolder(IntPtr pointer)
		{
			mSelfPointer = PInvokeUtilities.CheckNonNull(new HandleRef(this, pointer));
		}

		protected bool IsDisposed()
		{
			return PInvokeUtilities.IsNull(mSelfPointer);
		}

		protected HandleRef SelfPtr()
		{
			if (IsDisposed())
			{
				throw new InvalidOperationException("Attempted to use object after it was cleaned up");
			}
			return mSelfPointer;
		}

		protected abstract void CallDispose(HandleRef selfPointer);

		~BaseReferenceHolder()
		{
			Dispose(true);
		}

		public void Dispose()
		{
			Dispose(false);
			GC.SuppressFinalize(this);
		}

		internal IntPtr AsPointer()
		{
			return SelfPtr().Handle;
		}

		private void Dispose(bool fromFinalizer)
		{
			if (!PInvokeUtilities.IsNull(mSelfPointer))
			{
				CallDispose(mSelfPointer);
				mSelfPointer = new HandleRef(this, IntPtr.Zero);
			}
		}
	}
}
