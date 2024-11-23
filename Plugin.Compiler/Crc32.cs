using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.Compiler
{
	/// <summary>Performs 32-bit reversed cyclic redundancy checks.</summary>
	internal class Crc32
	{
		/// <summary>Generator polynomial (modulo 2) for the reversed CRC32 algorithm. </summary>
		private const UInt32 s_generator = 0xEDB88320;

		/// <summary>Contains a cache of calculated checksum chunks.</summary>
		private readonly UInt32[] m_checksumTable;

		/// <summary>Creates a new instance of the Crc32 class.</summary>
		public Crc32()
		{
			// Constructs the checksum lookup table. Used to optimize the checksum.
			m_checksumTable = Enumerable.Range(0, 256).Select(i =>
			{
				var tableEntry = (UInt32)i;
				for(Int32 j = 0; j < 8; ++j)
				{
					tableEntry = ((tableEntry & 1) != 0)
						? (s_generator ^ (tableEntry >> 1))
						: (tableEntry >> 1);
				}
				return tableEntry;
			}).ToArray();
		}

		public UInt32 Get(String text)
			=> this.Get(Encoding.UTF8.GetBytes(text));

		/// <summary>Calculates the checksum of the byte stream.</summary>
		/// <param name="byteStream">The byte stream to calculate the checksum for.</param>
		/// <returns>A 32-bit reversed checksum.</returns>
		public UInt32 Get(IEnumerable<Byte> byteStream)// Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
			=> ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
				(m_checksumTable[(checksumRegister & 0xFF) ^ currentByte] ^ (checksumRegister >> 8)));
	}
}