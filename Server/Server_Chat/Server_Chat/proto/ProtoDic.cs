using System.Collections;
using System.Collections.Generic;
using System;
using ServerMessage;

namespace Proto
{
    public class ProtoDic
    {
        private static List<int> _protoId = new List<int>
        {
            1001,
            1002,
            1003
        };
        private static List<Type> _protoType = new List<Type>
        {
            typeof(SignUpResponse),
            typeof(TocChat),
            typeof(TosChat)
        };

        public static Type GetProtoTypeById(int protoId)
        {
            int index = _protoId.IndexOf(protoId);
            if (index > -1)
                return _protoType[index];
            return null;
        }

        public static int GetProtoIdByType(Type type)
        {
            int index = _protoType.IndexOf(type);
            if (index > -1)
                return _protoId[index];
            return -1;
        }

        public static bool ContainProtoType(Type type)
        {
            return _protoType.Contains(type);
        }

        public static bool ContainProtoId(int protoId)
        {
            return _protoId.Contains(protoId);
        }

    }
}