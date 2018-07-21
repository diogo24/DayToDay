using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyManagementTemplate.Helpers
{
    public class WorldCupFileIOHelper
    {
        internal static string ReadFile()
        {
            string result = string.Empty;

            using (var streamReader = new StreamReader(@"wwwroot\docs\World Cup 2018 Games List Adapted.csv"))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        internal static bool UpdateFile(StringBuilder updatedFile)
        {

            using (var streamWriter = new StreamWriter(@"wwwroot\docs\World Cup 2018 Games List Adapted.csv"))
            {
                streamWriter.Write(updatedFile.ToString());
            }

            return true;
        }
    }
}
