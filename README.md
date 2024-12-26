# DateFileNameParser

A lightweight .NET library for parsing file names containing date-related keywords and expanding them into actual file names. This is particularly useful for tasks like processing files from FTP servers where file names include dynamic date components.

## Features

*   **Keyword Parsing:** Interprets and expands various date-related keywords.
*   **Localization Support:** Supports date and time formatting based on different cultures or locales using `IFormatProvider`.
*   **Flexible Keywords:**
    *   `NOW`: Current date and time.
    *   `TODAY`: Current date (time set to 00:00:00).
    *   `YESTERDAY`: Previous day's date (time set to 00:00:00).
    *   `TOMORROW`: Next day's date (time set to 00:00:00).
    *   `LASTWEEK`: Start of last week (based on the current or provided culture).
    *   `NEXTWEEK`: Start of next week (based on the current or provided culture).
    *   **Relative Offsets:** Any date keyword (`NOW`, `TODAY`, `YESTERDAY`, etc.) can be offset by up to four different time units simultaneously. The format is `KEYWORD+|-VALUEUNIT`, where:
        *   `KEYWORD` is the date keyword you wish to offset (e.g., `NOW`, `YESTERDAY`).
        *   `+|-` is the addition or subtraction operator.
        *   `VALUE` is an integer representing the amount of the unit.
        *   `UNIT` is the time unit:
            *   `d`: Days
            *   `h`: Hours
            *   `m`: Minutes
            *   `s`: Seconds

        Examples: `NOW+1d`, `NOW-2h`, `YESTERDAY+3d-1h+30m`, `NOW+1d-2h+30m-15s`.
    *   **Custom formatting:** You can specify a custom date/time format string using the optional `dateFormat` and `formatProvider` parameters in the `Parse` method.
        Examples: `Parse(template, "yyyy-MM-dd")`, `Parse(template, formatProvider: new CultureInfo("fr-CA"))`.
    **Note:** All keywords are case-insensitive (e.g., `now`, `Now`, and `NOW` are all equivalent).
*   **Duplicate Unit Detection:** Detects and throws an exception if duplicate time units are used in format strings (e.g., "yyyyMMddHHmmHH").
*   **Lightweight:** Minimal dependencies and efficient implementation.
*   **.NET Standard 2.0:** Compatible with a wide range of .NET frameworks.

## Installation

You can install the package via NuGet:

```bash
Install-Package DateFileNameParser
or using the .NET CLI:

dotnet add package DateFileNameParser

Usage

using DateFileNameParser;
using System;
using System.Globalization;

public class Example
{
    public static void Main(string[] args)
    {
        DateTime exampleDate = new DateTime(2024, 10, 27, 10, 30, 0); // October 27, 2024, 10:30 AM

        // Using the current culture (default)
        string fileNamePattern1 = "report_<YESTERDAY>.txt";
        string fileName1 = FileNameParser.Parse(fileNamePattern1);
        Console.WriteLine(fileName1); // Output: report_10/26/2024 12:00:00 AM.txt

        // Using a specific culture (French-Canada)
        string fileNamePattern2 = "data_<NOW-1d>.csv";
        string fileName2 = FileNameParser.Parse(fileNamePattern2, new CultureInfo("fr-CA"));
        Console.WriteLine(fileName2); // Output: data_2024-10-26 10 h 30 min 00 s.csv

        // Using custom formatting with dateFormat parameter
        string fileNamePattern3 = "backup_<NOW>.zip";
        string fileName3 = FileNameParser.Parse(fileNamePattern3, "yyyyMMdd");
        Console.WriteLine(fileName3); // Output: backup_20241027.zip

        // Using relative offsets
        string fileNamePattern4 = "file_<NOW+2h>_name.log";
        string fileName4 = FileNameParser.Parse(fileNamePattern4);
        Console.WriteLine(fileName4); // Output: file_10/27/2024 12:30:00 PM_name.log

        string fileNamePattern5 = "file_<NOW-30m>.log";
        string fileName5 = FileNameParser.Parse(fileNamePattern5);
        Console.WriteLine(fileName5); // Output: file_10/27/2024 10:00:00 AM.log

        string fileNamePattern6 = "file_<NOW-15s>.log";
        string fileName6 = FileNameParser.Parse(fileNamePattern6);
        Console.WriteLine(fileName6); // Output: file_10/27/2024 10:29:45 AM.log

        //Demonstrates multiple offsets
        string fileNamePattern7 = "file_<NOW+1d-2h+30m-15s>.log";
        string fileName7 = FileNameParser.Parse(fileNamePattern7);
        Console.WriteLine(fileName7); // Output: file_10/28/2024 8:59:45 AM.log

        // Using LASTWEEK and NEXTWEEK
        string lastWeekFile = "report_<LASTWEEK>.txt";
        string parsedLastWeek = FileNameParser.Parse(lastWeekFile);
        Console.WriteLine($"Last Week: {parsedLastWeek}"); // Output: Last Week: report_10/20/2024 12:00:00 AM.txt

        string nextWeekFile = "data_<NEXTWEEK>.csv";
        string parsedNextWeek = FileNameParser.Parse(nextWeekFile);
        Console.WriteLine("Next Week: {parsedNextWeek}"); // Output: Next Week: data_11/3/2024 12:00:00 AM.csv

        string lastWeekFileFR = "report_<LASTWEEK>_fr-CA.txt";
        string parsedLastWeekFR = FileNameParser.Parse(lastWeekFileFR, new CultureInfo("fr-CA"));
        Console.WriteLine($"Last Week (fr-CA): {parsedLastWeekFR}"); // Output: Last Week (fr-CA): report_2024-10-20 00 h 00 min 00 s_fr-CA.txt

        // Using invalid expressions
        string invalidPattern1 = "file<NOW-15s>.log";
        string parsedInvalidPattern1 = FileNameParser.Parse(invalidPattern1);
        // Output: Unhandled exception. System.ArgumentException: Invalid expression

        string invalidPattern2 = "file_NOW-15s.log";
        string parsedInvalidPattern2 = FileNameParser.Parse(invalidPattern2);
        // Output: Unhandled exception. System.ArgumentException: Invalid expression

        string invalidPattern3 = "file_<NOW-15s-8s>.log";
        string parsedInvalidPattern3 = FileNameParser.Parse(invalidPattern3);
        // Output: Unhandled exception. System.FormatException: Duplicate time unit(s): S

        string invalidPattern4 = "file_<NOW-15s-8g>.log";
        string parsedInvalidPattern4 = FileNameParser.Parse(invalidPattern4);
        // Output: Unhandled exception. System.ArgumentException: Invalid expression
    }
}

Libraries Used
.NET Standard Library


Contributing
Contributions are welcome! Please open an issue or submit a pull request on GitHub.
Fork the repository.
Create a new branch: git checkout -b feature/my-new-feature
Make your changes.
Commit your changes: git commit -m 'Add some feature'
Push to the branch: git push origin feature/my-new-feature
Submit a pull request.


## License

[MIT License](LICENSE)

Contact
[Olaniyi Abdul-Wadood]

[olaniyiwadood@gmail.com]