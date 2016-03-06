// Guids.cs
// MUST match guids.h
using System;

namespace privatedeveloperinc.Edizen
{
    static class GuidList
    {
        public const string guidEdizenPkgString = "58104dc7-f5ca-4b5d-9776-a8a031e2bd94";
        public const string guidEdizenCmdSetString = "f8e4a754-abfe-4977-a5ab-a6468adf15c0";

        public static readonly Guid guidEdizenCmdSet = new Guid(guidEdizenCmdSetString);
    };
}