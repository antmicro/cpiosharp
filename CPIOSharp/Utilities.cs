/*
  Copyright (c) 2013 Ant Micro <www.antmicro.com>

  Authors:
   * Konrad Kruczynski (kkruczynski@antmicro.com)

  Permission is hereby granted, free of charge, to any person obtaining
  a copy of this software and associated documentation files (the
  "Software"), to deal in the Software without restriction, including
  without limitation the rights to use, copy, modify, merge, publish,
  distribute, sublicense, and/or sell copies of the Software, and to
  permit persons to whom the Software is furnished to do so, subject to
  the following conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
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
			return Convert.ToInt32(data, 16);
		}

		public static void WriteASCIINumber(this Stream stream, int number, int totalWidth)
		{
			// TODO: culture stuff
			var data = Encoding.ASCII.GetBytes(number.ToString("X").PadLeft(totalWidth, '0'));
			stream.Write(data, 0, data.Length);
		}

		public static int PadToFour(this int value)
		{
			return value.PadTo(4);
		}

		public static int PadTo(this int value, int to)
		{
			return to * (value / to) + (value % to != 0 ? to : 0);
		}

		public static DateTime FromUnixDate(long seconds)
		{
			return Utilities.UnixEpoch + TimeSpan.FromSeconds(seconds);
		}

		public static int ToUnixDate(DateTime dateTime)
		{
			return (int)((dateTime - Utilities.UnixEpoch).TotalSeconds);
		}

		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}

