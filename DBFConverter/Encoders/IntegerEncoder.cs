using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBFConverter.Encoders
{
	public class IntegerEncoder: IEncoder
	{
		private static IntegerEncoder instance = null;

		private IntegerEncoder() { }

		public static IntegerEncoder Instance
		{
			get
			{
				if (instance == null) instance = new IntegerEncoder();
				return instance;
			}
		}

		public byte[] Encode(DbfField field, object data)
		{
			int value = 0;
			if (data != null) value = int.Parse(data.ToString());
			return BitConverter.GetBytes(value);
		}

        public object Decode(byte[] buffer, byte[] memoData)
        {
            return BitConverter.ToInt32(buffer, 0);
        }
    }
}
