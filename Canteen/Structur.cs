﻿namespace Canteen
{
    public enum Type
    {
        None = 0,
        Production = 1,
        Sale = 2,
        Incoming = 3,
        Inventarization = 4
    }
    public struct TypeOperation
    {
        public Type Type;
        public string TypeStr;

        public override string ToString()
        {
            return TypeStr.ToString();
        }
    }
}
