using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Runtime.InteropServices;

namespace CertumSigningTest;

internal static class Program
{
    private static void Main(string[] args)
    {
        try
        {
            const string thumbprint = "D75896DA61275CCA773682EA4622B9039BA3317F";
            const string message = "Certum private-key signing test";
            const string usage = "Usage: CertumSigningTest <relative-or-absolute-file-path>";
            Console.WriteLine("Validating the command-line file argument.");

            if (args.Length != 1)
            {
                throw new ArgumentException(usage);
            }

            string filePath = Path.GetFullPath(args[0]);
            Console.WriteLine($"Resolved the input file path to {filePath}.");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified input file does not exist.", filePath);
            }

            Console.WriteLine("Confirmed that the input file exists.");

            Console.WriteLine($"Looking up certificate {thumbprint} in CurrentUser\\My.");

            using X509Store store = new(StoreName.My, StoreLocation.CurrentUser);
            Console.WriteLine("Created the CurrentUser\\My certificate-store object.");

            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            Console.WriteLine("Opened CurrentUser\\My read-only.");

            X509Certificate2Collection matchingCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
            Console.WriteLine($"Found {matchingCertificates.Count} certificate(s) with the requested thumbprint.");

            if (matchingCertificates.Count == 0)
            {
                throw new InvalidOperationException($"No certificate with thumbprint {thumbprint} was found in CurrentUser\\My.");
            }

            using X509Certificate2 certificate = matchingCertificates[0];
            Console.WriteLine($"Selected certificate: {certificate.Subject}.");
            Console.WriteLine($"Certificate issuer: {certificate.Issuer}.");
            Console.WriteLine($"Certificate has private key: {certificate.HasPrivateKey}.");

            Console.WriteLine("Preparing direct legacy CSP access. The Certum PIN dialog may appear when the key is opened or used.");

            Console.WriteLine("Configuring direct access to the crypto3 CSP Exchange key container.");
            CspParameters cspParameters = new(1, "crypto3 CSP", "C6994C9E2FDDBCCD89A53F8FDAE306715CA2606D")
            {
                Flags = CspProviderFlags.UseExistingKey,
                KeyNumber = (int)KeyNumber.Exchange
            };
            Console.WriteLine("Opening the existing CSP key container directly.");
            using RSACryptoServiceProvider rsa = new(cspParameters);
            Console.WriteLine($"Acquired RSA private key: {rsa.GetType().FullName}.");

            byte[] data = Encoding.UTF8.GetBytes(message);
            Console.WriteLine($"Created {data.Length} bytes of UTF-8 test data.");

            Console.WriteLine("Signing the test data using SHA-256 and PKCS#1 v1.5 padding.");

            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            Console.WriteLine($"Signature created: {signature.Length} bytes.");

            using RSA publicKey = certificate.GetRSAPublicKey() ?? throw new CryptographicException("The selected certificate did not provide an RSA public key.");
            Console.WriteLine("Acquired the certificate RSA public key for signature verification.");

            bool signatureIsValid = publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            Console.WriteLine($"Signature verification using the certificate public key: {signatureIsValid}.");

            if (!signatureIsValid)
            {
                throw new CryptographicException("The generated signature did not verify with the certificate public key.");
            }

            Console.WriteLine("Binding the selected certificate to the Authenticode signing request.");
            NativeMethods.SignerCertificateStoreInfo signerCertificateStoreInfo = new()
            {
                Size = (uint)Marshal.SizeOf<NativeMethods.SignerCertificateStoreInfo>(),
                SigningCertificate = certificate.Handle,
                CertificatePolicy = NativeMethods.SignerCertPolicyChain,
                CertificateStore = IntPtr.Zero
            };
            IntPtr signerCertificateStoreInfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf<NativeMethods.SignerCertificateStoreInfo>());
            Marshal.StructureToPtr(signerCertificateStoreInfo, signerCertificateStoreInfoPointer, false);
            Console.WriteLine("Bound the certificate context for Authenticode signing.");

            NativeMethods.SignerCertificate signerCertificate = new()
            {
                Size = (uint)Marshal.SizeOf<NativeMethods.SignerCertificate>(),
                CertificateChoice = NativeMethods.SignerCertStore,
                CertificateStoreInfo = signerCertificateStoreInfoPointer,
                WindowHandle = IntPtr.Zero
            };
            Console.WriteLine("Configured the certificate-store signing source.");

            NativeMethods.SignerProviderInfo signerProviderInfo = new()
            {
                Size = (uint)Marshal.SizeOf<NativeMethods.SignerProviderInfo>(),
                ProviderName = "crypto3 CSP",
                ProviderType = 1,
                KeySpec = (uint)KeyNumber.Exchange,
                PrivateKeyChoice = NativeMethods.SignerProviderKeyContainer,
                KeyContainer = "C6994C9E2FDDBCCD89A53F8FDAE306715CA2606D"
            };
            Console.WriteLine("Bound crypto3 CSP and the Exchange key container to the signing request.");

            IntPtr signerFileInfoPointer = IntPtr.Zero;
            IntPtr signerSubjectIndexPointer = IntPtr.Zero;
            IntPtr signerContext = IntPtr.Zero;

            try
            {
                Console.WriteLine("Preparing the existing file for Authenticode signing.");
                NativeMethods.SignerFileInfo signerFileInfo = new()
                {
                    Size = (uint)Marshal.SizeOf<NativeMethods.SignerFileInfo>(),
                    FileName = filePath,
                    FileHandle = IntPtr.Zero
                };
                signerFileInfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf<NativeMethods.SignerFileInfo>());
                Marshal.StructureToPtr(signerFileInfo, signerFileInfoPointer, false);
                Console.WriteLine("Created the Authenticode file subject.");

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
                Console.WriteLine("Configured SHA-256 Authenticode signing.");

