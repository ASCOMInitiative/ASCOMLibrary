namespace PackageAccessProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // This project doesn't do anything but is required because it references the ASCOM exceptions package and so makes the exceptions DLL appear
            // in the output directory.
            // The help projects use the package DLL as a reference source in order to resolve references to exceptions that are supplied in the package.
            throw new ASCOM.NotImplementedException("Dummy exception");
        }
    }
}
