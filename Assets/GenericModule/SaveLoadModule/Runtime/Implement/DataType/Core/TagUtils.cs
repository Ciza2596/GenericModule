using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataType
{
	public class TagUtils
	{
		public const string TYPE_TAG  = "@type";
		public const string VALUE_TAG = "@value";

		public const string X_TAG = "x";
		public const string Y_TAG = "y";
		public const string Z_TAG = "z";

		public const string TICKS_TAG = "ticks";

		public const string NULL = "null";

		public const char END_OF_STREAM_TAG = (char)65535;

		public const char COMMA_TAG = ',';

		public const char LEFT_CURLY_BRACE  = '{';
		public const char RIGHT_CURLY_BRACE = '}';

		public const char LEFT_SQUARE_BRACE  = '[';
		public const char RIGHT_SQUARE_BRACE = ']';

		public const char   N_TAG                   = 'n';
		public const char   N_TAG_WITH_SLASH        = '\n';
		public const string N_TAG_WITH_DOUBLE_SLASH = "\\n";

		public const char U_TAG = 'u';
		public const char L_TAG = 'l';

		public const char   B_TAG                   = 'b';
		public const char   B_TAG_WITH_SLASH        = '\b';
		public const string B_TAG_WITH_DOUBLE_SLASH = "\\b";

		public const char   F_TAG                   = 'f';
		public const char   F_TAG_WITH_SLASH        = '\f';
		public const string F_TAG_WITH_DOUBLE_SLASH = "\\f";

		public const char   R_TAG                   = 'r';
		public const char   R_TAG_WITH_SLASH        = '\r';
		public const string R_TAG_WITH_DOUBLE_SLASH = "\\r";

		public const char   T_TAG                   = 't';
		public const char   T_TAG_WITH_SLASH        = '\t';
		public const string T_TAG_WITH_DOUBLE_SLASH = "\\t";

		public const char SPACE_TAG = ' ';
		public const char COLON_TAG = ':';

		public const char QUOTATION_TAG_1          = '“';
		public const char QUOTATION_TAG_2          = '”';
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

		public static readonly Type ListDataType = typeof(ListDataType);

		public static readonly Type   QueueDataType    = typeof(QueueDataType);
		public static readonly string QueueEnqueueName = nameof(Queue.Enqueue);

		public static readonly Type   StackDataType = typeof(StackDataType);
		public static readonly string StackPushName = nameof(Stack.Push);

		public static readonly Type   HashSetDataType = typeof(HashSetDataType);
		public static readonly string HashSetAddName  = nameof(HashSet<object>.Add);
	}
}
