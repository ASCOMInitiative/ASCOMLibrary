using System;
using Xunit;
using Xunit.Abstractions;

// Prevent parallelism in unit tests
//[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ASCOM.Alpaca.Tests
{
    static internal class TestSupport
    {
        internal static bool CompareBytes(byte[] supplied, byte[] required, ITestOutputHelper output)
        {
            //if (required.Length != supplied.Length) return false;
            for (int i = 0; i < required.Length; i++)
            {
                // output.WriteLine($"CompareBytes: {i}: {supplied[i]} {required[i]}");
                if (required[i] != supplied[i])
                {
                    // output.WriteLine($"CompareBytes: Returning FALSE");
                    return false;
                }
            }

            // output.WriteLine($"CompareBytes: Returning TRUE");
            return true;
        }

        internal static bool CompareArrays(Array sourceArray, Array responseArray, ITestOutputHelper output)
        {
            return TestSupport.CompareArrays(sourceArray, responseArray, true, output);
        }

        internal static bool CompareArrays(Array sourceArray, Array responseArray, bool includeElementTypeTest, ITestOutputHelper output)
        {
            if (sourceArray is null)
            {
                output.WriteLine($"Source Array is NULL!");
                return false;
            }
            if (responseArray is null)
            {
                output.WriteLine($"Response Array is NULL!");
                return false;
            }
            if (sourceArray.Rank != responseArray.Rank)
            {
                output.WriteLine($"Array ranks are not equal. Source: {sourceArray.Rank}, Response: {responseArray.Rank}");
                return false;
            }

            if (sourceArray.GetLength(0) != responseArray.GetLength(0))
            {
                output.WriteLine($"Dimension 1 lengths are not equal. Source: {sourceArray.GetLength(0)}, Response: {responseArray.GetLength(0)}");
                return false;
            }
            if (sourceArray.GetLength(1) != responseArray.GetLength(1))
            {
                output.WriteLine($"Dimension 2 lengths are not equal. Source: {sourceArray.GetLength(1)}, Response: {responseArray.GetLength(1)}");
                return false;
            }
            if (sourceArray.Rank == 3)
            {
                if (sourceArray.GetLength(2) != responseArray.GetLength(2))
                {
                    output.WriteLine($"Dimension 3 lengths are not equal. Source: {sourceArray.GetLength(2)}, Response: {responseArray.GetLength(2)}");
                    return false;
                }
            }
            if (includeElementTypeTest)
            {
                if (sourceArray.GetType().GetElementType() != responseArray.GetType().GetElementType())
                {
                    output.WriteLine($"Element types are not the same. Source: {sourceArray.GetType().GetElementType()}, Response: {responseArray.GetType().GetElementType()}");
                    return false;
                }
            }
            try
            {
                switch (sourceArray.Rank)
                {
                    case 2:
                        output.WriteLine($"Array element types. Source: {sourceArray.GetType().GetElementType()}, Response: {responseArray.GetType().GetElementType()}");
                        output.WriteLine($"Array element values at index [0,0]. Source: {sourceArray.GetValue(0, 0)}, Response: {responseArray.GetValue(0, 0)}");

                        for (int i = 0; i < sourceArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < sourceArray.GetLength(1); j++)
                            {
                                if (Convert.ToDouble(sourceArray.GetValue(i, j)) != Convert.ToDouble(responseArray.GetValue(i, j)))
                                {
                                    output.WriteLine($"Array element values at index [{i},{j}] are not equal. Source: {sourceArray.GetValue(i, j)}, Response: {responseArray.GetValue(i, j)}");
                                    return false;
                                }
                            }
                        }
                        break;

                    case 3:
                        output.WriteLine($"Array element types. Source: {sourceArray.GetType().GetElementType()}, Response: {responseArray.GetType().GetElementType()}");
                        output.WriteLine($"Array element values at index [0,0,0]. Source: {sourceArray.GetValue(0, 0, 0)}, Response: {responseArray.GetValue(0, 0, 0)}");
                        for (int i = 0; i < sourceArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < sourceArray.GetLength(1); j++)
                            {
                                for (int k = 0; k < sourceArray.GetLength(2); k++)
                                {
                                    if (Convert.ToDouble(sourceArray.GetValue(i, j, k)) != Convert.ToDouble(responseArray.GetValue(i, j, k)))
                                    {
                                        output.WriteLine($"Array element values at index [{i},{j},{k}] are not equal. Source: {sourceArray.GetValue(i, j, k)}, Response: {responseArray.GetValue(i, j, k)}");
                                        return false;
                                    }
                                }
                            }
                        }
                        break;

                    default:
                        output.WriteLine($"Unsupported rank:{sourceArray.Rank}");
                        return false;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return true;
        }

    }
}