                Console.WriteLine("Calling the Windows Authenticode signer. The Certum PIN dialog may appear now.");
                int signingResult = NativeMethods.SignerSignEx(
                    0,
                    ref signerSubjectInfo,
                    ref signerCertificate,
                    ref signerSignatureInfo,
                    ref signerProviderInfo,
                    null,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    out signerContext);

                if (signingResult != 0)
                {
                    throw new ExternalException($"The Windows Authenticode signer failed with HRESULT 0x{signingResult:X8}.", signingResult);
                }

                Console.WriteLine("The Windows Authenticode signer completed successfully.");
            }
            finally
            {
                if (signerContext != IntPtr.Zero)
                {
                    NativeMethods.SignerFreeSignerContext(signerContext);
                    Console.WriteLine("Released the Authenticode signer context.");
                }

                if (signerSubjectIndexPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(signerSubjectIndexPointer);
                    Console.WriteLine("Released the Authenticode subject-index resources.");
                }

                if (signerFileInfoPointer != IntPtr.Zero)
                {
                    Marshal.DestroyStructure<NativeMethods.SignerFileInfo>(signerFileInfoPointer);
                    Marshal.FreeHGlobal(signerFileInfoPointer);
                    Console.WriteLine("Released the Authenticode file-subject resources.");
                }

                Marshal.DestroyStructure<NativeMethods.SignerCertificateStoreInfo>(signerCertificateStoreInfoPointer);
                Marshal.FreeHGlobal(signerCertificateStoreInfoPointer);
                Console.WriteLine("Released the Authenticode certificate resources.");
            }

            Console.WriteLine("Verifying the Authenticode signature with Windows trust validation.");
            NativeMethods.WinTrustFileInfo verificationFileInfo = new()
            {
                Size = (uint)Marshal.SizeOf<NativeMethods.WinTrustFileInfo>(),
                FilePath = filePath,
                FileHandle = IntPtr.Zero,
                KnownSubject = IntPtr.Zero
            };
            IntPtr verificationFileInfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf<NativeMethods.WinTrustFileInfo>());
            Marshal.StructureToPtr(verificationFileInfo, verificationFileInfoPointer, false);

            try
            {
                NativeMethods.WinTrustData verificationData = new()
                {
                    Size = (uint)Marshal.SizeOf<NativeMethods.WinTrustData>(),
                    PolicyCallbackData = IntPtr.Zero,
                    SipClientData = IntPtr.Zero,
                    UiChoice = NativeMethods.WinTrustUiNone,
                    RevocationChecks = NativeMethods.WinTrustRevokeNone,
                    UnionChoice = NativeMethods.WinTrustChoiceFile,
                    FileInfo = verificationFileInfoPointer,
                    StateAction = NativeMethods.WinTrustStateActionIgnore,
                    StateData = IntPtr.Zero,
                    UrlReference = IntPtr.Zero,
                    ProviderFlags = 0,
                    UiContext = 0,
                    SignatureSettings = IntPtr.Zero
                };

                int verificationResult = NativeMethods.WinVerifyTrust(IntPtr.Zero, ref NativeMethods.WinTrustActionGenericVerifyV2, ref verificationData);
                if (verificationResult != 0)
                {
                    throw new ExternalException($"Windows trust validation failed with HRESULT 0x{verificationResult:X8}.", verificationResult);
                }

                Console.WriteLine("Windows trust validation succeeded.");
            }
            finally
            {
                Marshal.DestroyStructure<NativeMethods.WinTrustFileInfo>(verificationFileInfoPointer);
                Marshal.FreeHGlobal(verificationFileInfoPointer);
                Console.WriteLine("Released the Windows trust-validation resources.");
            }

            using X509Certificate2 embeddedSignerCertificate = new(X509Certificate.CreateFromSignedFile(filePath));
            Console.WriteLine($"Authenticode signer subject: {embeddedSignerCertificate.Subject}.");
            Console.WriteLine($"Authenticode signer thumbprint: {embeddedSignerCertificate.Thumbprint}.");

            Console.WriteLine("Authenticode file signing completed successfully.");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("Private-key signing test failed.");
            Console.Error.WriteLine($"Exception type: {exception.GetType().FullName}");
            Console.Error.WriteLine($"Message: {exception.Message}");
            Console.Error.WriteLine(exception);
            Environment.ExitCode = 1;
        }

        Console.ReadKey();
    }


    internal static class NativeMethods
    {
        internal const uint SignerSubjectFile = 1;
        internal const uint SignerCertStore = 2;
        internal const uint SignerNoAttributes = 0;
        internal const uint SignerProviderKeyContainer = 2;
        internal const uint SignerCertPolicyChain = 2;
        internal const uint CalgSha256 = 0x0000800C;

        [DllImport("mssign32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SignerSignEx(
            uint flags,
            ref SignerSubjectInfo subjectInfo,
            ref SignerCertificate signerCertificate,
            ref SignerSignatureInfo signatureInfo,
            ref SignerProviderInfo providerInfo,
            string? timestampUrl,
            IntPtr request,
            IntPtr sipData,
            out IntPtr signerContext);

        [DllImport("mssign32.dll")]
        internal static extern int SignerFreeSignerContext(IntPtr signerContext);

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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SignerProviderInfo
        {
            internal uint Size;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string ProviderName;
            internal uint ProviderType;
            internal uint KeySpec;
            internal uint PrivateKeyChoice;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string KeyContainer;
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
