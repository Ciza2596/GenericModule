using System;
using UnityEngine;

namespace CizaSaveLoadModule.Implement
{
	public class TagUtils
	{
		public const string NULL = "null";

		public const char END_OF_STREAM_TAG = (char)65535;

		public const char COMMA_TAG = ',';

		public const char LEFT_CURLY_BRACE  = '{';
		public const char RIGHT_CURLY_BRACE = '}';

		public const char LEFT_SQUARE_BRACE  = '[';
		public const char RIGHT_SQUARE_BRACE = ']';

		public const char   LOWER_N_TAG                   = 'n';
		public const char   LOWER_N_TAG_WITH_SLASH        = '\n';
		public const string LOWER_N_TAG_WITH_DOUBLE_SLASH = "\\n";

		public const char LOWER_U_TAG = 'u';
		public const char LOWER_L_TAG = 'l';

		public const char   LOWER_B_TAG                   = 'b';
		public const char   LOWER_B_TAG_WITH_SLASH        = '\b';
		public const string LOWER_B_TAG_WITH_DOUBLE_SLASH = "\\b";

		public const char   LOWER_F_TAG                   = 'f';
		public const char   LOWER_F_TAG_WITH_SLASH        = '\f';
		public const string LOWER_F_TAG_WITH_DOUBLE_SLASH = "\\f";

		public const char   LOWER_R_TAG                   = 'r';
		public const char   LOWER_R_TAG_WITH_SLASH        = '\r';
		public const string LOWER_R_TAG_WITH_DOUBLE_SLASH = "\\r";

		public const char   LOWER_T_TAG                   = 't';
		public const char   LOWER_T_TAG_WITH_SLASH        = '\t';
		public const string LOWER_T_TAG_WITH_DOUBLE_SLASH = "\\t";

		public const char SPACE_TAG = ' ';
		public const char COLON_TAG = ':';

		public const char QUOTATION_TAG            = '”';
		public const char SLASH_WITH_QUOTATION_TAG = '\"';
		public const char DOUBLE_SLASH_TAG         = '\\';

		public const char REVERSE_SLASH_TAG = '/';

		public static readonly Type BoolType    = typeof(bool);
		public static readonly Type ByteType    = typeof(byte);
		public static readonly Type SbyteType   = typeof(sbyte);
		public static readonly Type CharType    = typeof(char);
		public static readonly Type DecimalType = typeof(decimal);
		public static readonly Type DoubleType  = typeof(double);
		public static readonly Type FloatType   = typeof(float);
		public static readonly Type IntType     = typeof(int);
		public static readonly Type UintType    = typeof(uint);
		public static readonly Type LongType    = typeof(long);
		public static readonly Type UlongType   = typeof(ulong);
		public static readonly Type ShortType   = typeof(short);
		public static readonly Type UshortType  = typeof(ushort);
		public static readonly Type StringType  = typeof(string);
		public static readonly Type Vector2Type = typeof(Vector2);
		public static readonly Type Vector3Type = typeof(Vector3);
	}
}
