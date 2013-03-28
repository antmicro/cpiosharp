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
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AntMicro.CPIOSharp
{
	public class Archive
	{
		public Archive(IEnumerable<BaseFileEntry> entries)
		{
			this.entries = new List<BaseFileEntry>();
			this.entries.AddRange(entries);
		}

		public Archive(string path) : this(Enumerable.Empty<BaseFileEntry>())
		{
			using(var fStream = new FileStream(path, FileMode.Open))
			{
				while(true)
				{
					var fileEntry = new BaseFileEntry(fStream);
					if(!fileEntry.IsTrailer)
					{
						entries.Add(fileEntry);
					}
					else
					{
						break;
					}
				}
			}
		}

		public void SaveTo(string fileName)
		{
			using(var fStream = new FileStream(fileName, FileMode.Create))
			{
				foreach(var entry in entries)
				{
					entry.WriteTo(fStream);
				}
				var trailerBytes = Encoding.ASCII.GetBytes(TrailerEntry);
				fStream.Write(trailerBytes, 0, trailerBytes.Length);

				// as it seems, the whole CPIO is padded to 256 byte boundary
				var length = checked((int)fStream.Position);
				var lengthShouldBe = length.PadTo(256);
				var paddingArray = new byte[lengthShouldBe - length];
				fStream.Write(paddingArray, 0, paddingArray.Length);
			}
		}

		public IEnumerable<BaseFileEntry> Entries
		{
			get
			{
				return new ReadOnlyCollection<BaseFileEntry>(entries);
			}
		}

		private readonly List<BaseFileEntry> entries;
		private const string TrailerEntry = "07070100000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000B00000000TRAILER!!!";
	}
}

