using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;

namespace CertumSigningTest;

internal static class Program
{
    private static NativeMethods.AuthenticodeDigestSignEx? authenticodeDigestSign;

    private static readonly bool debug = false;

    private static void Main(string[] args)
    {
        try
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            const string thumbprint = "D75896DA61275CCA773682EA4622B9039BA3317F";
            const string message = "Certum private-key signing test";
            const string defaultTimestampUrl = "http://time.certum.pl";
            const string usage = "Usage: CertumSigningTest <relative-or-absolute-file-path> [RFC3161-timestamp-server-url]";

            char[] password = new char[] { '4', '6', '3', '5' };
            SecureString securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            securePassword.MakeReadOnly();

            if (debug) Console.WriteLine("Validating the command-line arguments.");

            if (args.Length is < 1 or > 2)
            {
                throw new ArgumentException(usage);
            }

            string filePath = Path.GetFullPath(args[0]);
            Console.WriteLine($"Signing {filePath}.");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified input file does not exist.", filePath);
            }

            if (debug) Console.WriteLine("Confirmed that the input file exists.");

            string timestampUrl = args.Length == 2 ? args[1] : defaultTimestampUrl;
            if (!Uri.TryCreate(timestampUrl, UriKind.Absolute, out Uri? timestampUri) ||
                (timestampUri.Scheme != Uri.UriSchemeHttp && timestampUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("The timestamp server URL must be an absolute HTTP or HTTPS URL.", nameof(args));
            }

            timestampUrl = timestampUri.AbsoluteUri;
            Console.WriteLine($"Using RFC 3161 timestamp server {timestampUrl}.");

            if (debug) Console.WriteLine($"Looking up certificate {thumbprint} in CurrentUser\\My.");

            using X509Store store = new(StoreName.My, StoreLocation.CurrentUser);
            if (debug) Console.WriteLine("Created the CurrentUser\\My certificate-store object.");

            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            if (debug) Console.WriteLine("Opened CurrentUser\\My read-only.");

            X509Certificate2Collection matchingCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
            if (debug) Console.WriteLine($"Found {matchingCertificates.Count} certificate(s) with the requested thumbprint.");

            if (matchingCertificates.Count == 0)
            {
                throw new InvalidOperationException($"No certificate with thumbprint {thumbprint} was found in CurrentUser\\My.");
            }

            using X509Certificate2 certificate = matchingCertificates[0];
            if (debug) Console.WriteLine($"Selected certificate: {certificate.Subject}.");
            if (debug) Console.WriteLine($"Certificate issuer: {certificate.Issuer}.");
            if (debug) Console.WriteLine($"Certificate has private key: {certificate.HasPrivateKey}.");

            if (!certificate.HasPrivateKey)
            {
                throw new CryptographicException("The selected certificate is not associated with a private key.");
            }

            if (debug) Console.WriteLine("Preparing direct legacy CSP access. The Certum PIN dialog may appear when the key is opened or used.");

            if (debug) Console.WriteLine("Configuring direct access to the crypto3 CSP Exchange key container.");
            CspParameters cspParameters = new(1, "crypto3 CSP", "C6994C9E2FDDBCCD89A53F8FDAE306715CA2606D")
            {
                Flags = CspProviderFlags.UseExistingKey,
                KeyNumber = (int)KeyNumber.Exchange,
                KeyPassword = securePassword
            };
            if (debug) Console.WriteLine("Opening the existing CSP key container directly.");
            using RSACryptoServiceProvider rsa = new(cspParameters);
            if (debug) Console.WriteLine($"Acquired RSA private key: {rsa.GetType().FullName}.");

            byte[] data = Encoding.UTF8.GetBytes(message);
            if (debug) Console.WriteLine($"Created {data.Length} bytes of UTF-8 test data.");

            if (debug) Console.WriteLine("Signing the test data using SHA-256 and PKCS#1 v1.5 padding.");

            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            if (debug) Console.WriteLine($"Signature created: {signature.Length} bytes.");

            using RSA publicKey = certificate.GetRSAPublicKey() ?? throw new CryptographicException("The selected certificate did not provide an RSA public key.");
            if (debug) Console.WriteLine("Acquired the certificate RSA public key for signature verification.");

            bool signatureIsValid = publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            if (debug) Console.WriteLine($"Signature verification using the certificate public key: {signatureIsValid}.");

            if (!signatureIsValid)
            {
                throw new CryptographicException("The generated signature did not verify with the certificate public key.");
            }

            using X509Certificate2 signerCertificateContext = X509CertificateLoader.LoadCertificate(certificate.RawData);
            if (debug) Console.WriteLine("Prepared the public signing certificate. The private key remains in the crypto3 CSP.");

            using X509Chain certificateChain = new();
            certificateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            certificateChain.Build(certificate);
            byte[][] chainCertificates = new byte[certificateChain.ChainElements.Count][];
            for (int index = 0; index < certificateChain.ChainElements.Count; index++)
            {
                chainCertificates[index] = certificateChain.ChainElements[index].Certificate.RawData;
            }

            if (debug) Console.WriteLine("Binding the selected certificate to the Authenticode signing request.");
            NativeMethods.SignerCertificateStoreInfo signerCertificateStoreInfo = new()
            {
                Size = (uint)Marshal.SizeOf<NativeMethods.SignerCertificateStoreInfo>(),
                SigningCertificate = signerCertificateContext.Handle,
                CertificatePolicy = NativeMethods.SignerCertPolicyChain,
                CertificateStore = IntPtr.Zero
            };
            IntPtr signerCertificateStoreInfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf<NativeMethods.SignerCertificateStoreInfo>());
            Marshal.StructureToPtr(signerCertificateStoreInfo, signerCertificateStoreInfoPointer, false);
            if (debug) Console.WriteLine("Bound the certificate context for Authenticode signing.");

