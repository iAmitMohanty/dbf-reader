using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBFConverter.Encoders
{
	internal class CurrencyEncoder: IEncoder
	{
		private static CurrencyEncoder instance = null;

		private CurrencyEncoder() { }

		public static CurrencyEncoder Instance
		{
			get
			{
				if (instance == null) instance = new CurrencyEncoder();
				return instance;
			}
		}

		public byte[] Encode(DbfField field, object data)
		{
			float value = 0;
			if (data != null) value = float.Parse(data.ToString());
			return BitConverter.GetBytes(value);
		}

        public object Decode(byte[] buffer, byte[] memoData)
        {
            float d;
            float.TryParse(BitConverter.ToInt32(buffer, 0).ToString(), out d);
            if (d > 0)
            {
                string val = d.ToString().Insert(d.ToString().Length - 4, ".");
                return val;
            }
            else
            {
                return d;
            }
        }
    }
}