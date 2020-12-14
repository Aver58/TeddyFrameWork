#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    CRC32.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/10 16:21:13
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

sealed class CRC32 : HashAlgorithm
{
    private const UInt32 DefaultPolynomial = 0xedb88320u;
    private const UInt32 DefaultSeed = 0xffffffffu;

    static UInt32[] _defaultTable;

    readonly UInt32 _seed;
    readonly UInt32[] _table;
    UInt32 _hash;

    public CRC32()
        : this(DefaultPolynomial, DefaultSeed)
    {
    }

    public CRC32(UInt32 polynomial, UInt32 seed)
    {
        if(!BitConverter.IsLittleEndian)
            throw new PlatformNotSupportedException("Not supported on Big Endian processors");

        _table = InitializeTable(polynomial);
        this._seed = _hash = seed;
    }

    public override void Initialize()
    {
        _hash = _seed;
    }

    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        _hash = CalculateHash(_table, _hash, array, ibStart, cbSize);
    }

    protected override byte[] HashFinal()
    {
        var hashBuffer = UInt32ToBigEndianBytes(~_hash);
        HashValue = hashBuffer;
        return hashBuffer;
    }

    public override int HashSize
    {
        get { return 32; }
    }

    public static UInt32 Compute(byte[] buffer)
    {
        return Compute(DefaultSeed, buffer);
    }

    public static UInt32 Compute(UInt32 seed, byte[] buffer)
    {
        return Compute(DefaultPolynomial, seed, buffer);
    }

    public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
    {
        return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
    }

    static UInt32[] InitializeTable(UInt32 polynomial)
    {
        if(polynomial == DefaultPolynomial && _defaultTable != null)
            return _defaultTable;

        var createTable = new UInt32[256];
        for(var i = 0; i < 256; i++)
        {
            var entry = (UInt32)i;
            for(var j = 0; j < 8; j++)
                if((entry & 1) == 1)
                    entry = (entry >> 1) ^ polynomial;
                else
                    entry >>= 1;
            createTable[i] = entry;
        }

        if(polynomial == DefaultPolynomial)
            _defaultTable = createTable;

        return createTable;
    }

    static UInt32 CalculateHash(UInt32[] table, UInt32 seed, IList<byte> buffer, int start, int size)
    {
        var hash = seed;
        for(var i = start; i < start + size; i++)
            hash = (hash >> 8) ^ table[buffer[i] ^ hash & 0xff];
        return hash;
    }

    static byte[] UInt32ToBigEndianBytes(UInt32 uint32)
    {
        var result = BitConverter.GetBytes(uint32);

        if(BitConverter.IsLittleEndian)
            Array.Reverse(result);

        return result;
    }
}