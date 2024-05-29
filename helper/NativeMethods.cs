using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace usbip_tunnel.helper
{
    public static class NativeMethods
    {
        private const string ADVAPI32 = "advapi32.dll";
        private const string CRYPT32 = "crypt32.dll";
        private const string KERNEL32 = "kernel32.dll";

        public const UInt32 GENERIC_WRITE = 0x40000000;
        public const UInt32 GENERIC_READ = 0x80000000;
        public const UInt32 FILE_SHARE_READ = 0x00000001;
        public const UInt32 FILE_SHARE_WRITE = 0x00000002;
        public const UInt32 OPEN_EXISTING = 0x00000003;
        public const UInt32 FILE_ATTRIBUTE_NORMAL = 0x80;
        public const UInt32 ERROR_ACCESS_DENIED = 5;
        public const UInt32 ATTACH_PARRENT = 0xFFFFFFFF;


        public const UInt32 STD_OUTPUT_HANDLE = 0xFFFFFFF5;
        public const UInt32 STD_ERROR_HANDLE = 0xFFFFFFF4;
        public const UInt32 DUPLICATE_SAME_ACCESS = 2;

        public const int MY_CODE_PAGE = 437;

        public const int HWND_BROADCAST = 0xFFFF;
        public const int WM_COPYDATA = 0x004A;

        public const uint AT_KEYEXCHANGE = 0x00000001;
        public const uint AT_SIGNATURE = 0x00000002;

        public const string OID_RSA_SHA256RSA = "1.2.840.113549.1.1.11";
        public const string szOID_ENHANCED_KEY_USAGE = "2.5.29.37";

        public const uint CERT_X500_NAME_STR = 3;
        public const uint X509_ASN_ENCODING = 0x00000001;
        public const uint PKCS_7_ASN_ENCODING = 0x00010000;
        public const uint CRYPT_VERIFYCONTEXT = 0xF0000000;	 //no private key access flag

        public const ulong X509_CERT_CRL_TO_BE_SIGNED = 3; // CRL_INFO
        public const ulong X509_CERT_REQUEST_TO_BE_SIGNED = 4; // CERT_REQUEST_INFO
        public const ulong X509_CERT_TO_BE_SIGNED = 2; // CERT_INFO
        public const ulong X509_KEYGEN_REQUEST_TO_BE_SIGNED = 21; // CERT_KEYGEN_REQUEST_INFO

        public struct BY_HANDLE_FILE_INFORMATION
        {
            public UInt32 FileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
            public UInt32 VolumeSerialNumber;
            public UInt32 FileSizeHigh;
            public UInt32 FileSizeLow;
            public UInt32 NumberOfLinks;
            public UInt32 FileIndexHigh;
            public UInt32 FileIndexLow;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public int dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        [Flags]
        public enum AnimateWindowFlags
        {
            AW_HOR_POSITIVE = 0x00000001,
            AW_HOR_NEGATIVE = 0x00000002,
            AW_VER_POSITIVE = 0x00000004,
            AW_VER_NEGATIVE = 0x00000008,
            AW_CENTER = 0x00000010,
            AW_HIDE = 0x00010000,
            AW_ACTIVATE = 0x00020000,
            AW_SLIDE = 0x00040000,
            AW_BLEND = 0x00080000
        }

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateFileW(string lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalAlloc(int flag, int size);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr p);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool AnimateWindow(IntPtr hWnd, int time, AnimateWindowFlags flags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", EntryPoint = "AttachConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 AttachConsole(UInt32 dwProcessId);

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern SafeFileHandle GetStdHandle(UInt32 nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern bool SetStdHandle(UInt32 nStdHandle, SafeFileHandle hHandle);

        [DllImport("kernel32.dll")]
        public static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        [DllImport("kernel32.dll")]
        public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, SafeFileHandle hSourceHandle, IntPtr hTargetProcessHandle, out SafeFileHandle lpTargetHandle, UInt32 dwDesiredAccess, Boolean bInheritHandle, UInt32 dwOptions);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptSignAndEncodeCertificate(IntPtr hCryptProvOrNCryptKey, uint dwKeySpec, uint dwCertEncodingType, ulong lpszStructType, IntPtr pvStructInfo, ref CRYPT_ALGORITHM_IDENTIFIER pSignatureAlgorithm, IntPtr pvHashAuxInfo, byte[] pbEncoded, ref uint pcbEncoded);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern IntPtr CertCreateSelfSignCertificate(IntPtr hProv, ref CERT_NAME_BLOB pSubjectIssuerBlob, uint dwFlagsm, ref CRYPT_KEY_PROV_INFO pKeyProvInfo, ref CRYPT_ALGORITHM_IDENTIFIER pSignatureAlgorithm, ref SYSTEM_TIME pStartTime, ref SYSTEM_TIME pEndTime, IntPtr other);

        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CertStrToName(uint dwCertEncodingType, string pszX500, uint dwStrType, IntPtr pvReserved, [In, Out] byte[] pbEncoded, ref uint pcbEncoded, IntPtr other);

        [DllImport(ADVAPI32)]
        public static extern bool CryptEnumProviders(int dwIndex, IntPtr pdwReserved, int dwFlags, ref int pdwProvType, StringBuilder pszProvName, ref int pcbProvName);

        [DllImport(ADVAPI32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CryptAcquireContext(ref IntPtr hProv, string pszContainer, string pszProvider, uint dwProvType, uint dwFlags);

        [DllImport(ADVAPI32)]
        public static extern bool CryptReleaseContext(IntPtr hProv, uint dwFlags);

        [DllImport(CRYPT32, SetLastError = true)]
        public static extern bool CertGetCertificateContextProperty(IntPtr pCertContext, uint dwPropId, IntPtr pvData, ref uint pcbData);

        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LocalAlloc([In] uint uFlags, [In] IntPtr sizetdwBytes);

        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_CONTEXT
        {
            public uint dwCertEncodingType;
            public IntPtr pbCertEncoded;
            public int cbCertEncoded;
            public IntPtr pCertInfo;
            public IntPtr hCertStore;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_INFO
        {
            public uint dwVersion;
            public CRYPTOAPI_BLOB SerialNumber;
            public CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
            public CRYPTOAPI_BLOB Issuer;
            public FILETIME NotBefore;
            public FILETIME NotAfter;
            public CRYPTOAPI_BLOB Subject;
            public CERT_PUBLIC_KEY_INFO SubjectPublicKeyInfo;
            public CRYPTOAPI_BLOB IssuerUniqueId;
            public CRYPTOAPI_BLOB SubjectUniqueId;
            public uint cExtension;
            public IntPtr rgExtension;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CERT_EXTENSION
        {
            public IntPtr pszObjId;
            public bool fCritical;
            public NativeMethods.CRYPTOAPI_BLOB Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_EXTENSIONS
        {
            public uint cExtension;
            public IntPtr rgExtension;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_NAME_BLOB : IDisposable
        {
            public int _cbData;
            public SafeGlobalMemoryHandle _pbData;

            public CERT_NAME_BLOB(int cb, SafeGlobalMemoryHandle handle)
            {
                this._cbData = cb;
                this._pbData = handle;
            }

            public void CopyData(byte[] encodedName)
            {
                this._pbData = new SafeGlobalMemoryHandle(encodedName);
                this._cbData = encodedName.Length;
            }

            public void Dispose()
            {
                if (this._pbData != null)
                {
                    this._pbData.Dispose();
                    this._pbData = null;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPT_ALGORITHM_IDENTIFIER
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pszObjId;
            public NativeMethods.CRYPTOAPI_BLOB parameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPT_KEY_PROV_INFO
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszContainerName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszProvName;
            public uint dwProvType;
            public uint dwFlags;
            public uint cProvParam;
            public IntPtr rgProvParam;
            public uint dwKeySpec;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_PUBLIC_KEY_INFO
        {
            public String SubjPKIAlgpszObjId;
            public int SubjPKIAlgParameterscbData;
            public IntPtr SubjPKIAlgParameterspbData;
            public int PublicKeycbData;
            public IntPtr PublicKeypbData;
            public int PublicKeycUnusedBits;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CRYPTOAPI_BLOB
        {
            public uint cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_TIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;

            public SYSTEM_TIME(DateTime dt)
            {
                this.wYear = (ushort)dt.Year;
                this.wMonth = (ushort)dt.Month;
                this.wDay = (ushort)dt.Day;
                this.wDayOfWeek = (ushort)dt.DayOfWeek;
                this.wHour = (ushort)dt.Hour;
                this.wMinute = (ushort)dt.Minute;
                this.wSecond = (ushort)dt.Second;
                this.wMilliseconds = (ushort)dt.Millisecond;
            }
        }

        public class SafeGlobalMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeGlobalMemoryHandle()
                : base(true)
            {
            }

            public SafeGlobalMemoryHandle(byte[] data)
                : base(true)
            {
                base.handle = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, base.handle, data.Length);
            }

            private SafeGlobalMemoryHandle(IntPtr handle)
                : base(true)
            {
                base.SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                Marshal.FreeHGlobal(base.handle);
                return true;
            }
        }
    }
}
