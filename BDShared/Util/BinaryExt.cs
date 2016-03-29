using System;
using System.Text;

namespace BDShared.Util
{
    public static class BinaryExt
    {

        public static byte[] Extract(this byte[] buffer, int offset, int length)
        {
            byte[] b = new byte[length];
            if(buffer.Length < (offset + length))
                throw new IndexOutOfRangeException("Given buffer is not long enough.");
            Buffer.BlockCopy(buffer, offset, b, 0, length);
            return b;
        }

        public static byte[] Extract(this byte[] buffer, int offset)
        {
            int length  = buffer.Length - offset;
            return buffer.Extract(offset, length);
        }

        public static byte[] ToByteArray(this string hex)
        {
            string _hex = hex.Replace(" ", "");
            int chars = hex.Length;
            if(chars % 2 != 0)
                throw new ArgumentException("Given string is invalid.");
            byte[] b = new byte[chars / 2];
            for(int i = 0; i < chars; i += 2)
                b[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return b;
        }

        public static string FormatHex(this byte[] data)
        {
            StringBuilder b = new StringBuilder();

            int totalBytes = data.Length;
            const int bytesPerRow = 16;
            int totalRows = (totalBytes / bytesPerRow) + 1;

            for(int i = 0; i < totalRows; i++)
            {
                b.AppendFormat("0x{0:X8} ", (i * bytesPerRow));

                int rowIndex = (i * bytesPerRow);
                int bytesRemaining = data.Length - rowIndex;

                byte[] row = data.Extract(rowIndex, bytesRemaining > bytesPerRow ? bytesPerRow : bytesRemaining);

                for(int j = 0; j < row.Length; j++)
                    b.AppendFormat("{0:X2} ", row[j]);

                int spacesRequired = bytesPerRow - row.Length;
                for(int j = 0; j < spacesRequired; j++)
                    b.Append("   ");

                for(int j = 0; j < row.Length; j++)
                {
                    if(row[j] > 0x1F && row[j] < 0x80)
                        b.Append((char)(row[j]));
                    else
                        b.Append(".");
                }

                b.AppendLine();
            }

            return b.ToString();
        }

    }
}