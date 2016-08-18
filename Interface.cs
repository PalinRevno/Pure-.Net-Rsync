#region Disclaimer / License
// Copyright (C) 2011, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
//
// Copyright (C) 2016, Pavel Ronin
// https://github.com/PalinRevno
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

namespace OvermindRsync
{
    /// <summary>
    /// This class wraps the usage of OvermindRsync into an easy to use static class
    /// </summary>
    public class RsyncInterface
    {

        /// <summary>
        /// Generates a signature from a stream, and writes it to another stream
        /// </summary>
        /// <param name="input">The stream to create the signature for</param>
        /// <param name="output">The stream to write the signature into</param>
        public static void GenerateSignature(Stream input, Stream output)
        {
            Signature signatureModel;

            // Generating the object model to store the specific signature parameters and stream
            signatureModel = new Signature(output);

            ChecksumFileWriter.WriteCommonHeader(signatureModel);
            ChecksumFileWriter.GenerateSignatureData(signatureModel, input);
        }

        /// <summary>
        /// Generates a signature file from existing file, and writes it to another file
        /// </summary>
        /// <param name="filename">The file to create the signature for</param>
        /// <param name="outputfile">The file write the signature to</param>
        public static void GenerateSignature(string filename, string outputfile)
        {
            if (File.Exists(outputfile))
            {
                File.Delete(outputfile);
            }

            using (FileStream fs1 = File.OpenRead(filename))
            {
                using (FileStream fs2 = File.Create(outputfile))
                {
                    GenerateSignature(fs1, fs2);
                }
            }
        }


        /// <summary>
        /// Generates a signature file from existing file, and write it to a given stream
        /// </summary>
        /// <param name="filename">The file to create the signature for</param>
        /// <param name="outputfile">The file write the signature to</param>
        public static void GenerateSignature(string filename, Stream outputStream)
        {
            using (FileStream fs1 = File.OpenRead(filename))
            {
                GenerateSignature(fs1, outputStream);
            }
        }


        /// <summary>
        /// Generates a delta stream
        /// </summary>
        /// <param name="signature">The signature for the stream</param>
        /// <param name="filename">The (possibly) altered stream to create the delta for</param>
        /// <param name="output">The delta output</param>
        public static void GenerateDelta(Stream signature, Stream input, Stream output)
        {
            ChecksumFileReader checksum;

            //Write header into output file
            output.Write(RDiffBinary.DELTA_MAGIC, 0, RDiffBinary.DELTA_MAGIC.Length);

            checksum = new ChecksumFileReader(signature);

            DeltaFile.GenerateDeltaFile(input, output, checksum);
        }

        /// <summary>
        /// Generates a delta stream
        /// </summary>
        /// <param name="signature">The signature for the stream</param>
        /// <param name="filename">The (possibly) altered stream to create the delta for</param>
        /// <param name="output">The delta output</param>
        public static void GenerateDelta(byte[] signature, string inputFile, Stream output)
        {
            FileStream inputStream;
            MemoryStream signatureStream;

            using (inputStream = File.OpenRead(inputFile))
            {
                using (signatureStream = new MemoryStream(signature))
                {
                    GenerateDelta(signatureStream, inputStream, output);
                }
            }
        }


        /// <summary>
        /// Generates a delta file
        /// </summary>
        /// <param name="signaturefile">The signature for the file</param>
        /// <param name="sourceFile">The file to create the delta for</param>
        /// <param name="deltafile">The delta output file</param>
        public static void GenerateDelta(string signaturefile, string sourceFile, string deltafile)
        {
            using (FileStream signatureStream = File.OpenRead(signaturefile))
            {
                using (FileStream sourceStream = File.OpenRead(sourceFile))
                {
                    using (FileStream deltaStream = File.Create(deltafile))
                    {
                        GenerateDelta(signatureStream, sourceStream, deltaStream);
                    }
                }
            }
        }

        /// <summary>
        /// Patches a file
        /// </summary>
        /// <param name="basefile">The most recent full copy of the file</param>
        /// <param name="deltafile">The delta file</param>
        /// <param name="outputfile">The restored file</param>
        public static void PatchFile(string basefile, string deltafile, string outputfile)
        {
            using (FileStream input = File.OpenRead(basefile))
            {
                using (FileStream delta = File.OpenRead(deltafile))
                {
                    using (FileStream output = File.Create(outputfile))
                    {
                        PatchFile(input, delta, output);
                    }
                }
            }
        }

        /// <summary>
        /// Constructs a stream from a basestream and a delta stream
        /// </summary>
        /// <param name="basefile">The most recent full copy of the file</param>
        /// <param name="deltafile">The delta file</param>
        /// <param name="outputfile">The restored file</param>
        public static void PatchFile(Stream basestream, Stream delta, Stream output)
        {
            DeltaFile.PatchFile(basestream, output, delta);
        }

    }
}