            NativeMethods.SignerCertificate signerCertificate = new()
            {
                Size = (uint)Marshal.SizeOf<NativeMethods.SignerCertificate>(),
                CertificateChoice = NativeMethods.SignerCertStore,
                CertificateStoreInfo = signerCertificateStoreInfoPointer,
                WindowHandle = IntPtr.Zero
            };
            if (debug) Console.WriteLine("Configured the certificate-store signing source.");

            IntPtr signerFileInfoPointer = IntPtr.Zero;
            IntPtr signerSubjectIndexPointer = IntPtr.Zero;
            IntPtr signerContext = IntPtr.Zero;
            IntPtr timestampAlgorithmOidPointer = IntPtr.Zero;
            IntPtr timestampUrlPointer = IntPtr.Zero;
            NativeMethods.AuthenticodeDigestSignEx digestSign = (metadata, digestAlgorithm, digest, digestLength, signedDigest, signerCertificatePointer, certificateChainStore) =>
                SignAuthenticodeDigest(rsa, signerCertificateContext.Handle, chainCertificates, digestAlgorithm, digest, digestLength, signedDigest, signerCertificatePointer, certificateChainStore);
            authenticodeDigestSign = digestSign;

            try
            {
                if (debug) Console.WriteLine("Preparing the existing file for Authenticode signing.");
                NativeMethods.SignerFileInfo signerFileInfo = new()
                {
                    Size = (uint)Marshal.SizeOf<NativeMethods.SignerFileInfo>(),
                    FileName = filePath,
                    FileHandle = IntPtr.Zero
                };
                signerFileInfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf<NativeMethods.SignerFileInfo>());
                Marshal.StructureToPtr(signerFileInfo, signerFileInfoPointer, false);
                if (debug) Console.WriteLine("Created the Authenticode file subject.");

                signerSubjectIndexPointer = Marshal.AllocHGlobal(Marshal.SizeOf<uint>());
                Marshal.WriteInt32(signerSubjectIndexPointer, 0);
                NativeMethods.SignerSubjectInfo signerSubjectInfo = new()
                {
                    Size = (uint)Marshal.SizeOf<NativeMethods.SignerSubjectInfo>(),
                    Index = signerSubjectIndexPointer,
                    SubjectChoice = NativeMethods.SignerSubjectFile,
                    Subject = signerFileInfoPointer
                };

                NativeMethods.SignerSignatureInfo signerSignatureInfo = new()
                {
                    Size = (uint)Marshal.SizeOf<NativeMethods.SignerSignatureInfo>(),
                    HashAlgorithm = NativeMethods.CalgSha256,
                    AttributeChoice = NativeMethods.SignerNoAttributes,
                    AttributeAuthCode = IntPtr.Zero,
                    AuthenticatedAttributes = IntPtr.Zero,
                    UnauthenticatedAttributes = IntPtr.Zero
                };
                if (debug) Console.WriteLine("Configured SHA-256 Authenticode signing.");

