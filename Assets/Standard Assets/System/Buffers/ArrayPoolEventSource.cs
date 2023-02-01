// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace System.Buffers
{
    internal sealed class ArrayPoolEventSource
    {
        internal static readonly ArrayPoolEventSource Log = new ArrayPoolEventSource();

        /// <summary>The reason for a BufferAllocated event.</summary>
        internal enum BufferAllocatedReason : int
        {
            /// <summary>The pool is allocating a buffer to be pooled in a bucket.</summary>
            Pooled,
            /// <summary>The requested buffer size was too large to be pooled.</summary>
            OverMaximumSize,
            /// <summary>The pool has already allocated for pooling as many buffers of a particular size as it's allowed.</summary>
            PoolExhausted
        }

        /// <summary>
        /// Event for when a buffer is rented.  This is invoked once for every successful call to Rent,
        /// regardless of whether a buffer is allocated or a buffer is taken from the pool.  In a
        /// perfect situation where all rented buffers are returned, we expect to see the number
        /// of BufferRented events exactly match the number of BuferReturned events, with the number
        /// of BufferAllocated events being less than or equal to those numbers (ideally significantly
        /// less than).
        /// </summary>
        internal unsafe void BufferRented(int bufferId, int bufferSize, int poolId, int bucketId)
        {
            //Debug.LogFormat("BufferRented(bufferId:{0},bufferSize:{1},poolId:{2},bucketId:{3})", bufferId, bufferSize, poolId, bucketId);
        }

        /// <summary>
        /// Event for when a buffer is allocated by the pool.  In an ideal situation, the number
        /// of BufferAllocated events is significantly smaller than the number of BufferRented and
        /// BufferReturned events.
        /// </summary>
        internal unsafe void BufferAllocated(int bufferId, int bufferSize, int poolId, int bucketId, BufferAllocatedReason reason)
        {
            //Debug.LogFormat("BufferAllocated(bufferId:{0},bufferSize:{1},poolId:{2},bucketId:{3},reason:{4})", bufferId, bufferSize, poolId, bucketId,reason);
        }

        /// <summary>
        /// Event raised when a buffer is returned to the pool.  This event is raised regardless of whether
        /// the returned buffer is stored or dropped.  In an ideal situation, the number of BufferReturned
        /// events exactly matches the number of BufferRented events.
        /// </summary>
        internal void BufferReturned(int bufferId, int bufferSize, int poolId)
        {
            //Debug.LogFormat("BufferReturned(bufferId:{0},bufferSize:{1},poolId:{2})", bufferId, bufferSize, poolId);
        }
    }
}
