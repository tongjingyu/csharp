using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace 透明时钟演示
{
    class IEHistory
    {

        [Flags()]
        public enum EntryType
        {
            Unknown = 0,
            Normal = 1,
            Sticky = 4,
            Edited = 8,
            TrackOffline = 16,
            TrackOnline = 32,
            Sparse = 65536,
            Cookie = 1048576,
            UrlHistory = 2097152
        }

        public class IeHistoryEntry
        {
            public string SourceUrlName = "";
            public string LastAccessDate = "";
            public string HitRate = "";
            public string LastModifiedTime = "";
            public string LastSyncTime = "";
            public string UseCount = "";
            public string Url = "";
            public string Extension = "";
            public string HeaderInfo = "";
            public EntryType Type = EntryType.Unknown;
        }

        public class IeHistory
        {

            //For PInvoke: Contains information about an entry in the Internet cache
            private struct FILETIME
            {
                public int dwLowDateTime;
                public int dwHighDateTime;
            }
            [DllImport("KERNEL32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            private static extern int FileTimeToLocalFileTime(ref FILETIME lpFileTime, ref FILETIME lpLocalFileTime);

            [DllImport("KERNEL32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            private static extern int FileTimeToSystemTime(ref FILETIME lpFileTime, ref SYSTEMTIME lpSystemTime);

            private struct SYSTEMTIME
            {
                public short wYear;
                public short wMonth;
                public short wDayOfWeek;
                public short wDay;
                public short wHour;
                public short wMinute;
                public short wSecond;
                public short wMilliseconds;
            }

            [StructLayout(LayoutKind.Explicit, Size = 80)]
            private struct INTERNET_CACHE_ENTRY_INFOA
            {
                [FieldOffset(0)]
                public UInt32 dwStructSize;
                [FieldOffset(4)]
                public IntPtr lpszSourceUrlName;
                [FieldOffset(8)]
                public IntPtr lpszLocalFileName;
                [FieldOffset(12)]
                public UInt32 CacheEntryType;
                [FieldOffset(16)]
                public UInt32 dwUseCount;
                [FieldOffset(20)]
                public UInt32 dwHitRate;
                [FieldOffset(24)]
                public UInt32 dwSizeLow;
                [FieldOffset(28)]
                public UInt32 dwSizeHigh;
                [FieldOffset(32)]
                public FILETIME LastModifiedTime;
                [FieldOffset(40)]
                public FILETIME ExpireTime;
                [FieldOffset(48)]
                public FILETIME LastAccessTime;
                [FieldOffset(56)]
                public FILETIME LastSyncTime;
                [FieldOffset(64)]
                public IntPtr lpHeaderInfo;
                [FieldOffset(68)]
                public UInt32 dwHeaderInfoSize;
                [FieldOffset(72)]
                public IntPtr lpszFileExtension;
                [FieldOffset(76)]
                public UInt32 dwReserved;
                [FieldOffset(76)]
                public UInt32 dwExemptDelta;
            }

            //For PInvoke: Initiates the enumeration of the cache groups in the Internet cache
            [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindFirstUrlCacheGroup", CallingConvention = CallingConvention.StdCall)]
            private static extern IntPtr FindFirstUrlCacheGroup(Int32 dwFlags, int dwFilter, IntPtr lpSearchCondition, Int32 dwSearchCondition, ref long lpGroupId, IntPtr lpReserved);

            //For PInvoke: Retrieves the next cache group in a cache group enumeration
            [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindNextUrlCacheGroup", CallingConvention = CallingConvention.StdCall)]
            private static extern bool FindNextUrlCacheGroup(IntPtr hFind, ref long lpGroupId, IntPtr lpReserved);

            //For PInvoke: Releases the specified GROUPID and any associated state in the cache index file
            [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DeleteUrlCacheGroup", CallingConvention = CallingConvention.StdCall)]
            private static extern bool DeleteUrlCacheGroup(long GroupId, Int32 dwFlags, IntPtr lpReserved);

            //For PInvoke: Begins the enumeration of the Internet cache
            [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindFirstUrlCacheEntryA", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr FindFirstUrlCacheEntry(
             [MarshalAs(UnmanagedType.LPStr)]string lpszUrlSearchPattern,
             IntPtr lpFirstCacheEntryInfo,
             [MarshalAs(UnmanagedType.U4)]ref int lpdwFirstCacheEntryInfoBufferSize);

            //For PInvoke: Retrieves the next entry in the Internet cache
            [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindNextUrlCacheEntryA", CallingConvention = CallingConvention.StdCall)]
            private static extern bool FindNextUrlCacheEntry(IntPtr hFind, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize);

            //For PInvoke: Removes the file that is associated with the source name from the cache, if the file exists
            [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DeleteUrlCacheEntryA", CallingConvention = CallingConvention.StdCall)]
            private static extern bool DeleteUrlCacheEntry(IntPtr lpszUrlName);

            private const int CACHEGROUP_SEARCH_ALL = 0x0;

            private const int ERROR_NO_MORE_ITEMS = 0x103;
            private const int ERROR_CACHE_FIND_FAIL = 0x0;
            private const int ERROR_CACHE_FIND_SUCCESS = 0x1;
            private const int ERROR_FILE_NOT_FOUND = 0x2;
            private const int ERROR_ACCESS_DENIED = 0x5;
            private const int ERROR_INSUFFICIENT_BUFFER = 0x7A;
            private const int MAX_PATH = 0x104;
            private const int MAX_CACHE_ENTRY_INFO_SIZE = 0x1000;

            private const int LMEM_FIXED = 0x0;
            private const int LMEM_ZEROINIT = 0x40;
            private const int LPTR = (LMEM_FIXED | LMEM_ZEROINIT);

            private const long NORMAL_CACHE_ENTRY = 0x200001;
            private const int EDITED_CACHE_ENTRY = 0x8;
            private const int TRACK_OFFLINE_CACHE_ENTRY = 0x10;
            private const int TRACK_ONLINE_CACHE_ENTRY = 0x20;
            private const int STICKY_CACHE_ENTRY = 0x40;
            private const int SPARSE_CACHE_ENTRY = 0x10000;
            private const int COOKIE_CACHE_ENTRY = 0x100000;
            private const int URLHISTORY_CACHE_ENTRY = 0x200000;
            private const long URLCACHE_FIND_DEFAULT_FILTER = NORMAL_CACHE_ENTRY | COOKIE_CACHE_ENTRY | URLHISTORY_CACHE_ENTRY | TRACK_OFFLINE_CACHE_ENTRY | TRACK_ONLINE_CACHE_ENTRY | STICKY_CACHE_ENTRY;

            public static void DeleteUrlCache(string url)
            {

                // Pointer to a GROUPID variable
                long groupId = 0;

                // Local variables
                int cacheEntryInfoBufferSizeInitial = 0;
                int cacheEntryInfoBufferSize = 0;
                bool returnValue = false;

                IntPtr enumHandle = FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);

                //If there are no items in the Cache, you are finished.
                if ((!enumHandle.Equals(IntPtr.Zero) & ERROR_NO_MORE_ITEMS.Equals(Marshal.GetLastWin32Error())))
                {
                    return;
                }

                //Loop through Cache Group.

                enumHandle = FindFirstUrlCacheEntry("", IntPtr.Zero, ref cacheEntryInfoBufferSizeInitial);

                if ((!enumHandle.Equals(IntPtr.Zero) & ERROR_NO_MORE_ITEMS.Equals(Marshal.GetLastWin32Error())))
                {
                    return;
                }

                cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                IntPtr cacheEntryInfoBuffer = Marshal.AllocHGlobal(cacheEntryInfoBufferSize);
                enumHandle = FindFirstUrlCacheEntry("", cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);

                while (true)
                {
                    INTERNET_CACHE_ENTRY_INFOA internetCacheEntry = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(cacheEntryInfoBuffer, typeof(INTERNET_CACHE_ENTRY_INFOA));
                    cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;
                    string sourceUrlName = Marshal.PtrToStringAnsi(internetCacheEntry.lpszSourceUrlName);
                    if (sourceUrlName == url)
                    {
                        returnValue = DeleteUrlCacheEntry(internetCacheEntry.lpszSourceUrlName);
                    }

                    returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                    if (!returnValue & Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
                    {
                        break;
                    }

                    if (!returnValue & cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize)
                    {

                        cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                        IntPtr tempIntPtr = new IntPtr(cacheEntryInfoBufferSize);
                        cacheEntryInfoBuffer = Marshal.ReAllocHGlobal(cacheEntryInfoBuffer, tempIntPtr);
                        returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                    }

                }
                Marshal.FreeHGlobal(cacheEntryInfoBuffer);
            }

            public static Dictionary<string, IeHistoryEntry> GetURLCache()
            {

                // Pointer to a GROUPID variable.
                long groupId = 0;

                // Local variables.
                int cacheEntryInfoBufferSizeInitial = 0;
                int cacheEntryInfoBufferSize = 0;
                bool returnValue = false;

                IntPtr enumHandle = FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);

                //If there are no items in the Cache, you are finished.

                if ((!enumHandle.Equals(IntPtr.Zero) & ERROR_NO_MORE_ITEMS.Equals(Marshal.GetLastWin32Error())))
                {
                    return null;
                }

                //Loop through Cache Group.
                enumHandle = FindFirstUrlCacheEntry(null, IntPtr.Zero, ref cacheEntryInfoBufferSizeInitial);

                if ((!enumHandle.Equals(IntPtr.Zero) & ERROR_NO_MORE_ITEMS.Equals(Marshal.GetLastWin32Error())))
                {
                    return null;
                }

                cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                IntPtr cacheEntryInfoBuffer = Marshal.AllocHGlobal(cacheEntryInfoBufferSize);
                enumHandle = FindFirstUrlCacheEntry(null, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                Dictionary<string, IeHistoryEntry> list = new Dictionary<string, IeHistoryEntry>();

                while (true)
                {
                    INTERNET_CACHE_ENTRY_INFOA internetCacheEntry = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(cacheEntryInfoBuffer, typeof(INTERNET_CACHE_ENTRY_INFOA));
                    cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;

                    IeHistoryEntry entry = new IeHistoryEntry();

                    entry.SourceUrlName = Marshal.PtrToStringAnsi(internetCacheEntry.lpszSourceUrlName);
                    entry.LastAccessDate = FileTime2SystemTime(ref internetCacheEntry.LastAccessTime).ToString();
                    entry.Type = (EntryType)Enum.Parse(typeof(EntryType), internetCacheEntry.CacheEntryType.ToString());
                    entry.Url = Marshal.PtrToStringAnsi(internetCacheEntry.lpszLocalFileName);
                    entry.Extension = Marshal.PtrToStringAnsi(internetCacheEntry.lpszFileExtension);
                    entry.HeaderInfo = Marshal.PtrToStringAnsi(internetCacheEntry.lpHeaderInfo);
                    entry.HitRate = internetCacheEntry.dwHitRate.ToString();
                    entry.LastModifiedTime = FileTime2SystemTime(ref internetCacheEntry.LastModifiedTime).ToString();
                    entry.LastSyncTime = FileTime2SystemTime(ref internetCacheEntry.LastSyncTime).ToString();
                    entry.UseCount = internetCacheEntry.dwUseCount.ToString();

                    string url = entry.SourceUrlName;
                    int position = url.IndexOf("@");
                    if (position != -1)
                        url = url.Substring(position + 1);

                    if (!list.ContainsKey(url))
                        list.Add(url, entry);

                    returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                    if (!returnValue & ERROR_NO_MORE_ITEMS.Equals(Marshal.GetLastWin32Error()))
                    {
                        break;
                    }

                    if (!returnValue & cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize)
                    {
                        cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                        IntPtr tempIntPtr = new IntPtr(cacheEntryInfoBufferSize);
                        cacheEntryInfoBuffer = Marshal.ReAllocHGlobal(cacheEntryInfoBuffer, tempIntPtr);
                        returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                    }

                }

                Marshal.FreeHGlobal(cacheEntryInfoBuffer);

                return list;

            }

            private static DateTime FileTime2SystemTime(ref FILETIME FileT)
            {
                SYSTEMTIME SysT = new SYSTEMTIME();
                FileTimeToLocalFileTime(ref FileT, ref FileT);
                FileTimeToSystemTime(ref FileT, ref SysT);

          /*      if (SysT.wYear != 0)
                    return new DateTime(SysT.wYear, SysT.wMonth, SysT.wDay, SysT.wHour, SysT.wMinute, SysT.wSecond);*/

                return DateTime.MinValue;
            }

        }
    }
}