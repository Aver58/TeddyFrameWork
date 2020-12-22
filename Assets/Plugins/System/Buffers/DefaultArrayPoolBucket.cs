// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;

namespace System.Buffers
{
    internal sealed partial class DefaultArrayPool<T> : ArrayPool<T>
    {
        /// <summary>Provides a thread-safe bucket containing buffers that can be Rent'd and Return'd.</summary>
        private sealed class Bucket
        {
            internal readonly int _bufferLength;
            private readonly T[][] _buffers;
            private readonly int _poolId;

            private object _lock; // do not make this readonly; it's a mutable struct
            private int _index;

            /// <summary>
            /// Creates the pool with numberOfBuffers arrays where each buffer is of bufferLength length.
            /// </summary>
            internal Bucket(int bufferLength, int numberOfBuffers, int poolId)
            {
                _lock = new Object();
                _buffers = new T[numberOfBuffers][];
                _bufferLength = bufferLength;
                _poolId = poolId;
            }

            /// <summary>Gets an ID for the bucket to use with events.</summary>
            internal int Id
            {
                get
                {
                    return GetHashCode();
                }
            }

            /// <summary>Takes an array from the bucket.  If the bucket is empty, returns null.</summary>
            internal T[] Rent()
            {
                T[][] buffers = _buffers;
                T[] buffer = null;

                // While holding the lock, grab whatever is at the next available index and
                // update the index.  We do as little work as possible while holding the spin
                // lock to minimize contention with other threads.  The try/finally is
                // necessary to properly handle thread aborts on platforms which have them.
                bool allocateBuffer = false;
                try
                {
                    Monitor.Enter(_lock);

                    if (_index < buffers.Length)
                    {
                        buffer = buffers[_index];
                        buffers[_index++] = null;
                        allocateBuffer = buffer == null;
                    }
                }
                finally
                {
                    Monitor.Exit(_lock);
                }

                // While we were holding the lock, we grabbed whatever was at the next available index, if
                // there was one.  If we tried and if we got back null, that means we hadn't yet allocated
                // for that slot, in which case we should do so now.
                if (allocateBuffer)
                {
                    buffer = new T[_bufferLength];

                    var log = ArrayPoolEventSource.Log;

                    log.BufferAllocated(buffer.GetHashCode(), _bufferLength, _poolId, Id,
                        ArrayPoolEventSource.BufferAllocatedReason.Pooled);
                }

                return buffer;
            }

            /// <summary>
            /// Attempts to return the buffer to the bucket.  If successful, the buffer will be stored
            /// in the bucket and true will be returned; otherwise, the buffer won't be stored, and false
            /// will be returned.
            /// </summary>
            internal void Return(T[] array)
            {
                // Check to see if the buffer is the correct size for this bucket
                if (array.Length != _bufferLength)
                {
                    throw new ArgumentException("BufferNotFromPool","array");
                }

                // While holding the spin lock, if there's room available in the bucket,
                // put the buffer into the next available slot.  Otherwise, we just drop it.
                // The try/finally is necessary to properly handle thread aborts on platforms
                // which have them.
                try
                {
                    Monitor.Enter(_lock);

                    if (_index != 0)
                    {
                        _buffers[--_index] = array;
                    }
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }
    }
}
