﻿namespace MRI.Core.Message
{
    public enum MongoOpCode
    {
        OP_REPLY =1,
        OP_MSG = 1000,
        OP_UPDATE = 2001,
        OP_INSERT = 2002,
        RESERVED = 2003,
        OP_QUERY = 2004,
        OP_GET_MORE = 2005,
        OP_DELETE = 2006,
        OP_KILL_CURSORS = 2007
    }
}
