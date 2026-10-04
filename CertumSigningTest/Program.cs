using System.Diagnostics;
using System.CommandLine;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static CertumSigningTest.Program.NativeMethods;

namespace CertumSigningTest;

/// <summary>
/// Defines the command-line entry point and Authenticode operations.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Enables diagnostic logging when the global <c>--debug</c> option is set.
    /// </summary>
    private static bool debug;

    /// <summary>
    /// Elapsed time of the most recently written log message.
    /// </summary>
    private static double lastElapsed = 0;

    /// <summary>
    /// Measures elapsed time for the current command action.
    /// </summary>
    private static readonly Stopwatch stopwatch = new();

    /// <summary>
    /// Builds and invokes the signing and validation command-line interface.
    /// </summary>
    /// <param name="args">Command-line arguments supplied by the host process.</param>
    /// <returns>The exit code returned by the selected command.</returns>
    private static int Main(string[] args)
    {
        RootCommand rootCommand = new("Sign and validate Authenticode files.");
        Option<bool> debugOption = new("--debug", "-d")
        {
            Description = "Enable detailed logging output.",
            Recursive = true
        };
        rootCommand.Options.Add(debugOption);

        Argument<string> signingFileArgument = new("file")
        {
            Description = "The file to sign.",
            Arity = ArgumentArity.ExactlyOne
        };
        Argument<string?> timestampUrlArgument = new("timestamp-url")
        {
            Description = "RFC 3161 timestamp server URL.",
            Arity = ArgumentArity.ZeroOrOne
        };

        Command signCommand = new("sign", "Sign a file using Authenticode.");
        signCommand.Arguments.Add(signingFileArgument);
        signCommand.Arguments.Add(timestampUrlArgument);

        // Keep post-sign validation local to the sign command.
        Option<bool> validateAfterSignOption = new("--validate", "-v")
        {
            Description = "Validate the file after signing."
        };
        signCommand.Options.Add(validateAfterSignOption);
        signCommand.SetAction(parseResult => RunAction(parseResult, debugOption, () =>
        {
            string fileName = parseResult.GetValue(signingFileArgument)!;
            int signingResult = Sign(fileName, parseResult.GetValue(timestampUrlArgument));

            // A failed signing operation must not proceed to validation.
            if (signingResult != 0 || !parseResult.GetValue(validateAfterSignOption))
            {
                return signingResult;
            }

            return Validate(fileName);
        }));

        Argument<string> validationFileArgument = new("file")
        {
            Description = "The file to validate.",
            Arity = ArgumentArity.ExactlyOne
        };

        Command validateCommand = new("validate", "Validate an Authenticode signature.");
        validateCommand.Arguments.Add(validationFileArgument);
        validateCommand.SetAction(parseResult => RunAction(parseResult, debugOption, () => Validate(parseResult.GetValue(validationFileArgument)!)));

        rootCommand.Subcommands.Add(signCommand);
        rootCommand.Subcommands.Add(validateCommand);

        return rootCommand.Parse(args).Invoke();
    }

    /// <summary>
    /// Initializes shared action state and invokes the selected operation.
    /// </summary>
    /// <param name="parseResult">Parsed command-line values.</param>
    /// <param name="debugOption">The global option controlling detailed logs.</param>
    /// <param name="action">The operation to invoke.</param>
    /// <returns>The operation's exit code.</returns>
    private static int RunAction(ParseResult parseResult, Option<bool> debugOption, Func<int> action)
    {
        debug = parseResult.GetValue(debugOption);
        stopwatch.Restart();
        lastElapsed = 0;
        return action();
    }

    /// <summary>
    /// Signs a file with the configured certificate and an RFC 3161 timestamp.
    /// </summary>
    /// <param name="fileName">Path of the file to sign.</param>
    /// <param name="requestedTimestampUrl">Optional timestamp server URL; the configured default is used when omitted.</param>
    /// <returns>Zero on success; otherwise, a nonzero exit code.</returns>
    private static int Sign(string fileName, string? requestedTimestampUrl)
    {
        try
        {
            const string thumbprint = "D75896DA61275CCA773682EA4622B9039BA3317F";
            const string defaultTimestampUrl = "http://time.certum.pl";
            char[] pin = ['4', '6', '3', '5'];

            LogDebug("Validating the command-line arguments.");

            string filePath = Path.GetFullPath(fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified input file does not exist.", filePath);
            }

            LogDebug("Confirmed that the input file exists.");

            string timestampUrl = requestedTimestampUrl ?? defaultTimestampUrl;
            if (!Uri.TryCreate(timestampUrl, UriKind.Absolute, out Uri? timestampUri) ||
                (timestampUri.Scheme != Uri.UriSchemeHttp && timestampUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("The timestamp server URL must be an absolute HTTP or HTTPS URL.", nameof(requestedTimestampUrl));
            }

            timestampUrl = timestampUri.AbsoluteUri;
            LogMessage($"Signing {filePath} using RFC 3161 timestamp server {timestampUrl}.");

            LogDebug($"Looking up certificate {thumbprint} in CurrentUser\\My.");

            using X509Store store = new(StoreName.My, StoreLocation.CurrentUser);
            LogDebug("Created the CurrentUser\\My certificate-store object.");

            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            LogDebug("Opened CurrentUser\\My read-only.");

            X509Certificate2Collection matchingCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
            LogDebug($"Found {matchingCertificates.Count} certificate(s) with the requested thumbprint.");

            if (matchingCertificates.Count == 0)
            {
                throw new InvalidOperationException($"No certificate with thumbprint {thumbprint} was found in CurrentUser\\My.");
            }

            using X509Certificate2 certificate = matchingCertificates[0];
            LogDebug($"Selected certificate: {certificate.Subject}.");
            LogDebug($"Certificate issuer: {certificate.Issuer}.");
            LogDebug($"Certificate has private key: {certificate.HasPrivateKey}.");

            if (!certificate.HasPrivateKey)
            {
                throw new CryptographicException("The selected certificate is not associated with a private key.");
            }

            using RSACng rsa = certificate.GetRSAPrivateKey() as RSACng
                ?? throw new CryptographicException("The selected certificate does not provide an RSA private key.");

            LogDebug($"Using the certificate-associated key through {rsa.Key.Provider?.Provider}.");
            rsa.Key.SetProperty(new CngProperty("SmartCardPin", Encoding.Unicode.GetBytes(new string(pin) + '\0'), CngPropertyOptions.None));
            LogDebug("Configured the KSP signing-key PIN.");
            LogDebug($"Acquired RSA private key: {rsa.GetType().FullName}.");

            using X509Certificate2 signerCertificateContext = X509CertificateLoader.LoadCertificate(certificate.RawData);
            LogDebug("Prepared the public signing certificate. The private key remains in the Microsoft Smart Card KSP.");

            using X509Chain certificateChain = new();
            certificateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            certificateChain.Build(certificate);
            byte[][] chainCertificates = new byte[certificateChain.ChainElements.Count][];
            for (int index = 0; index < certificateChain.ChainElements.Count; index++)
            {
                chainCertificates[index] = certificateChain.ChainElements[index].Certificate.RawData;
            }

            LogDebug("Binding the selected certificate to the Authenticode signing request.");
            NativeMethods.SignerCertificateStoreInfo signerCertificateStoreInfo = new()
            {
                Size = (uint)Marshal.SizeOf<NativeMethods.SignerCertificateStoreInfo>(),
                SigningCertificate = signerCertificateContext.Handle,
                CertificatePolicy = NativeMethods.SignerCertPolicyChain,
                CertificateStore = IntPtr.Zero
            };
            IntPtr signerCertificateStoreInfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf<NativeMethods.SignerCertificateStoreInfo>());
            Marshal.StructureToPtr(signerCertificateStoreInfo, signerCertificateStoreInfoPointer, false);
            LogDebug("Bound the certificate context for Authenticode signing.");

            NativeMethods.SignerCertificate signerCertificate = new()
            {
                Size = (uint)Marshal.SizeOf<NativeMethods.SignerCertificate>(),
                CertificateChoice = NativeMethods.SignerCertStore,
                CertificateStoreInfo = signerCertificateStoreInfoPointer,
                WindowHandle = IntPtr.Zero
            };
            LogDebug("Configured the certificate-store signing source.");

            IntPtr signerFileInfoPointer = IntPtr.Zero;
            IntPtr signerSubjectIndexPointer = IntPtr.Zero;
            IntPtr signerContext = IntPtr.Zero;
            IntPtr timestampAlgorithmOidPointer = IntPtr.Zero;
            IntPtr timestampUrlPointer = IntPtr.Zero;
            NativeMethods.AuthenticodeDigestSignEx digestSign = (metadata, digestAlgorithm, digest, digestLength, signedDigest, signerCertificatePointer, certificateChainStore) =>
                SignAuthenticodeDigest(rsa, signerCertificateContext.Handle, chainCertificates, digestAlgorithm, digest, digestLength, signedDigest, signerCertificatePointer, certificateChainStore);

            // Keep the managed callback rooted while the native signer uses its function pointer.
            NativeMethods.AuthenticodeDigestSignEx? authenticodeDigestSign = digestSign;

            try
            {
                LogDebug("Preparing the existing file for Authenticode signing.");
                NativeMethods.SignerFileInfo signerFileInfo = new()
                {
                    Size = (uint)Marshal.SizeOf<NativeMethods.SignerFileInfo>(),
                    FileName = filePath,
                    FileHandle = IntPtr.Zero
                };
                signerFileInfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf<NativeMethods.SignerFileInfo>());
                Marshal.StructureToPtr(signerFileInfo, signerFileInfoPointer, false);
                LogDebug("Created the Authenticode file subject.");

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
                LogDebug("Configured SHA-256 Authenticode signing.");

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
                LogDebug("Configured the Microsoft Smart Card KSP to sign the Authenticode digest directly.");

                timestampAlgorithmOidPointer = Marshal.StringToHGlobalAnsi(NativeMethods.Sha256Oid);
                timestampUrlPointer = Marshal.StringToHGlobalUni(timestampUrl);
                LogDebug("Configured RFC 3161 SHA-256 timestamping.");

                LogDebug("Calling the Windows Authenticode signer and timestamp server. The Certum PIN dialog may appear now.");
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

                LogDebug("The Windows Authenticode signer and RFC 3161 timestamping completed successfully.");
            }
            finally
            {
                if (signerContext != IntPtr.Zero)
                {
                    NativeMethods.SignerFreeSignerContext(signerContext);
                    LogDebug("Released the Authenticode signer context.");
                }

                if (signerSubjectIndexPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(signerSubjectIndexPointer);
                    LogDebug("Released the Authenticode subject-index resources.");
                }

                authenticodeDigestSign = null;

                if (timestampUrlPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(timestampUrlPointer);
                    LogDebug("Released the timestamp-server URL resources.");
                }

                if (timestampAlgorithmOidPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(timestampAlgorithmOidPointer);
                    LogDebug("Released the timestamp-algorithm resources.");
                }

                if (signerFileInfoPointer != IntPtr.Zero)
                {
                    Marshal.DestroyStructure<NativeMethods.SignerFileInfo>(signerFileInfoPointer);
                    Marshal.FreeHGlobal(signerFileInfoPointer);
                    LogDebug("Released the Authenticode file-subject resources.");
                }

                Marshal.DestroyStructure<NativeMethods.SignerCertificateStoreInfo>(signerCertificateStoreInfoPointer);
                Marshal.FreeHGlobal(signerCertificateStoreInfoPointer);
                LogDebug("Released the Authenticode certificate resources.");
            }

            LogMessage("Authenticode file signing completed successfully.");
            return 0;
        }
        catch (Exception exception)
        {
            LogError("Authenticode file signing failed.");
            LogError($"Exception type: {exception.GetType().FullName}");
            LogError($"Message: {exception.Message}");
            LogError(exception.ToString());
            return 1;
        }
    }

    /// <summary>
    /// Verifies a file's Authenticode signature using Windows trust policy.
    /// </summary>
    /// <param name="fileName">Path of the file to validate.</param>
    /// <returns>Zero when the signature is trusted; otherwise, a nonzero exit code.</returns>
    private static int Validate(string fileName)
    {
        try
        {
            string filePath = Path.GetFullPath(fileName);
            LogMessage($"Validating {filePath}.");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified input file does not exist.", filePath);
            }

            LogDebug("Verifying the Authenticode signature with Windows trust validation.");
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

                LogDebug("Windows trust validation succeeded.");
            }
            finally
            {
                Marshal.DestroyStructure<NativeMethods.WinTrustFileInfo>(verificationFileInfoPointer);
                Marshal.FreeHGlobal(verificationFileInfoPointer);
                LogDebug("Released the Windows trust-validation resources.");
            }

            using X509Certificate2 embeddedSignerCertificate = new(X509Certificate.CreateFromSignedFile(filePath));
            LogMessage($"Validated successfully: {embeddedSignerCertificate.Subject}, Thumbprint: {embeddedSignerCertificate.Thumbprint}.");
            return 0;
        }
        catch (Exception exception)
        {
            LogError("Authenticode file validation failed.");
            LogError($"Exception type: {exception.GetType().FullName}");
            LogError($"Message: {exception.Message}");
            LogError(exception.ToString());
            return 1;
        }
    }

    /// <summary>
    /// Writes an error message with elapsed-time information to standard error.
    /// </summary>
    /// <param name="message">Error text to write.</param>
    private static void LogError(string message)
    {
        double elapsed = stopwatch.Elapsed.TotalSeconds;
        Console.Error.WriteLine($"{elapsed:0.000}s, +{elapsed - lastElapsed:0.000}s - {message}");
        lastElapsed = elapsed;

    }

    /// <summary>
    /// Writes a diagnostic message when debug logging is enabled.
    /// </summary>
    /// <param name="message">Diagnostic text to write.</param>
    private static void LogDebug(string message)
    {
        if (debug)
        {
            LogMessage(message);
        }
    }

    /// <summary>
    /// Writes a progress message with elapsed-time information to standard output.
    /// </summary>
    /// <param name="message">Progress text to write.</param>
    private static void LogMessage(string message)
    {
        double elapsed = stopwatch.Elapsed.TotalSeconds;
        Console.WriteLine($"{elapsed:0.000}s, +{elapsed - lastElapsed:0.000}s - {message}");
        lastElapsed = elapsed;
    }

    /// <summary>
    /// Signs an Authenticode digest with the certificate's RSA key and supplies its certificate chain.
    /// </summary>
    /// <param name="signingKey">RSA key used to sign the digest.</param>
    /// <param name="publicCertificate">Native certificate context returned to the signer.</param>
    /// <param name="chainCertificates">Encoded certificates to add to the signer's chain store.</param>
    /// <param name="digestAlgorithm">Windows identifier for the requested digest algorithm.</param>
    /// <param name="digest">Pointer to the digest bytes.</param>
    /// <param name="digestLength">Length of the digest in bytes.</param>
    /// <param name="signedDigest">Native output structure that receives the signature.</param>
    /// <param name="signerCertificatePointer">Native output location for the signer certificate.</param>
    /// <param name="certificateChainStore">Optional native certificate store to populate.</param>
    /// <returns>An HRESULT indicating success or failure.</returns>
    private static int SignAuthenticodeDigest(RSA signingKey, IntPtr publicCertificate, byte[][] chainCertificates, uint digestAlgorithm, IntPtr digest, uint digestLength, IntPtr signedDigest, IntPtr signerCertificatePointer, IntPtr certificateChainStore)
    {
        try
        {
            if (digestAlgorithm != NativeMethods.CalgSha256)
            {
                LogError($"The Authenticode digest algorithm 0x{digestAlgorithm:X8} is not SHA-256.");
                return unchecked((int)0x80090027);
            }

            LogDebug($"Signing the {digestLength}-byte Authenticode digest with the Microsoft Smart Card KSP key.");
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

            LogDebug($"Created a {signature.Length}-byte Authenticode signature with the KSP key.");
            return 0;
        }
        catch (Exception exception)
        {
            LogError($"Authenticode digest signing failed: {exception.Message}");
            return exception.HResult;
        }
    }

    /// <summary>
    /// Holds the callback data required to sign a digest through the Windows Authenticode API.
    /// </summary>
    /// <param name="signingKey">RSA key used for digest signing.</param>
    /// <param name="publicCertificate">Native context for the public signing certificate.</param>
    /// <param name="chainCertificates">Encoded certificates supplied to the native signer.</param>
    private sealed class AuthenticodeDigestSigner(RSA signingKey, IntPtr publicCertificate, byte[][] chainCertificates)
    {
        /// <summary>
        /// Signs a digest supplied by the Windows Authenticode API.
        /// </summary>
        /// <param name="metadata">Reserved signer metadata.</param>
        /// <param name="digestAlgorithm">Windows identifier for the digest algorithm.</param>
        /// <param name="digest">Pointer to the digest bytes.</param>
        /// <param name="digestLength">Length of the digest in bytes.</param>
        /// <param name="signedDigest">Native output structure that receives the signature.</param>
        /// <param name="signerCertificatePointer">Native output location for the signer certificate.</param>
        /// <param name="certificateChainStore">Optional native certificate store to populate.</param>
        /// <returns>An HRESULT indicating success or failure.</returns>
        internal int Sign(IntPtr metadata, uint digestAlgorithm, IntPtr digest, uint digestLength, IntPtr signedDigest, IntPtr signerCertificatePointer, IntPtr certificateChainStore)
        {
            return SignAuthenticodeDigest(signingKey, publicCertificate, chainCertificates, digestAlgorithm, digest, digestLength, signedDigest, signerCertificatePointer, certificateChainStore);
        }
    }

    /// <summary>
    /// Defines the Windows APIs, constants, and data layouts used for code signing and trust verification.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>Signer subject choice identifying a file.</summary>
        internal const uint SignerSubjectFile = 1;

        /// <summary>Signer certificate choice identifying a certificate store.</summary>
        internal const uint SignerCertStore = 2;

        /// <summary>Signer attribute choice specifying no additional attributes.</summary>
        internal const uint SignerNoAttributes = 0;

        /// <summary>Certificate policy requesting chain-based validation.</summary>
        internal const uint SignerCertPolicyChain = 2;

        /// <summary>Windows algorithm identifier for SHA-256.</summary>
        internal const uint CalgSha256 = 0x0000800C;

        /// <summary>Signer flag enabling callback-based digest signing.</summary>
        internal const uint SpcDigestSignExFlag = 0x4000;

        /// <summary>Timestamp type selecting RFC 3161 timestamping.</summary>
        internal const uint SignerTimestampRfc3161 = 0x00000002;

        /// <summary>Digest-signing choice selecting an <see cref="AuthenticodeDigestSignEx"/> callback.</summary>
        internal const uint DigestSignEx = 3;

        /// <summary>Combined X.509 and PKCS #7 certificate encoding identifier.</summary>
        internal const uint CertEncoding = 0x00010001;

        /// <summary>Certificate-store disposition that always adds a certificate.</summary>
        internal const uint CertStoreAddAlways = 4;

        /// <summary>Object identifier for SHA-256.</summary>
        internal const string Sha256Oid = "2.16.840.1.101.3.4.2.1";

        /// <summary>
        /// Receives an Authenticode digest and writes its signature into the native signer structures.
        /// </summary>
        /// <param name="metadata">Optional metadata supplied by the native signer.</param>
        /// <param name="digestAlgorithm">Windows identifier for the digest algorithm.</param>
        /// <param name="digest">Pointer to the digest bytes.</param>
        /// <param name="digestLength">Length of the digest in bytes.</param>
        /// <param name="signedDigest">Native output structure that receives the signature.</param>
        /// <param name="signerCertificate">Native output location for the signer certificate.</param>
        /// <param name="certificateChainStore">Optional native certificate store to populate.</param>
        /// <returns>An HRESULT indicating success or failure.</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate int AuthenticodeDigestSignEx(
            IntPtr metadata,
            uint digestAlgorithm,
            IntPtr digest,
            uint digestLength,
            IntPtr signedDigest,
            IntPtr signerCertificate,
            IntPtr certificateChainStore);

        /// <summary>Creates and timestamps a Windows Authenticode signature.</summary>
        /// <param name="flags">Signer behavior flags.</param>
        /// <param name="subjectInfo">Description of the file being signed.</param>
        /// <param name="signerCertificate">Certificate source for the signature.</param>
        /// <param name="signatureInfo">Digest algorithm and signature attributes.</param>
        /// <param name="providerInfo">Optional cryptographic provider information.</param>
        /// <param name="timestampFlags">Timestamp server protocol flags.</param>
        /// <param name="timestampAlgorithmOid">OID of the timestamp digest algorithm.</param>
        /// <param name="timestampUrl">Timestamp server URL.</param>
        /// <param name="request">Optional timestamp request data.</param>
        /// <param name="sipData">Optional subject interface package data.</param>
        /// <param name="signerContext">Receives the signer context to release after signing.</param>
        /// <param name="cryptoPolicy">Optional cryptographic policy.</param>
        /// <param name="digestSignInfo">Callback configuration for digest signing.</param>
        /// <param name="reserved">Reserved for future use.</param>
        /// <returns>Zero on success; otherwise, the Windows error code.</returns>
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

        /// <summary>Releases a signer context returned by <see cref="SignerSignEx3"/>.</summary>
        /// <param name="signerContext">Signer context to release.</param>
        /// <returns>Zero on success; otherwise, the Windows error code.</returns>
        [DllImport("mssign32.dll")]
        internal static extern int SignerFreeSignerContext(IntPtr signerContext);

        /// <summary>Gets the current process heap.</summary>
        /// <returns>A handle to the process heap.</returns>
        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetProcessHeap();

        /// <summary>Allocates memory from a heap.</summary>
        /// <param name="heap">Heap from which to allocate memory.</param>
        /// <param name="flags">Allocation options.</param>
        /// <param name="size">Number of bytes to allocate.</param>
        /// <returns>A pointer to the allocated memory, or zero on failure.</returns>
        [DllImport("kernel32.dll")]
        internal static extern IntPtr HeapAlloc(IntPtr heap, uint flags, UIntPtr size);

        /// <summary>Duplicates a certificate context.</summary>
        /// <param name="certificateContext">Certificate context to duplicate.</param>
        /// <returns>A duplicated certificate context, or zero on failure.</returns>
        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern IntPtr CertDuplicateCertificateContext(IntPtr certificateContext);

        /// <summary>Creates a certificate context from encoded certificate data.</summary>
        /// <param name="encoding">Encoding type of the certificate.</param>
        /// <param name="encodedCertificate">Encoded certificate bytes.</param>
        /// <param name="length">Length of the encoded certificate in bytes.</param>
        /// <returns>A certificate context, or zero on failure.</returns>
        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern IntPtr CertCreateCertificateContext(uint encoding, byte[] encodedCertificate, uint length);

        /// <summary>Adds a certificate context to a certificate store.</summary>
        /// <param name="store">Certificate store to update.</param>
        /// <param name="certificateContext">Certificate context to add.</param>
        /// <param name="disposition">Action to take when the certificate already exists.</param>
        /// <param name="storedContext">Receives the context added to the store.</param>
        /// <returns><see langword="true"/> if the certificate was added; otherwise, <see langword="false"/>.</returns>
        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern bool CertAddCertificateContextToStore(IntPtr store, IntPtr certificateContext, uint disposition, out IntPtr storedContext);

        /// <summary>Releases a certificate context.</summary>
        /// <param name="certificateContext">Certificate context to release.</param>
        /// <returns><see langword="true"/> if the context was released; otherwise, <see langword="false"/>.</returns>
        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern bool CertFreeCertificateContext(IntPtr certificateContext);

        // Keep native field order and types aligned with the Windows SDK structures.
        /// <summary>Describes the file subject passed to the Authenticode signer.</summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SignerFileInfo
        {
            /// <summary>Size of this structure in bytes.</summary>
            internal uint Size;

            /// <summary>Null-terminated path of the file to sign.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string FileName;

            /// <summary>Optional open file handle; zero when signing by path.</summary>
            internal IntPtr FileHandle;
        }

        /// <summary>Identifies the subject of an Authenticode signing request.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerSubjectInfo
        {
            /// <summary>Size of this structure in bytes.</summary>
            internal uint Size;

            /// <summary>Pointer to the subject index.</summary>
            internal IntPtr Index;

            /// <summary>Choice identifying the type of subject.</summary>
            internal uint SubjectChoice;

            /// <summary>Pointer to the subject information selected by <see cref="SubjectChoice"/>.</summary>
            internal IntPtr Subject;
        }

        /// <summary>Provides the certificate source and policy for an Authenticode signer.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerCertificateStoreInfo
        {
            /// <summary>Size of this structure in bytes.</summary>
            internal uint Size;

            /// <summary>Certificate context used to sign the file.</summary>
            internal IntPtr SigningCertificate;

            /// <summary>Certificate validation policy.</summary>
            internal uint CertificatePolicy;

            /// <summary>Optional certificate store associated with the signer.</summary>
            internal IntPtr CertificateStore;
        }

        /// <summary>Identifies the certificate used by the Authenticode signer.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerCertificate
        {
            /// <summary>Size of this structure in bytes.</summary>
            internal uint Size;

            /// <summary>Choice identifying the certificate source.</summary>
            internal uint CertificateChoice;

            /// <summary>Pointer to certificate information selected by <see cref="CertificateChoice"/>.</summary>
            internal IntPtr CertificateStoreInfo;

            /// <summary>Optional window handle for signer UI.</summary>
            internal IntPtr WindowHandle;
        }

        /// <summary>Specifies the digest algorithm and attributes for an Authenticode signature.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerSignatureInfo
        {
            /// <summary>Size of this structure in bytes.</summary>
            internal uint Size;

            /// <summary>Windows identifier for the signature digest algorithm.</summary>
            internal uint HashAlgorithm;

            /// <summary>Choice identifying the signature attribute format.</summary>
            internal uint AttributeChoice;

            /// <summary>Pointer to optional authenticated-code attributes.</summary>
            internal IntPtr AttributeAuthCode;

            /// <summary>Pointer to optional authenticated attributes.</summary>
            internal IntPtr AuthenticatedAttributes;

            /// <summary>Pointer to optional unauthenticated attributes.</summary>
            internal IntPtr UnauthenticatedAttributes;
        }

        /// <summary>Configures the callback used to sign an Authenticode digest.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SignerDigestSignInfo
        {
            /// <summary>Size of this structure in bytes.</summary>
            internal uint Size;

            /// <summary>Choice identifying the digest-signing mechanism.</summary>
            internal uint Choice;

            /// <summary>Function pointer for the digest-signing callback.</summary>
            internal IntPtr Callback;

            /// <summary>Optional metadata passed to the callback.</summary>
            internal IntPtr Metadata;

            /// <summary>Reserved for future use.</summary>
            internal uint Reserved;

            /// <summary>Reserved for future use.</summary>
            internal uint Reserved2;

            /// <summary>Reserved for future use.</summary>
            internal uint Reserved3;
        }

        /// <summary>WinVerifyTrust UI setting that suppresses user-interface prompts.</summary>
        internal const uint WinTrustUiNone = 2;

        /// <summary>WinVerifyTrust setting that disables revocation checks.</summary>
        internal const uint WinTrustRevokeNone = 0;

        /// <summary>WinVerifyTrust union choice identifying file information.</summary>
        internal const uint WinTrustChoiceFile = 1;

        /// <summary>WinVerifyTrust state action that does not retain state data.</summary>
        internal const uint WinTrustStateActionIgnore = 0;

        /// <summary>Action identifier for generic Authenticode trust verification.</summary>
        internal static Guid WinTrustActionGenericVerifyV2 = new("00AAC56B-CD44-11D0-8CC2-00C04FC295EE");

        /// <summary>Verifies a file's signature using the Windows trust provider.</summary>
        /// <param name="windowHandle">Optional window handle for trust-provider UI.</param>
        /// <param name="actionId">Identifier selecting the trust policy to apply.</param>
        /// <param name="trustData">File and policy information for verification.</param>
        /// <returns>Zero if the file is trusted; otherwise, a Windows trust error code.</returns>
        [DllImport("wintrust.dll", ExactSpelling = true)]
        internal static extern int WinVerifyTrust(
            IntPtr windowHandle,
            ref Guid actionId,
            ref WinTrustData trustData);

        /// <summary>Describes the file whose signature is being verified.</summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WinTrustFileInfo
        {
            /// <summary>Size of this structure in bytes.</summary>
            internal uint Size;

            /// <summary>Null-terminated path of the file to verify.</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string FilePath;

            /// <summary>Optional open file handle; zero when verifying by path.</summary>
            internal IntPtr FileHandle;

            /// <summary>Optional known subject identifier.</summary>
            internal IntPtr KnownSubject;
        }

        /// <summary>Configures the policy and file data passed to WinVerifyTrust.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct WinTrustData
        {
            /// <summary>Size of this structure in bytes.</summary>
            internal uint Size;

            /// <summary>Optional policy callback data.</summary>
            internal IntPtr PolicyCallbackData;

            /// <summary>Optional subject interface package client data.</summary>
            internal IntPtr SipClientData;

            /// <summary>UI behavior for trust verification.</summary>
            internal uint UiChoice;

            /// <summary>Revocation-check policy.</summary>
            internal uint RevocationChecks;

            /// <summary>Choice identifying the verification subject data.</summary>
            internal uint UnionChoice;

            /// <summary>Pointer to the subject data selected by <see cref="UnionChoice"/>.</summary>
            internal IntPtr FileInfo;

            /// <summary>Action controlling trust-provider state data.</summary>
            internal uint StateAction;

            /// <summary>Optional trust-provider state data.</summary>
            internal IntPtr StateData;

            /// <summary>Optional URL reference for verification.</summary>
            internal IntPtr UrlReference;

            /// <summary>Flags controlling trust-provider behavior.</summary>
            internal uint ProviderFlags;

            /// <summary>Context identifying the type of verification UI.</summary>
            internal uint UiContext;

            /// <summary>Optional signature settings.</summary>
            internal IntPtr SignatureSettings;
        }
    }

}
