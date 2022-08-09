# PolarisILSCallLogs

This project is archived and probably won't see further development.

This is a very hacky and rudimentary .NET 4 Framework application for parsing out Polaris ILS call logs to track how many hold calls and how many overdue calls go out.

Magic happens in `PolarisCallLogParser\Parser.cs` which is basically searching for text in call log lines and using that text to tabulate what type of call happened. It then attempts to insert it into an Entity Framework database table whose schema mimics the `CallLogSummary` object.

## License

PolarisILSCallLogs is Copyright 2016 by the Maricopa County Library District and is distributed under the [MIT License](http://opensource.org/licenses/MIT).