                NativeMethods.SignerDigestSignInfo digestSignInfo = new()
                {
                    Size = (uint)Marshal.SizeOf<NativeMethods.SignerDigestSignInfo>(),
                    Choice = NativeMethods.DigestSignEx,
                    Callback = Marshal.GetFunctionPointerForDelegate(digestSign),
                    Metadata = IntPtr.Zero,
                    Reserved = 0,
                    Reserved2 = 0,
                    Reserved3 = 0
                };
                if (debug) Console.WriteLine("Configured the crypto3 CSP to sign the Authenticode digest directly.");

                timestampAlgorithmOidPointer = Marshal.StringToHGlobalAnsi(NativeMethods.Sha256Oid);
                timestampUrlPointer = Marshal.StringToHGlobalUni(timestampUrl);
                if (debug) Console.WriteLine("Configured RFC 3161 SHA-256 timestamping.");

                if (debug) Console.WriteLine("Calling the Windows Authenticode signer and timestamp server. The Certum PIN dialog may appear now.");
                int signingResult = NativeMethods.SignerSignEx3(
                    NativeMethods.SpcDigestSignExFlag,
                    ref signerSubjectInfo,
                    ref signerCertificate,
                    ref signerSignatureInfo,
                    IntPtr.Zero,
                    NativeMethods.SignerTimestampRfc3161,
                    timestampAlgorithmOidPointer,
                    timestampUrlPointer,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    out signerContext,
                    IntPtr.Zero,
                    ref digestSignInfo,
                    IntPtr.Zero);

                if (signingResult != 0)
                {
                    throw new ExternalException($"The Windows Authenticode signer failed with HRESULT 0x{signingResult:X8}.", signingResult);
                }

