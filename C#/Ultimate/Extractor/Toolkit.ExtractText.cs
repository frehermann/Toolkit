
using System;
using System.Text;

namespace ToolkitUltimate_Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            string strPath = System.AppDomain.CurrentDomain.BaseDirectory;

            // Starting with Toolkit version 10 native DLLs are no longer
            // copied to the system folder. The Toolkit constructor must
            // be called with the path to the native DLLs or place them
            // in your applications working directory. This example
            // assumes they are located in the default installation folder.
            // (Use x86 in the path for 32b applications)
            string toolkitPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\ActivePDF\Toolkit Ultimate\bin\x64";

            // Instantiate Object
            using (APToolkitNET.Toolkit toolkit = new APToolkitNET.Toolkit(toolkitPath))
            {
                // Get the Extractor object from Toolkit
                APToolkitNET.Extractor extractor = toolkit.GetExtractor();

                // Open the input PDF
                int result = toolkit.OpenInputFile(InputFileName: $"{strPath}Toolkit.Input.pdf");
                if (result == 0)
                {
                    // Extract the text from the whole document at once.
                    string extractedText = extractor.ExtractText();

                    string fileName = $"{System.IO.Path.GetRandomFileName()}.txt";
                    try
                    {
                        if (extractedText.Length != 0)
                        {
                            Console.WriteLine($"Writing full document text to: {fileName}");
                            System.IO.File.WriteAllText(
                                $"{System.IO.Directory.GetCurrentDirectory()}\\{fileName}",
                                extractedText);

                            // Get the number of pages in the input PDF
                            int numPages = toolkit.NumPages("");

                            // Extract Text by Page
                            for (int i = 1; i <= numPages; i++)
                            {
                                fileName = $"{System.IO.Path.GetRandomFileName()}_Page{i}.txt";
                                extractedText = extractor.ExtractText(i);
                                System.IO.File.WriteAllText(
                                    $"{strPath}\\{fileName}",
                                    extractedText);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No text found in document.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Exception caught creating image file ({fileName}): {e.Message}");
                    }

                    // Close the new file to complete PDF creation
                    toolkit.CloseInputFile();
                }
                else
                {
                    WriteResult($"Error opening input file: {result.ToString()}", toolkit);
                    return;
                }
            }

            // Process Complete
            WriteResult("Success!");
        }

        public static void WriteResult(string result, APToolkitNET.Toolkit toolkit = null)
        {
            StringBuilder resultText = new StringBuilder();
            resultText.AppendLine(result);
            if (toolkit != null)
            {
                resultText.AppendLine($"ErrorCode: {toolkit.ExtendedErrorCode.ToString()}");
                resultText.AppendLine($"Location: {toolkit.ExtendedErrorLocation}");
                resultText.AppendLine($"Description: {toolkit.ExtendedErrorDescription}");
            }
            Console.WriteLine(resultText.ToString());
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
