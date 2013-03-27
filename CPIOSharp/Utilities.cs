using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AntMicro.CPIOSharp
{
	internal static class Utilities
	{
		public static byte[] ReadOrThrow(this Stream stream, int bytesToRead)
		{
			var result = new byte[bytesToRead];
			var read = 0;
			while(read < bytesToRead)
			{
				var readThisTurn = stream.Read(result, read, bytesToRead);
				if(readThisTurn == 0)
				{
					throw new EndOfStreamException();
				}
				read += readThisTurn;
			}
			return result;
		}

		public static int ReadASCIINumber(this Stream stream, int length)
		{
			var data = Encoding.ASCII.GetString(stream.ReadOrThrow(length));
			return Convert.ToInt32(data, 10);
		}

		public static void WriteASCIINumber(this Stream stream, int number)
		{
			// TODO: culture stuff
			var data = Encoding.ASCII.GetBytes(number.ToString());
			stream.Write(data, 0, data.Length);
		}
	}
}