                if (debug) Console.WriteLine("The Windows Authenticode signer and RFC 3161 timestamping completed successfully.");
            }
            finally
            {
                if (signerContext != IntPtr.Zero)
                {
                    NativeMethods.SignerFreeSignerContext(signerContext);
                    if (debug) Console.WriteLine("Released the Authenticode signer context.");
                }

                if (signerSubjectIndexPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(signerSubjectIndexPointer);
                    if (debug) Console.WriteLine("Released the Authenticode subject-index resources.");
                }

                authenticodeDigestSign = null;

                if (timestampUrlPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(timestampUrlPointer);
                    if (debug) Console.WriteLine("Released the timestamp-server URL resources.");
                }

                if (timestampAlgorithmOidPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(timestampAlgorithmOidPointer);
                    if (debug) Console.WriteLine("Released the timestamp-algorithm resources.");
                }

                if (signerFileInfoPointer != IntPtr.Zero)
                {
                    Marshal.DestroyStructure<NativeMethods.SignerFileInfo>(signerFileInfoPointer);
                    Marshal.FreeHGlobal(signerFileInfoPointer);
                    if (debug) Console.WriteLine("Released the Authenticode file-subject resources.");
                }

                Marshal.DestroyStructure<NativeMethods.SignerCertificateStoreInfo>(signerCertificateStoreInfoPointer);
                Marshal.FreeHGlobal(signerCertificateStoreInfoPointer);
                if (debug) Console.WriteLine("Released the Authenticode certificate resources.");
            }

            //Console.WriteLine("Verifying the Authenticode signature with Windows trust validation.");
            //NativeMethods.WinTrustFileInfo verificationFileInfo = new()
            //{
            //    Size = (uint)Marshal.SizeOf<NativeMethods.WinTrustFileInfo>(),
            //    FilePath = filePath,
            //    FileHandle = IntPtr.Zero,
            //    KnownSubject = IntPtr.Zero
            //};
            //IntPtr verificationFileInfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf<NativeMethods.WinTrustFileInfo>());
            //Marshal.StructureToPtr(verificationFileInfo, verificationFileInfoPointer, false);

            //try
            //{
            //    NativeMethods.WinTrustData verificationData = new()
            //    {
            //        Size = (uint)Marshal.SizeOf<NativeMethods.WinTrustData>(),
            //        PolicyCallbackData = IntPtr.Zero,
            //        SipClientData = IntPtr.Zero,
            //        UiChoice = NativeMethods.WinTrustUiNone,
            //        RevocationChecks = NativeMethods.WinTrustRevokeNone,
            //        UnionChoice = NativeMethods.WinTrustChoiceFile,
            //        FileInfo = verificationFileInfoPointer,
            //        StateAction = NativeMethods.WinTrustStateActionIgnore,
            //        StateData = IntPtr.Zero,
            //        UrlReference = IntPtr.Zero,
            //        ProviderFlags = 0,
            //        UiContext = 0,
            //        SignatureSettings = IntPtr.Zero
            //    };

            //    int verificationResult = NativeMethods.WinVerifyTrust(IntPtr.Zero, ref NativeMethods.WinTrustActionGenericVerifyV2, ref verificationData);
            //    if (verificationResult != 0)
            //    {
            //        throw new ExternalException($"Windows trust validation failed with HRESULT 0x{verificationResult:X8}.", verificationResult);
            //    }

            //    Console.WriteLine("Windows trust validation succeeded.");
            //}
            //finally
            //{
            //    Marshal.DestroyStructure<NativeMethods.WinTrustFileInfo>(verificationFileInfoPointer);
            //    Marshal.FreeHGlobal(verificationFileInfoPointer);
            //    Console.WriteLine("Released the Windows trust-validation resources.");
            //}

            //using X509Certificate2 embeddedSignerCertificate = new(X509Certificate.CreateFromSignedFile(filePath));
            //Console.WriteLine($"Authenticode signer subject: {embeddedSignerCertificate.Subject}.");
            //Console.WriteLine($"Authenticode signer thumbprint: {embeddedSignerCertificate.Thumbprint}.");

            stopwatch.Stop();
            Console.WriteLine($"Authenticode file signing completed successfully in {stopwatch.Elapsed.TotalSeconds:0.0}s.");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("Private-key signing test failed.");
            Console.Error.WriteLine($"Exception type: {exception.GetType().FullName}");
            Console.Error.WriteLine($"Message: {exception.Message}");
            Console.Error.WriteLine(exception);
            Environment.ExitCode = 1;
        }

        if (debug)
        {
            if (!Console.IsInputRedirected && !Console.IsOutputRedirected)
            {
                Console.ReadKey();
            }
        }
    }

    private static int SignAuthenticodeDigest(
        RSACryptoServiceProvider signingKey,
        IntPtr publicCertificate,
        byte[][] chainCertificates,
        uint digestAlgorithm,
        IntPtr digest,
        uint digestLength,
        IntPtr signedDigest,
        IntPtr signerCertificatePointer,
        IntPtr certificateChainStore)
    {
        try
        {
            if (digestAlgorithm != NativeMethods.CalgSha256)
            {
                Console.Error.WriteLine($"The Authenticode digest algorithm 0x{digestAlgorithm:X8} is not SHA-256.");
                return unchecked((int)0x80090027);
            }

            if (debug) Console.WriteLine($"Signing the {digestLength}-byte Authenticode digest with the crypto3 CSP Exchange key.");
            byte[] hash = new byte[digestLength];
            Marshal.Copy(digest, hash, 0, hash.Length);
            byte[] signature = signingKey.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            IntPtr signatureBuffer = NativeMethods.HeapAlloc(NativeMethods.GetProcessHeap(), 0, (UIntPtr)signature.Length);
            if (signatureBuffer == IntPtr.Zero)
            {
                return unchecked((int)0x8007000E);
            }

            Marshal.Copy(signature, 0, signatureBuffer, signature.Length);
            Marshal.WriteInt32(signedDigest, signature.Length);
            Marshal.WriteIntPtr(signedDigest, IntPtr.Size, signatureBuffer);
            Marshal.WriteIntPtr(signerCertificatePointer, NativeMethods.CertDuplicateCertificateContext(publicCertificate));

            if (certificateChainStore != IntPtr.Zero)
            {
                foreach (byte[] encodedCertificate in chainCertificates)
                {
                    IntPtr certificateContext = NativeMethods.CertCreateCertificateContext(NativeMethods.CertEncoding, encodedCertificate, (uint)encodedCertificate.Length);
                    if (certificateContext == IntPtr.Zero)
                    {
                        continue;
                    }

                    NativeMethods.CertAddCertificateContextToStore(certificateChainStore, certificateContext, NativeMethods.CertStoreAddAlways, out IntPtr storedContext);
                    if (storedContext != IntPtr.Zero)
                    {
                        NativeMethods.CertFreeCertificateContext(storedContext);
                    }

                    NativeMethods.CertFreeCertificateContext(certificateContext);
                }
            }

            if (debug) Console.WriteLine($"Created a {signature.Length}-byte Authenticode signature with the CSP key.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Authenticode digest signing failed: {exception.Message}");
            return exception.HResult;
        }
    }

    internal static class NativeMethods
    {
        internal const uint SignerSubjectFile = 1;
        internal const uint SignerCertStore = 2;
        internal const uint SignerNoAttributes = 0;
        internal const uint SignerCertPolicyChain = 2;
        internal const uint CalgSha256 = 0x0000800C;
        internal const uint SpcDigestSignExFlag = 0x4000;
        internal const uint SignerTimestampRfc3161 = 0x00000002;
        internal const uint DigestSignEx = 3;
        internal const uint CertEncoding = 0x00010001;
        internal const uint CertStoreAddAlways = 4;
        internal const string Sha256Oid = "2.16.840.1.101.3.4.2.1";

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate int AuthenticodeDigestSignEx(
            IntPtr metadata,
            uint digestAlgorithm,
            IntPtr digest,
            uint digestLength,
            IntPtr signedDigest,
            IntPtr signerCertificate,
            IntPtr certificateChainStore);

        [DllImport("mssign32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SignerSignEx3(
            uint flags,
            ref SignerSubjectInfo subjectInfo,
            ref SignerCertificate signerCertificate,
            ref SignerSignatureInfo signatureInfo,
            IntPtr providerInfo,
            uint timestampFlags,
            IntPtr timestampAlgorithmOid,
            IntPtr timestampUrl,
            IntPtr request,
            IntPtr sipData,
            out IntPtr signerContext,
            IntPtr cryptoPolicy,
            ref SignerDigestSignInfo digestSignInfo,
            IntPtr reserved);

        [DllImport("mssign32.dll")]
        internal static extern int SignerFreeSignerContext(IntPtr signerContext);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetProcessHeap();

        [DllImport("kernel32.dll")]
        internal static extern IntPtr HeapAlloc(IntPtr heap, uint flags, UIntPtr size);

        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern IntPtr CertDuplicateCertificateContext(IntPtr certificateContext);

        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern IntPtr CertCreateCertificateContext(uint encoding, byte[] encodedCertificate, uint length);

        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern bool CertAddCertificateContextToStore(IntPtr store, IntPtr certificateContext, uint disposition, out IntPtr storedContext);

        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern bool CertFreeCertificateContext(IntPtr certificateContext);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SignerFileInfo
        {
            internal uint Size;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string FileName;
            internal IntPtr FileHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerSubjectInfo
        {
            internal uint Size;
            internal IntPtr Index;
            internal uint SubjectChoice;
            internal IntPtr Subject;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerCertificateStoreInfo
        {
            internal uint Size;
            internal IntPtr SigningCertificate;
            internal uint CertificatePolicy;
            internal IntPtr CertificateStore;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerCertificate
        {
            internal uint Size;
            internal uint CertificateChoice;
            internal IntPtr CertificateStoreInfo;
            internal IntPtr WindowHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerSignatureInfo
        {
            internal uint Size;
            internal uint HashAlgorithm;
            internal uint AttributeChoice;
            internal IntPtr AttributeAuthCode;
            internal IntPtr AuthenticatedAttributes;
            internal IntPtr UnauthenticatedAttributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerDigestSignInfo
        {
            internal uint Size;
            internal uint Choice;
            internal IntPtr Callback;
            internal IntPtr Metadata;
            internal uint Reserved;
            internal uint Reserved2;
            internal uint Reserved3;
        }

        internal const uint WinTrustUiNone = 2;
        internal const uint WinTrustRevokeNone = 0;
        internal const uint WinTrustChoiceFile = 1;
        internal const uint WinTrustStateActionIgnore = 0;
        internal static Guid WinTrustActionGenericVerifyV2 = new("00AAC56B-CD44-11D0-8CC2-00C04FC295EE");

        [DllImport("wintrust.dll", ExactSpelling = true)]
        internal static extern int WinVerifyTrust(
            IntPtr windowHandle,
            ref Guid actionId,
            ref WinTrustData trustData);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WinTrustFileInfo
        {
            internal uint Size;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string FilePath;
            internal IntPtr FileHandle;
            internal IntPtr KnownSubject;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WinTrustData
        {
            internal uint Size;
            internal IntPtr PolicyCallbackData;
            internal IntPtr SipClientData;
            internal uint UiChoice;
            internal uint RevocationChecks;
            internal uint UnionChoice;
            internal IntPtr FileInfo;
            internal uint StateAction;
            internal IntPtr StateData;
            internal IntPtr UrlReference;
            internal uint ProviderFlags;
            internal uint UiContext;
            internal IntPtr SignatureSettings;
        }
    }

}
