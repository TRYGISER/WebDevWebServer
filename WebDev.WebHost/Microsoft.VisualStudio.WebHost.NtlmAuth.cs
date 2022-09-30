namespace Microsoft.VisualStudio.WebHost
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

    internal sealed class NtlmAuth : IDisposable
    {
        private string _blob;
        private bool _completed;
        private SecHandle _credentialsHandle;
        private bool _credentialsHandleAcquired;
        private SecBuffer _inputBuffer;
        private SecBufferDesc _inputBufferDesc;
        private SecBuffer _outputBuffer;
        private SecBufferDesc _outputBufferDesc;
        private SecHandle _securityContext;
        private bool _securityContextAcquired;
        private uint _securityContextAttributes;
        private SecurityIdentifier _sid;
        private long _timestamp;
        private const int ISC_REQ_ALLOCATE_MEMORY = 0x100;
        private const int ISC_REQ_CONFIDENTIALITY = 0x10;
        private const int ISC_REQ_DELEGATE = 1;
        private const int ISC_REQ_MUTUAL_AUTH = 2;
        private const int ISC_REQ_PROMPT_FOR_CREDS = 0x40;
        private const int ISC_REQ_REPLAY_DETECT = 4;
        private const int ISC_REQ_SEQUENCE_DETECT = 8;
        private const int ISC_REQ_STANDARD_FLAGS = 20;
        private const int ISC_REQ_USE_SESSION_KEY = 0x20;
        private const int ISC_REQ_USE_SUPPLIED_CREDS = 0x80;
        private const int SEC_E_OK = 0;
        private const int SEC_I_COMPLETE_AND_CONTINUE = 0x90314;
        private const int SEC_I_COMPLETE_NEEDED = 0x90313;
        private const int SEC_I_CONTINUE_NEEDED = 0x90312;
        private const int SECBUFFER_DATA = 1;
        private const int SECBUFFER_EMPTY = 0;
        private const int SECBUFFER_TOKEN = 2;
        private const int SECBUFFER_VERSION = 0;
        private const int SECPKG_CRED_INBOUND = 1;
        private const int SECURITY_NETWORK_DREP = 0;

        public NtlmAuth()
        {
            if (AcquireCredentialsHandle(null, "NTLM", 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, ref this._credentialsHandle, ref this._timestamp) != 0)
            {
                throw new InvalidOperationException();
            }
            this._credentialsHandleAcquired = true;
        }

        [DllImport("SECUR32.DLL", CharSet=CharSet.Unicode)]
        private static extern int AcceptSecurityContext(ref SecHandle phCredential, IntPtr phContext, ref SecBufferDesc pInput, uint fContextReq, uint TargetDataRep, ref SecHandle phNewContext, ref SecBufferDesc pOutput, ref uint pfContextAttr, ref long ptsTimeStamp);
        [DllImport("SECUR32.DLL", CharSet=CharSet.Unicode)]
        private static extern int AcquireCredentialsHandle(string pszPrincipal, string pszPackage, uint fCredentialUse, IntPtr pvLogonID, IntPtr pAuthData, IntPtr pGetKeyFn, IntPtr pvGetKeyArgument, ref SecHandle phCredential, ref long ptsExpiry);
        public unsafe bool Authenticate(string blobString)
        {
            this._blob = null;
            byte[] buffer = Convert.FromBase64String(blobString);
            byte[] inArray = new byte[0x4000];
            fixed (void* voidRef = &this._securityContext)
            {
                fixed (void* voidRef2 = &this._inputBuffer)
                {
                    fixed (void* voidRef3 = &this._outputBuffer)
                    {
                        fixed (void* voidRef4 = buffer)
                        {
                            fixed (void* voidRef5 = inArray)
                            {
                                IntPtr zero = IntPtr.Zero;
                                if (this._securityContextAcquired)
                                {
                                    zero = (IntPtr) voidRef;
                                }
                                this._inputBufferDesc.ulVersion = 0;
                                this._inputBufferDesc.cBuffers = 1;
                                this._inputBufferDesc.pBuffers = (IntPtr) voidRef2;
                                this._inputBuffer.cbBuffer = (uint) buffer.Length;
                                this._inputBuffer.BufferType = 2;
                                this._inputBuffer.pvBuffer = (IntPtr) voidRef4;
                                this._outputBufferDesc.ulVersion = 0;
                                this._outputBufferDesc.cBuffers = 1;
                                this._outputBufferDesc.pBuffers = (IntPtr) voidRef3;
                                this._outputBuffer.cbBuffer = (uint) inArray.Length;
                                this._outputBuffer.BufferType = 2;
                                this._outputBuffer.pvBuffer = (IntPtr) voidRef5;
                                int num = AcceptSecurityContext(ref this._credentialsHandle, zero, ref this._inputBufferDesc, 20, 0, ref this._securityContext, ref this._outputBufferDesc, ref this._securityContextAttributes, ref this._timestamp);
                                if (num == 0x90312)
                                {
                                    this._securityContextAcquired = true;
                                    this._blob = Convert.ToBase64String(inArray, 0, (int) this._outputBuffer.cbBuffer);
                                }
                                else
                                {
                                    if (num != 0)
                                    {
                                        return false;
                                    }
                                    IntPtr phToken = IntPtr.Zero;
                                    if (QuerySecurityContextToken(ref this._securityContext, ref phToken) != 0)
                                    {
                                        return false;
                                    }
                                    try
                                    {
                                        using (WindowsIdentity identity = new WindowsIdentity(phToken))
                                        {
                                            this._sid = identity.User;
                                        }
                                    }
                                    finally
                                    {
                                        CloseHandle(phToken);
                                    }
                                    this._completed = true;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        [DllImport("KERNEL32.DLL", CharSet=CharSet.Unicode)]
        private static extern int CloseHandle(IntPtr phToken);
        [DllImport("SECUR32.DLL", CharSet=CharSet.Unicode)]
        private static extern int DeleteSecurityContext(ref SecHandle phContext);
        ~NtlmAuth()
        {
            this.FreeUnmanagedResources();
        }

        [DllImport("SECUR32.DLL", CharSet=CharSet.Unicode)]
        private static extern int FreeCredentialsHandle(ref SecHandle phCredential);
        private void FreeUnmanagedResources()
        {
            if (this._securityContextAcquired)
            {
                DeleteSecurityContext(ref this._securityContext);
            }
            if (this._credentialsHandleAcquired)
            {
                FreeCredentialsHandle(ref this._credentialsHandle);
            }
        }

        [DllImport("SECUR32.DLL", CharSet=CharSet.Unicode)]
        private static extern int QuerySecurityContextToken(ref SecHandle phContext, ref IntPtr phToken);
        void IDisposable.Dispose()
        {
            this.FreeUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        public string Blob
        {
            get
            {
                return this._blob;
            }
        }

        public bool Completed
        {
            get
            {
                return this._completed;
            }
        }

        public SecurityIdentifier SID
        {
            get
            {
                return this._sid;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SecBuffer
        {
            public uint cbBuffer;
            public uint BufferType;
            public IntPtr pvBuffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SecBufferDesc
        {
            public uint ulVersion;
            public uint cBuffers;
            public IntPtr pBuffers;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SecHandle
        {
            public IntPtr dwLower;
            public IntPtr dwUpper;
        }
    }
}
