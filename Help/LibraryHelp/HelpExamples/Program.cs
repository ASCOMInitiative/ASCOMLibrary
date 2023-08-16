using HelpExamples;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Running Client async methods test...");
            await Task.Run(async () =>
            {
                Console.WriteLine("Staring task");
                await ClientAsyncMethods.AsyncMethods();
            });

            //Task.WaitAll(t);
            Console.WriteLine("After WaitAll");


            //Console.WriteLine("Running AsynchronousDiscovery test...");
            //AsynchronousDiscoveryClass.AsynchronousDiscovery();

            //Console.WriteLine("Running AsyncMethodsAwait test...");
            //AsyncMethodsAwaitClass.AsyncMethodsAwait();

            //Console.WriteLine("Running AsyncMethodsTask test...");
            //AsyncMethodsTaskClass.AsyncMethodsTask();

            //Console.WriteLine("Running DetailedClientCreation test...");
            //DetailedClientCreationClass.DetailedClientCreation();

            //Console.WriteLine("Running DeviceSelection test...");
            //DeviceSelectionClass.DeviceSelection();

            //Console.WriteLine("Running ManualClientCreation test...");
            //ManualClientCreationClass.ManualClientCreation();

            //Console.WriteLine("Running SeamlessClientAccess test...");
            //SeamlessClientAccessClass.SeamlessClientAccess();

            //Console.WriteLine("Running SimpleClientCreation test...");
            //SimpleClientCreationClass.SimpleClientCreation();

            //Console.WriteLine("Running SynchronousDiscovery test...");
            //SynchronousDiscoveryClass.SynchronousDiscovery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        Console.WriteLine("Finished");
        Console.ReadKey();
    }
}