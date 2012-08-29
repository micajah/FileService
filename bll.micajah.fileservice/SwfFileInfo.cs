using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace Micajah.FileService.Bll
{
    /// <summary>
    /// This class is used to determine the basic data from an SWF file header.
    /// </summary>
    public class SwfFileInfo
    {
        #region Members

        private string m_FileName;
        private string m_MagicBytes;
        private bool m_IsCompressed;
        private short m_Version;
        private int m_Size;
        private int m_Width;
        private int m_Height;
        private bool m_IsValid;
        private double m_FrameRate;
        private int m_FrameCount;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the fully qualified name of the SWF filename.
        /// </summary>
        public string FileName
        {
            get { return m_FileName; }
            set { this.LoadSwfFile(value); }
        }

        /// <summary>
        /// Gets the magic bytes in a SWF file (FWS or CWS).
        /// </summary>
        public string MagicBytes
        {
            get { return m_MagicBytes; }
        }

        /// <summary>
        /// Gets a flag indicating whether the SWF file is compressed (CWS).
        /// </summary>
        public bool IsCompressed
        {
            get { return m_IsCompressed; }
        }

        /// <summary>
        /// Gets the flash major version.
        /// </summary>
        public short Version
        {
            get { return m_Version; }
        }

        /// <summary>
        /// Gets the uncompressed file size in bytes.
        /// </summary>
        public int Size
        {
            get { return m_Size; }
        }

        /// <summary>
        /// Gets the flash movie native width.
        /// </summary>
        public int Width
        {
            get { return m_Width; }
        }

        /// <summary>
        /// Gets the flash movie native height.
        /// </summary>
        public int Height
        {
            get { return m_Height; }
        }

        /// <summary>
        /// Gets a flag indicating whether the SWF file is valid.
        /// </summary>
        public bool IsValid
        {
            get { return m_IsValid; }
        }

        /// <summary>
        /// Gets the flash movie native frame-rate.
        /// </summary>
        public double FrameRate
        {
            get { return m_FrameRate; }
        }

        /// <summary>
        /// Gets the flash movie total frames.
        /// </summary>
        public int FrameCount
        {
            get { return m_FrameCount; }
        }

        #endregion

        #region Constructors

        public SwfFileInfo() { }

        public SwfFileInfo(string fileName)
        {
            this.LoadSwfFile(fileName);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Decompress stream.
        /// </summary>
        /// <param name="buffer">The array of unsigned bytes from which to create the stream.</param>
        private static void Decompress(ref byte[] buffer)
        {
            // Let's set GZip magic bytes which GZipStream can process
            Array.Resize(ref buffer, buffer.Length + 8);
            for (int i = buffer.Length - 1; i > 9; i--)
            {
                buffer[i] = buffer[i - 8];
            }
            ((Array)(new byte[] { 31, 139, 8, 0, 0, 0, 0, 0, 4, 0 })).CopyTo(buffer, 0);

            MemoryStream ms = new MemoryStream(buffer);
            GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);

            byte[] decompressedBuffer = new byte[buffer.Length + 1000000];

            int gzipLength = ReadAllBytesFromStream(gzip, decompressedBuffer);

            gzip.Close();
            ms.Close();

            Array.Resize(ref buffer, gzipLength);
            Array.Resize(ref decompressedBuffer, gzipLength);
            decompressedBuffer.CopyTo(buffer, 0);

            Array.Clear(decompressedBuffer, 0, decompressedBuffer.Length);
        }

        private void LoadSwfFile(string fileName)
        {
            this.Reset();
            m_FileName = fileName;

            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                // Read MAGIC FIELD
                m_MagicBytes = new String(reader.ReadChars(3));

                if ((string.Compare(m_MagicBytes, "FWS", StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(m_MagicBytes, "CWS", StringComparison.OrdinalIgnoreCase) == 0))
                    m_IsValid = true;
                else
                    return;

                // Compression
                m_IsCompressed = m_MagicBytes.StartsWith("C", StringComparison.OrdinalIgnoreCase);

                // Version
                m_Version = Convert.ToInt16(reader.ReadByte());

                // Size
                // 4 LSB-MSB
                for (int i = 0; i < 4; i++)
                {
                    byte t = reader.ReadByte();
                    m_Size += t << (8 * i);
                }

                // RECT... we will "simulate" a stream from now on... read remaining file
                byte[] buffer = reader.ReadBytes((int)m_Size);

                if (m_IsCompressed) Decompress(ref buffer);

                byte cbyte = buffer[0];
                int bits = (int)cbyte >> 3;

                Array.Reverse(buffer);
                Array.Resize(ref buffer, buffer.Length - 1);
                Array.Reverse(buffer);

                BitArray cval = new BitArray(bits, false);

                // Current byte
                cbyte &= 7;
                cbyte <<= 5;

                // Current bit (first byte starts off already shifted)
                int cbit = 2;

                // Must get all 4 values in the RECT
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < cval.Count; j++)
                    {
                        if ((cbyte & 128) > 0)
                        {
                            cval[j] = true;
                        }

                        cbyte <<= 1;
                        cbyte &= 255;
                        cbit--;

                        // We will be needing a new byte if we run out of bits
                        if (cbit < 0)
                        {
                            cbyte = buffer[0];

                            Array.Reverse(buffer);
                            Array.Resize(ref buffer, buffer.Length - 1);
                            Array.Reverse(buffer);

                            cbit = 7;
                        }
                    }

                    // O.k. full value stored... calculate
                    int c = 1;
                    int val = 0;

                    for (int j = cval.Count - 1; j >= 0; j--)
                    {
                        if (cval[j])
                        {
                            val += c;
                        }
                        c *= 2;
                    }

                    val /= 20;

                    switch (i)
                    {
                        case 0:
                            // tmp value
                            m_Width = val;
                            break;
                        case 1:
                            m_Width = val - m_Width;
                            break;
                        case 2:
                            // tmp value
                            m_Height = val;
                            break;
                        case 3:
                            m_Height = val - m_Height;
                            break;
                    }

                    cval.SetAll(false);
                }

                // Frame rate
                m_FrameRate += buffer[1];
                m_FrameRate += Convert.ToSingle(buffer[0] / 100);

                // Frames
                m_FrameCount += BitConverter.ToInt16(buffer, 2);
            }
        }

        private void Reset()
        {
            m_FileName = null;
            m_MagicBytes = null;
            m_IsCompressed = false;
            m_Version = (short)0;
            m_Size = 0;
            m_Width = 0;
            m_Height = 0;
            m_IsValid = false;
            m_FrameRate = 0.0;
            m_FrameCount = 0;
        }

        /// <summary>
        /// Reads all bytes from a stream.
        /// </summary>
        /// <param name="stream">A stream to read from.</param>
        /// <param name="buffer">An array of bytes to read the stream in.</param>
        /// <returns>The length of the stream in bytes.</returns>
        private static int ReadAllBytesFromStream(Stream stream, byte[] buffer)
        {
            int offset = 0;
            int totalCount = 0;
            while (true)
            {
                int bytesRead = stream.Read(buffer, offset, 100);
                if (bytesRead == 0)
                    break;
                offset += bytesRead;
                totalCount += bytesRead;
            }
            return totalCount;
        }

        #endregion
    }
}
