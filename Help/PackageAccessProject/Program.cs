namespace PackageAccessProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // This project doesn't do anything but is required because it references the ASCOM exceptions and System.Text.Json packages and so makes their DLLs appear
            // in the output directory.

            // The help projects use the package DLL as a reference source in order to resolve references in the XML Help text.
            
            // SHow that System.Text.Json is referenced
            System.Text.Json.JsonElement element=new System.Text.Json.JsonElement();
            element.GetGuid();

            // SHow that ASCOM.Exceptions is referenced
            throw new ASCOM.NotImplementedException("Dummy exception");
        }
    }
}
