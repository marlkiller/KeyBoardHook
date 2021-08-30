using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KeyBoardHook.Common.Native
{
    public class MemUtil
    {
        private readonly IntPtr _processHwnd;
        private readonly IntPtr _threadId;
        
        public const byte ASCII_CHAR_LENGTH = 1;
        public const byte UNICODE_CHAR_LENGTH = 2;
        public static readonly IntPtr DEFAULT_MEMORY_SIZE = new IntPtr(0x1000);


        public static int ReadRawMemory(IntPtr hProcess, IntPtr dwAddress, IntPtr lpBuffer, int nSize)
		{
			int lpBytesRead = 0;

			try
			{
				if (!NativeMethods.ReadProcessMemory(hProcess, dwAddress, lpBuffer, nSize, out lpBytesRead))
					throw new Exception("ReadProcessMemory failed");

				return lpBytesRead;
			}
			catch
			{
				return 0;
			}
		}

		/// <summary>
		/// Reads an array of bytes of specified size out of an external process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the external process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="nSize">Number of bytes to be read.</param>
		/// <returns>Returns null on failure.</returns>
		public static byte[] ReadBytes(IntPtr hProcess, IntPtr dwAddress, int nSize)
		{
			IntPtr lpBuffer = IntPtr.Zero;
			int iBytesRead;
			byte[] baRet;

			try
			{
				lpBuffer = Marshal.AllocHGlobal(nSize);

				iBytesRead = ReadRawMemory(hProcess, dwAddress, lpBuffer, nSize);
				if (iBytesRead != nSize)
					throw new Exception("ReadProcessMemory error in ReadBytes");

				baRet = new byte[iBytesRead];
				Marshal.Copy(lpBuffer, baRet, 0, iBytesRead);
			}
			catch
			{
				return null;
			}
			finally
			{
				if (lpBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(lpBuffer);
			}

			return baRet;
		}

		/// <summary>
		/// Reads a structure/object from an external process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="objType">Type of object to be read (hint: use typeof(ObjectType) macro)</param>
		/// <returns>Returns null on failure.</returns>
		public static object ReadObject(IntPtr hProcess, IntPtr dwAddress, Type objType)
		{
			IntPtr lpBuffer = IntPtr.Zero;
			int iBytesRead;
			int iObjectSize;
			object objRet;

			try
			{
				iObjectSize = Marshal.SizeOf(objType);
				lpBuffer = Marshal.AllocHGlobal(iObjectSize);

				iBytesRead = ReadRawMemory(hProcess, dwAddress, lpBuffer, iObjectSize);
				if (iBytesRead != iObjectSize)
					throw new Exception("ReadProcessMemory error in ReadObject.");

				objRet = Marshal.PtrToStructure(lpBuffer, objType);
			}
			catch
			{
				return null;
			}
			finally
			{
				if (lpBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(lpBuffer);
			}

			return objRet;
		}

		/// <summary>
		/// Reads an unsigned byte from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static byte ReadByte(IntPtr hProcess, IntPtr dwAddress)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(byte));
			if (buf == null)
				throw new Exception("ReadByte failed.");

			return buf[0];
		}

		/// <summary>
		/// Reads a signed byte from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static sbyte ReadSByte(IntPtr hProcess, IntPtr dwAddress)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(sbyte));
			if (buf == null)
				throw new Exception("ReadSByte failed.");

			return (sbyte)buf[0];
		}

		/// <summary>
		/// Reads an unsigned short from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="bReverse">Determines whether bytes will be reversed before returning or not (big-endian or little-endian)</param>
		/// <returns>Returns memory read from external process.</returns>
		public static ushort ReadUShort(IntPtr hProcess, IntPtr dwAddress, bool bReverse)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(ushort));
			if (buf == null)
				throw new Exception("ReadUShort failed.");

			if (bReverse)
				Array.Reverse(buf);

			return BitConverter.ToUInt16(buf, 0);
		}

		/// <summary>
		/// Reads an unsigned short from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static ushort ReadUShort(IntPtr hProcess, IntPtr dwAddress)
		{
			return ReadUShort(hProcess, dwAddress, false);
		}

		/// <summary>
		/// Reads a signed short from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="bReverse">Determines whether bytes will be reversed before returning or not (big-endian or little-endian)</param>
		/// <returns>Returns memory read from external process.</returns>
		public static short ReadShort(IntPtr hProcess, IntPtr dwAddress, bool bReverse)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(short));
			if (buf == null)
				throw new Exception("ReadShort failed.");

			if (bReverse)
				Array.Reverse(buf);

			return BitConverter.ToInt16(buf, 0);
		}

		/// <summary>
		/// Reads a signed short from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static short ReadShort(IntPtr hProcess, IntPtr dwAddress)
		{
			return ReadShort(hProcess, dwAddress, false);
		}

		/// <summary>
		/// Reads an unsigned integer from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="bReverse">Determines whether bytes will be reversed before returning or not (big-endian or little-endian)</param>
		/// <returns>Returns memory read from external process.</returns>
		public static uint ReadUInt(IntPtr hProcess, IntPtr dwAddress, bool bReverse)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(uint));
			if (buf == null)
				throw new Exception("ReadUInt failed.");

			if (bReverse)
				Array.Reverse(buf);

			return BitConverter.ToUInt32(buf, 0);
		}

		/// <summary>
		/// Reads an unsigned integer from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static uint ReadUInt(IntPtr hProcess, IntPtr dwAddress)
		{
			return ReadUInt(hProcess, dwAddress, false);
		}

		/// <summary>
		/// Reads a signed integer from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="bReverse">Determines whether bytes will be reversed before returning or not (big-endian or little-endian)</param>
		/// <returns>Returns memory read from external process.</returns>
		public static int ReadInt(IntPtr hProcess, IntPtr dwAddress, bool bReverse)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(int));
			if (buf == null)
				throw new Exception("ReadInt failed.");

			if (bReverse)
				Array.Reverse(buf);

			return BitConverter.ToInt32(buf, 0);
		}

		/// <summary>
		/// Reads a signed integer from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static int ReadInt(IntPtr hProcess, IntPtr dwAddress)
		{
			return ReadInt(hProcess, dwAddress, false);
		}

		/// <summary>
		/// Reads an unsigned 64-bit integer from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="bReverse">Determines whether bytes will be reversed before returning or not (big-endian or little-endian)</param>
		/// <returns>Returns memory read from external process.</returns>
		public static UInt64 ReadUInt64(IntPtr hProcess, IntPtr dwAddress, bool bReverse)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(UInt64));
			if (buf == null)
				throw new Exception("ReadUInt64 failed.");

			if (bReverse)
				Array.Reverse(buf);

			return BitConverter.ToUInt64(buf, 0);
		}

		/// <summary>
		/// Reads an unsigned 64-bit integer from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static UInt64 ReadUInt64(IntPtr hProcess, IntPtr dwAddress)
		{
			return ReadUInt64(hProcess, dwAddress, false);
		}

		/// <summary>
		/// Reads a signed 64-bit integer from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="bReverse">Determines whether bytes will be reversed before returning or not (big-endian or little-endian)</param>
		/// <returns>Returns memory read from external process.</returns>
		public static Int64 ReadInt64(IntPtr hProcess, IntPtr dwAddress, bool bReverse)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(Int64));
			if (buf == null)
				throw new Exception("ReadInt64 failed.");

			if (bReverse)
				Array.Reverse(buf);

			return BitConverter.ToInt64(buf, 0);
		}

		/// <summary>
		/// Reads a signed 64-bit integer from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static Int64 ReadInt64(IntPtr hProcess, IntPtr dwAddress)
		{
			return ReadInt64(hProcess, dwAddress, false);
		}

		/// <summary>
		/// Reads a single-precision float from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="bReverse">Determines whether bytes will be reversed before returning or not (big-endian or little-endian)</param>
		/// <returns>Returns memory read from external process.</returns>
		public static float ReadFloat(IntPtr hProcess, IntPtr dwAddress, bool bReverse)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(float));
			if (buf == null)
				throw new Exception("ReadFloat failed.");

			if (bReverse)
				Array.Reverse(buf);

			return BitConverter.ToSingle(buf, 0);
		}

		/// <summary>
		/// Reads a single-precision float from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static float ReadFloat(IntPtr hProcess, IntPtr dwAddress)
		{
			return ReadFloat(hProcess, dwAddress, false);
		}

		/// <summary>
		/// Reads a double-precision float from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="bReverse">Determines whether bytes will be reversed before returning or not (big-endian or little-endian)</param>
		/// <returns>Returns memory read from external process.</returns>
		public static double ReadDouble(IntPtr hProcess, IntPtr dwAddress, bool bReverse)
		{
			byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(double));
			if (buf == null)
				throw new Exception("ReadDouble failed.");

			if (bReverse)
				Array.Reverse(buf);

			return BitConverter.ToDouble(buf, 0);
		}

		/// <summary>
		/// Reads a double-precision float from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <returns>Returns memory read from external process.</returns>
		public static double ReadDouble(IntPtr hProcess, IntPtr dwAddress)
		{
			return ReadDouble(hProcess, dwAddress, false);
		}

		/// <summary>
		/// Reads an ASCII/ANSI string from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="nLength"></param>
		/// <returns>Returns memory read from external process.</returns>
		public static string ReadASCIIString(IntPtr hProcess, IntPtr dwAddress, int nLength)
		{
			IntPtr lpBuffer = IntPtr.Zero;
			int iBytesRead, nSize;
			string sRet;

			try
			{
				nSize = nLength * ASCII_CHAR_LENGTH;
				lpBuffer = Marshal.AllocHGlobal(nSize + ASCII_CHAR_LENGTH);
				Marshal.WriteByte(lpBuffer, nLength, 0);

				iBytesRead = ReadRawMemory(hProcess, dwAddress, lpBuffer, nSize);
				if (iBytesRead != nSize)
					throw new Exception();

				sRet = Marshal.PtrToStringAnsi(lpBuffer);
			}
			catch
			{
				return String.Empty;
			}
			finally
			{
				if (lpBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(lpBuffer);
			}

			return sRet;
		}

		/// <summary>
		/// Reads a Unicode string from memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process in question.</param>
		/// <param name="dwAddress">Address at which memory will be read.</param>
		/// <param name="nLength"></param>
		/// <returns>Returns memory read from external process.</returns>
		public static string ReadUnicodeString(IntPtr hProcess, IntPtr dwAddress, int nLength)
		{
			IntPtr lpBuffer = IntPtr.Zero;
			int iBytesRead, nSize;
			string sRet;

			try
			{
				nSize = nLength * UNICODE_CHAR_LENGTH;
				lpBuffer = Marshal.AllocHGlobal(nSize + UNICODE_CHAR_LENGTH);
				Marshal.WriteInt16(lpBuffer, nLength * UNICODE_CHAR_LENGTH, 0);

				iBytesRead = ReadRawMemory(hProcess, dwAddress, lpBuffer, nSize);
				if (iBytesRead != nSize)
					throw new Exception();

				sRet = Marshal.PtrToStringUni(lpBuffer);
			}
			catch
			{
				return String.Empty;
			}
			finally
			{
				if (lpBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(lpBuffer);
			}

			return sRet;
		}

		static int WriteRawMemory(IntPtr hProcess, IntPtr dwAddress, IntPtr lpBuffer, int nSize)
		{
			IntPtr iBytesWritten = IntPtr.Zero;

			if (!NativeMethods.WriteProcessMemory(hProcess, dwAddress, lpBuffer, nSize, out iBytesWritten))
				return 0;

			return (int)iBytesWritten;
		}

		/// <summary>
		/// Converts a managed byte array to an unmanaged C-style byte array and writes it to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which memory will be written.</param>
		/// <param name="dwAddress">Address at which bytes will be written.</param>
		/// <param name="lpBytes">Byte array that will be written to other process' memory.</param>
		/// <param name="nSize">Number of bytes to be written.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteBytes(IntPtr hProcess, IntPtr dwAddress, byte[] lpBytes, int nSize)
		{
			IntPtr lpBuffer = IntPtr.Zero;
			int iBytesWritten = 0;

			try
			{
				lpBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(lpBytes[0]) * nSize); //allocate unmanaged memory

				Marshal.Copy(lpBytes, 0, lpBuffer, nSize);

				iBytesWritten = WriteRawMemory(hProcess, dwAddress, lpBuffer, nSize);

				if (nSize != iBytesWritten)
					throw new Exception("WriteBytes failed!  Number of bytes actually written differed from request.");
			}
			catch(Exception e)
			{
				throw e;
				return false;
			}
			finally
			{
				if (lpBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(lpBuffer);
			}

			return true;
		}

		/// <summary>
		/// Converts a managed byte array to an unmanaged C-style byte array and writes it to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which memory will be written.</param>
		/// <param name="dwAddress">Address at which bytes will be written.</param>
		/// <param name="lpBytes">Byte array that will be written to other process' memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteBytes(IntPtr hProcess, IntPtr dwAddress, byte[] lpBytes)
		{
			return WriteBytes(hProcess, dwAddress, lpBytes, lpBytes.Length);
		}

		/// <summary>
		/// Copies a managed object/structure to unmanaged memory, then writes that buffer to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which memory will be written.</param>
		/// <param name="dwAddress">Address at which bytes will be written.</param>
		/// <param name="objBuffer">The object which will be written to memory.</param>
		/// <param name="objType">The Type associated with the object to be written. (hint: use typeof macro or GetType() method)</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteObject(IntPtr hProcess, IntPtr dwAddress, object objBuffer, Type objType)
		{
			int nSize = 0;
			int iBytesWritten = 0;
			IntPtr lpBuffer = IntPtr.Zero;

			try
			{
				nSize = Marshal.SizeOf(objType);

				lpBuffer = Marshal.AllocHGlobal(nSize);

				Marshal.StructureToPtr(objBuffer, lpBuffer, false);

				iBytesWritten = WriteRawMemory(hProcess, dwAddress, lpBuffer, nSize);

				if (nSize != iBytesWritten)
					throw new Exception("WriteObject failed!  Number of bytes actually written differed from request.");
			}
			catch
			{
				return false;
			}
			finally
			{
				if (lpBuffer != IntPtr.Zero)
				{
					Marshal.DestroyStructure(lpBuffer, objType);
					Marshal.FreeHGlobal(lpBuffer);
				}
			}

			return true;
		}

		/// <summary>
		/// Copies a managed object/structure to unmanaged memory, then writes that buffer to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which memory will be written.</param>
		/// <param name="dwAddress">Address at which bytes will be written.</param>
		/// <param name="objBuffer">The object which will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteObject(IntPtr hProcess, IntPtr dwAddress, object objBuffer)
		{
			return WriteObject(hProcess, dwAddress, objBuffer, objBuffer.GetType());
		}

		/// <summary>
		/// Writes an unsigned byte to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteByte(IntPtr hProcess, IntPtr dwAddress, byte Value)
		{
			byte[] lpBytes = { Value };
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(byte));
		}

		/// <summary>
		/// Writes a signed byte to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteSByte(IntPtr hProcess, IntPtr dwAddress, sbyte Value)
		{
			byte[] lpBytes = { (byte)Value };
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(sbyte));
		}


		/// <summary>
		/// Writes an unsigned 16-bit short to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteUShort(IntPtr hProcess, IntPtr dwAddress, ushort Value)
		{
			byte[] lpBytes = BitConverter.GetBytes(Value);
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(ushort));
		}

		/// <summary>
		/// Writes a signed 16-bit short to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteShort(IntPtr hProcess, IntPtr dwAddress, short Value)
		{
			byte[] lpBytes = BitConverter.GetBytes(Value);
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(short));
		}

		/// <summary>
		/// Writes an unsigned 32-bit integer to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteUInt(IntPtr hProcess, IntPtr dwAddress, uint Value)
		{
			byte[] lpBytes = BitConverter.GetBytes(Value);
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(uint));
		}

		/// <summary>
		/// Writes a signed 32-bit integer to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteInt(IntPtr hProcess, IntPtr dwAddress, int Value)
		{
			byte[] lpBytes = BitConverter.GetBytes(Value);
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(int));
		}

		/// <summary>
		/// Writes an unsigned 64-bit integer to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteUInt64(IntPtr hProcess, IntPtr dwAddress, UInt64 Value)
		{
			byte[] lpBytes = BitConverter.GetBytes(Value);
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(UInt64));
		}

		/// <summary>
		/// Writes a signed 64-bit integer to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteInt64(IntPtr hProcess, IntPtr dwAddress, Int64 Value)
		{
			byte[] lpBytes = BitConverter.GetBytes(Value);
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(Int64));
		}

		/// <summary>
		/// Writes a floating point, single precision integer to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteFloat(IntPtr hProcess, IntPtr dwAddress, float Value)
		{
			byte[] lpBytes = BitConverter.GetBytes(Value);
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(float));
		}

		/// <summary>
		/// Writes a floating point, double precision integer to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteDouble(IntPtr hProcess, IntPtr dwAddress, double Value)
		{
			byte[] lpBytes = BitConverter.GetBytes(Value);
			return WriteBytes(hProcess, dwAddress, lpBytes, sizeof(double));
		}

		/// <summary>
		/// Writes an Ansi string (each character is represented by one byte) to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteASCIIString(IntPtr hProcess, IntPtr dwAddress, string Value)
		{
			IntPtr lpBuffer = IntPtr.Zero;
			int iBytesWritten = 0;
			int nSize = 0;

			try
			{
				nSize = Value.Length * ASCII_CHAR_LENGTH;
				lpBuffer = Marshal.StringToHGlobalAnsi(Value);

				iBytesWritten = WriteRawMemory(hProcess, dwAddress, lpBuffer, nSize);

				if (nSize != iBytesWritten)
					throw new Exception();
			}
			catch
			{
				return false;
			}
			finally
			{
				if (lpBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(lpBuffer);
			}

			return true;
		}

		/// <summary>
		/// Writes a Unicode string (each character is represented by two bytes) to another process' memory.
		/// </summary>
		/// <param name="hProcess">Handle to the process to which value will be written.</param>
		/// <param name="dwAddress">Address to which value will be written.</param>
		/// <param name="Value">Value that will be written to memory.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool WriteUnicodeString(IntPtr hProcess, IntPtr dwAddress, string Value)
		{
			IntPtr lpBuffer = IntPtr.Zero;
			int iBytesWritten = 0;
			int nSize = 0;

			try
			{
				nSize = Value.Length * UNICODE_CHAR_LENGTH;
				lpBuffer = Marshal.StringToHGlobalUni(Value);

				iBytesWritten = WriteRawMemory(hProcess, dwAddress, lpBuffer, nSize);

				if (nSize != iBytesWritten)
					throw new Exception();
			}
			catch
			{
				return false;
			}
			finally
			{
				if (lpBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(lpBuffer);
			}

			return true;
		}
 
		#region Memory Allocation
		/// <summary>
		/// Allocates a block of memory in the target process.
		/// </summary>
		/// <param name="hProcess">Handle to the process in which memory will be allocated.</param>
		/// <param name="nSize">Number of bytes to be allocated.  Default is 0x1000.</param>
		/// <param name="dwAllocationType">The type of memory allocation.  See <see cref="MemoryAllocType"/></param>
		/// <param name="dwProtect">The memory protection for the region of pages to be allocated. If the pages are being committed, you can specify any one of the <see cref="MemoryProtectType"/> constants.</param>
		/// <returns>Returns zero on failure, or the base address of the allocated block of memory on success.</returns>
		public static IntPtr AllocateMemory(IntPtr hProcess, IntPtr nSize, int dwAllocationType, int dwProtect)
		{
			return NativeMethods.VirtualAllocEx(hProcess, (IntPtr)null, nSize, dwAllocationType, dwProtect);
		}

		/// <summary>
		/// Allocates a block of memory in the target process.
		/// </summary>
		/// <param name="hProcess">Handle to the process in which memory will be allocated.</param>
		/// <param name="nSize">Number of bytes to be allocated.  Default is 0x1000.</param>
		/// <returns>Returns zero on failure, or the base address of the allocated block of memory on success.</returns>
		/// <remarks>Uses <see cref="MemoryAllocType.MEM_COMMIT"/> for allocation type and <see cref="MemoryProtectType.PAGE_EXECUTE_READWRITE"/> for protect type.</remarks>
		public static IntPtr AllocateMemory(IntPtr hProcess, IntPtr nSize)
		{
			return AllocateMemory(hProcess, nSize, NativeMethods.Commit, NativeMethods.ExecuteReadWrite);
		}

		/// <summary>
		/// Allocates 0x1000 bytes of memory in the target process.
		/// </summary>
		/// <param name="hProcess">Handle to the process in which memory will be allocated.</param>
		/// <returns>Returns zero on failure, or the base address of the allocated block of memory on success.</returns>
		/// <remarks>Uses <see cref="MemoryAllocType.MEM_COMMIT"/> for allocation type and <see cref="MemoryProtectType.PAGE_EXECUTE_READWRITE"/> for protect type.</remarks>
		public static IntPtr AllocateMemory(IntPtr hProcess)
		{
			return AllocateMemory(hProcess, DEFAULT_MEMORY_SIZE);
		}

		/// <summary>
		/// Releases, decommits, or releases and decommits a region of memory within the virtual address space of a specified process.
		/// </summary>
		/// <param name="hProcess">Handle to the process in which memory will be freed.</param>
		/// <param name="dwAddress">A pointer to the starting address of the region of memory to be freed. </param>
		/// <param name="nSize">
		/// The size of the region of memory to free, in bytes. 
		///
		/// If the dwFreeType parameter is MEM_RELEASE, dwSize must be 0 (zero). The function frees the entire region that is reserved in the initial allocation call to VirtualAllocEx.</param>
		/// <param name="dwFreeType">The type of free operation.  See <see cref="MemoryFreeType"/>.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool FreeMemory(IntPtr hProcess, IntPtr dwAddress, IntPtr nSize, int dwFreeType)
		{
			if (dwFreeType == NativeMethods.Release)
				nSize = new IntPtr(0);

			return NativeMethods.VirtualFreeEx(hProcess, dwAddress, nSize, dwFreeType);
		}

		/// <summary>
		/// Releases, decommits, or releases and decommits a region of memory within the virtual address space of a specified process.
		/// </summary>
		/// <param name="hProcess">Handle to the process in which memory will be freed.</param>
		/// <param name="dwAddress">A pointer to the starting address of the region of memory to be freed. </param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <remarks>
		/// Uses <see cref="MemoryFreeType.MEM_RELEASE"/> to free the page(s) specified.
		/// </remarks>
		public static bool FreeMemory(IntPtr hProcess, IntPtr dwAddress)
		{
			return FreeMemory(hProcess, dwAddress, new IntPtr(0), NativeMethods.Release);
		}
		#endregion
    }
}