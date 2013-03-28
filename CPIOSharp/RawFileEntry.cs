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
using Mono.Unix.Native;

namespace AntMicro.CPIOSharp
{
	public sealed class RawFileEntry : BaseFileEntry
	{
		public RawFileEntry(BaseFileEntry baseOn) : base(baseOn)
		{
		}

		public int InodeNumber 
		{
			get
			{
				return RawInodeNumber;
			}
			set
			{
				RawInodeNumber = value;
			}
		}

		public byte[] Contents
		{
			get
			{
				return RawContents;
			}
			set
			{
				RawContents = value;
			}
		}

		public FilePermissions Permissions
		{
			get
			{
				return RawPermissions;
			}
			set
			{
				RawPermissions = value;
			}
		}

		public int NumberOfLinks
		{
			get
			{
				return RawNumberOfLinks;
			}
			set
			{
				RawNumberOfLinks = value;
			}
		}

		public int DeviceMajorNumber
		{
			get
			{
				return RawDeviceMajorNumber;
			}
			set
			{
				RawDeviceMajorNumber = value;
			}
		}

		public int DeviceMinorNumber
		{
			get
			{
				return RawDeviceMinorNumber;
			}
			set
			{
				RawDeviceMinorNumber = value;
			}
		}

		public int HoldingDeviceMajor
		{
			get
			{
				return RawHoldingDeviceMajor;
			}
			set
			{
				RawHoldingDeviceMajor = value;
			}
		}

		public int HoldingDeviceMinor
		{
			get
			{
				return RawHoldingDeviceMinor;
			}
			set
			{
				RawHoldingDeviceMinor = value;
			}
		}
	}
}

