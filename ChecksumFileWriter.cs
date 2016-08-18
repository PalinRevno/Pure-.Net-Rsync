#region Disclaimer / License
// Copyright (C) 2011, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.IO;
using System.Security.Cryptography;

namespace OvermindRsync
{
    /// <summary>
    /// This class writes signature files in the same format as RDiff
    /// </summary>
    internal static class ChecksumFileWriter
    {
        public static void WriteCommonHeader(Signature signature)
        {
            // Writing the initial header
            signature.Output.Write(RDiffBinary.SIGNATURE_MAGIC, 0, RDiffBinary.SIGNATURE_MAGIC.Length);
            signature.Output.Write(RDiffBinary.FixEndian(BitConverter.GetBytes(signature.BlockLength)), 0, 4);
            signature.Output.Write(RDiffBinary.FixEndian(BitConverter.GetBytes(signature.StrongLength)), 0, 4);
        }

        /// <summary>
        /// Adds all checksums in an entire stream 
        /// </summary>
        /// <param name="sourceData">The stream to read checksums from</param>
        /// <param name="signature">Holds the signature data parameters</param>
        public static void GenerateSignatureData(Signature signature, Stream sourceData)
        {
            byte[] buffer;
            int readBytes;

            buffer = new byte[signature.BlockLength];
            
            while ((readBytes = Utility.ForceStreamRead(sourceData, buffer, buffer.Length)) != 0)
            {
                AddChunk(signature, buffer, 0, readBytes);
            }
        }

        /// <summary>
        /// Adds a chunck of data to checksum list
        /// </summary>
        /// <param name="buffer">The data to add a checksum entry for</param>
        /// <param name="index">The index in the buffer to start reading from</param>
        /// <param name="count">The number of bytes to extract from the array</param>
        private static void AddChunk(Signature signature,byte[] buffer, int index, int count)
        {
            //Add weak checksum (Adler-32) to the chunk
            signature.Output.Write(RDiffBinary.FixEndian(BitConverter.GetBytes(Adler32Checksum.Calculate(buffer, index, count))), 0, 4);

            //Add strong checksum
            signature.Output.Write(Utility.Hash.ComputeHash(buffer, index, count), 0, signature.StrongLength);
        }
    }
}
