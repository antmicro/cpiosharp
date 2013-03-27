using System;
using System.IO;
using System.Linq;

namespace AntMicro.CPIOSharp
{
	public class FileEntry
	{
		internal FileEntry(Stream stream)
		{
			var magic = stream.ReadOrThrow(Magic.Length);
			if(Enumerable.SequenceEqual(magic, Magic))
			{
				throw new InvalidOperationException(string.Format("Unexpected file entry magic {0}.", magic.Select(x => ((char)x).ToString()).Aggregate((x, y) => x + y)));
			}
			InodeNumber = stream.ReadASCIINumber(6);
			Mode = stream.ReadASCIINumber(8);
			Uid = stream.ReadASCIINumber(8);
			Gid = stream.ReadASCIINumber(8);
			NumberOfLinks = stream.ReadASCIINumber(8);
		}

		public int InodeNumber { get; private set; }
		public int Mode { get; private set; } // TODO: this field is probably temporary
		public int Uid { get; private set; }
		public int Gid { get; private set; }
		public int NumberOfLinks { get; private set; }

		private static readonly byte[] Magic = new byte[] { 0x30, 0x37, 0x30, 0x37, 0x30, 0x31 }; // 070701, the "new ASCII" format
	}
}

