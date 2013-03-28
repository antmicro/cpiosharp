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
using Mono.Unix.Native;
using Mono.Unix;

namespace AntMicro.CPIOSharp
{
	public class BaseFileEntry
	{
		public RawFileEntry ToRawEntry()
		{
			return new RawFileEntry(this);
		}

		public static BaseFileEntry FromPath(string path)
		{
			// we're dereferencing the hard link (only one file is to be added, so it is contextless
			return new BaseFileEntry(path, 1);
		}

		protected BaseFileEntry()
		{
		}

		protected BaseFileEntry(string path, int? overwritingNumberOfLinks = null)
		{
			Stat statData;
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.stat(path, out statData));
			Uid = (int)statData.st_uid;
			Gid = (int)statData.st_gid;
			ModificationDate = Utilities.FromUnixDate(statData.st_mtime);
			Path = path;
			RawInodeNumber = (int)statData.st_ino;
			RawContents = File.ReadAllBytes(path);
			RawPermissions = statData.st_mode;
			RawNumberOfLinks = overwritingNumberOfLinks ?? (int)statData.st_nlink;
			RawDeviceMajorNumber = (int)(statData.st_rdev >> 2);
			RawDeviceMinorNumber = (int)(statData.st_rdev & 0xFF);
			RawHoldingDeviceMajor = (int)(statData.st_dev >> 2);
			RawHoldingDeviceMinor = (int)(statData.st_dev & 0xFF);
		}

		protected BaseFileEntry(BaseFileEntry entry)
		{
			Uid = entry.Uid;
			Gid = entry.Gid;
			ModificationDate = entry.ModificationDate;
			Path = entry.Path;
			IsTrailer = entry.IsTrailer;
			RawInodeNumber = entry.RawInodeNumber;
			RawContents = entry.RawContents;
			RawPermissions = entry.RawPermissions;
			RawNumberOfLinks = entry.RawNumberOfLinks;
			RawDeviceMajorNumber = entry.RawDeviceMajorNumber;
			RawDeviceMinorNumber = entry.RawDeviceMinorNumber;
			RawHoldingDeviceMajor = entry.RawHoldingDeviceMajor;
			RawHoldingDeviceMinor = entry.RawHoldingDeviceMinor;
		}

		internal BaseFileEntry(FileStream stream)
		{
			var magic = stream.ReadOrThrow(Magic.Length);
			if(!Enumerable.SequenceEqual(magic, Magic))
			{
				throw new InvalidOperationException(string.Format("Unexpected file entry magic {0}.", magic.Select(x => ((char)x).ToString()).Aggregate((x, y) => x + y)));
			}
			RawInodeNumber = stream.ReadASCIINumber(8);
			RawPermissions = (FilePermissions)stream.ReadASCIINumber(8);
			Uid = stream.ReadASCIINumber(8);
			Gid = stream.ReadASCIINumber(8);
			RawNumberOfLinks = stream.ReadASCIINumber(8);
			ModificationDate = Utilities.FromUnixDate(stream.ReadASCIINumber(8));
			var fileSize = stream.ReadASCIINumber(8);

			RawHoldingDeviceMajor = stream.ReadASCIINumber(8);
			RawHoldingDeviceMinor = stream.ReadASCIINumber(8);
			RawDeviceMajorNumber = stream.ReadASCIINumber(8);
			RawDeviceMinorNumber = stream.ReadASCIINumber(8);

			var pathLength = stream.ReadASCIINumber(8);
			var paddedLength = (110 + pathLength).PadToFour() - 110;
			var check = stream.ReadASCIINumber(8);
			if(check != 0)
			{
				throw new InvalidOperationException("Invalid value of the 'check' field: should be all zero.");
			}
			var pathBytes = stream.ReadOrThrow(pathLength - 1); // no need to read NUL
			stream.Seek(1 + paddedLength - pathLength, SeekOrigin.Current);
			Path = Encoding.UTF8.GetString(pathBytes);

			IsTrailer = Path == TrailerPath;

			RawContents = stream.ReadOrThrow(fileSize.PadToFour());
		}

		internal void WriteTo(Stream stream)
		{
			stream.Write(Magic, 0, Magic.Length);
			stream.WriteASCIINumber(RawInodeNumber, 8);
			stream.WriteASCIINumber((int)RawPermissions, 8);
			stream.WriteASCIINumber(Uid, 8);
			stream.WriteASCIINumber(Gid, 8);
			stream.WriteASCIINumber(RawNumberOfLinks, 8);
			stream.WriteASCIINumber(Utilities.ToUnixDate(ModificationDate), 8);
			stream.WriteASCIINumber(RawContents.Length, 8);
			stream.WriteASCIINumber(RawHoldingDeviceMajor, 8);
			stream.WriteASCIINumber(RawHoldingDeviceMinor, 8);
			stream.WriteASCIINumber(RawDeviceMajorNumber, 8);
			stream.WriteASCIINumber(RawDeviceMinorNumber, 8);
			stream.WriteASCIINumber(Path.Length + 1, 8); // include ending NUL
			stream.Write(CheckBytes, 0, CheckBytes.Length);
			var pathBytes = Encoding.UTF8.GetBytes(Path);
			stream.Write(pathBytes, 0, pathBytes.Length);
			var paddingArray = new byte[(111 + pathBytes.Length).PadToFour() - 110 - pathBytes.Length];
			stream.Write(paddingArray, 0, paddingArray.Length);
			stream.Write(RawContents, 0, RawContents.Length);
			paddingArray = new byte[RawContents.Length.PadToFour() - RawContents.Length];
			stream.Write(paddingArray, 0, paddingArray.Length);
		}

		public int Uid { get; set; }
		public int Gid { get; set; }
		public DateTime ModificationDate { get; set; }
		public string Path { get; set; }

		internal bool IsTrailer { get; private set; }

		protected int RawInodeNumber { get; set; }
		protected byte[] RawContents { get; set; }
		protected FilePermissions RawPermissions { get; set; }
		protected int RawNumberOfLinks { get; set; }
		protected int RawDeviceMajorNumber { get; set; }
		protected int RawDeviceMinorNumber { get; set; }
		protected int RawHoldingDeviceMajor { get; set; }
		protected int RawHoldingDeviceMinor { get; set; }

		private static readonly byte[] Magic = new byte[] { 0x30, 0x37, 0x30, 0x37, 0x30, 0x31 }; // 070701, the "new ASCII" format
		private static readonly byte[] CheckBytes = Enumerable.Repeat((byte)0x30, 8).ToArray();
		private const string TrailerPath = "TRAILER!!!";
	}
}

