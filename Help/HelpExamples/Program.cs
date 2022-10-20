using HelpExamples;

try
{
    Console.WriteLine("Running AsynchronousDiscovery test...");
    AsynchronousDiscoveryClass.AsynchronousDiscovery();

    Console.WriteLine("Running AsyncMethodsAwait test...");
    AsyncMethodsAwaitClass.AsyncMethodsAwait();

    Console.WriteLine("Running AsyncMethodsTask test...");
    AsyncMethodsTaskClass.AsyncMethodsTask();

    Console.WriteLine("Running DetailedClientCreation test...");
    DetailedClientCreationClass.DetailedClientCreation();

    Console.WriteLine("Running DeviceSelection test...");
    DeviceSelectionClass.DeviceSelection();

    Console.WriteLine("Running ManualClientCreation test...");
    ManualClientCreationClass.ManualClientCreation();

    Console.WriteLine("Running SeamlessClientAccess test...");
    SeamlessClientAccessClass.SeamlessClientAccess();

    Console.WriteLine("Running SimpleClientCreation test...");
    SimpleClientCreationClass.SimpleClientCreation();

    Console.WriteLine("Running SynchronousDiscovery test...");
    SynchronousDiscoveryClass.SynchronousDiscovery();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

Console.WriteLine("Finished");
Console.ReadKey();
